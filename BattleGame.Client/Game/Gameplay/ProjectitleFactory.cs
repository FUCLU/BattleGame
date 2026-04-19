using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Models;
using System;

namespace BattleGame.Client.Game.Gameplay
{
    public static class ProjectileFactory
    {
        public static Entity Create(Entity caster, EffectData effect)
        {
            var mv = caster.Get<MovementComponent>();

            var e = new Entity(Guid.NewGuid().GetHashCode());

            e.Add(new ProjectileComponent
            {
                X = mv.X + (mv.FacingRight ? 60 : -60),
                Y = mv.Y - 50,
                VelocityX = mv.FacingRight ? effect.Speed : -effect.Speed,
                VelocityY = 0,
                Damage = effect.Damage,
                StunDuration = effect.Stun,
                Range = effect.Range,
                Owner = caster,
                AnimationKey = effect.ProjectileAnim
            });

            return e;
        }
    }
}