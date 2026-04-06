using System.Net.Sockets;
using BattleGame.Shared.Network;

namespace BattleGame.Server.Network
{
    public class ServerSocket : BaseSocket
    {
        public ServerSocket(TcpClient acceptedClient)
        {
            _client = acceptedClient;
            _stream = acceptedClient.GetStream();
        }
    }
}