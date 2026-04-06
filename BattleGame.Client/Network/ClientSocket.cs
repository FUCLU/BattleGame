using System.Net.Sockets;
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
            _client = new TcpClient();
            await _client.ConnectAsync(_config.ServerIP, _config.ServerPort);
            _stream = _client.GetStream();
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
    }
}