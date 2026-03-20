using BattleGame.Server.Network;
using BattleGame.Server.Logging;

ServerLogger.Info("BattleGame Server Starting...");

var server = new GameServer();

ServerLogger.Info($"Server listening on port {server.Port}");

server.Start();

await Task.Delay(Timeout.Infinite);