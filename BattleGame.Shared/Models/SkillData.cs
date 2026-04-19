using System.Collections.Generic;

namespace BattleGame.Shared.Models
{
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
        public string Type { get; set; } = "";    // melee / projectile
        public string Trigger { get; set; } = "";    // onStart / onFrame / onFrames / onEnd
        public List<int>? Frames { get; set; }
        public int Damage { get; set; }
        public float Stun { get; set; }
        public float Speed { get; set; }
        public string ProjectileAnim { get; set; } = "";  
        public float Range { get; set; } = 50f;  
    }
}