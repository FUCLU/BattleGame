namespace BattleGame.Shared.Models;

public class CharacterData
{
    public string Id { get; set; } = "";
    public int Hp { get; set; }
    public int Def { get; set; }
    public int Mana { get; set; }
    public int Atk { get; set; }
    public float Speed { get; set; }
    public float AtkSpeed { get; set; } = 1.0f;
    public float StunDuration { get; set; } = 0f;
}