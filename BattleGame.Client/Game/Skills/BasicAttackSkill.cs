using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BattleGame.Shared.Models;

namespace BattleGame.Client.Game.Skills
{
    public class BasicAttackSkill : Skill
    {
        public BasicAttackSkill()
            : base("Attack", SkillType.None, 10, 0.3f) // cooldown ngắn
        {
        }

        protected override void Use(Character caster)
        {
            // 👉 Logic đánh thường (tạm thời)
            // Sau này có thể:
            // - tạo hitbox
            // - check va chạm
            // - gửi damage lên server

            // Debug (optional)
            // Console.WriteLine($"{caster.GetType().Name} used Basic Attack");
        }
    }
}