using System;
using System.Collections.Generic;
using System.Text;
using BattleGame.Server.Logging;

public class GameServer
{
    public int Port => 9000;

    public void Start()
    {
        ServerLogger.Info($"Server started on port {Port}");
    }
}
