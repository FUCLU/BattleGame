using BattleGame.Server.Config;
using StackExchange.Redis;

namespace BattleGame.Server.Services
{
    public class RedisServerRegistry
    {
        private readonly ServerConfig _config;
        private readonly IDatabase _db;
        private readonly string _prefix;

        public RedisServerRegistry(ServerConfig config, IConnectionMultiplexer redis, string prefix = "battlegame")
        {
            _config = config;
            _db = redis.GetDatabase();
            _prefix = prefix;
        }

        public async Task StartHeartbeatAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await WriteHeartbeatAsync();
                await Task.Delay(TimeSpan.FromSeconds(_config.HeartbeatIntervalSeconds), token);
            }
        }

        private async Task WriteHeartbeatAsync()
        {
            string key = $"{_prefix}:server:{_config.ServerId}";
            var values = new HashEntry[]
            {
                new HashEntry("serverId", _config.ServerId),
                new HashEntry("host", _config.PublicHost),
                new HashEntry("port", _config.PublicPort),
                new HashEntry("updatedAtUnix", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _db.HashSetAsync(key, values);
            await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(_config.HeartbeatTtlSeconds));
            await _db.SetAddAsync($"{_prefix}:servers", _config.ServerId);
        }
    }
}
