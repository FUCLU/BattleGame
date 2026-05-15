using System;
using System.IO;
using System.Text.Json;

namespace BattleGame.Client.Game.Dungeon;

public static class DungeonContentLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static DungeonDefinition? TryLoadDefinition(string clientRoot, string mapFolderName)
    {
        string path = Path.Combine(clientRoot, "Assets", "dungeon", mapFolderName, "encounter.json");
        if (!File.Exists(path))
            return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DungeonDefinition>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dungeon] Failed to load encounter {path}: {ex.Message}");
            return null;
        }
    }

    public static DungeonMonsterCatalog? TryLoadMonsterCatalog(string clientRoot)
    {
        string path = Path.Combine(clientRoot, "Assets", "dungeon", "monster", "catalog.json");
        if (!File.Exists(path))
            return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DungeonMonsterCatalog>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dungeon] Failed to load monster catalog {path}: {ex.Message}");
            return null;
        }
    }
}
