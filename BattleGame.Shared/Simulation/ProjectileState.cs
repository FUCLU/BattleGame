namespace BattleGame.Shared.Simulation;

using BattleGame.Shared.Models;

public class ProjectileState
{
    public int ProjectileId { get; set; }
    public int OwnerPlayerId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public int Damage { get; set; }
    public float Stun { get; set; }
    public float Range { get; set; } = 50f;
    public float Lifetime { get; set; } = 3f;
    public float Timer { get; set; }
    public string AnimationKey { get; set; } = string.Empty;
    public int CurrentFrame { get; set; }
    public bool FacingRight { get; set; }
    public float RenderOffsetX { get; set; }
    public float RenderOffsetY { get; set; }
    public EffectRenderData Render { get; set; } = new();
}
