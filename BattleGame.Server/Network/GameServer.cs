using BattleGame.Server.Logging;
using BattleGame.Server.Services;

public class GameServer
{
    private readonly int _port;
    private readonly string _connectionString;
    private readonly OtpService _otpService;

    public int Port => _port;

    public GameServer(int port, string connectionString, OtpService otpService)
    {
        _port = port;
        _connectionString = connectionString;
        _otpService = otpService;
    }

    public void Start()
    {
        ServerLogger.Info($"Server started on port {Port}");
    }
}
