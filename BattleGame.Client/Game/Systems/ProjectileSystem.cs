using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BattleGame.Client.Game.Systems
{
    public class ProjectileSystem
    {
        private readonly List<Entity> _projectiles = new();
        private readonly List<Entity> _targets = new();
        private readonly Dictionary<string, SpriteAnimation> _animations;

        public ProjectileSystem(Dictionary<string, SpriteAnimation> animations)
        {
            _animations = animations;
        }

        // ===== REGISTER =====
        public void RegisterTarget(Entity entity)
        {
            if (!_targets.Contains(entity))
                _targets.Add(entity);
        }

        public void Spawn(Entity projectile) 
        { 
            _projectiles.Add(projectile);
            var p = projectile.Get<ProjectileComponent>();
            System.Diagnostics.Debug.WriteLine($"[ProjectileSystem.Spawn] Total projectiles: {_projectiles.Count}, Anim: {p.AnimationKey}");
        }

        // ===== UPDATE =====
        public void Update(float dt)
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                var e = _projectiles[i];
                var p = e.Get<ProjectileComponent>();

                // Di chuyển
                p.X += p.VelocityX * dt;
                p.Y += p.VelocityY * dt;

                // Advance animation frame
                if (!string.IsNullOrEmpty(p.AnimationKey) &&
                    _animations.TryGetValue(p.AnimationKey, out var anim))
                {
                    p.FrameTimer += dt;
                    if (p.FrameTimer >= anim.FrameDuration)
                    {
                        p.FrameTimer = 0f;
                        p.CurrentFrame = (p.CurrentFrame + 1) % anim.Frames.Length;
                    }
                }

                // Lifetime
                p.Timer += dt;
                if (p.Timer >= p.Lifetime)
                {
                    _projectiles.RemoveAt(i);
                    continue;
                }

                // Collision
                foreach (var target in _targets)
                {
                    if (target == p.Owner) continue;

                    var mv = target.Get<MovementComponent>();
                    var ch = target.Get<CharacterComponent>();
                    if (ch.IsDead) continue;

                    if (CheckHit(p, mv))
                    {
                        ApplyDamage(target, p);
                        p.IsDestroyed = true;
                        break;
                    }
                }

                if (p.IsDestroyed)
                    _projectiles.RemoveAt(i);
            }
        }

        // ===== DRAW =====
        public void Draw(Graphics g)
        {
            System.Diagnostics.Debug.WriteLine($"[ProjectileSystem.Draw] Drawing {_projectiles.Count} projectiles");
            foreach (var e in _projectiles)
            {
                var p = e.Get<ProjectileComponent>();
                if (p.IsDestroyed) continue;

                if (string.IsNullOrEmpty(p.AnimationKey) ||
                    !_animations.TryGetValue(p.AnimationKey, out var anim)) continue;

                var frame = anim.Frames[Math.Min(p.CurrentFrame, anim.Frames.Length - 1)];
                int drawW = frame.Width;
                int drawH = frame.Height;
                int drawX = (int)p.X - drawW / 2;
                int drawY = (int)p.Y - drawH / 2;

                var state = g.Save();

                if (p.VelocityX >= 0)
                {
                    g.DrawImage(frame, drawX, drawY, drawW, drawH);
                }
                else
                {
                    g.TranslateTransform(drawX + drawW / 2f, drawY + drawH / 2f);
                    g.ScaleTransform(-1, 1);
                    g.DrawImage(frame, -drawW / 2, -drawH / 2, drawW, drawH);
                }

                g.Restore(state);
            }
        }

        // ===== HITBOX =====
        private bool CheckHit(ProjectileComponent p, MovementComponent mv)
        {
            float dx = Math.Abs(p.X - mv.X);
            float dy = Math.Abs(p.Y - mv.Y);
            // Dùng range từ effect config thay vì hardcode
            return dx < p.Range && dy < p.Range * 1.6f;
        }

        private void ApplyDamage(Entity target, ProjectileComponent p)
        {
            var ch = target.Get<CharacterComponent>();

            int dmg = Math.Max(0, p.Damage - ch.BaseStats.Def);
            ch.Hp = Math.Max(0, ch.Hp - dmg);

            ch.IsHurt = true;
            ch.HurtTimer = ch.HurtDuration;

            if (p.StunDuration > 0)
            {
                ch.IsStunned = true;
                ch.StunTimer = p.StunDuration;
            }

            if (ch.Hp <= 0) ch.IsDead = true;
        }
    }
}