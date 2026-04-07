using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using BattleGame.LoadBalancer.Routing;

namespace BattleGame.LoadBalancer.Health
{
    public class HealthChecker
    {
        private readonly RoundRoubinRouter router;
        private readonly int intervalSeconds;

        public HealthChecker(RoundRoubinRouter router, int intervalSeconds)
        {
            this.router = router;
            this.intervalSeconds = intervalSeconds;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(intervalSeconds * 1000, ct);
                await CheckAllAsync();
            }
        }

        private async Task CheckAllAsync()
        {
            var servers = router.GetAll();
            foreach (var server in servers)
            {
                bool alive = await PingAsync(server);
                if (!alive)
                {
                    router.Remove(server);
                    Console.WriteLine($"Server {server} is down and removed from routing.");
                }
            }
        }

        private async Task<bool> PingAsync(ServerEndpoint server)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(server.Host, server.Port).WaitAsync(TimeSpan.FromSeconds(2));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
