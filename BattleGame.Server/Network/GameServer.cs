using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Server.Game;
using BattleGame.Server.Logging;
using System.Net;
using System.Net.Sockets;
using StackExchange.Redis;

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
            ServerLogger.Event("network", "listen",
                ("port", _config.Port),
                ("serverId", _config.ServerId),
                ("public", $"{_config.PublicHost}:{_config.PublicPort}"));

            var userRepo = new UserRepository(_config.ConnectionString);
            var otpRepo = new OtpRepository(_config.ConnectionString);
            var emailSvc = new EmailService(_config.Smtp);
            var otpSvc = new OtpService(otpRepo, emailSvc);
            var authSvc = new AuthService(userRepo);
            ServerLogger.Event("mail", "smtp_config",
                ("mode", _config.Smtp.Mode),
                ("host", $"{_config.Smtp.Host}:{_config.Smtp.Port}"),
                ("ssl", _config.Smtp.EnableSsl));

            var matchRepo = new MatchRepository(_config.ConnectionString);
            var redis = await ConnectionMultiplexer.ConnectAsync(_config.RedisConnection);
            var roomStore = new RedisRoomStore(redis);
            var matchmaking = new MatchmakingService(matchRepo, roomStore, _config);
            var heartbeat = new RedisServerRegistry(_config, redis);
            _ = Task.Run(() => heartbeat.StartHeartbeatAsync(CancellationToken.None));

            ServerLogger.Info("services initialized, waiting for connections", "startup");

            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    var handler = new ClientHandler(client, authSvc, otpSvc, userRepo, matchRepo, matchmaking);
                    ServerLogger.Event("client", "connected", ("remote", client.Client.RemoteEndPoint));
                    _ = Task.Run(() => handler.HandleAsync()); 
                }
                catch (Exception ex)
                {
                    ServerLogger.Error($"accept failed: {ex.Message}", "network");
                }
            }
        }
    }
}
