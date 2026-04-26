using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Models;
using System;

namespace BattleGame.Client.Game.Gameplay
{
    public static class ProjectileFactory
    {
        public static Entity Create(Entity caster, Entity? target, EffectData effect)
        {
            var mv = caster.Get<MovementComponent>();
            var spawn = ResolveSpawn(caster, target, effect);
            var velocity = ResolveVelocity(mv, effect);

            var e = new Entity(Guid.NewGuid().GetHashCode());

            e.Add(new ProjectileComponent
            {
                X = spawn.X,
                Y = spawn.Y,
                VelocityX = velocity.Vx,
                VelocityY = velocity.Vy,
                Damage = effect.Damage,
                StunDuration = effect.Stun,
                Range = effect.Range,
                Owner = caster,
                AnimationKey = effect.ProjectileAnim,
                Render = effect.Render
            });

            return e;
        }

        private static (float X, float Y) ResolveSpawn(Entity caster, Entity? target, EffectData effect)
        {
            var casterMv = caster.Get<MovementComponent>();
            var mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();

            if (target != null && (mode == "targettop" || mode == "targetabove" || mode == "targettopdown"))
            {
                var targetMv = target.Get<MovementComponent>();
                return (targetMv.X + effect.SpawnOffsetX, targetMv.Y + effect.SpawnOffsetY);
            }

            return (
                casterMv.X + (casterMv.FacingRight ? 80 : -80),
                casterMv.Y - 50);
        }

        private static (float Vx, float Vy) ResolveVelocity(MovementComponent casterMv, EffectData effect)
        {
            var mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();

            if (mode == "targettop" || mode == "targetabove" || mode == "targettopdown")
                return (0f, Math.Abs(effect.Speed));

            return (casterMv.FacingRight ? effect.Speed : -effect.Speed, 0f);
        }

        public static Entity CreateAttackProjectile(Entity caster, string projectileAnim, int damage, float speed)
        {
            var mv = caster.Get<MovementComponent>();

            var e = new Entity(Guid.NewGuid().GetHashCode());

            e.Add(new ProjectileComponent
            {
                X = mv.X + (mv.FacingRight ? 30 : -80),
                Y = mv.Y - 50,
                VelocityX = mv.FacingRight ? speed : -speed,
                VelocityY = 0,
                Damage = damage,
                StunDuration = 0,
                Range = 45,  // nhỏ hơn để projectile bay ra thấy rõ trước khi va chạm
                Owner = caster,
                AnimationKey = projectileAnim
            });

            return e;
        }
    }
}
