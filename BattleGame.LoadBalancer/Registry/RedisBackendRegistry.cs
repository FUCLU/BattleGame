using StackExchange.Redis;

namespace BattleGame.LoadBalancer.Registry
{
    public class RedisBackendRegistry
    {
        private readonly IDatabase _db;
        private readonly string _setKey;
        private readonly string _backendPrefix;

        public RedisBackendRegistry(IConnectionMultiplexer redis, string prefix)
        {
            _db = redis.GetDatabase();
            _setKey = $"{prefix}:backends";
            _backendPrefix = $"{prefix}:backend:";
        }

        public async Task SeedAsync(IEnumerable<ServerEndpoint> servers)
        {
            foreach (var server in servers.Where(s => s.IsValid()))
            {
                await UpsertAsync(server, true);
            }
        }

        public async Task UpsertAsync(ServerEndpoint endpoint, bool isHealthy)
        {
            string key = BackendKey(endpoint.Key);
            HashEntry[] values =
            {
                new HashEntry("host", endpoint.Host),
                new HashEntry("serverId", endpoint.ServerId),
                new HashEntry("port", endpoint.Port),
                new HashEntry("publicHost", endpoint.PublicHost),
                new HashEntry("publicPort", endpoint.PublicPort),
                new HashEntry("healthy", isHealthy ? 1 : 0),
                new HashEntry("updatedAtUnix", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _db.HashSetAsync(key, values);
            await _db.SetAddAsync(_setKey, endpoint.Key);
        }

        public async Task SetHealthAsync(ServerEndpoint endpoint, bool isHealthy)
        {
            string key = BackendKey(endpoint.Key);
            await _db.HashSetAsync(key, new HashEntry[]
            {
                new HashEntry("healthy", isHealthy ? 1 : 0),
                new HashEntry("updatedAtUnix", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            });
        }

        public async Task<List<ServerEndpoint>> GetAllAsync()
        {
            RedisValue[] ids = await _db.SetMembersAsync(_setKey);
            var result = new List<ServerEndpoint>();
            foreach (var id in ids)
            {
                var endpoint = await GetByIdAsync(id.ToString());
                if (endpoint != null)
                    result.Add(endpoint);
            }

            return result;
        }

        public async Task<List<ServerEndpoint>> GetHealthyAsync()
        {
            RedisValue[] ids = await _db.SetMembersAsync(_setKey);
            var result = new List<ServerEndpoint>();
            foreach (var id in ids)
            {
                string endpointId = id.ToString();
                string key = BackendKey(endpointId);
                var entries = await _db.HashGetAllAsync(key);
                if (entries.Length == 0)
                    continue;

                bool healthy = ParseInt(entries, "healthy") == 1;
                if (!healthy)
                    continue;

                var endpoint = ToEndpoint(entries);
                if (endpoint != null)
                    result.Add(endpoint);
            }

            return result;
        }

        public async Task<string?> ResolvePreferredServerIdAsync(int? userId, int? roomId)
        {
            if (roomId.HasValue && roomId.Value > 0)
            {
                var roomServer = await _db.StringGetAsync($"battlegame:room:{roomId.Value}:server");
                if (!roomServer.IsNullOrEmpty)
                    return roomServer.ToString();
            }

            if (userId.HasValue && userId.Value > 0)
            {
                var userServer = await _db.StringGetAsync($"battlegame:user:{userId.Value}:server");
                if (!userServer.IsNullOrEmpty)
                    return userServer.ToString();
            }

            return null;
        }

        private async Task<ServerEndpoint?> GetByIdAsync(string id)
        {
            var entries = await _db.HashGetAllAsync(BackendKey(id));
            if (entries.Length == 0)
                return null;

            return ToEndpoint(entries);
        }

        private ServerEndpoint? ToEndpoint(HashEntry[] entries)
        {
            string host = ReadString(entries, "host");
            string serverId = ReadString(entries, "serverId");
            string publicHost = ReadString(entries, "publicHost");
            int port = ParseInt(entries, "port");
            int publicPort = ParseInt(entries, "publicPort");

            var endpoint = new ServerEndpoint
            {
                Host = host,
                ServerId = serverId,
                Port = port,
                PublicHost = publicHost,
                PublicPort = publicPort
            };

            return endpoint.IsValid() ? endpoint : null;
        }

        private string BackendKey(string endpointId) => $"{_backendPrefix}{endpointId}";

        private static string ReadString(HashEntry[] entries, string field)
        {
            foreach (var entry in entries)
            {
                if (entry.Name == field)
                    return entry.Value.ToString();
            }

            return string.Empty;
        }

        private static int ParseInt(HashEntry[] entries, string field)
        {
            foreach (var entry in entries)
            {
                if (entry.Name != field)
                    continue;

                if (int.TryParse(entry.Value.ToString(), out int value))
                    return value;
            }

            return 0;
        }
    }
}
