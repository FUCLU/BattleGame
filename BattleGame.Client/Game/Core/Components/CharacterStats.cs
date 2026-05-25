using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Client.Game.Core.Components
{
    public class CharacterStats
    {
        public int Hp { get; set; }
        public int Def { get; set; }
        public int ArmorPen { get; set; }
        public int Mana { get; set; }
        public float ManaRegen { get; set; } = 8f;
        public float Atk { get; set; }
        public float Speed { get; set; }
        public float AtkSpeed { get; set; } = 1.0f;
        public float StunDuration { get; set; } = 0f;
        public float AttackRange { get; set; } = 150f;  // Phạm vi đánh thường (pixel)
        public string? AttackProjectile { get; set; } = null;  // Tên projectile bắn khi attack (vd: "Fire")
        public float AttackProjectileSpeed { get; set; } = 0f;  // Tốc độ projectile (0 = không bắn)
        public float AttackProjectileSpawnOffsetX { get; set; } = 30f;
        public float AttackProjectileSpawnOffsetY { get; set; } = -50f;
        public float AttackProjectileScale { get; set; } = 1f;
    }
}
