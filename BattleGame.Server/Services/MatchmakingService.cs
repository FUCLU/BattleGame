using BattleGame.Server.Database;
using BattleGame.Server.Network;
using BattleGame.Shared.Packets;
using BattleGame.Shared.Simulation;

namespace BattleGame.Server.Services
{
    public class MatchmakingService
    {
        private const float MatchTickSeconds = 1f / 60f;
        private const int BroadcastEveryTicks = 2;
        private readonly MatchRepository _matchRepo;
        private readonly Dictionary<int, RoomData> _rooms = new();
        private readonly object _roomsLock = new();
        private int _nextRoomId = new Random().Next(100000, 999999);

        public class RoomData
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int MapId { get; set; } = -1;
            public DateTime? MatchStartTime { get; set; }

            public int OwnerId { get; set; } = -1;
            public string OwnerName { get; set; } = string.Empty;
            public ClientHandler? OwnerHandler { get; set; }

            public int Player1Id { get; set; }
            public string Player1Name { get; set; } = string.Empty;
            public ClientHandler? Player1Handler { get; set; }
            public int Player1CharId { get; set; } = -1;

            public int Player2Id { get; set; } = -1;
            public string Player2Name { get; set; } = string.Empty;
            public ClientHandler? Player2Handler { get; set; }
            public int Player2CharId { get; set; } = -1;
            public BattleSimulation? Simulation { get; set; }
            public CancellationTokenSource? SimulationCts { get; set; }
        }

        public MatchmakingService(MatchRepository matchRepo)
        {
            _matchRepo = matchRepo;
        }

        public (int RoomId, bool Success) CreateRoom(
            string roomName, string password, int player1Id, string player1Name, ClientHandler handler)
        {
            lock (_roomsLock)
            {
                int roomId = _nextRoomId++;
                var room = new RoomData
                {
                    RoomId = roomId,
                    RoomName = roomName,
                    Password = password,
                    OwnerId = player1Id,
                    OwnerName = player1Name,
                    OwnerHandler = handler,
                    Player1Id = -1,
                    Player1Name = string.Empty,
                    Player1Handler = null,
                    Player1CharId = -1,
                    Player2Id = -1,
                    Player2Name = string.Empty,
                    Player2Handler = null,
                    Player2CharId = -1
                };
                _rooms[roomId] = room;
                return (roomId, true);
            }
        }

        public bool RemoveRoom(int roomId, int userId, ClientHandler handler)
        {
            lock (_roomsLock)
            {
                if (!_rooms.TryGetValue(roomId, out var room))
                    return false;
                if (room.OwnerId != userId)
                    return false;
                if (room.Player2Id != -1)
                    return false;

                _rooms.Remove(roomId);
                return true;
            }
        }

        public void LeaveRoom(int roomId, int userId, ClientHandler handler)
        {
            lock (_roomsLock)
            {
                if (!_rooms.TryGetValue(roomId, out var room))
                    return;

                if (room.Player1Id == userId)
                {
                    room.Player1Id = -1;
                    room.Player1Name = string.Empty;
                    room.Player1Handler = null;
                    room.Player1CharId = -1;
                    return;
                }

                if (room.Player2Id == userId)
                {
                    room.Player2Id = -1;
                    room.Player2Name = string.Empty;
                    room.Player2Handler = null;
                    room.Player2CharId = -1;
                }
            }
        }

        public bool JoinRoom(int roomId, string password, int player2Id, string player2Name, ClientHandler handler, out string message)
        {
            lock (_roomsLock)
            {
                if (!_rooms.ContainsKey(roomId))
                {
                    message = "Phòng không tồn tại.";
                    return false;
                }
                var room = _rooms[roomId];
                if (room.Player2Id != -1)
                {
                    message = "Phòng đã đủ người.";
                    return false;
                }
                if (room.Password != password)
                {
                    message = "Mật khẩu phòng không đúng.";
                    return false;
                }

                if (room.Player1Id == -1)
                {
                    if (room.OwnerId != player2Id)
                    {
                        message = "Chủ phòng chưa vào. Chỉ chủ phòng mới có thể vào trước.";
                        return false;
                    }

                    room.Player1Id = player2Id;
                    room.Player1Name = player2Name;
                    room.Player1Handler = handler;
                    room.Player1CharId = -1;
                    message = string.Empty;
                    return true;
                }

                room.Player2Id = player2Id;
                room.Player2Name = player2Name;
                room.Player2Handler = handler;
                room.Player2CharId = -1;
                message = string.Empty;
                return true;
            }
        }

        public RoomData? GetRoom(int roomId)
        {
            lock (_roomsLock)
            {
                return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
            }
        }

        public List<RoomInfo> GetRooms(int userId)
        {
            lock (_roomsLock)
            {
                return _rooms.Values
                    // Hiển thị room còn slot trống (0/2 hoặc 1/2), hoặc room của chính owner.
                    .Where(r => r.Player2Id == -1 || r.OwnerId == userId)
                    .Select(r => new RoomInfo
                    {
                        RoomId = r.RoomId,
                        RoomName = r.RoomName,
                        CurrentPlayers = (r.Player1Id != -1 ? 1 : 0) + (r.Player2Id != -1 ? 1 : 0),
                        HasPassword = !string.IsNullOrWhiteSpace(r.Password),
                        IsOwner = r.OwnerId == userId
                    })
                    .ToList();
            }
        }

        public void HandleDisconnect(int userId, ClientHandler handler)
        {
            if (userId <= 0)
                return;

            lock (_roomsLock)
            {
                var roomsToUpdate = _rooms.Values.ToList();
                foreach (var room in roomsToUpdate)
                {
                    // Không xóa ngay room của owner khi đổi form/reconnect ngắn.
                    // Chỉ clear phiên online hiện tại để room vẫn còn trong danh sách.
                    if (room.OwnerId == userId || room.OwnerHandler == handler)
                    {
                        room.OwnerHandler = null;
                    }

                    if (room.Player1Id == userId || room.Player1Handler == handler)
                    {
                        room.Player1Id = -1;
                        room.Player1Name = string.Empty;
                        room.Player1Handler = null;
                        room.Player1CharId = -1;
                    }

                    if (room.Player2Id == userId || room.Player2Handler == handler)
                    {
                        room.Player2Id = -1;
                        room.Player2Name = string.Empty;
                        room.Player2Handler = null;
                        room.Player2CharId = -1;
                    }
                }
            }
        }

        public void SetMap(int roomId, int mapId)
        {
            lock (_roomsLock)
            {
                if (_rooms.ContainsKey(roomId))
                    _rooms[roomId].MapId = mapId;
            }
        }

        public void SetCharacter(int roomId, int playerId, int charId)
        {
            lock (_roomsLock)
            {
                if (!_rooms.ContainsKey(roomId))
                    return;
                var room = _rooms[roomId];
                if (room.Player1Id == playerId)
                    room.Player1CharId = charId;
                else if (room.Player2Id == playerId)
                    room.Player2CharId = charId;
            }
        }

        public bool AreAllReady(int roomId)
        {
            lock (_roomsLock)
            {
                if (!_rooms.ContainsKey(roomId))
                    return false;
                var room = _rooms[roomId];
                return room.Player1CharId != -1 && room.Player2CharId != -1 && room.MapId != -1;
            }
        }

        public MatchFoundPacket? BuildMatchFoundPacket(int roomId)
        {
            lock (_roomsLock)
            {
                if (!_rooms.ContainsKey(roomId))
                    return null;
                var room = _rooms[roomId];
                return new MatchFoundPacket
                {
                    RoomId = room.RoomId,
                    MapId = room.MapId,
                    Player1Id = room.Player1Id,
                    Player1Name = room.Player1Name,
                    Player1CharacterId = room.Player1CharId,
                    Player2Id = room.Player2Id,
                    Player2Name = room.Player2Name,
                    Player2CharacterId = room.Player2CharId
                };
            }
        }

        public void StartMatch(int roomId)
        {
            lock (_roomsLock)
            {
                if (!_rooms.TryGetValue(roomId, out var room))
                    return;

                room.MatchStartTime = DateTime.UtcNow;
                room.SimulationCts?.Cancel();
                room.SimulationCts?.Dispose();
                room.SimulationCts = new CancellationTokenSource();
                room.Simulation = BattleSimulation.Create(
                    room.Player1Id,
                    BattleCharacterCatalog.FromNetworkId(room.Player1CharId),
                    room.Player2Id,
                    BattleCharacterCatalog.FromNetworkId(room.Player2CharId),
                    ResolveConfigRoot());

                _ = Task.Run(() => RunSimulationLoopAsync(roomId, room.SimulationCts.Token));
            }
        }

        public void ApplyInput(int roomId, BattleInput input)
        {
            lock (_roomsLock)
            {
                if (_rooms.TryGetValue(roomId, out var room))
                    room.Simulation?.ApplyInput(input);
            }
        }

        private async Task RunSimulationLoopAsync(int roomId, CancellationToken token)
        {
            int broadcastCounter = 0;

            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(MatchTickSeconds));
                while (await timer.WaitForNextTickAsync(token))
                {
                    WorldStatePacket? packet = null;
                    GameOverPacket? gameOver = null;
                    ClientHandler? player1Handler = null;
                    ClientHandler? player2Handler = null;
                    int winnerId = -1;

                    lock (_roomsLock)
                    {
                        if (!_rooms.TryGetValue(roomId, out var room) || room.Simulation == null)
                            return;

                        room.Simulation.Update(MatchTickSeconds);
                        player1Handler = room.Player1Handler;
                        player2Handler = room.Player2Handler;

                        broadcastCounter++;
                        if (broadcastCounter >= BroadcastEveryTicks || room.Simulation.State.IsGameOver)
                        {
                            broadcastCounter = 0;
                            packet = new WorldStatePacket { State = CopyState(room.Simulation.State) };
                        }

                        if (room.Simulation.State.IsGameOver)
                        {
                            winnerId = room.Simulation.State.WinnerPlayerId;
                            gameOver = new GameOverPacket
                            {
                                WinnerPlayerId = winnerId,
                                Duration = room.MatchStartTime.HasValue
                                    ? (int)(DateTime.UtcNow - room.MatchStartTime.Value).TotalSeconds
                                    : 0
                            };
                        }
                    }

                    if (packet != null)
                    {
                        if (player1Handler != null)
                            await player1Handler.SendAsync(packet);
                        if (player2Handler != null)
                            await player2Handler.SendAsync(packet);
                    }

                    if (gameOver != null)
                    {
                        if (player1Handler != null)
                            await player1Handler.SendAsync(gameOver);
                        if (player2Handler != null)
                            await player2Handler.SendAsync(gameOver);

                        EndMatch(roomId, winnerId);
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static BattleState CopyState(BattleState state)
        {
            return new BattleState
            {
                ServerTick = state.ServerTick,
                Player1 = CopyPlayer(state.Player1),
                Player2 = CopyPlayer(state.Player2),
                Projectiles = state.Projectiles.Select(p => new ProjectileState
                {
                    ProjectileId = p.ProjectileId,
                    OwnerPlayerId = p.OwnerPlayerId,
                    X = p.X,
                    Y = p.Y,
                    VelocityX = p.VelocityX,
                    VelocityY = p.VelocityY,
                    Damage = p.Damage,
                    Stun = p.Stun,
                    Range = p.Range,
                    Lifetime = p.Lifetime,
                    Timer = p.Timer,
                    AnimationKey = p.AnimationKey,
                    CurrentFrame = p.CurrentFrame,
                    FacingRight = p.FacingRight,
                    RenderOffsetX = p.RenderOffsetX,
                    RenderOffsetY = p.RenderOffsetY,
                    Render = p.Render
                }).ToList(),
                Effects = state.Effects.Select(e => new EffectState
                {
                    EffectId = e.EffectId,
                    OwnerPlayerId = e.OwnerPlayerId,
                    EffectType = e.EffectType,
                    AnimationKey = e.AnimationKey,
                    X = e.X,
                    Y = e.Y,
                    Damage = e.Damage,
                    Stun = e.Stun,
                    CollisionWidth = e.CollisionWidth,
                    CollisionHeight = e.CollisionHeight,
                    BlockEnemyAttack = e.BlockEnemyAttack,
                    BlockEnemyProjectile = e.BlockEnemyProjectile,
                    BlockEnemySkill = e.BlockEnemySkill,
                    CurrentFrame = e.CurrentFrame,
                    RemainingTime = e.RemainingTime,
                    FacingRight = e.FacingRight,
                    LastDamageTick = e.LastDamageTick,
                    Render = e.Render
                }).ToList(),
                IsGameOver = state.IsGameOver,
                WinnerPlayerId = state.WinnerPlayerId
            };
        }

        private static PlayerBattleState CopyPlayer(PlayerBattleState player)
        {
            return new PlayerBattleState
            {
                PlayerId = player.PlayerId,
                CharacterId = player.CharacterId,
                Stats = player.Stats,
                X = player.X,
                Y = player.Y,
                VelocityX = player.VelocityX,
                VelocityY = player.VelocityY,
                FacingRight = player.FacingRight,
                IsGrounded = player.IsGrounded,
                Hp = player.Hp,
                Mana = player.Mana,
                IsProtecting = player.IsProtecting,
                IsAttacking = player.IsAttacking,
                IsUsingSkill = player.IsUsingSkill,
                IsDashing = player.IsDashing,
                IsHurt = player.IsHurt,
                IsStunned = player.IsStunned,
                IsDead = player.IsDead,
                ActionTimer = player.ActionTimer,
                ActionDuration = player.ActionDuration,
                ActionHitDone = player.ActionHitDone,
                CurrentSkillSlot = player.CurrentSkillSlot,
                CurrentSkillAnimation = player.CurrentSkillAnimation,
                HurtTimer = player.HurtTimer,
                StunTimer = player.StunTimer,
                DashTimer = player.DashTimer,
                Skill1Cooldown = player.Skill1Cooldown,
                Skill2Cooldown = player.Skill2Cooldown,
                CurrentAnimation = player.CurrentAnimation,
                CurrentFrame = player.CurrentFrame,
                CurrentActionId = player.CurrentActionId,
                CurrentActionTick = player.CurrentActionTick
            };
        }

        private static string? ResolveConfigRoot()
        {
            string current = AppContext.BaseDirectory;
            while (!string.IsNullOrWhiteSpace(current))
            {
                if (Directory.Exists(Path.Combine(current, "Config", "Characters")))
                    return current;

                string siblingClient = Path.Combine(current, "BattleGame.Client");
                if (Directory.Exists(Path.Combine(siblingClient, "Config", "Characters")))
                    return siblingClient;

                var parent = Directory.GetParent(current);
                if (parent == null)
                    break;

                current = parent.FullName;
            }

            return null;
        }

        public void EndMatch(int roomId, int winnerId)
        {
            lock (_roomsLock)
            {
                if (!_rooms.ContainsKey(roomId))
                    return;
                var room = _rooms[roomId];
                string winnerName = room.Player1Id == winnerId ? room.Player1Name : room.Player2Name;
                string loserName = room.Player1Id == winnerId ? room.Player2Name : room.Player1Name;
                int duration = room.MatchStartTime.HasValue
                    ? (int)(DateTime.UtcNow - room.MatchStartTime.Value).TotalSeconds
                    : 0;
                var match = new BattleGame.Server.Game.Match
                {
                    WinnerName = winnerName,
                    LoserName = loserName,
                    Duration = duration,
                    PlayedAt = room.MatchStartTime ?? DateTime.UtcNow
                };
                room.SimulationCts?.Cancel();
                room.SimulationCts?.Dispose();
                _matchRepo.Save(match);
                _rooms.Remove(roomId);
            }
        }
    }
}
