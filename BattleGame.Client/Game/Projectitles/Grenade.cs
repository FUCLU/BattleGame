using BattleGame.Client.Game;
using BattleGame.Client.Managers;
using System;
using System.Drawing;

namespace BattleGame.Client.Game.Projectitles
{
    public class Grenade
    {
        private const float Gravity = 0.4f;
        private const float SpeedX = 5f;
        private const float InitVelocityY = -10f;
        private const float GroundY = 400f;  // chỉnh theo map

        public float X { get; private set; }
        public float Y { get; private set; }
        public bool IsExpired { get; private set; } = false;
        public bool IsExploding { get; private set; } = false;
        public int Damage { get; private set; }

        public float ExplosionRadius => 80f;
        public PointF ExplosionCenter => new PointF(_explodeX, _explodeY);

        private readonly float _dirX;
        private float _velY;
        private float _explodeX, _explodeY;

        // Chỉ dùng Explosion.png — 9 frame × 128×128
        private readonly SpriteAnimation _explodeAnim;

        public Grenade(float x, float y, bool facingRight, int damage)
        {
            X = x;
            Y = y;
            _dirX = facingRight ? 1f : -1f;
            _velY = InitVelocityY;
            Damage = damage;

            _explodeAnim = new SpriteAnimation(
                "Assets/Characters/Soldier/Explosion.png", 128, 128, 60);
            _explodeAnim.Loop = false;
        }

        public void Update()
        {
            if (IsExpired) return;

            if (IsExploding)
            {
                _explodeAnim.Update();
                if (_explodeAnim.IsFinished) IsExpired = true;
                return;
            }

            // Bay parabolic
            X += _dirX * SpeedX;
            _velY += Gravity;
            Y += _velY;

            if (Y >= GroundY)
            {
                Y = GroundY;
                Explode();
            }
        }

        // Hitbox nhỏ của quả lựu đạn khi đang bay
        public RectangleF Hitbox => new RectangleF(X - 6, Y - 6, 12, 12);

        public void Explode()
        {
            if (IsExploding) return;
            IsExploding = true;
            _explodeX = X;
            _explodeY = Y;
            _explodeAnim.Reset();
        }

        public void Draw(Graphics g)
        {
            if (IsExpired) return;

            if (IsExploding)
            {
                // Căn giữa explosion quanh tâm nổ
                _explodeAnim.Draw(g, _explodeX - 64, _explodeY - 64, false);

#if DEBUG
                using var pen = new Pen(Color.Red, 1f);
                g.DrawEllipse(pen,
                    _explodeX - ExplosionRadius, _explodeY - ExplosionRadius,
                    ExplosionRadius * 2, ExplosionRadius * 2);
#endif
            }
            else
            {
                // Vẽ quả lựu đạn — hình tròn nhỏ xám xịt
                using var body = new SolidBrush(Color.FromArgb(80, 80, 80));
                using var pin = new SolidBrush(Color.FromArgb(200, 180, 50));
                g.FillEllipse(body, X - 6, Y - 6, 12, 12);
                g.FillEllipse(pin, X - 2, Y - 9, 4, 4); // chốt

#if DEBUG
                using var penD = new Pen(Color.Orange, 1f);
                g.DrawRectangle(penD, Rectangle.Round(Hitbox));
#endif
            }
        }
    }
}