using System.ComponentModel;

namespace BattleGame.Client.Game.Core.Components;

public class MovementComponent : IComponent
{
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public bool FacingRight { get; set; } = true;
    public bool IsGrounded { get; set; } = true;
    public float Speed { get; set; } = 200f;
    public float GroundY { get; set; } = 400f;
}