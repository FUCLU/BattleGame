using System;
using System.Collections.Generic;
using System.Text;
using BattleGame.Server.Network;

namespace BattleGame.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Battle Game Server Starting...");

            GameServer server = new GameServer();
            server.Start();

            Console.ReadLine();
        }
    }
}
