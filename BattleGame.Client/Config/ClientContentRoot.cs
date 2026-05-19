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

        while (!string.IsNullOrWhiteSpace(current))
        {
            bool hasAssets = Directory.Exists(Path.Combine(current, "Assets"));
            bool hasConfig = Directory.Exists(Path.Combine(current, "Config"));

            if (hasAssets && hasConfig)
            {
                // Prefer the nearest runnable root (usually bin/publish) to avoid
                // loading stale source assets/config when running online/deployed builds.
                return current;
            }

            var parent = Directory.GetParent(current);
            if (parent == null)
                break;

            current = parent.FullName;
        }

        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
    }
}
