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
        private readonly MatchRepository _matchRepo;
        private readonly MatchmakingService _matchmaking;

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
            UserRepository userRepo,
            MatchRepository matchRepo,
            MatchmakingService matchmaking)
        {
            _client = client;
            _authService = authService;
            _otpService = otpService;
            _userRepo = userRepo;
            _matchRepo = matchRepo;
            _matchmaking = matchmaking;
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
                case PacketType.CreateRoom:
                    if (!_client.IsAuthenticated) return;
                    await HandleCreateRoomAsync((CreateRoomPacket)packet);
                    break;
                case PacketType.JoinRoom:
                    if (!_client.IsAuthenticated) return;
                    await HandleJoinRoomAsync((JoinRoomPacket)packet);
                    break;
                case PacketType.LeaveRoom:
                    if (!_client.IsAuthenticated) return;
                    HandleLeaveRoom((LeaveRoomPacket)packet);
                    break;
                case PacketType.RemoveRoom:
                    if (!_client.IsAuthenticated) return;
                    await HandleRemoveRoomAsync((RemoveRoomPacket)packet);
                    break;
                case PacketType.GetRoom:
                    if (!_client.IsAuthenticated) return;
                    await HandleGetRoomAsync((GetRoomPacket)packet);
                    break;
                case PacketType.SelectMap:
                    if (!_client.IsAuthenticated) return;
                    await HandleSelectMapAsync((SelectMapPacket)packet);
                    break;
                case PacketType.SelectCharacter:
                    if (!_client.IsAuthenticated) return;
                    await HandleSelectCharacterAsync((SelectionCharacterPacket)packet);
                    break;
                case PacketType.MatchRequest:
                    if (!_client.IsAuthenticated) return;
                    await HandleMatchRequestAsync((MatchRequestPacket)packet);
                    break;
                case PacketType.GetLeaderboard:
                    if (!_client.IsAuthenticated) return;
                    await HandleGetLeaderboardAsync((GetLeaderboardPacket)packet);
                    break;
                case PacketType.Move:
                case PacketType.Attack:
                case PacketType.GameState:
                case PacketType.Disconnect:
                    if (!_client.IsAuthenticated) return;
                    await HandleGamePacketAsync(packet);
                    break;
            }
        }

        private void HandleLeaveRoom(LeaveRoomPacket p)
        {
            _matchmaking.LeaveRoom(p.RoomId, _client.UserId, _client);
            if (_client.CurrentRoomId == p.RoomId.ToString())
            {
                _client.CurrentRoomId = string.Empty;
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

        private async Task HandleCreateRoomAsync(CreateRoomPacket p)
        {
            var (roomId, success) = _matchmaking.CreateRoom(
                p.RoomName ?? "Room",
                p.Password ?? "",
                _client.UserId,
                _client.Username,
                _client);

            await _client.SendAsync(new CreateRoomResultPacket
            {
                RoomId = roomId
            });
        }

        private async Task HandleJoinRoomAsync(JoinRoomPacket p)
        {
            bool success = _matchmaking.JoinRoom(
                p.RoomId,
                p.Password ?? "",
                _client.UserId,
                _client.Username,
                _client,
                out string message);

            if (success)
            {
                _client.CurrentRoomId = p.RoomId.ToString();
                var room = _matchmaking.GetRoom(p.RoomId);
                var resultPacket = new JoinRoomResultPacket
                {
                    Success = true,
                    RoomId = p.RoomId,
                    Player1Name = room?.Player1Name ?? string.Empty,
                    Player2Name = room?.Player2Name ?? string.Empty,
                    IsOwner = room != null && room.OwnerId == _client.UserId,
                    Message = "Join room thành công."
                };

                await _client.SendAsync(resultPacket);

                if (room?.Player1Handler != null && room.Player1Handler != _client)
                {
                    await room.Player1Handler.SendAsync(new JoinRoomResultPacket
                    {
                        Success = true,
                        RoomId = p.RoomId,
                        Player1Name = room.Player1Name,
                        Player2Name = room.Player2Name,
                        IsOwner = true,
                        Message = "Người chơi đã vào phòng."
                    });
                }

                if (room != null)
                {
                    var readySnapshot = new ReadyPacket
                    {
                        Player1Ready = room.Player1CharId != -1,
                        Player2Ready = room.Player2CharId != -1
                    };

                    if (room.Player1Handler != null)
                        await room.Player1Handler.SendAsync(readySnapshot);

                    if (room.Player2Handler != null)
                        await room.Player2Handler.SendAsync(readySnapshot);
                }
            }
            else
            {
                await _client.SendAsync(new JoinRoomResultPacket
                {
                    Success = false,
                    RoomId = p.RoomId,
                    Message = message
                });
            }
        }

        public void HandleClientDisconnect()
        {
            _matchmaking.HandleDisconnect(_client.UserId, _client);
        }

        private async Task HandleGetRoomAsync(GetRoomPacket p)
        {
            var rooms = _matchmaking.GetRooms(_client.UserId);

            await _client.SendAsync(new GetRoomResultPacket
            {
                Rooms = rooms
            });
        }

        private async Task HandleRemoveRoomAsync(RemoveRoomPacket p)
        {
            bool success = _matchmaking.RemoveRoom(p.RoomId, _client.UserId, _client);
            string message = success ? "" : "Không thể xóa phòng.";

            await _client.SendAsync(new RemoveRoomResultPacket
            {
                Success = success,
                Message = message
            });
        }

        private async Task HandleSelectMapAsync(SelectMapPacket p)
        {
            _matchmaking.SetMap(p.RoomId, p.MapId);
            var room = _matchmaking.GetRoom(p.RoomId);

            if (room != null && room.Player1Handler != null && room.Player2Handler != null)
            {
                await room.Player1Handler.SendAsync(p);
                await room.Player2Handler.SendAsync(p);
            }
        }

        private async Task HandleSelectCharacterAsync(SelectionCharacterPacket p)
        {
            if (!int.TryParse(_client.CurrentRoomId, out int roomId))
                return;

            _matchmaking.SetCharacter(roomId, _client.UserId, p.CharacterId);

            var room = _matchmaking.GetRoom(roomId);
            if (room == null)
                return;

            var readyPacket = new ReadyPacket
            {
                Player1Ready = room.Player1CharId != -1,
                Player2Ready = room.Player2CharId != -1
            };

            if (room.Player1Handler != null)
                await room.Player1Handler.SendAsync(readyPacket);

            if (room.Player2Handler != null)
                await room.Player2Handler.SendAsync(readyPacket);
        }

        private async Task HandleMatchRequestAsync(MatchRequestPacket p)
        {
            if (!int.TryParse(_client.CurrentRoomId, out int roomId))
                return;

            var room = _matchmaking.GetRoom(roomId);
            if (room == null)
                return;

            // Chỉ chủ phòng được quyền start.
            if (room.OwnerId != _client.UserId)
                return;

            // Chỉ start khi cả 2 người đã chọn character + map.
            if (!_matchmaking.AreAllReady(roomId))
                return;

            _matchmaking.StartMatch(roomId);
            var matchFoundPacket = _matchmaking.BuildMatchFoundPacket(roomId);

            if (matchFoundPacket != null && room.Player1Handler != null && room.Player2Handler != null)
            {
                await room.Player1Handler.SendAsync(matchFoundPacket);
                await room.Player2Handler.SendAsync(matchFoundPacket);
            }
        }

        private async Task HandleGetLeaderboardAsync(GetLeaderboardPacket p)
        {
            var rankings = _matchRepo.GetLeaderboard(100);
            var entries = new List<LeaderboardEntry>();

            int rank = 1;
            foreach (var (username, wins, losses) in rankings)
            {
                entries.Add(new LeaderboardEntry
                {
                    Rank = rank,
                    Username = username,
                    Wins = wins,
                    Losses = losses
                });
                rank++;
            }

            await _client.SendAsync(new GetLeaderboardResultPacket
            {
                Entries = entries
            });
        }


        private async Task HandleGamePacketAsync(Packet packet)
        {
            if (!int.TryParse(_client.CurrentRoomId, out int roomId))
                return;

            var room = _matchmaking.GetRoom(roomId);
            if (room == null)
                return;

            ClientHandler? opponent = room.Player1Handler == _client
                ? room.Player2Handler
                : room.Player1Handler;

            if (opponent == null)
                return;

            switch (packet.Type)
            {
                case PacketType.Move:
                case PacketType.Attack:
                case PacketType.GameState:
                case PacketType.Disconnect:
                    await opponent.SendAsync(packet);
                    break;
            }
        }
    }
}
