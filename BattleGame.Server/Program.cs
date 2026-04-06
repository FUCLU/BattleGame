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

Console.WriteLine($"[INFO] Environment: {environment}");
Console.WriteLine($"[INFO] ConnectionString: {serverConfig.ConnectionString}");

try
{
    DbInitializer.Initialize(serverConfig.ConnectionString);
}
catch (Exception dbEx)
{
    ServerLogger.Error($"[FATAL] Database init failed: {dbEx.Message}");
    if (dbEx.StackTrace != null)
        ServerLogger.Error(dbEx.StackTrace);
    throw;
}

try
{
    var server = new GameServer(serverConfig);
    await server.StartAsync();
}
catch (Exception srvEx)
{
    ServerLogger.Error($"[FATAL] Server start failed: {srvEx.Message}");
    if (srvEx.StackTrace != null)
        ServerLogger.Error(srvEx.StackTrace);
    throw;
}
await Task.Delay(Timeout.Infinite);