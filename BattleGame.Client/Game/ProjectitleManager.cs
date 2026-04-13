using BattleGame.Client.Game.Characters;
using BattleGame.Client.Game.Projectitles;
using System.Collections.Generic;
using System.Drawing;

namespace BattleGame.Client.Game
{
    /// <summary>
    /// Quản lý toàn bộ đạn và lựu đạn trên màn hình.
    /// GameEngine tạo 1 instance duy nhất, truyền vào ShootSkill và GrenadeSkill.
    /// </summary>
    public class ProjectileManager
    {
        private readonly List<Bullet> _bullets = new();
        private readonly List<Grenade> _grenades = new();

        // ═══════════════════════════════════════
        //  SPAWN
        // ═══════════════════════════════════════
        public void SpawnBullet(Soldier shooter)
        {
            float spawnX = shooter.FacingRight
                ? shooter.X + 90    // đầu nòng súng bên phải
                : shooter.X - 20;   // đầu nòng súng bên trái
            float spawnY = shooter.Y + 80;  // ngang tầm ngực

            _bullets.Add(new Bullet(spawnX, spawnY, shooter.FacingRight, shooter.ATK * 2));
        }

        public void SpawnGrenade(Soldier thrower)
        {
            float spawnX = thrower.FacingRight
                ? thrower.X + 70
                : thrower.X + 40;
            float spawnY = thrower.Y + 30;

            _grenades.Add(new Grenade(spawnX, spawnY, thrower.FacingRight, thrower.ATK * 5));
        }

        // ═══════════════════════════════════════
        //  UPDATE
        // ═══════════════════════════════════════
        public void Update()
        {
            foreach (var b in _bullets) b.Update();
            foreach (var g in _grenades) g.Update();

            // Dọn object đã hết hạn
            _bullets.RemoveAll(b => b.IsExpired);
            _grenades.RemoveAll(g => g.IsExpired);
        }

        // ═══════════════════════════════════════
        //  COLLISION CHECK
        //  Gọi từ GameEngine sau Update(), truyền vào danh sách enemy
        // ═══════════════════════════════════════
        public void CheckHits(IEnumerable<Soldier> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead()) continue;

                // Bullet hit
                foreach (var bullet in _bullets)
                {
                    if (bullet.IsExpired) continue;
                    if (bullet.Hitbox.IntersectsWith(enemy.Hitbox))
                    {
                        enemy.TakeDamage(bullet.Damage);
                        bullet.OnHit();
                    }
                }

                // Grenade AOE hit (chỉ khi đang nổ)
                foreach (var grenade in _grenades)
                {
                    if (grenade.IsExpired || !grenade.IsExploding) continue;

                    float dx = enemy.Hitbox.X + enemy.Hitbox.Width / 2 - grenade.ExplosionCenter.X;
                    float dy = enemy.Hitbox.Y + enemy.Hitbox.Height / 2 - grenade.ExplosionCenter.Y;
                    float dist = System.MathF.Sqrt(dx * dx + dy * dy);

                    if (dist <= grenade.ExplosionRadius)
                    {
                        // Damage giảm dần theo khoảng cách
                        float falloff = 1f - (dist / grenade.ExplosionRadius);
                        int dmg = (int)(grenade.Damage * falloff);
                        enemy.TakeDamage(dmg);
                    }
                }
            }
        }

        // ═══════════════════════════════════════
        //  DRAW
        // ═══════════════════════════════════════
        public void Draw(Graphics g)
        {
            foreach (var b in _bullets) b.Draw(g);
            foreach (var gr in _grenades) gr.Draw(g);
        }
    }
}