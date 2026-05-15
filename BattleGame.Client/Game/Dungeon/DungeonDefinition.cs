using System.Collections.Generic;

namespace BattleGame.Client.Game.Dungeon;

public sealed class DungeonDefinition
{
    public string MapId { get; set; } = string.Empty;
    public float PortalX { get; set; }
    public List<DungeonWaveDefinition> Waves { get; set; } = new();
    public List<DungeonWaveDefinition> BossWaves { get; set; } = new();
}

public sealed class DungeonWaveDefinition
{
    public string WaveId { get; set; } = string.Empty;
    public List<DungeonSpawnGroupDefinition> Spawns { get; set; } = new();
}

public sealed class DungeonSpawnGroupDefinition
{
    public string PrefabId { get; set; } = string.Empty;
    public int Count { get; set; } = 1;
    public List<float> SpawnXs { get; set; } = new();
    public float SpawnY { get; set; }
    public float SpawnIntervalSeconds { get; set; } = 0f;
}

public sealed class DungeonMonsterCatalog
{
    public List<DungeonMonsterPrefab> Monsters { get; set; } = new();
}

public sealed class DungeonMonsterPrefab
{
    public string PrefabId { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public string AiProfile { get; set; } = string.Empty;
}
