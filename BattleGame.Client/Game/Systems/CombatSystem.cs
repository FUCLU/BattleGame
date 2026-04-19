using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Game.Systems
{
    public class CombatSystem
    {
        private static readonly Random _rng = new();

        public void Update(Entity entity, float deltaTime)
        {
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();

            // Kết thúc attack khi animation xong hoặc timer hết
            if (ch.IsAttacking)
            {
                ch.ActionTimer -= deltaTime;
                if (ch.ActionTimer <= 0 || sp.AnimationFinished)
                    ch.IsAttacking = false;
            }

            // Kết thúc skill khi animation xong
            if (ch.IsUsingSkill)
            {
                if (sp.AnimationFinished)
                    ch.IsUsingSkill = false;
            }

            // Đếm ngược hurt
            if (ch.IsHurt)
            {
                ch.HurtTimer -= deltaTime;
                if (ch.HurtTimer <= 0) ch.IsHurt = false;
            }

            // Đếm ngược stun
            if (ch.IsStunned)
            {
                ch.StunTimer -= deltaTime;
                if (ch.StunTimer <= 0) ch.IsStunned = false;
            }

            // Đếm ngược cooldown
            if (ch.Skill1Cooldown > 0) ch.Skill1Cooldown -= deltaTime;
            if (ch.Skill2Cooldown > 0) ch.Skill2Cooldown -= deltaTime;
        }

        public void Attack(Entity attacker)
        {
            var ch = attacker.Get<CharacterComponent>();
            if (ch.IsBusy) return;

            int idx = _rng.Next(1, ch.AttackAnimCount + 1);
            ch.CurrentAttackAnim = $"Attack_{idx}";
            ch.IsAttacking = true;
            ch.ActionTimer = ch.ActionDuration;
        }

        public void UseSkill(Entity caster, int slot)
        {
            var ch = caster.Get<CharacterComponent>();
            if (ch.IsBusy) return;

            var skill = slot == 1 ? ch.Skill1 : ch.Skill2;
            if (skill == null) return;

            float cd = slot == 1 ? ch.Skill1Cooldown : ch.Skill2Cooldown;
            if (cd > 0) return;
            if (ch.Mana < skill.ManaCost) return;

            ch.Mana -= skill.ManaCost;
            ch.IsUsingSkill = true;
            ch.CurrentSkillAnim = $"Skill{slot}";

            // Reset sprite để animation chạy từ frame đầu
            var sp = caster.Get<SpriteComponent>();
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0f;
            sp.AnimationFinished = false;

            if (slot == 1) ch.Skill1Cooldown = skill.Cooldown;
            else ch.Skill2Cooldown = skill.Cooldown;
        }

        public void TakeDamage(Entity target, int rawDamage, float stunDuration = 0f)
        {
            var ch = target.Get<CharacterComponent>();
            if (ch.IsDead) return;

            int damage = ch.IsProtecting
                ? Math.Max(0, rawDamage - ch.BaseStats.Def * 2)
                : Math.Max(0, rawDamage - ch.BaseStats.Def);

            ch.Hp = Math.Max(0, ch.Hp - damage);

            if (!ch.IsProtecting)
            {
                ch.IsHurt = true;
                ch.HurtTimer = ch.HurtDuration;
            }

            if (stunDuration > 0 && !ch.IsProtecting)
            {
                ch.IsStunned = true;
                ch.StunTimer = stunDuration;
            }

            if (ch.Hp <= 0) ch.IsDead = true;
        }
    }
}