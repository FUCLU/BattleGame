namespace BattleGame.Shared.Simulation;

public static class BattleCharacterCatalog
{
    public static string FromNetworkId(int characterId)
    {
        return characterId switch
        {
            0 => "lord",
            1 => "samurai",
            2 => "kitsune",
            3 => "wizard",
            4 => "haladin",
            5 => "heavycrystal",
            _ => "lord"
        };
    }

    public static BattleCharacterStats GetStats(string characterId)
    {
        return characterId.ToLowerInvariant() switch
        {
            "lord" => new BattleCharacterStats
            {
                Hp = 130,
                Mana = 140,
                Atk = 26,
                Def = 14,
                Speed = 250f,
                AttackRange = 180f,
                AttackDuration = 0.77f,
                ProtectionBlocksAllDirections = true
            },
            "samurai" => new BattleCharacterStats
            {
                Hp = 100,
                Mana = 100,
                Atk = 20,
                Def = 10,
                Speed = 350f,
                AttackRange = 100f,
                AttackDuration = 0.5f
            },
            "wizard" => new BattleCharacterStats
            {
                Hp = 100,
                Mana = 100,
                Atk = 20,
                Def = 10,
                Speed = 200f,
                AttackRange = 100f,
                AttackDuration = 1f
            },
            _ => new BattleCharacterStats()
        };
    }
}
