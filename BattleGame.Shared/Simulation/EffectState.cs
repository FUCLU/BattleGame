namespace BattleGame.Shared.Simulation;

using BattleGame.Shared.Models;

public class EffectState
{
    public int EffectId { get; set; }
    public int OwnerPlayerId { get; set; }
    public string EffectType { get; set; } = string.Empty;
    public string AnimationKey { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public int Damage { get; set; }
    public float Stun { get; set; }
    public int CollisionWidth { get; set; } = 80;
    public int CollisionHeight { get; set; } = 80;
    public bool BlockEnemyAttack { get; set; } = true;
    public bool BlockEnemyProjectile { get; set; } = true;
    public bool BlockEnemySkill { get; set; } = true;
    public int CurrentFrame { get; set; }
    public float RemainingTime { get; set; }
    public bool FacingRight { get; set; }
    public int LastDamageTick { get; set; } = -1;
    public EffectRenderData Render { get; set; } = new();
}
