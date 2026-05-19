using BattleGame.LoadBalancer;
using BattleGame.LoadBalancer.Health;
using BattleGame.LoadBalancer.Logging;
using BattleGame.LoadBalancer.Network;
using BattleGame.LoadBalancer.Registry;
using BattleGame.LoadBalancer.Routing;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Globalization;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var lbConfig = config.GetSection("LoadBalancer").Get<LoadBalancerConfig>()
    ?? new LoadBalancerConfig();

var lbPortValue = Environment.GetEnvironmentVariable("LB_PORT") ?? config["LB_PORT"];
if (int.TryParse(lbPortValue, out int lbPort) && lbPort > 0)
{
    lbConfig.Port = lbPort;
}

var healthIntervalValue = Environment.GetEnvironmentVariable("LB_HEALTH_INTERVAL_SECONDS")
    ?? config["LB_HEALTH_INTERVAL_SECONDS"];
if (int.TryParse(healthIntervalValue, out int interval) && interval > 0)
{
    lbConfig.HealthCheckIntervalSeconds = interval;
}

var envServers = ParseServersFromEnv(Environment.GetEnvironmentVariable("SERVERS") ?? config["SERVERS"]);
if (envServers.Count > 0)
{
    lbConfig.Servers = envServers;
}

lbConfig.Redis.ConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
    ?? config["LoadBalancer:Redis:ConnectionString"]
    ?? lbConfig.Redis.ConnectionString;
lbConfig.Redis.Prefix = Environment.GetEnvironmentVariable("REDIS_PREFIX")
    ?? config["LoadBalancer:Redis:Prefix"]
    ?? lbConfig.Redis.Prefix;

var redis = await ConnectionMultiplexer.ConnectAsync(lbConfig.Redis.ConnectionString);
var registry = new RedisBackendRegistry(redis, lbConfig.Redis.Prefix);
await registry.SeedAsync(lbConfig.Servers);

var router = new RoundRoubinRouter(registry);
foreach (var server in lbConfig.Servers)
{
    if (!server.IsValid())
    {
        LbLogger.Warn($"skip invalid server config {server.Host}:{server.Port}", "config");
        continue;
    }

    await router.RegisterAsync(server);
    LbLogger.Event("backend", "registered",
        ("serverId", server.ServerId),
        ("private", server.ToString()),
        ("public", server.ToPublicString()));
}

var cts = new CancellationTokenSource();

var healthChecker = new HealthChecker(router, lbConfig.HealthCheckIntervalSeconds);
_ = Task.Run(() => healthChecker.StartAsync(cts.Token));

LbLogger.Event("startup", "config",
    ("redis", lbConfig.Redis.ConnectionString),
    ("prefix", lbConfig.Redis.Prefix),
    ("activeServers", await router.ActiveCountAsync()));
var lb = new LoadBalancerSocket(lbConfig.Port, router);
await lb.StartAsync(cts.Token);

static List<ServerEndpoint> ParseServersFromEnv(string? raw)
{
    // Format:
    // host:port[:publicHost[:publicPort[:serverId]]],...
    // Example:
    // battlegame_server:9001:127.0.0.1:9999,battlegame_server_2:9001:127.0.0.1:10000
    var result = new List<ServerEndpoint>();
    if (string.IsNullOrWhiteSpace(raw))
        return result;

    var entries = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    foreach (var entry in entries)
    {
        var parts = entry.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length < 2)
            continue;

        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int port))
            continue;

        string publicHost = parts.Length >= 3 ? parts[2] : "127.0.0.1";
        int publicPort = port;
        if (parts.Length >= 4 && int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedPublicPort))
            publicPort = parsedPublicPort;

        string serverId = parts.Length >= 5 ? parts[4] : $"{parts[0]}-{port}";

        result.Add(new ServerEndpoint
        {
            ServerId = serverId,
            Host = parts[0],
            Port = port,
            PublicHost = publicHost,
            PublicPort = publicPort
        });
    }

    return result;
}
