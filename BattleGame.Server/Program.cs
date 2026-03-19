using BattleGame.Server.Network;
using BattleGame.Server.Config;

Console.WriteLine("=== BATTLEGAME SERVER STARTING ===");

var config = ServerConfig.Load();
var server = new GameServer(config.Port);

Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    Console.WriteLine("[INFO] Shutting down...");
    server.Stop();
};

Console.WriteLine($"[INFO] Server listening on port {config.Port}");
server.Start();
