using BattleGame.LoadBalancer;
using BattleGame.LoadBalancer.Health;
using BattleGame.LoadBalancer.Network;
using BattleGame.LoadBalancer.Routing;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var lbConfig = config.GetSection("LoadBalancer").Get<LoadBalancerConfig>()
    ?? new LoadBalancerConfig();

var router = new RoundRoubinRouter();
foreach(var server in lbConfig.Servers)
{
    router.Register(server);
    Console.WriteLine($"Registered server: {server}");
}

var cts = new CancellationTokenSource();

var healthChecker = new HealthChecker(router, lbConfig.HealthCheckIntervalSeconds);
_ = Task.Run(() => healthChecker.StartAsync(cts.Token));

var lb = new LoadBalancerSocket(lbConfig.Port, router);
await lb.StartAsync(cts.Token);


