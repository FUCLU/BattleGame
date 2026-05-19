using Microsoft.Extensions.Configuration;

namespace BattleGame.Server.Config
{
    public enum SmtpMode
    {
        Mailpit,
        Real
    }

    public class SmtpConfig
    {
        public SmtpMode Mode { get; init; } = SmtpMode.Mailpit;
        public string Host { get; init; } = "localhost";
        public int Port { get; init; } = 1025;
        public string FromEmail { get; init; } = "no-reply@battlegame.local";
        public string FromName { get; init; } = "BattleGame";
        public bool EnableSsl { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;

        public bool HasCredentials =>
            !string.IsNullOrWhiteSpace(Username) &&
            !string.IsNullOrWhiteSpace(Password);

        public static SmtpConfig Load(IConfiguration config)
        {
            var mode = ParseMode(ReadString(config, "SMTP_MODE", "Smtp:Mode") ?? "Mailpit");

            return mode switch
            {
                SmtpMode.Real => LoadReal(config),
                _ => LoadMailpit(config)
            };
        }

        private static SmtpConfig LoadMailpit(IConfiguration config)
        {
            return new SmtpConfig
            {
                Mode = SmtpMode.Mailpit,
                Host = ReadString(config, "SMTP_MAILPIT_HOST", "Smtp:Mailpit:Host") ?? "localhost",
                Port = ReadInt(config, 1025, "SMTP_MAILPIT_PORT", "Smtp:Mailpit:Port"),
                FromEmail = ReadString(config, "SMTP_MAILPIT_FROM_EMAIL", "Smtp:Mailpit:FromEmail", "SMTP_FROM_EMAIL", "Smtp:FromEmail")
                    ?? "no-reply@battlegame.local",
                FromName = ReadString(config, "SMTP_FROM_NAME", "Smtp:FromName") ?? "BattleGame",
                EnableSsl = ReadBool(config, false, "SMTP_MAILPIT_ENABLE_SSL", "Smtp:Mailpit:EnableSsl")
            };
        }

        private static SmtpConfig LoadReal(IConfiguration config)
        {
            var username = Required(ReadString(config, "SMTP_REAL_USERNAME", "Smtp:Real:Username"), "SMTP_REAL_USERNAME");

            return new SmtpConfig
            {
                Mode = SmtpMode.Real,
                Host = Required(ReadString(config, "SMTP_REAL_HOST", "Smtp:Real:Host"), "SMTP_REAL_HOST"),
                Port = ReadInt(config, 587, "SMTP_REAL_PORT", "Smtp:Real:Port"),
                FromEmail = ReadString(config, "SMTP_REAL_FROM_EMAIL", "Smtp:Real:FromEmail", "SMTP_FROM_EMAIL", "Smtp:FromEmail")
                    ?? username,
                FromName = ReadString(config, "SMTP_FROM_NAME", "Smtp:FromName") ?? "BattleGame",
                EnableSsl = ReadBool(config, true, "SMTP_REAL_ENABLE_SSL", "Smtp:Real:EnableSsl"),
                Username = username,
                Password = Required(ReadString(config, "SMTP_REAL_PASSWORD", "Smtp:Real:Password"), "SMTP_REAL_PASSWORD")
            };
        }

        private static SmtpMode ParseMode(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "mailpit" or "local" or "dev" => SmtpMode.Mailpit,
                "real" or "smtp" or "email" or "gmail" => SmtpMode.Real,
                _ => throw new InvalidOperationException("SMTP_MODE must be Mailpit or Real.")
            };
        }

        private static string Required(string? value, string key)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            throw new InvalidOperationException($"{key} is required when SMTP_MODE=Real.");
        }

        private static string? ReadString(IConfiguration config, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = config[key];
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return null;
        }

        private static int ReadInt(IConfiguration config, int defaultValue, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (int.TryParse(config[key], out var value))
                    return value;
            }

            return defaultValue;
        }

        private static bool ReadBool(IConfiguration config, bool defaultValue, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (bool.TryParse(config[key], out var value))
                    return value;
            }

            return defaultValue;
        }
    }
}
