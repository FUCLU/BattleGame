using System;
using System.IO;

namespace BattleGame.Client.Config;

public static class ClientContentRoot
{
    public static string Resolve(string startDirectory)
    {
        string current = string.IsNullOrWhiteSpace(startDirectory)
            ? AppDomain.CurrentDomain.BaseDirectory
            : startDirectory;

        string? firstRunnableRoot = null;
        while (!string.IsNullOrWhiteSpace(current))
        {
            bool hasAssets = Directory.Exists(Path.Combine(current, "Assets"));
            bool hasConfig = Directory.Exists(Path.Combine(current, "Config"));

            if (hasAssets && hasConfig)
            {
                if (File.Exists(Path.Combine(current, "BattleGame.Client.csproj")))
                    return current;

                firstRunnableRoot ??= current;
            }

            var parent = Directory.GetParent(current);
            if (parent == null)
                break;

            current = parent.FullName;
        }

        return firstRunnableRoot
            ?? Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
    }
}
