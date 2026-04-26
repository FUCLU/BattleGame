using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Rendering;
using BattleGame.Shared.Models;
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
        private Func<IEnumerable<Entity>> _barrierProvider = () => Array.Empty<Entity>();

        public ProjectileSystem(Dictionary<string, SpriteAnimation> animations)
        {
            _animations = animations;
        }

        public void SetBarrierProvider(Func<IEnumerable<Entity>> barrierProvider) => _barrierProvider = barrierProvider;

        // ===== REGISTER =====
        public void RegisterTarget(Entity entity)
        {
            if (!_targets.Contains(entity))
                _targets.Add(entity);
        }

        public void Spawn(Entity projectile) 
        { 
            _projectiles.Add(projectile);
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

                if (IsBlockedByBarrier(p))
                {
                    var visualCenter = GetCollisionPoint(p);
                    System.Diagnostics.Debug.WriteLine($"[ProjectileSystem] Projectile blocked by barrier at ({visualCenter.X}, {visualCenter.Y})");
                    p.IsDestroyed = true;
                }

                // Lifetime
                p.Timer += dt;
                if (p.Timer >= p.Lifetime)
                {
                    _projectiles.RemoveAt(i);
                    continue;
                }

                // Collision
                if (!p.IsDestroyed)
                {
                    foreach (var target in _targets)
                    {
                        if (target == p.Owner) continue;

                        var mv = target.Get<MovementComponent>();
                        var ch = target.Get<CharacterComponent>();
                        if (ch.IsDead) continue;

                        if (CheckHit(p, mv))
                        {
                            if (IsBlockedByProtection(p, target))
                            {
                                p.IsDestroyed = true;
                                break;
                            }

                            var visualCenter = GetCollisionPoint(p);
                            System.Diagnostics.Debug.WriteLine($"[ProjectileSystem] Collision! Projectile at ({visualCenter.X}, {visualCenter.Y}), Target at ({mv.X}, {mv.Y}), Range={p.Range}");
                            ApplyDamage(target, p);
                            p.IsDestroyed = true;
                            break;
                        }
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
                int baseW = p.Render.UseSpriteSize ? frame.Width : 80;
                int baseH = p.Render.UseSpriteSize ? frame.Height : 80;
                int drawW = (int)MathF.Round(baseW * p.Render.Scale);
                int drawH = (int)MathF.Round(baseH * p.Render.Scale);
                int drawX = (int)MathF.Round(p.X + p.Render.OffsetX - drawW / 2f);
                int drawY = ResolveDrawY(p.Y, drawH, p.Render);

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

        private static int ResolveDrawY(float y, int drawHeight, EffectRenderData render)
        {
            float finalY = (render.AlignY ?? "center").Trim().ToLowerInvariant() switch
            {
                "bottom" => y + render.OffsetY - drawHeight,
                "top" => y + render.OffsetY,
                _ => y + render.OffsetY - drawHeight / 2f
            };

            return (int)MathF.Round(finalY);
        }

        private bool IsBlockedByBarrier(ProjectileComponent p)
        {
            var collisionPoint = GetCollisionPoint(p);

            foreach (var barrier in _barrierProvider())
            {
                var bc = barrier.Get<BarrierComponent>();
                if (!bc.IsActive || bc.RemainingTime <= 0)
                    continue;

                if (bc.Owner == p.Owner || !bc.BlockEnemyProjectile)
                    continue;

                if (IsInsideBarrierFrame(collisionPoint.X, collisionPoint.Y, bc))
                    return true;
            }

            return false;
        }

        private static bool IsInsideBarrierFrame(float x, float y, BarrierComponent bc)
        {
            float halfW = bc.CollisionWidth / 2f;
            float halfH = bc.CollisionHeight / 2f;
            return x >= bc.X - halfW && x <= bc.X + halfW && y >= bc.Y - halfH && y <= bc.Y + halfH;
        }

        private static bool IsBlockedByProtection(ProjectileComponent projectile, Entity target)
        {
            var targetCh = target.Get<CharacterComponent>();
            if (!targetCh.IsProtecting || targetCh.IsDead)
                return false;

            if (targetCh.Render.ProtectionBlocksAllDirections)
                return true;

            var targetMv = target.Get<MovementComponent>();
            var collisionPoint = GetCollisionPoint(projectile);

            return targetMv.FacingRight
                ? collisionPoint.X >= targetMv.X
                : collisionPoint.X <= targetMv.X;
        }

        // ===== HITBOX =====
        private bool CheckHit(ProjectileComponent p, MovementComponent mv)
        {
            var collisionPoint = GetCollisionPoint(p);
            float dx = Math.Abs(collisionPoint.X - mv.X);
            float dy = Math.Abs(collisionPoint.Y - mv.Y);
            // Dùng range từ effect config thay vì hardcode
            return dx < p.Range && dy < p.Range * 1.6f;
        }

        private static (float X, float Y) GetCollisionPoint(ProjectileComponent p)
        {
            return (p.X + p.Render.OffsetX, p.Y + p.Render.OffsetY);
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
