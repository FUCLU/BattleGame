using BattleGame.Client.Game.Core;
using BattleGame.Shared.Models;
using System.Collections.Generic;

namespace BattleGame.Client.Game.Core.Components
{
    public class BarrierComponent : IComponent
    {
        public bool IsActive { get; set; } = false;
        public float RemainingTime { get; set; } = 0;
        public float MaxDuration { get; set; } = 3.0f;
        public float X { get; set; }
        public float Y { get; set; }
        public int Damage { get; set; } = 10;
        public List<int> HitFrames { get; set; } = new();
        public int CollisionWidth { get; set; } = 80;
        public int CollisionHeight { get; set; } = 80;
        public bool BlockEnemyAttack { get; set; } = true;
        public bool BlockEnemyProjectile { get; set; } = true;
        public bool BlockEnemySkill { get; set; } = true;
        public string AnimationKey { get; set; } = "Object";
        public Entity Owner { get; set; } = null!;
        public bool FacingRight { get; set; } = true;
        public int LastDamageFrame { get; set; } = -1;
        public EffectRenderData Render { get; set; } = new();
    }
}
