using Microsoft.Extensions.Configuration;

namespace BattleGame.Server.Config
{
    public class ServerConfig
    {
        public int Port { get; private set; }
        public string ServerId { get; private set; } = "server-1";
        public string PublicHost { get; private set; } = "127.0.0.1";
        public int PublicPort { get; private set; } = 9001;
        public string RedisConnection { get; private set; } = "localhost:6379";
        public int HeartbeatIntervalSeconds { get; private set; } = 2;
        public int HeartbeatTtlSeconds { get; private set; } = 6;
        public string ConnectionString { get; private set; } = string.Empty;
        public SmtpConfig Smtp { get; private set; } = new();

        public void Load(IConfiguration config)  
        {
            Port = config.GetValue<int?>("SERVER_PORT")
                ?? config.GetValue<int?>("Server:Port")
                ?? 9000;
            ServerId = config["SERVER_ID"]
                ?? config["Server:ServerId"]
                ?? $"server-{Port}";
            PublicHost = config["SERVER_PUBLIC_HOST"]
                ?? config["Server:PublicHost"]
                ?? "127.0.0.1";
            PublicPort = config.GetValue<int?>("SERVER_PUBLIC_PORT")
                ?? config.GetValue<int?>("Server:PublicPort")
                ?? Port;
            RedisConnection = config["REDIS_CONNECTION"]
                ?? config["Server:RedisConnection"]
                ?? "localhost:6379";
            HeartbeatIntervalSeconds = config.GetValue<int?>("HEARTBEAT_INTERVAL_SECONDS")
                ?? config.GetValue<int?>("Server:HeartbeatIntervalSeconds")
                ?? 2;
            HeartbeatTtlSeconds = config.GetValue<int?>("HEARTBEAT_TTL_SECONDS")
                ?? config.GetValue<int?>("Server:HeartbeatTtlSeconds")
                ?? 6;
            ConnectionString =
                config["DB_CONNECTION"]
                ?? config.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5433;Database=battlegame;Username=admin;Password=admin";
            Smtp = SmtpConfig.Load(config);
        }
    }
}
