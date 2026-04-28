using BattleGame.Server.Database;
using BattleGame.Server.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Server.Services
{
    public class MatchmakingService
    {
        private readonly MatchRepository _matchRepo;
        private readonly Dictionary<int, RoomData> _rooms = new();
        private int _nextRoomId = new Random().Next(100000, 999999);

        public class RoomData
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int MapId { get; set; } = -1;
            public DateTime? MatchStartTime { get; set; }

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
            int roomId = _nextRoomId++;
            var room = new RoomData
            {
                RoomId = roomId,
                RoomName = roomName,
                Password = password,
                Player1Id = player1Id,
                Player1Name = player1Name,
                Player1Handler = handler
            };
            _rooms[roomId] = room;
            return (roomId, true);
        }

        public bool JoinRoom(int roomId, string password, int player2Id, string player2Name, ClientHandler handler)
        {
            if (!_rooms.ContainsKey(roomId))
                return false;
            var room = _rooms[roomId];
            if (room.Password != password)
                return false;
            room.Player2Id = player2Id;
            room.Player2Name = player2Name;
            room.Player2Handler = handler;
            return true;
        }

        public RoomData? GetRoom(int roomId)
        {
            return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
        }

        public List<RoomInfo> GetRooms()
        {
            return _rooms.Values
                .Where(r => r.Player2Id == -1)
                .Select(r => new RoomInfo
                {
                    RoomId = r.RoomId,
                    RoomName = r.RoomName,
                    CurrentPlayers = 1
                })
                .ToList();
        }

        public void SetMap(int roomId, int mapId)
        {
            if (_rooms.ContainsKey(roomId))
                _rooms[roomId].MapId = mapId;
        }

        public void SetCharacter(int roomId, int playerId, int charId)
        {
            if (!_rooms.ContainsKey(roomId))
                return;
            var room = _rooms[roomId];
            if (room.Player1Id == playerId)
                room.Player1CharId = charId;
            else if (room.Player2Id == playerId)
                room.Player2CharId = charId;
        }

        public bool AreAllReady(int roomId)
        {
            if (!_rooms.ContainsKey(roomId))
                return false;
            var room = _rooms[roomId];
            return room.Player1CharId != -1 && room.Player2CharId != -1 && room.MapId != -1;
        }

        public MatchFoundPacket? BuildMatchFoundPacket(int roomId)
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

        public void StartMatch(int roomId)
        {
            if (_rooms.ContainsKey(roomId))
                _rooms[roomId].MatchStartTime = DateTime.UtcNow;
        }

        public void EndMatch(int roomId, int winnerId)
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
