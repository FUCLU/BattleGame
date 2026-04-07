using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Server.Game;
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

        public async Task StartAsync()
        {
            var listener = new TcpListener(IPAddress.Any, _config.Port);
            listener.Start();
            Console.WriteLine($"[INFO] Server started on port {_config.Port}");

            var userRepo = new UserRepository(_config.ConnectionString);
            var otpRepo = new OtpRepository(_config.ConnectionString);
            var emailSvc = new EmailService(_config.Smtp);
            var otpSvc = new OtpService(otpRepo, emailSvc);
            var authSvc = new AuthService(userRepo);

            var gameManager = new GameManager();
            var matchmaking = new MatchmakingService();

            Console.WriteLine("[INFO] Services initialized, waiting for connections...");

            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    var handler = new ClientHandler(client, authSvc, otpSvc, userRepo);
                    Console.WriteLine($"[INFO] Client connected: {client.Client.RemoteEndPoint}");
                    _ = Task.Run(() => handler.HandleAsync()); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Accept Error: {ex.Message}");
                }
            }
        }
    }
}