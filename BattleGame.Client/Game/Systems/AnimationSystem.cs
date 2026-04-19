using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Game.Systems;

public class AnimationSystem
{
    public void Update(Entity entity, float deltaTime)
    {
        var ch = entity.Get<CharacterComponent>();
        var mv = entity.Get<MovementComponent>();
        var sp = entity.Get<SpriteComponent>();

        string target;

        // ===== PRIORITY =====
        if (ch.IsDead)
        {
            target = "Dead";
        }
        else if (ch.IsStunned)
        {
            target = "Hurt";
        }
        else if (ch.IsHurt)
        {
            target = "Hurt";
        }
        else if (ch.IsUsingSkill)
        {
            target = ch.CurrentSkillAnim;
        }
        else if (ch.IsAttacking)
        {
            target = ch.CurrentAttackAnim;
        }
        else if (ch.IsProtecting)
        {
            target = "Protection";
        }
        else if (!mv.IsGrounded)
        {
            target = "Jump";
        }
        else if (MathF.Abs(mv.VelocityX) > 150)
        {
            target = "Run";
        }
        else if (MathF.Abs(mv.VelocityX) > 0)
        {
            target = "Walk";
        }
        else
        {
            target = "Idle";
        }

        // ===== CHANGE ANIMATION (ANTI RESET) =====
        if (sp.CurrentAnimation != target)
        {
            if (target == "Hurt" && sp.CurrentAnimation == "Hurt")
                return;

            sp.CurrentAnimation = target;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0f;
            sp.AnimationFinished = false;
        }
    }
}