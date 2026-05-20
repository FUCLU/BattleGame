using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Game.Systems;

public class MovementSystem
{
    private const float Gravity = 800f;
    public float MapLeft { get; set; } = 50f;
    public float MapRight { get; set; } = 750f;

    public void Update(Entity entity, float deltaTime)
    {
        var mv = entity.Get<MovementComponent>();
        var ch = entity.Get<CharacterComponent>();

        if (ch.IsDashing)
        {
            ch.DashTimer = Math.Max(0f, ch.DashTimer - deltaTime);
            ch.ActionTimer = Math.Max(0f, ch.ActionTimer - deltaTime);
            mv.VelocityX = ch.DashTimer > 0f
                ? (mv.FacingRight ? 1 : -1) * mv.Speed * ch.DashSpeedMultiplier
                : 0f;

            if (ch.DashTimer <= 0f)
            {
                ch.IsDashing = false;
                ch.ActionTimer = 0f;
                mv.VelocityX = 0f;
            }
        }

        if (ch.IsHurt || ch.IsStunned || ch.IsDead || (ch.IsBusy && !ch.IsDashing))
            mv.VelocityX = 0;

        if (!mv.IsGrounded)
            mv.VelocityY += Gravity * deltaTime;

        mv.X += mv.VelocityX * deltaTime;
        mv.Y += mv.VelocityY * deltaTime;

        if (mv.Y >= mv.GroundY)
        {
            mv.Y = mv.GroundY;
            mv.VelocityY = 0;
            mv.IsGrounded = true;
        }

        mv.X = Math.Clamp(mv.X, MapLeft, MapRight);
    }
}
