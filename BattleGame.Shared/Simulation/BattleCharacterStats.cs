using BattleGame.Shared.Models;
using System;

namespace BattleGame.Shared.Simulation;

public class BattleCharacterStats
{
    public sealed class AnimationMeta
    {
        public int FrameCount { get; set; } = 1;
        public float Fps { get; set; } = 10f;
    }

    public int Hp { get; set; } = 100;
    public int Mana { get; set; } = 100;
    public float ManaRegen { get; set; } = 8f;
    public float Atk { get; set; } = 20;
    public int Def { get; set; } = 10;
    public int ArmorPen { get; set; }
    public float Speed { get; set; } = 250f;
    public float AtkSpeed { get; set; } = 1f;
    public float StunDuration { get; set; }
    public float AttackRange { get; set; } = 100f;
    public float AttackDuration { get; set; } = 0.5f;
    public int AttackAnimCount { get; set; } = 1;
    public string? AttackProjectile { get; set; }
    public float AttackProjectileSpeed { get; set; }
    public float AttackProjectileSpawnOffsetX { get; set; } = 30f;
    public float AttackProjectileSpawnOffsetY { get; set; } = -50f;
    public float AttackProjectileScale { get; set; } = 1f;
    public bool ProtectionBlocksAllDirections { get; set; }
    public List<EffectData> AttackEffects { get; set; } = new();
    public SkillData? Skill1 { get; set; }
    public SkillData? Skill2 { get; set; }
    public Dictionary<string, AnimationMeta> Animations { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
