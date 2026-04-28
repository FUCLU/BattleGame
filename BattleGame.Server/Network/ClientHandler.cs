using System.Net.Sockets;
using System.Text.Json;
using BattleGame.Server.Database;
using BattleGame.Server.Game;
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
                        Console.WriteLine($"[ERROR] IOException: {ex.Message}");
                        break;
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"[ERROR] JsonException: {ex.Message}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Unexpected: {ex.Message}");
                        break;
                    }
                }
            }
            finally
            {
                Console.WriteLine($"[INFO] Client disconnected");
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