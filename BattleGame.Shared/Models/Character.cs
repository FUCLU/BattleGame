namespace BattleGame.Shared.Models;

public class Character
{
    public string Name { get; set; } = string.Empty;
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int Speed { get; set; }
    public List<Skill> Skills { get; set; } = new();

    public bool IsAlive => CurrentHP > 0;

    public Character(string name, int hp, int atk, int def, int speed)
    {
        Name = name;
        MaxHP = hp;
        CurrentHP = hp;
        ATK = atk;
        DEF = def;
        Speed = speed;
    }

    public int TakeDamage(int rawDamage)
    {
        int actualDamage = Math.Max(1, rawDamage - DEF);
        CurrentHP = Math.Max(0, CurrentHP - actualDamage);
        return actualDamage;
    }

    public void TickCooldowns()
    {
        foreach (var skill in Skills)
            skill.ReduceCooldown();
    }

    public override string ToString() =>
        $"{Name,-12} | HP: {CurrentHP}/{MaxHP} | ATK: {ATK} | DEF: {DEF} | SPD: {Speed}";
}