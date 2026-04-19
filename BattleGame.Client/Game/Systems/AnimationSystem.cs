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

        string target = ch switch
        {
            { IsDead: true } => "Dead",
            { IsStunned: true } => "Hurt",
            { IsHurt: true } => "Hurt",
            { IsUsingSkill: true } => ch.CurrentSkillAnim,
            { IsAttacking: true } => ch.CurrentAttackAnim,
            { IsProtecting: true } => "Protection",
            _ when !mv.IsGrounded => "Jump",
            _ when MathF.Abs(mv.VelocityX) > 150 => "Run",
            _ when MathF.Abs(mv.VelocityX) > 0 => "Walk",
            _ => "Idle"
        };

        if (sp.CurrentAnimation != target)
        {
            sp.CurrentAnimation = target;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0f;
            sp.AnimationFinished = false;
        }
    }
}