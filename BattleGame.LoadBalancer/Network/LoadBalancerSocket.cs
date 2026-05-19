using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BattleGame.LoadBalancer.Logging;
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
            LbLogger.Event("network", "listen", ("port", port));

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync(ct);
                    LbLogger.Event("client", "accepted", ("remote", client.Client.RemoteEndPoint));
                    _ = Task.Run(() => HandleClientAsync(client), ct);

                }
                catch (OperationCanceledException)
                {
                    LbLogger.Info("load balancer is shutting down", "network");
                    break;
                }
                catch (Exception ex)
                {
                    LbLogger.Error($"accept failed: {ex.Message}", "network");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            (int? userId, int? roomId) = await TryReadAffinityHintAsync(client);
            var server = await router.GetNextAsync(userId, roomId);
            if(server == null)
            {
                LbLogger.Warn("no available backend to route client", "routing");
                client.Close();
                return;
            }

            await Redirect.SendAsync(client, server);
        }

        private static async Task<(int? UserId, int? RoomId)> TryReadAffinityHintAsync(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();

                byte[] lenBuf = new byte[4];
                if (!await TryReadExactWithTimeoutAsync(stream, lenBuf, 4, TimeSpan.FromMilliseconds(150)))
                    return (null, null);

                int size = BitConverter.ToInt32(lenBuf, 0);
                if (size <= 0 || size > 128)
                    return (null, null);

                byte[] data = new byte[size];
                if (!await TryReadExactWithTimeoutAsync(stream, data, size, TimeSpan.FromMilliseconds(150)))
                    return (null, null);

                string text = Encoding.UTF8.GetString(data);
                int? userId = null;
                int? roomId = null;
                foreach (var part in text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var kv = part.Split('=', 2, StringSplitOptions.TrimEntries);
                    if (kv.Length != 2) continue;
                    if (kv[0] == "uid" && int.TryParse(kv[1], out int uid)) userId = uid;
                    if (kv[0] == "rid" && int.TryParse(kv[1], out int rid)) roomId = rid;
                }

                return (userId, roomId);
            }
            catch
            {
                return (null, null);
            }
        }

        private static async Task<bool> TryReadExactWithTimeoutAsync(
            NetworkStream stream,
            byte[] buffer,
            int count,
            TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            int received = 0;
            while (received < count)
            {
                int read = await stream.ReadAsync(buffer.AsMemory(received, count - received), cts.Token);
                if (read == 0)
                    return false;

                received += read;
            }

            return true;
        }
    }
}
