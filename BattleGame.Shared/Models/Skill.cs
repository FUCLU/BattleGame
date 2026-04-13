using System;

namespace BattleGame.Shared.Models
{
    public enum SkillType
    {
        None = 0,
        Shoot,
        Grenade
    }

    public abstract class Skill
    {
        public string Name { get; protected set; } = string.Empty;
        public SkillType Type { get; protected set; }

        public int Damage { get; protected set; }
        public float Cooldown { get; protected set; } // seconds

        public string AnimationName { get; protected set; } = string.Empty;

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

        protected abstract void Use(Character caster);
    }
}