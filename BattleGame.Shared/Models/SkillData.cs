using System.Collections.Generic;

namespace BattleGame.Shared.Models
{
    public class EffectRenderData
    {
        public float Scale { get; set; } = 1f;
        public float OffsetX { get; set; } = 0f;
        public float OffsetY { get; set; } = 0f;
        public bool UseSpriteSize { get; set; } = true;
        public string AlignY { get; set; } = "center"; // center / bottom / top
        public string FacingSource { get; set; } = "owner"; // owner / target / fixed
    }

    public class SkillData
    {
        public string Id { get; set; } = "";
        public int ManaCost { get; set; }
        public float Cooldown { get; set; }
        public string Animation { get; set; } = "";
        public List<EffectData> Effects { get; set; } = new();
    }

    public class EffectData
    {
        public string Type { get; set; } = ""; // melee / projectile / barrier
        public string Trigger { get; set; } = ""; // onStart / onFrame / onFrames / onMiddle / onEnd
        public List<int>? Frames { get; set; }
        public int Damage { get; set; }
        public float Stun { get; set; }
        public float Speed { get; set; }
        public string ProjectileAnim { get; set; } = "";
        public string ObjectAnim { get; set; } = "";
        public string SpawnMode { get; set; } = "between"; // between / targetFront / casterFront
        public float SpawnOffsetX { get; set; } = 10f;
        public float SpawnOffsetY { get; set; } = -30f;
        public int CollisionWidth { get; set; } = 80;
        public int CollisionHeight { get; set; } = 80;
        public bool BlockEnemyAttack { get; set; } = true;
        public bool BlockEnemyProjectile { get; set; } = true;
        public bool BlockEnemySkill { get; set; } = true;
        public float Range { get; set; } = 50f;
        public float Duration { get; set; } = 3.0f;
        public EffectRenderData Render { get; set; } = new();
    }
}
