using Microsoft.Extensions.Configuration;
using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Server.Logging;
using BattleGame.Server.Network;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var serverConfig = config.GetSection("Server").Get<ServerConfig>() ?? new ServerConfig();
serverConfig.Load(config);

ServerLogger.Event("startup", "config",
    ("env", environment),
    ("database", MaskConnectionString(serverConfig.ConnectionString)),
    ("logLevel", Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "INFO"),
    ("inputLogs", ServerLogger.InputPacketsEnabled));

try
{
    DbInitializer.Initialize(serverConfig.ConnectionString);
}
catch (Exception dbEx)
{
    ServerLogger.Error($"fatal database init failed: {dbEx.Message}", "startup");
    if (dbEx.StackTrace != null)
        ServerLogger.Debug(dbEx.StackTrace, "startup");
    throw;
}

try
{
    var server = new GameServer(serverConfig);
    await server.StartAsync();
}
catch (Exception srvEx)
{
    ServerLogger.Error($"fatal server start failed: {srvEx.Message}", "startup");
    if (srvEx.StackTrace != null)
        ServerLogger.Debug(srvEx.StackTrace, "startup");
    throw;
}
await Task.Delay(Timeout.Infinite);

static string MaskConnectionString(string connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        return "-";

    var parts = connectionString
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(part =>
        {
            int idx = part.IndexOf('=', StringComparison.Ordinal);
            if (idx <= 0)
                return part;

            string key = part[..idx];
            string value = part[(idx + 1)..];
            return key.Equals("Password", StringComparison.OrdinalIgnoreCase)
                ? $"{key}=***"
                : $"{key}={value}";
        });

    return string.Join(';', parts);
}
