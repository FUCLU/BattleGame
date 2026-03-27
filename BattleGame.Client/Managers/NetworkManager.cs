using BattleGame.Client.Config;
using BattleGame.Client.Network;
using BattleGame.Shared.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Managers
{
    public class NetworkManager
    {
        private static NetworkManager? _instance;
        public static NetworkManager Instance => _instance ??= new NetworkManager();
        private readonly ClientSocket _socket;
        private bool _connected = false;
        private NetworkManager()
        {
            var cfg = new ClientConfig();
            _socket = new ClientSocket(cfg);
            Connect();
        }

        public void Connect()
        {
            if (_connected) return;
            _socket.Connect();
            _connected = true;
        }

        public bool IsConnected => _socket.IsConnected();

        public LoginResultPacket Login(LoginPacket lg)
        {
            SendLG(lg);
            return (LoginResultPacket)Receive();
        }

        public LoginResultPacket Register(RegisterPacket lg)
        {
            SendRG(lg);
            return (LoginResultPacket)Receive();
        }

        public void Otpsend(OtpPacket lg)
        {
            SendOTP(lg);
            return;
        }

        public LoginResultPacket Forgotpass(ForgotPasswordPacket lg)
        {
            SendFG(lg);
            return (LoginResultPacket)Receive();
        }

        public LoginResultPacket VerifyOtp(OtpVerifyPacket lg)
        {
            SendVRF(lg);
            return (LoginResultPacket)Receive();
        }

        private void SendLG(LoginPacket packet)
        {
            string json = PacketSerializer.Serialize(packet);
            _socket.SendMessage(json);
        }
        private void SendRG(RegisterPacket packet)
        {
            string json = PacketSerializer.Serialize(packet);
            _socket.SendMessage(json);
        }
        private void SendOTP(OtpPacket packet)
        {
            string json = PacketSerializer.Serialize(packet);
            _socket.SendMessage(json);
        }
        private void SendFG(ForgotPasswordPacket packet)
        {
            string json = PacketSerializer.Serialize(packet);
            _socket.SendMessage(json);
        }
        private void SendVRF(OtpVerifyPacket packet)
        {
            string json = PacketSerializer.Serialize(packet);
            _socket.SendMessage(json);
        }

        private Packet Receive()
        {
            string json = _socket.ReceiveMessage();
            return PacketSerializer.Deserialize(json);
        }
    }
}