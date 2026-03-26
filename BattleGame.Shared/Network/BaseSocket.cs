using BattleGame.Shared.Config;
using System.Net.Sockets;
using System.Text;

namespace BattleGame.Shared.Network
{
    public abstract class BaseSocket
    {
        protected TcpClient? client;
        protected NetworkStream? stream;
        public bool IsConnected()
        {
            if (client != null && client.Connected) return true;
            return false;
        }
        public void SendMessage(string message)
        {
            if (stream == null)
                throw new InvalidOperationException("Stream=null");
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }
        public string ReceiveMessage()
        {
            if (stream == null)
                throw new InvalidOperationException("Stream=null");
            var buffer = new byte[GameConstants.BUFFER_SIZE];
            var sb = new StringBuilder();
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    throw new IOException("Ket noi dong");
                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                sb.Append(chunk);
                if (sb.ToString().Contains('\n'))
                    break;
            }
            return sb.ToString().TrimEnd('\n', '\r');
        }
        public void Close()
        {
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch {}
        }
    }
}