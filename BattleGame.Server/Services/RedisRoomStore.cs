using StackExchange.Redis;
using System.Text.Json;

namespace BattleGame.Server.Services
{
    public class RedisRoomStore
    {
        private readonly IDatabase _db;
        private readonly string _prefix;

        public RedisRoomStore(IConnectionMultiplexer redis, string prefix = "battlegame")
        {
            _db = redis.GetDatabase();
            _prefix = prefix;
        }

        public bool CreateRoom(RoomMeta room)
        {
            string key = RoomKey(room.RoomId);
            if (_db.KeyExists(key))
                return false;

            _db.StringSet(key, JsonSerializer.Serialize(room));
            _db.SetAdd(RoomSetKey(), room.RoomId);
            _db.StringSet(RoomServerKey(room.RoomId), room.ServerId);
            return true;
        }

        public RoomMeta? GetRoom(int roomId)
        {
            var value = _db.StringGet(RoomKey(roomId));
            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<RoomMeta>(value!);
        }

        public void SaveRoom(RoomMeta room)
        {
            _db.StringSet(RoomKey(room.RoomId), JsonSerializer.Serialize(room));
            _db.SetAdd(RoomSetKey(), room.RoomId);
        }

        public bool RemoveRoom(int roomId)
        {
            _db.SetRemove(RoomSetKey(), roomId);
            _db.KeyDelete(RoomServerKey(roomId));
            return _db.KeyDelete(RoomKey(roomId));
        }

        public List<RoomMeta> GetAllRooms()
        {
            var ids = _db.SetMembers(RoomSetKey());
            var list = new List<RoomMeta>();
            foreach (var id in ids)
            {
                if (!int.TryParse(id.ToString(), out int roomId))
                    continue;

                var room = GetRoom(roomId);
                if (room != null)
                {
                    list.Add(room);
                    continue;
                }

                _db.SetRemove(RoomSetKey(), id);
                _db.KeyDelete(RoomServerKey(roomId));
            }

            return list;
        }

        public string? GetRoomServerId(int roomId)
        {
            var value = _db.StringGet(RoomServerKey(roomId));
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        public void SetUserAffinity(int userId, string serverId)
        {
            _db.StringSet(UserServerKey(userId), serverId, TimeSpan.FromHours(12));
        }

        public string? GetUserAffinity(int userId)
        {
            var value = _db.StringGet(UserServerKey(userId));
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        private string RoomKey(int roomId) => $"{_prefix}:room:{roomId}";
        private string RoomSetKey() => $"{_prefix}:rooms";
        private string RoomServerKey(int roomId) => $"{_prefix}:room:{roomId}:server";
        private string UserServerKey(int userId) => $"{_prefix}:user:{userId}:server";
    }

    public class RoomMeta
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int MapId { get; set; } = -1;
        public int TimeLimitMinutes { get; set; } = 3;
        public DateTime? MatchStartTime { get; set; }
        public string ServerId { get; set; } = string.Empty;
        public int OwnerId { get; set; } = -1;
        public string OwnerName { get; set; } = string.Empty;
        public int Player1Id { get; set; } = -1;
        public string Player1Name { get; set; } = string.Empty;
        public int Player1CharId { get; set; } = -1;
        public int Player2Id { get; set; } = -1;
        public string Player2Name { get; set; } = string.Empty;
        public int Player2CharId { get; set; } = -1;
    }
}
