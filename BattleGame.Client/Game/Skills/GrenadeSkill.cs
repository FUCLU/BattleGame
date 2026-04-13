using BattleGame.Client.Game;
using BattleGame.Client.Game.Characters;
using BattleGame.Shared.Models;
using System;

namespace BattleGame.Client.Game.Skills
{
    public class GrenadeSkill : Skill
    {
        private ProjectileManager _projectileManager;
        private Soldier _pendingThrower;
        private DateTime _throwTime;

        // Khớp với Grenade.png: 9 frame × 70ms = 630ms
        // Spawn grenade gần cuối animation
        private const float DelaySeconds = 0.55f;

        public GrenadeSkill()
            : base("Grenade", SkillType.Grenade, 50, 2.0f) { }

        public void Init(ProjectileManager pm) => _projectileManager = pm;

        protected override void Use(Character caster)
        {
            if (caster is not Soldier soldier) return;
            _pendingThrower = soldier;
            _throwTime = DateTime.Now;
        }

        public void UpdateDelay()
        {
            if (_pendingThrower == null) return;

            if ((DateTime.Now - _throwTime).TotalSeconds >= DelaySeconds)
            {
                _projectileManager?.SpawnGrenade(_pendingThrower);
                _pendingThrower = null;
            }
        }
    }
}