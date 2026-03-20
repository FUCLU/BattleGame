using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Server.Logging;

public static class ServerLogger
{
    public static void Info(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    public static void Error(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }

    public static void Warn(string message)
    {
        Console.WriteLine($"[WARN] {message}");
    }
}