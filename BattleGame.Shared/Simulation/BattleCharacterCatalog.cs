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
            6 => "stonegolem",
            _ => "lord"
        };
    }

}
