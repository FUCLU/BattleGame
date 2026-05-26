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
                ArmorPen = effect.ArmorPen ?? caster.Get<CharacterComponent>().BaseStats.ArmorPen,
                StunDuration = effect.Stun,
                Range = effect.Range,
                CollisionWidth = Math.Max(effect.CollisionWidth, (int)MathF.Round(effect.Range * 2f)),
                CollisionHeight = Math.Max(effect.CollisionHeight, (int)MathF.Round(effect.Range * 2f)),
                Lifetime = effect.Duration > 0f ? effect.Duration : 3f,
                Owner = caster,
                AnimationKey = effect.ProjectileAnim,
                HitFrames = effect.HitFrames ?? new List<int>(),
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

            if (mode == "casterfront" || mode == "ownerfront")
                return (
                    casterMv.X + (casterMv.FacingRight ? effect.SpawnOffsetX : -effect.SpawnOffsetX),
                    casterMv.Y + effect.SpawnOffsetY);

            if (mode == "casterself" || mode == "ownerself")
                return (casterMv.X + effect.SpawnOffsetX, casterMv.Y + effect.SpawnOffsetY);

            if (target != null && mode == "targetfront")
            {
                var targetMv = target.Get<MovementComponent>();
                return (
                    targetMv.X + (casterMv.X < targetMv.X ? effect.SpawnOffsetX : -effect.SpawnOffsetX),
                    targetMv.Y + effect.SpawnOffsetY);
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

        public static Entity CreateAttackProjectile(
            Entity caster,
            string projectileAnim,
            float damage,
            int armorPen,
            float speed,
            float spawnOffsetX,
            float spawnOffsetY,
            float scale)
        {
            var mv = caster.Get<MovementComponent>();

            var e = new Entity(Guid.NewGuid().GetHashCode());

            e.Add(new ProjectileComponent
            {
                X = mv.X + (mv.FacingRight ? spawnOffsetX : -spawnOffsetX),
                Y = mv.Y + spawnOffsetY,
                VelocityX = mv.FacingRight ? speed : -speed,
                VelocityY = 0,
                Damage = damage,
                ArmorPen = armorPen,
                StunDuration = 0,
                Range = 45,  // nhỏ hơn để projectile bay ra thấy rõ trước khi va chạm
                Owner = caster,
                AnimationKey = projectileAnim,
                Render = new EffectRenderData
                {
                    Scale = scale,
                    UseSpriteSize = true
                }
            });

            return e;
        }
    }
}
