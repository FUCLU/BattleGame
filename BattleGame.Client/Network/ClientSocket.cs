using System.Net.Sockets;
using BattleGame.Client.Config;
using BattleGame.Shared.Network;

namespace BattleGame.Client.Network
{
    public class ClientSocket : BaseSocket
    {
        private readonly ClientConfig config;

        public ClientSocket(ClientConfig _config)
        {
            config = _config;
        }
        public void Connect()
        {
            client = new TcpClient();
            client.Connect(config.ServerIp, config.ServerPort);
            stream = client.GetStream();
        }
    }
}