using BattleGame.Server.Services;
using System.Net.Sockets;

namespace BattleGame.Server.Network
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly OtpService _otpService;

        // Session — set sau khi login/OTP verify thành công
        private int _userId = 0;
        private bool _isAuthenticated = false;
        private string _pendingEmail = "";

        public ClientHandler(TcpClient client, OtpService otpService)
        {
            _client = client;
            _otpService = otpService;
        }

        // TODO (A1 Hồng Phúc): 
        // 1. Implement vòng lặp đọc packet từ _client
        // 2. Gọi HandlePacket() cho từng packet nhận được
        // 3. Xử lý disconnect
        public void Handle()
        {
            // TODO: vòng lặp nhận packet
        }

        // TODO (A1 Hồng Phúc):
        // Thêm các case: Login, MatchRequest, Move, Attack, Disconnect
        // Case OTP đã được chuẩn bị sẵn bên dưới — điền vào khi có UserRepository
        private void HandlePacket(object packet)
        {
            // TODO: switch theo PacketType
            // Case Register  → _otpService.SendOtp(email, "REGISTER")
            // Case OtpVerify → _otpService.Verify(email, purpose, code)
            // Case ForgotPassword → _otpService.SendOtp(email, "RESET_PASSWORD")
        }

        private void Send(object packet)
        {
            // TODO: serialize và gửi packet qua _client
        }
    }
}