using BattleGame.Server.Database;
using BattleGame.Server.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Server.Services
{
    public class MatchmakingService
    {
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
                if (_rooms.ContainsKey(roomId))
                    _rooms[roomId].MatchStartTime = DateTime.UtcNow;
            }
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
                _matchRepo.Save(match);
                _rooms.Remove(roomId);
            }
        }
    }
}
