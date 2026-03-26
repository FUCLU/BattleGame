using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Logging;
using BattleGame.Server.Services;
using System.Net;
using System.Net.Sockets;

namespace BattleGame.Server.Network
{
    public class GameServer
    {
        private readonly ServerConfig _config;
        public GameServer(ServerConfig config)
        {
            _config = config;
        }
        public void Start()
        {
            var listener = new TcpListener(IPAddress.Any, _config.Port);
            listener.Start();
            var userRepo = new UserRepository(_config.ConnectionString);
            var otpRepo = new OtpRepository(_config.ConnectionString);
            var emailSvc = new EmailService(_config.Smtp);
            var otpSvc = new OtpService(otpRepo, emailSvc);
            var authSvc = new AuthService(userRepo);
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    var handler = new ClientHandler(client, authSvc, otpSvc, userRepo);
                    Task.Run(() => handler.Handle());
                }
                catch (Exception ex)
                {
                    ServerLogger.Error($"Accept Error: {ex.Message}");
                }
            }
        }
    }
}