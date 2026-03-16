namespace BattleGame.Shared.Models;

public enum SkillType
{
    Fixed,    // Sát thương cố định
    HPScale   // Sát thương theo % HP hiện tại của target
}

public class Skill
{
    public string Name { get; set; } = string.Empty;
    public SkillType Type { get; set; }
    public int MaxCooldown { get; set; }
    public int CurrentCooldown { get; private set; } = 0;

    public int FixedDamage { get; set; }       // Dùng cho Fixed
    public float HPScaleRatio { get; set; }    // Dùng cho HPScale (0.3 = 30%)

    public bool IsReady => CurrentCooldown == 0;

    // Factory methods
    public static Skill CreateFixed(string name, int damage, int cooldown) => new()
    {
        Name = name,
        Type = SkillType.Fixed,
        FixedDamage = damage,
        MaxCooldown = cooldown
    };

    public static Skill CreateHPScale(string name, float ratio, int cooldown) => new()
    {
        Name = name,
        Type = SkillType.HPScale,
        HPScaleRatio = ratio,
        MaxCooldown = cooldown
    };

    public int CalculateDamage(Character target) => Type switch
    {
        SkillType.Fixed => FixedDamage,
        SkillType.HPScale => (int)(target.CurrentHP * HPScaleRatio),
        _ => 0
    };

    public int GetDamagePreview(Character target) => CalculateDamage(target);

    public void Use()
    {
        if (!IsReady) throw new InvalidOperationException($"{Name} đang cooldown!");
        CurrentCooldown = MaxCooldown;
    }

    public void ReduceCooldown()
    {
        if (CurrentCooldown > 0) CurrentCooldown--;
    }

    public override string ToString()
    {
        string dmgInfo = Type == SkillType.Fixed
            ? $"DMG: {FixedDamage}"
            : $"DMG: {HPScaleRatio * 100}% HP";
        return $"[{Name}] {dmgInfo} | CD: {(IsReady ? "Ready" : $"{CurrentCooldown} turns")}";
    }
}