using System.Net.Sockets;
using System.Text.Json;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Shared.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Server.Network
{
    public class ClientHandler
    {
        private readonly TcpClient _tcpClient;
        private readonly AuthService _authService;
        private readonly OtpService _otpService;
        private readonly UserRepository _userRepo;
        private readonly ServerSocket _socket;
        private string _pendingEmail = string.Empty;

        public ClientHandler(
            TcpClient tcpClient,
            AuthService authService,
            OtpService otpService,
            UserRepository userRepo)
        {
            _tcpClient = tcpClient;
            _authService = authService;
            _otpService = otpService;
            _userRepo = userRepo;
            _socket = new ServerSocket(tcpClient);
        }

        public async Task HandleAsync()
        {
            try
            {
                while (_socket.IsConnected())
                {
                    try
                    {
                        string json = await _socket.ReceiveAsync();
                        var packet = PacketSerializer.Deserialize(json);
                        await RouteAsync(packet);
                    }
                    catch (IOException) { break; }
                    catch (JsonException) { break; }
                }
            }
            finally
            {
                _socket.Close();
            }
        }

        private async Task RouteAsync(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.Login:
                    await HandleLoginAsync((LoginPacket)packet);
                    break;
                case PacketType.Register:
                    await HandleRegisterAsync((RegisterPacket)packet);
                    break;
                case PacketType.ForgotPassword:
                    await HandleForgotPasswordAsync((ForgotPasswordPacket)packet);
                    break;
                case PacketType.OtpVerify:
                    await HandleOtpVerifyAsync((OtpVerifyPacket)packet);
                    break;
                case PacketType.ResetPassword:
                    await HandleResetPasswordAsync((ResetPasswordPacket)packet);
                    break;
                default:
                    break;
            }
        }

        // Đăng nhập
        private async Task HandleLoginAsync(LoginPacket p)
        {
            var result = _authService.Login(p.Username, p.Password);
            await SendAsync(result switch
            {
                AuthService.LoginResult.Success =>
                    new LoginResultPacket { Success = true, Message = "Đăng nhập thành công!" },
                AuthService.LoginResult.UserNotFound =>
                    new LoginResultPacket { Success = false, Message = "Tên đăng nhập không tồn tại." },
                _ =>
                    new LoginResultPacket { Success = false, Message = "Mật khẩu không đúng." }
            });
        }

        // Đăng ký → gửi OTP về email
        private async Task HandleRegisterAsync(RegisterPacket p)
        {
            var result = _authService.Register(p.Username, p.Password, p.Email);
            if (result == AuthService.RegisterResult.Success)
            {
                // Gửi OTP về email
                bool sent = _otpService.SendOtp(p.Email);
                _pendingEmail = p.Email;
                await SendAsync(new OtpPacket
                {
                    Email = p.Email,
                    Status = sent ? "pending" : "fail",
                    Message = sent ? "Mã OTP đã được gửi về email!" : "Gửi OTP thất bại!"
                });
            }
            else
            {
                await SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = result switch
                    {
                        AuthService.RegisterResult.UsernameTaken => "Tên đăng nhập đã tồn tại!",
                        AuthService.RegisterResult.EmailTaken => "Email đã được sử dụng!",
                        _ => "Đăng ký thất bại, vui lòng thử lại."
                    }
                });
            }
        }

        // Quên mật khẩu → gửi OTP về email
        private async Task HandleForgotPasswordAsync(ForgotPasswordPacket p)
        {
            bool sent = _otpService.SendOtp(p.Email);
            _pendingEmail = p.Email;
            await SendAsync(new OtpPacket
            {
                Email = p.Email,
                Status = sent ? "pending" : "fail",
                Message = sent ? "Mã OTP đã được gửi về email!" : "Email không tồn tại!"
            });
        }

        // Xác thực OTP
        private async Task HandleOtpVerifyAsync(OtpVerifyPacket p)
        {
            var result = _otpService.VerifyOtp(p.Email, p.Code);
            await SendAsync(result switch
            {
                OtpService.VerifyResult.Success =>
                    new OtpPacket { Status = "success", Message = "Xác thực thành công!" },
                OtpService.VerifyResult.InvalidCode =>
                    new OtpPacket { Status = "fail", Message = "Mã OTP không đúng." },
                OtpService.VerifyResult.MaxAttemptsReached =>
                    new OtpPacket { Status = "fail", Message = "Đã nhập sai quá 3 lần. Vui lòng yêu cầu mã mới." },
                _ =>
                    new OtpPacket { Status = "fail", Message = "Mã OTP không tồn tại hoặc đã hết hạn." }
            });
        }

        // Đặt lại mật khẩu
        private async Task HandleResetPasswordAsync(ResetPasswordPacket p)
        {
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(p.NewPassword);
                _userRepo.UpdatePassword(p.Email, hash);
                await SendAsync(new OtpPacket
                {
                    Status = "success",
                    Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập."
                });
            }
            catch (Exception ex)
            {
                await SendAsync(new OtpPacket
                {
                    Status = "fail",
                    Message = $"Lỗi: {ex.Message}"
                });
            }
        }

        private async Task SendAsync(Packet packet)
        {
            string json = PacketSerializer.Serialize(packet);
            await _socket.SendAsync(json);
        }
    }
}