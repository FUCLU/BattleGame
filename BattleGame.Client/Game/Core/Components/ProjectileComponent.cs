using BattleGame.Client.Game.Core;
using BattleGame.Shared.Models;

namespace BattleGame.Client.Game.Core.Components
{
    public class ProjectileComponent : IComponent
    {
        public float X;
        public float Y;

        public float VelocityX;
        public float VelocityY;

        public int Damage;
        public float StunDuration;
        public float Range = 50f;

        public float Lifetime = 3f;
        public float Timer = 0f;

        public Entity Owner = null!;

        public bool IsDestroyed = false;

        public string AnimationKey { get; set; } = "";
        public int CurrentFrame { get; set; } = 0;
        public float FrameTimer { get; set; } = 0f;
        public EffectRenderData Render { get; set; } = new();
    }
}
