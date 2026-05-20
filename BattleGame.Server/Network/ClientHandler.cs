using System.Net.Sockets;
using System.Text.Json;
using BattleGame.Server.Database;
using BattleGame.Server.Game;
using BattleGame.Server.Logging;
using BattleGame.Server.Services;
using BattleGame.Shared.Network;
using BattleGame.Shared.Packets;

namespace BattleGame.Server.Network
{
    public class ClientHandler
    {
        private readonly ServerSocket _socket;
        private readonly PacketProcessor _processor;

        // Session
        public int UserId { get; set; } = -1;
        public string Username { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
        public string? CurrentRoomId { get; set; } = null;


        public ClientHandler(
            TcpClient tcpClient,
            AuthService authService,
            OtpService otpService,
            UserRepository userRepo,
            MatchRepository matchRepo,
            MatchmakingService matchmaking)
        {
            _socket = new ServerSocket(tcpClient);
            _processor = new PacketProcessor(this, authService, otpService, userRepo, matchRepo, matchmaking);
        }

        public async Task HandleAsync()
        {
            try
            {
                while (_socket.IsConnected())
                {
                    try
                    {
                        string json = await _socket.ReceiveAsync();
                        var packet = PacketSerializer.Deserialize(json);
                        await _processor.ProcessAsync(packet);
                    }
                    catch (IOException ex)
                    {
                        // Load balancer health-check opens/closes short-lived sockets without sending packets.
                        // Avoid noisy error logs for unauthenticated transient disconnects.
                        if (IsAuthenticated)
                            ServerLogger.Warn($"io disconnected: {ex.Message}", "client");
                        else
                            ServerLogger.Debug($"closed before auth: {ex.Message}", "client");
                        break;
                    }
                    catch (JsonException ex)
                    {
                        ServerLogger.Warn($"invalid json: {ex.Message}", "client");
                        break;
                    }
                    catch (Exception ex)
                    {
                        ServerLogger.Error($"unexpected: {ex.Message}", "client");
                        break;
                    }
                }
            }
            finally
            {
                await _processor.HandleClientDisconnectAsync();
                ServerLogger.Event("client", "disconnected", ("user", UserId), ("room", CurrentRoomId ?? "-"));
                _socket.Close();
            }
        }

        // Public để PacketProcessor gọi được
        public async Task SendAsync(Packet packet)
        {
            string json = PacketSerializer.Serialize(packet);
            await _socket.SendAsync(json);
        }
    }
}
