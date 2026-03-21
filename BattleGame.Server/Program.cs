using Microsoft.Extensions.Configuration;
using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Server.Logging;

// ── Đọc config từ appsettings.json ──
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var serverConfig = config.GetSection("Server").Get<ServerConfig>() ?? new ServerConfig();
var connStr = config.GetConnectionString("DefaultConnection")
                   ?? throw new Exception("Thiếu DefaultConnection trong appsettings.json");
var smtpConfig = config.GetSection("Smtp").Get<SmtpConfig>() ?? new SmtpConfig();

// Override bằng env var khi chạy trong Docker
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_CONNECTION")))
    connStr = Environment.GetEnvironmentVariable("DB_CONNECTION")!;

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SMTP_HOST")))
{
    smtpConfig.Host = Environment.GetEnvironmentVariable("SMTP_HOST")!;
    smtpConfig.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "1025");
    smtpConfig.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
    smtpConfig.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
    smtpConfig.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL") ?? "false");
}

// ── Khởi tạo DB ──
ServerLogger.Info("Initializing database...");
DbInitializer.Initialize(connStr);
ServerLogger.Info("Database ready.");

// ── Khởi tạo OTP stack ──
var emailService = new EmailService(smtpConfig);
var otpRepository = new OtpRepository(connStr);
var otpService = new OtpService(otpRepository, emailService);

// ── Start server ──
ServerLogger.Info("BattleGame Server Starting...");
var server = new GameServer(serverConfig.Port, connStr, otpService);
ServerLogger.Info($"Server listening on port {serverConfig.Port}");
server.Start();

await Task.Delay(Timeout.Infinite);