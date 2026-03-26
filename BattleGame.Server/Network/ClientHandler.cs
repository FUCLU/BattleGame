    using System.Net.Sockets;
    using System.Text.Json;
    using BattleGame.Server.Database;
    using BattleGame.Server.Logging;
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
            private string _pendingNewPwHash = string.Empty;

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

            public void Handle()
            {
                try
                {
                    while (_tcpClient.Connected)
                    {
                        try
                        {
                            string json = _socket.ReceiveMessage();
                            var packet = PacketSerializer.Deserialize(json);
                            Route(packet);
                        }
                        catch (IOException)
                        {
                            break;
                        }
                        catch (JsonException)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    _socket.Close();
                }
            }

            private void Route(Packet packet)
            {
                switch (packet.Type)
                {
                    case PacketType.Login:
                        HandleLogin((LoginPacket)packet);
                        break;
                    case PacketType.Register:
                        HandleRegister((RegisterPacket)packet);
                        break;
                    case PacketType.ForgotPassword:
                        HandleForgotPassword((ForgotPasswordPacket)packet);
                        break;
                    case PacketType.OtpVerify:
                        HandleOtpVerify((OtpVerifyPacket)packet);
                        break;
                    case PacketType.OtpSend:
                        HandleOtpSend((OtpPacket)packet);
                        break;
                    default:
                        break;
                }
            }
            
            private void HandleOtpSend(OtpPacket p)
            {
                bool sent = _otpService.SendOtp(p.Email);
            }

            private void HandleLogin(LoginPacket p)
            {
                var result = _authService.Login(p.Username, p.Password);
                Send(result switch
                {
                    AuthService.LoginResult.Success =>
                        new LoginResultPacket { Success = true, Message = "Đăng nhập thành công!"},
                    AuthService.LoginResult.UserNotFound =>
                        new LoginResultPacket { Success = false, Message = "Tên đăng nhập không tồn tại." },
                    _ =>
                        new LoginResultPacket { Success = false, Message = "Mật khẩu không đúng." }
                });
            }

            private void HandleRegister(RegisterPacket p)
            {
                var result = _authService.Register(p.Username, p.Password, p.Email);
                Send(result switch
                {
                    AuthService.RegisterResult.Success =>
                        new LoginResultPacket { Success = true, Message = "Đăng ký thành công!" },
                    AuthService.RegisterResult.UsernameTaken =>
                        new LoginResultPacket { Success = false, Message = "Tên đăng nhập đã tồn tại!" },
                    AuthService.RegisterResult.EmailTaken =>
                        new LoginResultPacket { Success = false, Message = "Email đã được sử dụng!" },
                    _ =>
                        new LoginResultPacket { Success = false, Message = "Đăng ký thất bại, vui lòng thử lại." }
                });
            }

            private void HandleForgotPassword(ForgotPasswordPacket p)
            {
                try
                {
                    _pendingEmail = p.Email;
                    _pendingNewPwHash = BCrypt.Net.BCrypt.HashPassword(p.Password);
                    _userRepo.UpdatePassword(_pendingEmail, _pendingNewPwHash);
                    _pendingEmail = string.Empty;
                    _pendingNewPwHash = string.Empty;
                    Send(new LoginResultPacket { Success = true, Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới." });
                }
                catch (Exception ex)
                {
                    Send(new LoginResultPacket { Success = false, Message = $"Lỗi khi đặt lại mật khẩu: {ex.Message}" });
                }
            }

            private void HandleOtpVerify(OtpVerifyPacket p)
            {
                var verifyResult = _otpService.VerifyOtp(p.Email, p.OtpCode);
                switch (verifyResult)
                {
                    case OtpService.VerifyResult.Success:
                        Send(new LoginResultPacket { Success = true, Message = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới." });
                        break;
                    case OtpService.VerifyResult.InvalidCode:
                        Send(new LoginResultPacket { Success = false, Message = "Mã OTP không đúng." });
                        break;
                    case OtpService.VerifyResult.MaxAttemptsReached:
                        Send(new LoginResultPacket { Success = false, Message = "Đã nhập sai quá 3 lần. Vui lòng yêu cầu mã mới." });
                        break;
                    case OtpService.VerifyResult.NotFound:
                        Send(new LoginResultPacket { Success = false, Message = "Mã OTP không tồn tại hoặc đã hết hạn." });
                        break;
                }
            }

            private void Send(Packet packet)
            {
                string json = PacketSerializer.Serialize(packet);
                _socket.SendMessage(json);
            }
        }
    }