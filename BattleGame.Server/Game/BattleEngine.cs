using System;
using System.Linq;
using BattleGame.Shared.Models;

namespace BattleGame.Server.Game
{
    public class BattleEngine
    {
        private static readonly Random _rng = new();

        public TurnResult ProcessTurn(Character attacker, Character defender)
        {
            if (attacker == null || defender == null)
                throw new ArgumentNullException("Attacker or Defender is null");

            // ===== 1. CHỌN SKILL =====
            var skill = attacker.Skills
                .Where(s => s.CanCast())
                .OrderByDescending(s => s.Damage)
                .FirstOrDefault();

            int damage;
            string actionName;

            // ===== 2. EXECUTE =====
            if (skill != null)
            {
                actionName = skill.Name;

                damage = CalculateDamage(attacker, skill);

                skill.TryCast(attacker); // ✅ đúng design mới
            }
            else
            {
                actionName = "Basic Attack";
                damage = attacker.ATK;
            }

            // ===== 3. CRITICAL =====
            bool isCrit = IsCritical();
            if (isCrit)
                damage = (int)(damage * 1.5f);

            // ===== 4. APPLY DAMAGE =====
            int actualDamage = ApplyDamage(defender, damage);

            // ===== 5. COOLDOWN TICK =====


            // ===== 6. RESULT =====
            return new TurnResult
            {
                Attacker = attacker.Name,
                Defender = defender.Name,
                ActionName = actionName,
                Damage = actualDamage,
                IsCritical = isCrit,
                DefenderHP = defender.CurrentHP,
                DefenderMaxHP = defender.MaxHP,
                IsDefenderDead = defender.IsDead()
            };
        }

        // ================= PRIVATE =================

        private int CalculateDamage(Character attacker, Skill skill)
        {
            return attacker.ATK + skill.Damage;
        }

        private bool IsCritical()
        {
            return _rng.Next(100) < 20;
        }

        private int ApplyDamage(Character defender, int damage)
        {
            int before = defender.CurrentHP;

            defender.TakeDamage(damage);

            return before - defender.CurrentHP; // damage thực tế
        }
    }

    // ================= RESULT DTO =================

    public class TurnResult
    {
        public string Attacker { get; set; } = string.Empty;
        public string Defender { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public int Damage { get; set; }
        public bool IsCritical { get; set; }
        public int DefenderHP { get; set; }
        public int DefenderMaxHP { get; set; }
        public bool IsDefenderDead { get; set; }

        public override string ToString()
        {
            string crit = IsCritical ? " ✦CRIT" : "";
            string dead = IsDefenderDead ? " → DEFEATED!" : "";

            return $"{Attacker,-12} [{ActionName}]{crit,-8} → {Defender,-12} " +
                   $"-{Damage} HP ({DefenderHP}/{DefenderMaxHP}){dead}";
        }
    }
}