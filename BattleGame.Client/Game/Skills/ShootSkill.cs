using BattleGame.Client.Game;
using BattleGame.Client.Game.Characters;
using BattleGame.Shared.Models;

namespace BattleGame.Client.Game.Skills
{
    public class ShootSkill : Skill
    {
        private ProjectileManager _projectileManager;

        public ShootSkill() : base("Shoot", SkillType.Shoot, 40, 0.5f) { }

        /// <summary>
        /// GameEngine gọi cái này 1 lần sau khi tạo ProjectileManager.
        /// </summary>
        public void Init(ProjectileManager pm) => _projectileManager = pm;

        protected override void Use(Character caster)
        {
            if (caster is not Soldier soldier) return;
            _projectileManager?.SpawnBullet(soldier);
        }
    }
}