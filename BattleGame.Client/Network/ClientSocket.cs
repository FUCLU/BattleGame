using System.Net.Sockets;
using System.Text;
using BattleGame.Client.Config;
using BattleGame.Shared.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Network
{
    public class ClientSocket : BaseSocket
    {
        private readonly ClientConfig _config;

        public ClientSocket(ClientConfig config)
        {
            _config = config;
        }

        public async Task ConnectAsync()
        {
            Console.WriteLine("[Client] Connecting to LoadBalancer...");
            _client = new TcpClient();
            await _client.ConnectAsync(_config.ServerIP, _config.ServerPort);
            _stream = _client.GetStream();
            Console.WriteLine("[Client] Connected to LB, waiting for redirect...");

            byte[] lenBuf = new byte[4];
            await ReadExactAsync(lenBuf, 4);
            int size = BitConverter.ToInt32(lenBuf, 0);
            byte[] dataBuf = new byte[size];
            await ReadExactAsync(dataBuf, size);
            string redirect = Encoding.UTF8.GetString(dataBuf);
            Console.WriteLine($"[Client] Redirect received: {redirect}");

            var parts = redirect.Split(':');
            string host = parts[0];
            int port = int.Parse(parts[1]);

            _stream.Close();
            _client.Close();

            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            _stream = _client.GetStream();
            Console.WriteLine($"[Client] Connected to GameServer {host}:{port}");
        }

        // Dùng riêng cho đọc redirect — plain text, không qua AES
        private async Task ReadExactAsync(byte[] buffer, int count)
        {
            int received = 0;
            while (received < count)
            {
                int n = await _stream!.ReadAsync(buffer, received, count - received);
                if (n == 0) throw new IOException("Mất kết nối");
                received += n;
            }
        }

        public async Task SendPacketAsync(Packet packet)
        {
            string json = PacketSerializer.Serialize(packet);
            await SendAsync(json);
        }

        public async Task<Packet> ReceivePacketAsync()
        {
            string json = await ReceiveAsync();
            return PacketSerializer.Deserialize(json);
        }

        public async Task<Packet> ReceivePacketAsync(CancellationToken token)
        {
            string json = await ReceiveAsync(token);
            return PacketSerializer.Deserialize(json);
        }

        public bool HasDataAvailable()
        {
            return _stream != null && _stream.DataAvailable;
        }
    }
}
