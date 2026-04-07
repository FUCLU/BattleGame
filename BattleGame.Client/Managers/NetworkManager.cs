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

        private NetworkManager()
        {
            var cfg = new ClientConfig();
            _socket = new ClientSocket(cfg);
        }

        // Kết nối tới Server
        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync();
        }

        public bool IsConnected => _socket.IsConnected();

        // Gửi packet bất kỳ
        public async Task SendAsync(Packet packet)
        {
            await _socket.SendPacketAsync(packet);
        }

        // Nhận packet bất kỳ
        public async Task<Packet> ReceiveAsync()
        {
            return await _socket.ReceivePacketAsync();
        }

        // Auth
        public async Task<LoginResultPacket> LoginAsync(LoginPacket packet)
        {
            await SendAsync(packet);
            return (LoginResultPacket)await ReceiveAsync();
        }

        public async Task<OtpPacket> RegisterAsync(RegisterPacket packet)
        {
            await SendAsync(packet);
            return (OtpPacket)await ReceiveAsync();
        }

        public async Task<OtpPacket> ForgotPasswordAsync(ForgotPasswordPacket packet)
        {
            await SendAsync(packet);
            return (OtpPacket)await ReceiveAsync();
        }

        public async Task<OtpPacket> VerifyOtpAsync(OtpVerifyPacket packet)
        {
            await SendAsync(packet);
            return (OtpPacket)await ReceiveAsync();
        }

        public async Task<OtpPacket> ResetPasswordAsync(ResetPasswordPacket packet)
        {
            await SendAsync(packet);
            return (OtpPacket)await ReceiveAsync();
        }
    }
}