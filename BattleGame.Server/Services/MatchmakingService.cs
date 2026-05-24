using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Logging;
using BattleGame.Server.Network;
using BattleGame.Shared.Packets;
using BattleGame.Shared.Simulation;

namespace BattleGame.Server.Services
{
    public class MatchmakingService
    {
        private const float MatchTickSeconds = 1f / 60f;
        private const int BroadcastEveryTicks = 2;
        private const int MaxRounds = 3;
        private const int RequiredRoundWins = 2;
        private const int DefaultTimeLimitMinutes = 3;
        private const int MinTimeLimitMinutes = 1;
        private const int MaxTimeLimitMinutes = 5;
        private const float DefaultRoundDurationSeconds = DefaultTimeLimitMinutes * 60f;
        private const float SuddenDeathDurationSeconds = 20f;
        private const float HpRatioTieTolerance = 0.0001f;
        private const double PostDeathRoundDelaySeconds = 2.5;

        private readonly MatchRepository _matchRepo;
        private readonly RedisRoomStore _roomStore;
        private readonly ServerConfig _config;

        private readonly Dictionary<int, LocalRoomRuntime> _runtimeRooms = new();
        private readonly object _runtimeLock = new();
        private int _nextRoomId = new Random().Next(100000, 999999);

        public class LocalRoomRuntime
        {
            public ClientHandler? Player1Handler { get; set; }
            public ClientHandler? Player2Handler { get; set; }
            public BattleSimulation? Simulation { get; set; }
            public CancellationTokenSource? SimulationCts { get; set; }
            public CancellationTokenSource? CountdownCts { get; set; }
            public bool CountdownActive { get; set; }
            public int CurrentRound { get; set; } = 1;
            public int Player1RoundWins { get; set; }
            public int Player2RoundWins { get; set; }
            public bool RoundEndPending { get; set; }
            public DateTime RoundEndReadyAtUtc { get; set; }
            public int PendingRoundWinnerId { get; set; }
            public int PendingMatchWinnerId { get; set; }
            public float RoundDurationSeconds { get; set; } = DefaultRoundDurationSeconds;
            public float RoundSecondsRemaining { get; set; } = DefaultRoundDurationSeconds;
            public bool SuddenDeathActive { get; set; }
        }

        public class RoomData
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int MapId { get; set; } = -1;
            public int TimeLimitMinutes { get; set; } = DefaultTimeLimitMinutes;
            public DateTime? MatchStartTime { get; set; }

            public int OwnerId { get; set; } = -1;
            public string OwnerName { get; set; } = string.Empty;

            public int Player1Id { get; set; }
            public string Player1Name { get; set; } = string.Empty;
            public ClientHandler? Player1Handler { get; set; }
            public int Player1CharId { get; set; } = -1;

            public int Player2Id { get; set; } = -1;
            public string Player2Name { get; set; } = string.Empty;
            public ClientHandler? Player2Handler { get; set; }
            public int Player2CharId { get; set; } = -1;
        }

        public sealed class RoomCloseNotification
        {
            public int RoomId { get; init; }
            public ClientHandler Handler { get; init; } = null!;
        }

        public MatchmakingService(MatchRepository matchRepo, RedisRoomStore roomStore, ServerConfig config)
        {
            _matchRepo = matchRepo;
            _roomStore = roomStore;
            _config = config;
        }

        public (int RoomId, bool Success) CreateRoom(string roomName, string password, int timeLimitMinutes, int ownerId, string ownerName, ClientHandler? handler, bool autoJoin = true)
        {
            RemoveExistingRoomsForOwner(ownerId);
            int safeTimeLimitMinutes = NormalizeTimeLimitMinutes(timeLimitMinutes);
            bool joinOwnerNow = autoJoin && handler != null;

            for (int i = 0; i < 50; i++)
            {
                int roomId = Interlocked.Increment(ref _nextRoomId);
                var room = new RoomMeta
                {
                    RoomId = roomId,
                    RoomName = roomName,
                    Password = password,
                    TimeLimitMinutes = safeTimeLimitMinutes,
                    OwnerId = ownerId,
                    OwnerName = ownerName,
                    OwnerWaitingToJoin = !joinOwnerNow,
                    Player1Id = joinOwnerNow ? ownerId : -1,
                    Player1Name = joinOwnerNow ? ownerName : string.Empty,
                    Player1CharId = -1,
                    Player2Id = -1,
                    Player2Name = string.Empty,
                    Player2CharId = -1,
                    ServerId = _config.ServerId
                };

                if (!_roomStore.CreateRoom(room))
                    continue;

                lock (_runtimeLock)
                {
                    _runtimeRooms[roomId] = new LocalRoomRuntime
                    {
                        Player1Handler = joinOwnerNow ? handler : null,
                        RoundDurationSeconds = safeTimeLimitMinutes * 60f,
                        RoundSecondsRemaining = safeTimeLimitMinutes * 60f
                    };
                }

                if (joinOwnerNow)
                    _roomStore.SetUserAffinity(ownerId, _config.ServerId);
                return (roomId, true);
            }

            return (-1, false);
        }

        private void RemoveExistingRoomsForOwner(int ownerId)
        {
            if (ownerId <= 0)
                return;

            var ownedRooms = _roomStore.GetAllRooms()
                .Where(room => room.ServerId == _config.ServerId && room.OwnerId == ownerId)
                .Select(room => room.RoomId)
                .ToList();

            foreach (int roomId in ownedRooms)
                RemoveRoomInternal(roomId);
        }

        public bool RemoveRoom(int roomId, int userId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null || room.OwnerId != userId || room.Player2Id != -1)
                return false;

            RemoveRoomInternal(roomId);
            return true;
        }

        public List<RoomCloseNotification> LeaveRoom(int roomId, int userId, ClientHandler handler)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
                return new List<RoomCloseNotification>();

            if (room.OwnerId == userId)
            {
                ClientHandler? notifyHandler = null;
                lock (_runtimeLock)
                {
                    if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                    {
                        notifyHandler = runtime.Player1Handler == handler
                            ? runtime.Player2Handler
                            : runtime.Player1Handler;
                    }
                }

                RemoveRoomInternal(roomId);
                if (notifyHandler == null)
                    return new List<RoomCloseNotification>();

                return new List<RoomCloseNotification>
                {
                    new RoomCloseNotification { RoomId = roomId, Handler = notifyHandler }
                };
            }

            if (room.Player1Id == userId)
            {
                room.Player1Id = -1;
                room.Player1Name = string.Empty;
                room.Player1CharId = -1;
            }
            else if (room.Player2Id == userId)
            {
                room.Player2Id = -1;
                room.Player2Name = string.Empty;
                room.Player2CharId = -1;
            }

            _roomStore.SaveRoom(room);

            lock (_runtimeLock)
            {
                if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                {
                    runtime.CountdownCts?.Cancel();
                    runtime.CountdownCts?.Dispose();
                    runtime.CountdownCts = null;
                    runtime.CountdownActive = false;
                    if (runtime.Player1Handler == handler) runtime.Player1Handler = null;
                    if (runtime.Player2Handler == handler) runtime.Player2Handler = null;
                }
            }

            return new List<RoomCloseNotification>();
        }

        public bool JoinRoom(int roomId, string password, int userId, string userName, ClientHandler handler, out string message)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
            {
                message = "Phong khong ton tai.";
                return false;
            }

            if (room.ServerId != _config.ServerId)
            {
                message = "Phong dang o server khac. Vui long reconnect.";
                return false;
            }

            bool isExistingPlayer = room.Player1Id == userId || room.Player2Id == userId;
            if (!isExistingPlayer && GetCurrentPlayerCount(room) >= 2)
            {
                message = "Phong da du nguoi.";
                return false;
            }

            if (!isExistingPlayer && room.Password != password)
            {
                message = "Mat khau phong khong dung.";
                return false;
            }

            lock (_runtimeLock)
            {
                if (!_runtimeRooms.TryGetValue(roomId, out var runtime))
                {
                    runtime = new LocalRoomRuntime();
                    _runtimeRooms[roomId] = runtime;
                }

                // Rejoin: user already belongs to this room, only refresh handler.
                if (room.Player1Id == userId)
                {
                    if (room.OwnerId == userId)
                    {
                        room.OwnerWaitingToJoin = false;
                        _roomStore.SaveRoom(room);
                    }
                    runtime.Player1Handler = handler;
                    _roomStore.SetUserAffinity(userId, _config.ServerId);
                    message = string.Empty;
                    return true;
                }

                if (room.Player2Id == userId)
                {
                    if (room.OwnerId == userId)
                    {
                        room.OwnerWaitingToJoin = false;
                        _roomStore.SaveRoom(room);
                    }
                    runtime.Player2Handler = handler;
                    _roomStore.SetUserAffinity(userId, _config.ServerId);
                    message = string.Empty;
                    return true;
                }

                if (room.Player1Id == -1)
                {
                    room.Player1Id = userId;
                    room.Player1Name = userName;
                    room.Player1CharId = -1;
                    if (room.OwnerId == userId)
                        room.OwnerWaitingToJoin = false;
                    runtime.Player1Handler = handler;
                }
                else
                {
                    room.Player2Id = userId;
                    room.Player2Name = userName;
                    room.Player2CharId = -1;
                    if (room.OwnerId == userId)
                        room.OwnerWaitingToJoin = false;
                    runtime.Player2Handler = handler;
                }
            }

            _roomStore.SaveRoom(room);
            _roomStore.SetUserAffinity(userId, _config.ServerId);
            TryScheduleAutoStart(roomId);
            message = string.Empty;
            return true;
        }

        public RoomData? GetRoom(int roomId)
        {
            var meta = _roomStore.GetRoom(roomId);
            if (meta == null)
                return null;

            lock (_runtimeLock)
            {
                _runtimeRooms.TryGetValue(roomId, out var runtime);
                return ToRoomData(meta, runtime);
            }
        }

        public List<RoomInfo> GetRooms(int userId)
        {
            return _roomStore.GetAllRooms()
                .Select(r => new RoomInfo
                {
                    RoomId = r.RoomId,
                    ServerId = r.ServerId,
                    RoomName = r.RoomName,
                    MapId = r.MapId,
                    TimeLimitMinutes = NormalizeTimeLimitMinutes(r.TimeLimitMinutes),
                    CurrentPlayers = GetCurrentPlayerCount(r),
                    HasPassword = !string.IsNullOrWhiteSpace(r.Password),
                    IsOwner = r.OwnerId == userId,
                    Player1Id = r.Player1Id,
                    Player1Name = r.Player1Name,
                    Player1Ready = r.Player1CharId != -1,
                    Player2Id = r.Player2Id,
                    Player2Name = r.Player2Name,
                    Player2Ready = r.Player2CharId != -1
                }).ToList();
        }

        public string? GetRoomServerId(int roomId)
        {
            return _roomStore.GetRoomServerId(roomId)
                ?? _roomStore.GetRoom(roomId)?.ServerId;
        }

        public bool IsLocalServer(string? serverId)
        {
            return !string.IsNullOrWhiteSpace(serverId)
                && string.Equals(serverId, _config.ServerId, StringComparison.OrdinalIgnoreCase);
        }

        public List<RoomCloseNotification> HandleDisconnect(int userId, ClientHandler handler)
        {
            var notifications = new List<RoomCloseNotification>();
            if (userId <= 0)
                return notifications;

            foreach (var room in _roomStore.GetAllRooms().Where(r => r.ServerId == _config.ServerId))
            {
                if (room.OwnerId == userId)
                {
                    ClientHandler? notifyHandler = null;
                    lock (_runtimeLock)
                    {
                        if (_runtimeRooms.TryGetValue(room.RoomId, out var runtime))
                        {
                            notifyHandler = runtime.Player1Handler == handler
                                ? runtime.Player2Handler
                                : runtime.Player1Handler;
                        }
                    }

                    RemoveRoomInternal(room.RoomId);
                    if (notifyHandler != null)
                    {
                        notifications.Add(new RoomCloseNotification
                        {
                            RoomId = room.RoomId,
                            Handler = notifyHandler
                        });
                    }
                    continue;
                }

                bool changed = false;
                if (room.Player1Id == userId)
                {
                    room.Player1Id = -1;
                    room.Player1Name = string.Empty;
                    room.Player1CharId = -1;
                    changed = true;
                }

                if (room.Player2Id == userId)
                {
                    room.Player2Id = -1;
                    room.Player2Name = string.Empty;
                    room.Player2CharId = -1;
                    changed = true;
                }

                if (changed)
                    _roomStore.SaveRoom(room);
            }

            lock (_runtimeLock)
            {
                foreach (var runtime in _runtimeRooms.Values)
                {
                    if (runtime.Player1Handler == handler) runtime.Player1Handler = null;
                    if (runtime.Player2Handler == handler) runtime.Player2Handler = null;
                    runtime.CountdownCts?.Cancel();
                    runtime.CountdownCts?.Dispose();
                    runtime.CountdownCts = null;
                    runtime.CountdownActive = false;
                }
            }

            return notifications;
        }

        public void SetMap(int roomId, int mapId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null || room.ServerId != _config.ServerId)
                return;

            room.MapId = mapId;
            _roomStore.SaveRoom(room);
            TryScheduleAutoStart(roomId);
        }

        private void RemoveRoomInternal(int roomId)
        {
            _roomStore.RemoveRoom(roomId);
            lock (_runtimeLock)
            {
                if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                {
                    runtime.CountdownCts?.Cancel();
                    runtime.CountdownCts?.Dispose();
                    runtime.SimulationCts?.Cancel();
                    runtime.SimulationCts?.Dispose();
                }
                _runtimeRooms.Remove(roomId);
            }
        }

        public void SetCharacter(int roomId, int playerId, int charId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null || room.ServerId != _config.ServerId)
                return;

            if (room.Player1Id == playerId) room.Player1CharId = charId;
            else if (room.Player2Id == playerId) room.Player2CharId = charId;
            _roomStore.SaveRoom(room);
            TryScheduleAutoStart(roomId);
        }

        public bool AreAllReady(int roomId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null || room.ServerId != _config.ServerId)
                return false;

            return room.Player1Id != -1
                && room.Player2Id != -1
                && room.Player1CharId != -1
                && room.Player2CharId != -1
                && room.MapId != -1;
        }

        public MatchFoundPacket? BuildMatchFoundPacket(int roomId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
                return null;

            return new MatchFoundPacket
            {
                RoomId = room.RoomId,
                MapId = room.MapId,
                TimeLimitMinutes = NormalizeTimeLimitMinutes(room.TimeLimitMinutes),
                Player1Id = room.Player1Id,
                Player1Name = room.Player1Name,
                Player1CharacterId = room.Player1CharId,
                Player2Id = room.Player2Id,
                Player2Name = room.Player2Name,
                Player2CharacterId = room.Player2CharId
            };
        }

        public bool StartMatch(int roomId)
        {
            var roomMeta = _roomStore.GetRoom(roomId);
            if (roomMeta == null || roomMeta.ServerId != _config.ServerId)
                return false;

            lock (_runtimeLock)
            {
                if (!_runtimeRooms.TryGetValue(roomId, out var runtime))
                    return false;

                runtime.CountdownCts?.Cancel();
                runtime.CountdownCts?.Dispose();
                runtime.CountdownCts = null;
                runtime.CountdownActive = false;

                roomMeta.MatchStartTime = DateTime.UtcNow;
                _roomStore.SaveRoom(roomMeta);

                runtime.SimulationCts?.Cancel();
                runtime.SimulationCts?.Dispose();
                runtime.SimulationCts = new CancellationTokenSource();
                runtime.CurrentRound = 1;
                runtime.Player1RoundWins = 0;
                runtime.Player2RoundWins = 0;
                runtime.RoundEndPending = false;
                runtime.RoundEndReadyAtUtc = default;
                runtime.PendingRoundWinnerId = 0;
                runtime.PendingMatchWinnerId = 0;
                runtime.RoundDurationSeconds = NormalizeTimeLimitMinutes(roomMeta.TimeLimitMinutes) * 60f;
                ResetRoundTimer(runtime);
                string? configRoot = ResolveConfigRoot();
                ServerLogger.Event("match", "start",
                    ("room", roomId),
                    ("p1", $"{roomMeta.Player1Name}:{BattleCharacterCatalog.FromNetworkId(roomMeta.Player1CharId)}"),
                    ("p2", $"{roomMeta.Player2Name}:{BattleCharacterCatalog.FromNetworkId(roomMeta.Player2CharId)}"),
                    ("configRoot", configRoot ?? "null"));
                try
                {
                    runtime.Simulation = BattleSimulation.Create(
                        roomMeta.Player1Id,
                        BattleCharacterCatalog.FromNetworkId(roomMeta.Player1CharId),
                        roomMeta.Player2Id,
                        BattleCharacterCatalog.FromNetworkId(roomMeta.Player2CharId),
                        configRoot);
                    ApplyRoundState(runtime.Simulation.State, runtime);
                }
                catch (Exception ex)
                {
                    ServerLogger.Error($"start match failed room={roomId}: {ex.Message}", "match");
                    ServerLogger.Debug(ex.ToString(), "match");
                    runtime.SimulationCts?.Cancel();
                    runtime.SimulationCts?.Dispose();
                    runtime.SimulationCts = null;
                    runtime.Simulation = null;
                    return false;
                }

                _ = Task.Run(() => RunSimulationLoopAsync(roomId, runtime.SimulationCts.Token));
                return true;
            }
        }

        public void ApplyInput(int roomId, BattleInput input)
        {
            lock (_runtimeLock)
            {
                if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                    runtime.Simulation?.ApplyInput(input);
            }
        }

        public void TryScheduleAutoStart(int roomId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null || room.ServerId != _config.ServerId)
                return;

            if (!AreAllReady(roomId))
            {
                lock (_runtimeLock)
                {
                    if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                    {
                        runtime.CountdownCts?.Cancel();
                        runtime.CountdownCts?.Dispose();
                        runtime.CountdownCts = null;
                        runtime.CountdownActive = false;
                    }
                }
                return;
            }

            lock (_runtimeLock)
            {
                if (!_runtimeRooms.TryGetValue(roomId, out var runtime))
                    return;

                if (runtime.CountdownActive || runtime.Simulation != null)
                    return;

                runtime.CountdownActive = true;
                runtime.CountdownCts = new CancellationTokenSource();
                _ = Task.Run(() => CountdownAndStartAsync(roomId, runtime.CountdownCts.Token));
            }
        }

        private async Task CountdownAndStartAsync(int roomId, CancellationToken token)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3), token);
                if (token.IsCancellationRequested)
                    return;

                if (!AreAllReady(roomId))
                    return;

                bool started = StartMatch(roomId);
                if (!started)
                {
                    ServerLogger.Warn($"countdown aborted room={roomId}: simulation did not start", "match");
                    return;
                }
                var matchFoundPacket = BuildMatchFoundPacket(roomId);
                if (matchFoundPacket == null)
                    return;

                ClientHandler? p1;
                ClientHandler? p2;
                lock (_runtimeLock)
                {
                    if (!_runtimeRooms.TryGetValue(roomId, out var runtime))
                        return;
                    p1 = runtime.Player1Handler;
                    p2 = runtime.Player2Handler;
                }

                if (p1 != null)
                    await p1.SendAsync(matchFoundPacket);
                if (p2 != null)
                    await p2.SendAsync(matchFoundPacket);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                lock (_runtimeLock)
                {
                    if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                    {
                        runtime.CountdownActive = false;
                        runtime.CountdownCts?.Dispose();
                        runtime.CountdownCts = null;
                    }
                }
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
                    Packet? player1MatchEnd = null;
                    Packet? player2MatchEnd = null;
                    ClientHandler? player1Handler = null;
                    ClientHandler? player2Handler = null;
                    int winnerId = -1;
                    int finalWinnerId = -1;

                    lock (_runtimeLock)
                    {
                        if (!_runtimeRooms.TryGetValue(roomId, out var runtime) || runtime.Simulation == null)
                            return;

                        var roomMeta = _roomStore.GetRoom(roomId);
                        if (roomMeta == null)
                            return;

                        player1Handler = runtime.Player1Handler;
                        player2Handler = runtime.Player2Handler;

                        if (runtime.RoundEndPending)
                        {
                            winnerId = runtime.PendingRoundWinnerId;
                            ApplyRoundState(runtime.Simulation.State, runtime);

                            broadcastCounter++;
                            if (broadcastCounter >= BroadcastEveryTicks)
                            {
                                broadcastCounter = 0;
                                packet = new WorldStatePacket { State = CopyState(runtime.Simulation.State) };
                            }

                            if (DateTime.UtcNow >= runtime.RoundEndReadyAtUtc)
                            {
                                if (runtime.PendingMatchWinnerId > 0)
                                {
                                    finalWinnerId = runtime.PendingMatchWinnerId;
                                    AssignMatchEndPackets(
                                        roomMeta,
                                        runtime,
                                        finalWinnerId,
                                        out player1MatchEnd,
                                        out player2MatchEnd);
                                }
                                else
                                {
                                    runtime.CurrentRound++;
                                    try
                                    {
                                        string? configRoot = ResolveConfigRoot();
                                        runtime.Simulation = BattleSimulation.Create(
                                            roomMeta.Player1Id,
                                            BattleCharacterCatalog.FromNetworkId(roomMeta.Player1CharId),
                                            roomMeta.Player2Id,
                                            BattleCharacterCatalog.FromNetworkId(roomMeta.Player2CharId),
                                            configRoot);
                                        ResetRoundEndState(runtime);
                                        ResetRoundTimer(runtime);
                                        ApplyRoundState(runtime.Simulation.State, runtime);
                                        packet = new WorldStatePacket { State = CopyState(runtime.Simulation.State) };
                                        broadcastCounter = 0;
                                        ServerLogger.Event("match", "next_round",
                                            ("room", roomId),
                                            ("round", runtime.CurrentRound),
                                            ("score", $"{runtime.Player1RoundWins}-{runtime.Player2RoundWins}"));
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerLogger.Error($"next round failed room={roomId}: {ex.Message}", "match");
                                        ServerLogger.Debug(ex.ToString(), "match");
                                        finalWinnerId = winnerId;
                                        AssignMatchEndPackets(
                                            roomMeta,
                                            runtime,
                                            finalWinnerId,
                                            out player1MatchEnd,
                                            out player2MatchEnd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            runtime.Simulation.Update(MatchTickSeconds);
                            UpdateRoundTimer(runtime, roomMeta);
                            ApplyRoundState(runtime.Simulation.State, runtime);

                            broadcastCounter++;
                            if (broadcastCounter >= BroadcastEveryTicks || runtime.Simulation.State.IsGameOver)
                            {
                                broadcastCounter = 0;
                                packet = new WorldStatePacket { State = CopyState(runtime.Simulation.State) };
                            }

                            if (runtime.Simulation.State.IsGameOver)
                            {
                                winnerId = runtime.Simulation.State.WinnerPlayerId;
                                bool player1WonRound = winnerId == roomMeta.Player1Id;
                                if (player1WonRound) runtime.Player1RoundWins++;
                                else runtime.Player2RoundWins++;

                                bool hasMatchWinner = runtime.Player1RoundWins >= RequiredRoundWins
                                    || runtime.Player2RoundWins >= RequiredRoundWins
                                    || runtime.CurrentRound >= MaxRounds;

                                runtime.RoundEndPending = true;
                                runtime.PendingRoundWinnerId = winnerId;
                                runtime.PendingMatchWinnerId = hasMatchWinner
                                    ? ResolveMatchWinner(roomMeta, runtime, winnerId)
                                    : 0;
                                runtime.RoundEndReadyAtUtc = DateTime.UtcNow.AddSeconds(GetRoundEndHoldSeconds(runtime.Simulation.State, winnerId));
                                ApplyRoundState(runtime.Simulation.State, runtime);
                                packet = new WorldStatePacket { State = CopyState(runtime.Simulation.State) };
                                broadcastCounter = 0;

                                ServerLogger.Event("match", "round_end",
                                    ("room", roomId),
                                    ("round", runtime.CurrentRound),
                                    ("winner", winnerId),
                                    ("score", $"{runtime.Player1RoundWins}-{runtime.Player2RoundWins}"),
                                    ("nextAt", runtime.RoundEndReadyAtUtc.ToString("O")));
                            }
                        }
                    }

                    if (packet != null)
                    {
                        if (player1Handler != null) await player1Handler.SendAsync(packet);
                        if (player2Handler != null) await player2Handler.SendAsync(packet);
                    }

                    if (player1MatchEnd != null || player2MatchEnd != null)
                    {
                        if (player1Handler != null && player1MatchEnd != null)
                            await player1Handler.SendAsync(player1MatchEnd);
                        if (player2Handler != null && player2MatchEnd != null)
                            await player2Handler.SendAsync(player2MatchEnd);

                        EndMatch(roomId, finalWinnerId > 0 ? finalWinnerId : winnerId);
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void EndMatch(int roomId, int winnerId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
                return;

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

            lock (_runtimeLock)
            {
                if (_runtimeRooms.TryGetValue(roomId, out var runtime))
                {
                    runtime.CountdownCts?.Cancel();
                    runtime.CountdownCts?.Dispose();
                    runtime.SimulationCts?.Cancel();
                    runtime.SimulationCts?.Dispose();
                }
                _runtimeRooms.Remove(roomId);
            }

            _matchRepo.Save(match);
            _roomStore.RemoveRoom(roomId);
        }

        private RoomData ToRoomData(RoomMeta meta, LocalRoomRuntime? runtime)
        {
            return new RoomData
            {
                RoomId = meta.RoomId,
                RoomName = meta.RoomName,
                Password = meta.Password,
                MapId = meta.MapId,
                TimeLimitMinutes = NormalizeTimeLimitMinutes(meta.TimeLimitMinutes),
                MatchStartTime = meta.MatchStartTime,
                OwnerId = meta.OwnerId,
                OwnerName = meta.OwnerName,
                Player1Id = meta.Player1Id,
                Player1Name = meta.Player1Name,
                Player1CharId = meta.Player1CharId,
                Player1Handler = runtime?.Player1Handler,
                Player2Id = meta.Player2Id,
                Player2Name = meta.Player2Name,
                Player2CharId = meta.Player2CharId,
                Player2Handler = runtime?.Player2Handler
            };
        }

        private static BattleState CopyState(BattleState state)
        {
            return new BattleState
            {
                ServerTick = state.ServerTick,
                RoundNumber = state.RoundNumber,
                Player1RoundWins = state.Player1RoundWins,
                Player2RoundWins = state.Player2RoundWins,
                RoundSecondsRemaining = state.RoundSecondsRemaining,
                RoundDurationSeconds = state.RoundDurationSeconds,
                IsSuddenDeath = state.IsSuddenDeath,
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
                    HitFrames = new List<int>(e.HitFrames),
                    DamagedFrames = new HashSet<int>(e.DamagedFrames),
                    Duration = e.Duration,
                    RemainingTime = e.RemainingTime,
                    FacingRight = e.FacingRight,
                    LastDamageTick = e.LastDamageTick,
                    Render = e.Render
                }).ToList(),
                IsGameOver = state.IsGameOver,
                WinnerPlayerId = state.WinnerPlayerId
            };
        }

        private static void ApplyRoundState(BattleState state, LocalRoomRuntime runtime)
        {
            state.RoundNumber = runtime.CurrentRound;
            state.Player1RoundWins = runtime.Player1RoundWins;
            state.Player2RoundWins = runtime.Player2RoundWins;
            state.RoundSecondsRemaining = Math.Max(0f, runtime.RoundSecondsRemaining);
            state.RoundDurationSeconds = runtime.SuddenDeathActive
                ? SuddenDeathDurationSeconds
                : runtime.RoundDurationSeconds;
            state.IsSuddenDeath = runtime.SuddenDeathActive;
        }

        private static int GetCurrentPlayerCount(RoomMeta room)
        {
            int count = 0;
            if (room.Player1Id != -1 && !(room.OwnerWaitingToJoin && room.Player1Id == room.OwnerId)) count++;
            if (room.Player2Id != -1) count++;
            return count;
        }

        private static void ResetRoundEndState(LocalRoomRuntime runtime)
        {
            runtime.RoundEndPending = false;
            runtime.RoundEndReadyAtUtc = default;
            runtime.PendingRoundWinnerId = 0;
            runtime.PendingMatchWinnerId = 0;
        }

        private static void ResetRoundTimer(LocalRoomRuntime runtime)
        {
            runtime.RoundSecondsRemaining = Math.Max(60f, runtime.RoundDurationSeconds);
            runtime.SuddenDeathActive = false;
        }

        private static int NormalizeTimeLimitMinutes(int minutes)
            => Math.Clamp(minutes <= 0 ? DefaultTimeLimitMinutes : minutes, MinTimeLimitMinutes, MaxTimeLimitMinutes);

        private static void UpdateRoundTimer(LocalRoomRuntime runtime, RoomMeta roomMeta)
        {
            if (runtime.Simulation == null || runtime.Simulation.State.IsGameOver)
                return;

            runtime.RoundSecondsRemaining = Math.Max(0f, runtime.RoundSecondsRemaining - MatchTickSeconds);
            if (runtime.RoundSecondsRemaining > 0f)
                return;

            int timedWinnerId = ResolveTimedRoundWinner(roomMeta, runtime.Simulation.State, allowFinalTieBreak: runtime.SuddenDeathActive);
            if (timedWinnerId > 0)
            {
                EndRoundByTimeout(runtime.Simulation.State, timedWinnerId);
                ServerLogger.Event("match", "timeout",
                    ("room", roomMeta.RoomId),
                    ("round", runtime.CurrentRound),
                    ("winner", timedWinnerId),
                    ("sudden", runtime.SuddenDeathActive));
                return;
            }

            runtime.SuddenDeathActive = true;
            runtime.RoundSecondsRemaining = SuddenDeathDurationSeconds;
            ServerLogger.Event("match", "sudden_death",
                ("room", roomMeta.RoomId),
                ("round", runtime.CurrentRound));
        }

        private static int ResolveTimedRoundWinner(RoomMeta roomMeta, BattleState state, bool allowFinalTieBreak)
        {
            float p1Ratio = GetHpRatio(state.Player1);
            float p2Ratio = GetHpRatio(state.Player2);

            if (p1Ratio > p2Ratio + HpRatioTieTolerance)
                return state.Player1.PlayerId;
            if (p2Ratio > p1Ratio + HpRatioTieTolerance)
                return state.Player2.PlayerId;

            if (!allowFinalTieBreak)
                return 0;

            if (state.Player1.Hp > state.Player2.Hp)
                return state.Player1.PlayerId;
            if (state.Player2.Hp > state.Player1.Hp)
                return state.Player2.PlayerId;

            return roomMeta.Player1Id;
        }

        private static float GetHpRatio(PlayerBattleState player)
        {
            int maxHp = Math.Max(1, player.Stats.Hp);
            return Math.Clamp(player.Hp / (float)maxHp, 0f, 1f);
        }

        private static void EndRoundByTimeout(BattleState state, int winnerId)
        {
            state.IsGameOver = true;
            state.WinnerPlayerId = winnerId;
        }

        private static double GetRoundEndHoldSeconds(BattleState state, int winnerId)
        {
            PlayerBattleState defeated = state.Player1.PlayerId == winnerId
                ? state.Player2
                : state.Player1;

            if (!defeated.IsDead)
                return PostDeathRoundDelaySeconds;

            double deadAnimationSeconds = 0.7;
            if (defeated.Stats.Animations.TryGetValue("Dead", out var meta))
            {
                deadAnimationSeconds = Math.Max(0.05, meta.FrameCount / Math.Max(1.0, meta.Fps));
            }

            return deadAnimationSeconds + PostDeathRoundDelaySeconds;
        }

        private static int ResolveMatchWinner(RoomMeta roomMeta, LocalRoomRuntime runtime, int roundWinnerId)
        {
            if (runtime.Player1RoundWins > runtime.Player2RoundWins)
                return roomMeta.Player1Id;
            if (runtime.Player2RoundWins > runtime.Player1RoundWins)
                return roomMeta.Player2Id;
            return roundWinnerId;
        }

        private static void AssignMatchEndPackets(
            RoomMeta roomMeta,
            LocalRoomRuntime runtime,
            int winnerId,
            out Packet player1Packet,
            out Packet player2Packet)
        {
            var victory = BuildVictoryPacket(roomMeta, runtime, winnerId);
            var gameOver = BuildGameOverPacket(roomMeta, runtime, winnerId);

            if (winnerId == roomMeta.Player1Id)
            {
                player1Packet = victory;
                player2Packet = gameOver;
            }
            else
            {
                player1Packet = gameOver;
                player2Packet = victory;
            }
        }

        private static VictoryPacket BuildVictoryPacket(RoomMeta roomMeta, LocalRoomRuntime runtime, int winnerId)
        {
            return new VictoryPacket
            {
                WinnerPlayerId = winnerId,
                Duration = roomMeta.MatchStartTime.HasValue
                    ? (int)(DateTime.UtcNow - roomMeta.MatchStartTime.Value).TotalSeconds
                    : 0,
                FinalRound = runtime.CurrentRound,
                Player1RoundWins = runtime.Player1RoundWins,
                Player2RoundWins = runtime.Player2RoundWins
            };
        }

        private static GameOverPacket BuildGameOverPacket(RoomMeta roomMeta, LocalRoomRuntime runtime, int winnerId)
        {
            return new GameOverPacket
            {
                WinnerPlayerId = winnerId,
                Duration = roomMeta.MatchStartTime.HasValue
                    ? (int)(DateTime.UtcNow - roomMeta.MatchStartTime.Value).TotalSeconds
                    : 0,
                FinalRound = runtime.CurrentRound,
                Player1RoundWins = runtime.Player1RoundWins,
                Player2RoundWins = runtime.Player2RoundWins
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
            // Server should resolve battle config from its own runtime directory only.
            // This keeps online behavior deterministic and independent from client/source tree.
            string root = AppContext.BaseDirectory;
            return Directory.Exists(Path.Combine(root, "Config", "Characters"))
                ? root
                : null;
        }
    }
}
