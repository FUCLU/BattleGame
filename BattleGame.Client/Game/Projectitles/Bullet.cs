using System.Drawing;

namespace BattleGame.Client.Game.Projectitles
{
    public class Bullet
    {
        private const float Speed = 12f;
        private const float MaxTravel = 900f;

        public float X { get; private set; }
        public float Y { get; private set; }
        public bool IsExpired { get; private set; } = false;
        public int Damage { get; private set; }

        private readonly float _dirX;
        private float _traveled = 0f;

        // Kích thước viên đạn vẽ
        private const int W = 14;
        private const int H = 5;

        public Bullet(float x, float y, bool facingRight, int damage)
        {
            X = x;
            Y = y;
            _dirX = facingRight ? 1f : -1f;
            Damage = damage;
        }

        public void Update()
        {
            if (IsExpired) return;
            X += _dirX * Speed;
            _traveled += Speed;
            if (_traveled >= MaxTravel) IsExpired = true;
        }

        public RectangleF Hitbox => new RectangleF(X, Y, W, H);

        public void OnHit() => IsExpired = true;

        public void Draw(Graphics g)
        {
            if (IsExpired) return;

            // Thân đạn — vàng sáng
            using var brush = new SolidBrush(Color.FromArgb(255, 220, 80));
            g.FillRectangle(brush, X, Y, W, H);

            // Đuôi đạn — trắng mờ
            using var tail = new SolidBrush(Color.FromArgb(120, 255, 255, 200));
            float tailX = _dirX > 0 ? X - 10 : X + W;
            g.FillRectangle(tail, tailX, Y + 1, 10, H - 2);
        }
    }
}