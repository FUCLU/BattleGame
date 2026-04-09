using System;

namespace BattleGame.Shared.Models
{
    public abstract class Skill
    {
        // ================= INFO =================
        public string Name { get; protected set; } = string.Empty;
        public SkillType Type { get; protected set; }

        public int Damage { get; protected set; }
        public float Cooldown { get; protected set; }   // seconds

        public string AnimationName { get; protected set; } = string.Empty;

        // ================= STATE =================
        private DateTime _lastCastTime = DateTime.MinValue;

        protected Skill() { }

        protected Skill(string name, SkillType type, int damage, float cooldown)
        {
            Name = name;
            Type = type;
            Damage = damage;
            Cooldown = cooldown;
            AnimationName = name;
        }

        // ================= CORE =================

        public bool CanCast()
        {
            return (DateTime.Now - _lastCastTime).TotalSeconds >= Cooldown;
        }

        public void TryCast(Character caster)
        {
            if (!CanCast()) return;


            Use(caster);

            _lastCastTime = DateTime.Now;
        }

        // ================= ABSTRACT =================
        protected abstract void Use(Character caster);

        // ================= OPTIONAL =================
        public virtual void TickCooldown()
        {
            // để trống (dùng realtime)
            // sau này có thể chuyển sang deltaTime
        }
    }

    public enum SkillType
    {
        None = 0,
        Shoot,
        Grenade
    }
}