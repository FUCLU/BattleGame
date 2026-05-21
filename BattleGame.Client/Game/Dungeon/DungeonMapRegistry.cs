using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleGame.Client.Game.Dungeon;

public sealed class DungeonParallaxLayerDefinition
{
    public DungeonParallaxLayerDefinition(string fileName, float speed)
    {
        FileName = fileName;
        Speed = speed;
    }

    public string FileName { get; }
    public float Speed { get; }
}

public sealed class DungeonMapDefinition
{
    public DungeonMapDefinition(
        string mapId,
        string displayName,
        string defaultCharacterId,
        string folderName,
        float worldWidth,
        string previewFileName,
        IReadOnlyList<DungeonParallaxLayerDefinition> layers,
        string? foregroundFileName)
    {
        MapId = mapId;
        DisplayName = displayName;
        DefaultCharacterId = defaultCharacterId;
        FolderName = folderName;
        WorldWidth = worldWidth;
        PreviewFileName = previewFileName;
        Layers = layers;
        ForegroundFileName = foregroundFileName;
    }

    public string MapId { get; }
    public string DisplayName { get; }
    public string DefaultCharacterId { get; }
    public string FolderName { get; }
    public float WorldWidth { get; }
    public string PreviewFileName { get; }
    public IReadOnlyList<DungeonParallaxLayerDefinition> Layers { get; }
    public string? ForegroundFileName { get; }
}

public static class DungeonMapRegistry
{
    private static readonly IReadOnlyList<DungeonMapDefinition> Maps = new List<DungeonMapDefinition>
    {
        new(
            mapId: "map1",
            displayName: "Cursed Stalactites",
            defaultCharacterId: "haladin",
            folderName: "map1",
            worldWidth: 8000f,
            previewFileName: "background.png",
            layers: new[]
            {
                new DungeonParallaxLayerDefinition("plan5.png", 0.10f),
                new DungeonParallaxLayerDefinition("plan4.png", 0.20f),
                new DungeonParallaxLayerDefinition("plan3.png", 0.23f),
                new DungeonParallaxLayerDefinition("plan2.png", 0.38f)
            },
            foregroundFileName: "plan1.png"),
        new(
            mapId: "map2",
            displayName: "Gothic Forest",
            defaultCharacterId: "lord",
            folderName: "map2",
            worldWidth: 12000f,
            previewFileName: "preview.png",
            layers: new[]
            {
                new DungeonParallaxLayerDefinition("back.png", 0.00f),
                new DungeonParallaxLayerDefinition("middle.png", 0.35f)
            },
            foregroundFileName: "front.png")
    };

    public static IReadOnlyList<DungeonMapDefinition> All => Maps;

    public static bool TryGet(string mapId, out DungeonMapDefinition definition)
    {
        definition = Maps.FirstOrDefault(
            map => string.Equals(map.MapId, mapId, StringComparison.OrdinalIgnoreCase))!;
        return definition != null;
    }

    public static DungeonMapDefinition Get(string mapId)
    {
        if (TryGet(mapId, out var definition))
            return definition;

        throw new ArgumentException($"Unknown dungeon map '{mapId}'.", nameof(mapId));
    }

    public static bool IsDungeonMap(string mapId) => TryGet(mapId, out _);
}
