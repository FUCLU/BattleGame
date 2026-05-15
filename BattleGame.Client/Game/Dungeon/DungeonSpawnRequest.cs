namespace BattleGame.Client.Game.Dungeon;

public sealed class DungeonSpawnRequest
{
    public required string PrefabId { get; init; }
    public required string CharacterId { get; init; }
    public required float X { get; init; }
    public required float Y { get; init; }
    public required bool IsBoss { get; init; }
    public required string WaveId { get; init; }
    public required string SpawnToken { get; init; }
}
