using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Game.Systems;

public class MovementSystem
{
    private const float Gravity = 800f;
    private const float MapLeft = 0f;
    private const float MapRight = 800f;

    public void Update(Entity entity, float deltaTime)
    {
        var mv = entity.Get<MovementComponent>();
        var ch = entity.Get<CharacterComponent>();

        // Không di chuyển khi bị stun hoặc dead
        if (ch.IsHurt || ch.IsStunned || ch.IsDead || ch.IsBusy)
            mv.VelocityX = 0;

        // Gravity
        if (!mv.IsGrounded)
            mv.VelocityY += Gravity * deltaTime;

        mv.X += mv.VelocityX * deltaTime;
        mv.Y += mv.VelocityY * deltaTime;

        // Chạm đất
        if (mv.Y >= mv.GroundY)
        {
            mv.Y = mv.GroundY;
            mv.VelocityY = 0;
            mv.IsGrounded = true;
        }

        mv.X = Math.Clamp(mv.X, MapLeft, MapRight);
    }
}