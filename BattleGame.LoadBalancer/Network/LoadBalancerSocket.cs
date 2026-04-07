using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BattleGame.LoadBalancer.Routing;

namespace BattleGame.LoadBalancer.Network
{
    public class LoadBalancerSocket
    {
        private readonly int port;
        private readonly RoundRoubinRouter router;

        public LoadBalancerSocket(int port, RoundRoubinRouter router)
        {
            this.port = port;
            this.router = router;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"[LB] Load balancer started on port {port}.");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync(ct);
                    Console.WriteLine($"[LB] Accepted connection from {client.Client.RemoteEndPoint}.");
                    _ = Task.Run(() => HandleClientAsync(client), ct);

                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[LB] Load balancer is shutting down...");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LB] Accept error: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var server = router.GetNext();
            if(server == null)
            {
                Console.WriteLine("[LB] No available servers to route the client.");
                client.Close();
                return;
            }

            await Redirect.SendAsync(client, server);
        }
    }
}
