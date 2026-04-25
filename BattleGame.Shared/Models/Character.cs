using System.Collections.Generic;

namespace BattleGame.Shared.Models
{
    public class Character
    {
        // ===== IDENTITY =====
        public string Name { get; protected set; } = "Unknown";

        // ===== STATS =====
        public int MaxHP { get; protected set; } = 100;
        public int CurrentHP { get; protected set; } = 100;
        public int ATK { get; protected set; } = 10;
        public int DEF { get; protected set; } = 0;
        public int Speed { get; protected set; } = 0;
        public bool IsAlive => !IsDead();
        public List<Skill> Skills { get; protected set; } = new();

        public Character(string name, int maxHp, int atk, int def, int speed)
        {
            Name = name;
            MaxHP = maxHp;
            CurrentHP = maxHp;
            ATK = atk;
            DEF = def;
            Speed = speed;
        }
        public virtual int TakeDamage(int rawDamage)
        {
            if (rawDamage <= 0) return 0;

            int actualDamage = System.Math.Max(1, rawDamage - DEF);
            CurrentHP -= actualDamage;

            if (CurrentHP < 0)
                CurrentHP = 0;

            return actualDamage;
        }

        public virtual bool IsDead()
        {
            return CurrentHP <= 0;
        }


        public virtual void Update()
        {
            // dùng cho animation / realtime logic
        }

        public override string ToString()
        {
            return $"{Name,-12} | HP: {CurrentHP}/{MaxHP} | ATK: {ATK} | DEF: {DEF} | SPD: {Speed}";
        }
    }
}