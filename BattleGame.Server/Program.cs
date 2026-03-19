using BattleGame.Server.Network;

Console.WriteLine("[INFO] === BattleGame Server Starting ===");
Console.WriteLine("[INFO] Server listening on port 9002");

var server = new GameServer();

Console.CancelKeyPress += (_, e) => {
    e.Cancel = true;
    Console.WriteLine("[INFO] Server shutting down...");
};

server.Start();

await Task.Delay(Timeout.Infinite);
