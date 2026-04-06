using Microsoft.Extensions.Configuration;

namespace BattleGame.Server.Config
{
    public class ServerConfig
    {
        public int Port { get; private set; }
        public string ConnectionString { get; private set; } = string.Empty;
        public SmtpConfig Smtp { get; private set; } = new();

        public void Load(IConfiguration config)  
        {
            Port = config.GetValue<int?>("SERVER_PORT")
                ?? config.GetValue<int?>("Server:Port")
                ?? 9000;
            ConnectionString =
                config["DB_CONNECTION"]
                ?? config.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=battlegame;Username=postgres;Password=battlegame123";
            Smtp = new SmtpConfig
            {
                Host = config["SMTP_HOST"] ?? config["Smtp:Host"] ?? "localhost",
                Port = config.GetValue<int?>("SMTP_PORT")
                           ?? config.GetValue<int?>("Smtp:Port")
                           ?? 1025,
                Username = config["SMTP_USERNAME"] ?? config["Smtp:Username"] ?? "",
                Password = config["SMTP_PASSWORD"] ?? config["Smtp:Password"] ?? "",
                FromName = config["SMTP_FROM_NAME"] ?? config["Smtp:FromName"] ?? "BattleGame",
                EnableSsl = config.GetValue<bool?>("SMTP_ENABLE_SSL")
                            ?? config.GetValue<bool?>("Smtp:EnableSsl")
                            ?? false
            };
        }
    }

    public class SmtpConfig
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 1025;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromName { get; set; } = "BattleGame";
        public bool EnableSsl { get; set; } = false;
    }
}