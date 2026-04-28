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
        public async Task<GetRoomResultPacket> GetRoomAsync(GetRoomPacket packet)
        {
            await SendAsync(packet);
            return (GetRoomResultPacket)await ReceiveAsync();
        }
        public async Task<JoinRoomResultPacket> JoinRoomAsync(JoinRoomPacket packet)
        {
            await SendAsync(packet);
            return (JoinRoomResultPacket)await ReceiveAsync();
        }
        public async Task<CreateRoomResultPacket> CreateRoomAsync(CreateRoomPacket packet)
        {
            await SendAsync(packet);
            return (CreateRoomResultPacket)await ReceiveAsync();
        }
        public async Task<GetLeaderboardResultPacket> GetLeaderboardAsync(GetLeaderboardPacket packet)
        {
            await SendAsync(packet);
            return (GetLeaderboardResultPacket)await ReceiveAsync();
        }
        public async Task SelectMapAsync(SelectMapPacket packet)
        {
            await SendAsync(packet);
        }
        public async Task SelectCharacterAsync(SelectionCharacterPacket packet)
        {
            await SendAsync(packet);
        }
        public async Task MoveAsync(MovePacket packet)
        {
            await SendAsync(packet);
        }
        public async Task AttackAsync(AttackPacket packet)
        {
            await SendAsync(packet);
        }
        public async Task DisconnectAsync(DisconnectPacket packet)
        {
            await SendAsync(packet);
        }
        public async Task<MatchFoundPacket> MatchRequestAsync(MatchRequestPacket packet)
        {
            await SendAsync(packet);
            return (MatchFoundPacket)await ReceiveAsync();
        }
    }
}