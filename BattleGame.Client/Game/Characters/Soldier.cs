using BattleGame.Client.Game.Skills;
using BattleGame.Shared.Models;
using System.Collections.Generic;
using System.Drawing;

namespace BattleGame.Client.Game.Characters
{
    public class Soldier : Character
    {
        // ═══════════════════════════════════════
        //  CONSTANTS
        // ═══════════════════════════════════════
        private const float MoveSpeed = 3.0f;
        private const float MinX = 0f;
        private const float MaxX = 1100f;
        public const int FrameWidth = 128;
        public const int FrameHeight = 128;

        // ═══════════════════════════════════════
        //  TRANSFORM
        // ═══════════════════════════════════════
        public float X { get; private set; } = 100f;
        public float Y { get; private set; } = 300f;
        public bool FacingRight { get; private set; } = true;

        public RectangleF Hitbox =>
            new RectangleF(X + 40, Y + 16, 48, 112);

        // ═══════════════════════════════════════
        //  ANIMATION
        // ═══════════════════════════════════════
        public string CurrentAnimationName { get; private set; } = "Idle";

        private readonly Dictionary<string, SpriteAnimation> _animations;
        private SpriteAnimation _currentAnim;

        // Phát 1 lần rồi về Idle
        private static readonly HashSet<string> _oneShots =
            new() { "Attack", "Shoot", "Shoot2", "Grenade", "Hurt" };

        // ═══════════════════════════════════════
        //  STATE FLAGS
        // ═══════════════════════════════════════
        private bool _isCastingSkill = false;
        private bool _isHurt = false;
        private bool _isDead = false;

        // ═══════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════
        public Soldier()
        {
            Name = "Soldier";
            MaxHP = 100;
            CurrentHP = 100;
            ATK = 10;

            _animations = new Dictionary<string, SpriteAnimation>
            {
                { "Idle",     new SpriteAnimation("Assets/Characters/Soldier/Idle.png",     128, 128, 120) },
                { "Run",      new SpriteAnimation("Assets/Characters/Soldier/Run.png",      128, 128,  80) },
                { "Walk",     new SpriteAnimation("Assets/Characters/Soldier/Walk.png",     128, 128, 100) },
                { "Attack",   new SpriteAnimation("Assets/Characters/Soldier/Attack.png",   128, 128,  50) },
                { "Shoot",    new SpriteAnimation("Assets/Characters/Soldier/Shot_1.png",   128, 128,  50) },
                { "Shoot2",   new SpriteAnimation("Assets/Characters/Soldier/Shot_2.png",   128, 128,  50) },
                { "Grenade",  new SpriteAnimation("Assets/Characters/Soldier/Grenade.png",  128, 128,  70) },
                { "Hurt",     new SpriteAnimation("Assets/Characters/Soldier/Hurt.png",     128, 128,  60) },
                { "Dead",     new SpriteAnimation("Assets/Characters/Soldier/Dead.png",     128, 128,  80) },
                { "Recharge", new SpriteAnimation("Assets/Characters/Soldier/Recharge.png", 128, 128,  90) },
            };

            // Loop = true cho các animation lặp, false cho one-shot
            foreach (var anim in _animations)
                anim.Value.Loop = !_oneShots.Contains(anim.Key)
                                  && anim.Key != "Dead";

            _currentAnim = _animations["Idle"];

            Skills.Add(new BasicAttackSkill()); // J
            Skills.Add(new ShootSkill());       // U
            Skills.Add(new GrenadeSkill());     // I
        }

        // ═══════════════════════════════════════
        //  MOVEMENT
        // ═══════════════════════════════════════
        public void Move(float dx)
        {
            if (_isCastingSkill || _isDead || _isHurt) return;

            X += dx * MoveSpeed;
            X = System.Math.Clamp(X, MinX, MaxX);

            if (dx > 0) FacingRight = true;
            if (dx < 0) FacingRight = false;

            PlayAnimation(dx != 0 ? "Run" : "Idle");
        }

        //  SKILLS
        public void UseSkill(int index)
        {
            if (_isCastingSkill || _isDead || _isHurt) return;
            if (index < 0 || index >= Skills.Count) return;

            var skill = Skills[index];

            if (!skill.CanCast()) return;

            skill.TryCast(this);

            _isCastingSkill = true;
            PlayAnimation(skill.AnimationName);
        }

        //  COMBAT
        public override int TakeDamage(int amount)
        {
            if (_isDead) return 0;

            // 🔥 FIX: dùng logic từ Character (có DEF)
            int actualDamage = base.TakeDamage(amount);

            if (CurrentHP <= 0)
            {
                _isDead = true;
                _isCastingSkill = false;
                _isHurt = false;
                PlayAnimation("Dead");
            }
            else
            {
                _isHurt = true;
                _isCastingSkill = false;
                PlayAnimation("Hurt");
            }

            return actualDamage;
        }

        public override bool IsDead() => _isDead;

        // Heal không có trong Character base → gọi trực tiếp từ Soldier
        public void Heal(int amount)
        {
            if (_isDead) return;
            CurrentHP = System.Math.Min(CurrentHP + amount, MaxHP);
        }

        //  UPDATE
        public override void Update()
        {
            _currentAnim.Update();

            if (!_currentAnim.IsFinished) return;

            string cur = CurrentAnimationName;

            if (cur == "Dead")
                return;

            if (cur == "Hurt")
            {
                _isHurt = false;
                PlayAnimation("Idle");
                return;
            }

            if (_oneShots.Contains(cur))
            {
                _isCastingSkill = false;
                PlayAnimation("Idle");
            }
        }

        //  DRAW
        public void Draw(Graphics g)
        {
            _currentAnim.Draw(g, X, Y, !FacingRight);

#if DEBUG
            using var pen = new Pen(Color.LimeGreen, 1f);
            g.DrawRectangle(pen, Rectangle.Round(Hitbox));
#endif
        }

        // ═══════════════════════════════════════
        //  ANIMATION HELPER
        // ═══════════════════════════════════════
        public void PlayAnimation(string name)
        {
            if (!_animations.ContainsKey(name)) return;
            if (CurrentAnimationName == name) return;
            if (_isDead && name != "Dead") return;

            CurrentAnimationName = name;
            _currentAnim = _animations[name];
            _currentAnim.Reset();
        }

    }
}