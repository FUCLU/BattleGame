using BattleGame.Shared.Models;

namespace BattleGame.Shared.Simulation;

public class BattleCharacterStats
{
    public int Hp { get; set; } = 100;
    public int Mana { get; set; } = 100;
    public int Atk { get; set; } = 20;
    public int Def { get; set; } = 10;
    public float Speed { get; set; } = 250f;
    public float AtkSpeed { get; set; } = 1f;
    public float StunDuration { get; set; }
    public float AttackRange { get; set; } = 100f;
    public float AttackDuration { get; set; } = 0.5f;
    public string? AttackProjectile { get; set; }
    public float AttackProjectileSpeed { get; set; }
    public bool ProtectionBlocksAllDirections { get; set; }
    public List<EffectData> AttackEffects { get; set; } = new();
    public SkillData? Skill1 { get; set; }
    public SkillData? Skill2 { get; set; }
}
