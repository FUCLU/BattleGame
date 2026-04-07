using BattleGame.Server.Database;
using BattleGame.Server.Network;
using BattleGame.Server.Services;
using BattleGame.Shared.Packets;

namespace BattleGame.Server.Game
{
    public class PacketProcessor
    {
        private readonly ClientHandler _client;
        private readonly AuthService _authService;
        private readonly OtpService _otpService;
        private readonly UserRepository _userRepo;

        // Trạng thái OTP đang chờ của client này
        private string _pendingEmail = string.Empty;
        private string _pendingUsername = string.Empty;
        private string _pendingPasswordHash = string.Empty;
        private PendingOtpAction _pendingAction = PendingOtpAction.None;

        private enum PendingOtpAction { None, Register, ResetPassword }

        public PacketProcessor(
            ClientHandler client,
            AuthService authService,
            OtpService otpService,
            UserRepository userRepo)
        {
            _client = client;
            _authService = authService;
            _otpService = otpService;
            _userRepo = userRepo;
        }

        public async Task ProcessAsync(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.Login:
                    await HandleLoginAsync((LoginPacket)packet);
                    break;
                case PacketType.Register:
                    await HandleRegisterAsync((RegisterPacket)packet);
                    break;
                case PacketType.OtpVerify:
                    await HandleOtpVerifyAsync((OtpVerifyPacket)packet);
                    break;
                case PacketType.ForgotPassword:
                    await HandleForgotPasswordAsync((ForgotPasswordPacket)packet);
                    break;
                case PacketType.ResetPassword:
                    await HandleResetPasswordAsync((ResetPasswordPacket)packet);
                    break;

                // Game packets — chỉ xử lý khi đã login
                case PacketType.MatchRequest:
                case PacketType.SelectCharacter:
                case PacketType.Move:
                case PacketType.Attack:
                case PacketType.Disconnect:
                    if (!_client.IsAuthenticated) return; // bỏ qua nếu chưa login
                    await HandleGamePacketAsync(packet);
                    break;
            }
        }

        private async Task HandleLoginAsync(LoginPacket p)
        {
            var result = _authService.Login(p.Username, p.Password);

            if (result is AuthService.LoginResult.Success success)
            {
                // Lưu vào session của ClientHandler
                _client.UserId = success.UserId;
                _client.Username = success.Username;
                _client.IsAuthenticated = true;

                await _client.SendAsync(new LoginResultPacket
                {
                    Success = true,
                    Message = "Đăng nhập thành công!"
                });
            }
            else
            {
                await _client.SendAsync(new LoginResultPacket
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                });
            }
        }

        private async Task HandleRegisterAsync(RegisterPacket p)
        {
            var result = _authService.Register(p.Username, p.Email);

            if (result != AuthService.RegisterResult.Success)
            {
                await _client.SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = result switch
                    {
                        AuthService.RegisterResult.UsernameTaken => "Tên đăng nhập đã tồn tại!",
                        AuthService.RegisterResult.EmailTaken => "Email đã được sử dụng!",
                        _ => "Đăng ký thất bại, vui lòng thử lại."
                    }
                });
                return;
            }

            // Lưu tạm, chờ OTP xác thực xong mới lưu DB
            _pendingUsername = p.Username;
            _pendingPasswordHash = BCrypt.Net.BCrypt.HashPassword(p.Password);
            _pendingEmail = p.Email;
            _pendingAction = PendingOtpAction.Register;

            bool sent = _otpService.SendOtp(p.Email);
            await _client.SendAsync(new OtpPacket
            {
                Email = p.Email,
                Status = sent ? "pending" : "fail",
                Message = sent ? "Mã OTP đã được gửi về email!" : "Gửi OTP thất bại, thử lại sau."
            });
        }

        private async Task HandleOtpVerifyAsync(OtpVerifyPacket p)
        {
            var result = _otpService.VerifyOtp(_pendingEmail, p.Code);

            if (result != OtpService.VerifyResult.Success)
            {
                await _client.SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = result switch
                    {
                        OtpService.VerifyResult.InvalidCode => "Mã OTP không đúng.",
                        OtpService.VerifyResult.MaxAttemptsReached => "Đã sai quá 3 lần. Yêu cầu mã mới.",
                        _ => "Mã OTP không tồn tại hoặc đã hết hạn."
                    }
                });
                return;
            }

            // OTP đúng — thực hiện hành động tương ứng
            switch (_pendingAction)
            {
                case PendingOtpAction.Register:
                    _userRepo.Save(_pendingUsername, _pendingPasswordHash, _pendingEmail);
                    await _client.SendAsync(new OtpPacket
                    {
                        Status = "success",
                        Message = "Đăng ký thành công! Vui lòng đăng nhập."
                    });
                    break;

                case PendingOtpAction.ResetPassword:
                    await _client.SendAsync(new OtpPacket
                    {
                        Status = "success",
                        Message = "Xác thực thành công! Vui lòng nhập mật khẩu mới."
                    });
                    break;
            }

            // Reset pending state
            _pendingAction = PendingOtpAction.None;
            _pendingUsername = string.Empty;
            _pendingPasswordHash = string.Empty;
        }

        private async Task HandleForgotPasswordAsync(ForgotPasswordPacket p)
        {
            _pendingEmail = p.Email;
            _pendingAction = PendingOtpAction.ResetPassword;

            bool sent = _otpService.SendOtp(p.Email);
            await _client.SendAsync(new OtpPacket
            {
                Email = p.Email,
                Status = sent ? "pending" : "fail",
                Message = sent ? "Mã OTP đã được gửi về email!" : "Email không tồn tại hoặc gửi thất bại."
            });
        }

        private async Task HandleResetPasswordAsync(ResetPasswordPacket p)
        {
            // Chỉ cho reset nếu đã verify OTP xong (_pendingAction đã reset về None)
            if (_pendingAction != PendingOtpAction.None)
            {
                await _client.SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = "Vui lòng xác thực OTP trước."
                });
                return;
            }

            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(p.NewPassword);
                _userRepo.UpdatePassword(_pendingEmail, hash);
                _pendingEmail = string.Empty;

                await _client.SendAsync(new OtpPacket
                {
                    Status = "success",
                    Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập."
                });
            }
            catch (Exception ex)
            {
                await _client.SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        private Task HandleGamePacketAsync(Packet packet)
        {
            return Task.CompletedTask;
        }
    }
}