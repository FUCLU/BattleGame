using System.Collections.Generic;

namespace BattleGame.Shared.Models
{
    public abstract class Character
    {
        // ===== IDENTITY =====
        public string Name { get; protected set; } = "Unknown";

        // ===== STATS =====
        public int MaxHP { get; protected set; } = 100;
        public int CurrentHP { get; protected set; } = 100;
        public int ATK { get; protected set; } = 10;

        // ===== SKILLS =====
        public List<Skill> Skills { get; protected set; } = new();

        // ================= CORE =================

        public virtual void TakeDamage(int damage)
        {
            if (damage <= 0) return;

            CurrentHP -= damage;

            if (CurrentHP < 0)
                CurrentHP = 0;
        }

        public virtual bool IsDead()
        {
            return CurrentHP <= 0;
        }

        // ================= OPTIONAL =================

        public virtual void Update()
        {
            // Client override nếu cần (animation, movement...)
        }
    }
}