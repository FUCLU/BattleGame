using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Gameplay;
using BattleGame.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleGame.Client.Game.Systems
{
    public class CombatSystem
    {
        private static readonly Random _rng = new();
        private const float CharacterCollisionWidth = 128f;
        private const float CharacterCollisionHeight = 128f;

        private readonly ProjectileSystem _projectileSystem;
        private Entity? _target;
        private readonly List<Entity> _barriers = new();
        private Func<IEnumerable<Entity>> _barrierProvider = () => Array.Empty<Entity>();

        public CombatSystem(ProjectileSystem projectileSystem)
        {
            _projectileSystem = projectileSystem;
        }

        public void SetTarget(Entity target) => _target = target;
        public List<Entity> GetBarriers() => _barriers;
        public void SetBarrierProvider(Func<IEnumerable<Entity>> barrierProvider) => _barrierProvider = barrierProvider;

        private static bool IntersectsBarrier(Entity target, BarrierComponent bc)
        {
            var targetMv = target.Get<MovementComponent>();
            float barrierHalfW = bc.CollisionWidth / 2f;
            float barrierHalfH = bc.CollisionHeight / 2f;
            float targetHalfW = CharacterCollisionWidth / 2f;
            float targetHalfH = CharacterCollisionHeight / 2f;

            return Math.Abs(targetMv.X - bc.X) <= barrierHalfW + targetHalfW &&
                   Math.Abs(targetMv.Y - bc.Y) <= barrierHalfH + targetHalfH;
        }

        private bool IsBlockedByBarrier(Entity attacker, Entity target, string effectType)
        {
            foreach (var barrier in _barrierProvider())
            {
                var bc = barrier.Get<BarrierComponent>();
                if (!bc.IsActive || bc.RemainingTime <= 0)
                    continue;

                if (bc.Owner != target)
                    continue;

                bool blocksEffect = effectType switch
                {
                    "projectile" => bc.BlockEnemyProjectile,
                    "melee" => bc.BlockEnemyAttack,
                    _ => bc.BlockEnemySkill
                };

                if (!blocksEffect)
                    continue;

                if (IntersectsBarrier(attacker, bc) || IntersectsBarrier(target, bc))
                    return true;
            }

            return false;
        }

        private static bool IsBlockedByProtection(Entity attacker, Entity target)
        {
            var targetCh = target.Get<CharacterComponent>();
            if (!targetCh.IsProtecting || targetCh.IsDead)
                return false;

            if (targetCh.Render.ProtectionBlocksAllDirections)
                return true;

            var attackerMv = attacker.Get<MovementComponent>();
            var targetMv = target.Get<MovementComponent>();

            return targetMv.FacingRight
                ? attackerMv.X >= targetMv.X
                : attackerMv.X <= targetMv.X;
        }

        public void Update(Entity entity, float deltaTime)
        {
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();

            // ===== ATTACK =====
            if (ch.IsAttacking)
            {
                ch.ActionTimer -= deltaTime;

                bool hasAttackEffects = ch.AttackEffects.Count > 0;

                if (hasAttackEffects)
                {
                    for (int i = 0; i < ch.AttackEffects.Count; i++)
                    {
                        var effect = ch.AttackEffects[i];
                        if (ShouldTriggerAttack(effect, sp, ch, i))
                            ExecuteAttackEffect(entity, effect, ch, sp, i);
                    }
                }
                else if (!ch.AttackHitDone && sp.CurrentFrame >= ch.AttackHitFrame)
                {
                    if (_target != null)
                    {
                        var attMv = entity.Get<MovementComponent>();
                        var tgtMv = _target.Get<MovementComponent>();

                        // Chỉ gây damage nếu còn trong phạm vi và không bị barrier chặn
                        if (Math.Abs(attMv.X - tgtMv.X) < ch.BaseStats.AttackRange
                            && !IsBlockedByBarrier(entity, _target, "melee")
                            && !IsBlockedByProtection(entity, _target))
                            TakeDamage(_target, ch.BaseStats.Atk);
                    }
                    ch.AttackHitDone = true;
                }

                if (ch.ActionTimer <= 0 || sp.AnimationFinished)
                {
                    // Bắn projectile khi attack kết thúc (nếu có)
                    System.Diagnostics.Debug.WriteLine($"[CombatSystem] Attack ending - AnimFinished={sp.AnimationFinished}, ActionTimer={ch.ActionTimer}, AttackProjectile={ch.BaseStats.AttackProjectile}, Speed={ch.BaseStats.AttackProjectileSpeed}");

                    // Legacy projectile path only for characters that have no config-driven attack effects.
                    if (!hasAttackEffects && ch.BaseStats.AttackProjectile != null && ch.BaseStats.AttackProjectileSpeed > 0)
                    {
                        var proj = ProjectileFactory.CreateAttackProjectile(
                            entity,
                            ch.BaseStats.AttackProjectile,
                            ch.BaseStats.Atk,
                            ch.BaseStats.AttackProjectileSpeed
                        );
                        _projectileSystem.Spawn(proj);
                        System.Diagnostics.Debug.WriteLine($"[CombatSystem] Attack projectile spawned: {ch.BaseStats.AttackProjectile}");
                    }
                    else if (!hasAttackEffects)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CombatSystem] Attack projectile NOT spawned - Check conditions!");
                    }
                    ch.IsAttacking = false;
                }
            }

            // ===== SKILL =====
            if (ch.IsUsingSkill)
            {
                var skill = ch.CurrentSkillSlot == 1 ? ch.Skill1 : ch.Skill2;

                if (skill?.Effects != null)
                {
                    for (int i = 0; i < skill.Effects.Count; i++)
                    {
                        var effect = skill.Effects[i];
                        if (ShouldTrigger(effect, sp, ch, i))
                            ExecuteEffect(entity, effect, ch, sp, i);
                    }
                }

                if (sp.AnimationFinished)
                    ch.IsUsingSkill = false;
            }

            // ===== HURT =====
            if (ch.IsHurt)
            {
                ch.HurtTimer -= deltaTime;
                if (ch.HurtTimer <= 0) ch.IsHurt = false;
            }

            // ===== STUN =====
            if (ch.IsStunned)
            {
                ch.StunTimer -= deltaTime;
                if (ch.StunTimer <= 0) ch.IsStunned = false;
            }

            // ===== COOLDOWN =====
            if (ch.Skill1Cooldown > 0) ch.Skill1Cooldown -= deltaTime;
            if (ch.Skill2Cooldown > 0) ch.Skill2Cooldown -= deltaTime;

            // ===== UPDATE BARRIERS =====
            for (int i = _barriers.Count - 1; i >= 0; i--)
            {
                var barrier = _barriers[i];
                var bc = barrier.Get<BarrierComponent>();
                var spBarrier = barrier.Get<SpriteComponent>();

                if (_target != null && bc.Owner == entity && bc.IsActive && bc.RemainingTime > 0)
                {
                    var targetCh = _target.Get<CharacterComponent>();
                    if (!targetCh.IsDead && IntersectsBarrier(_target, bc))
                    {
                        int currentFrame = spBarrier.CurrentFrame + 1;
                        bool frameIsConfigured = bc.HitFrames.Count == 0 || bc.HitFrames.Contains(currentFrame);

                        if (frameIsConfigured && bc.LastDamageFrame != currentFrame)
                        {
                            TakeDamage(_target, bc.Damage, 0f);
                            bc.LastDamageFrame = currentFrame;
                        }
                    }
                }

                bc.RemainingTime -= deltaTime;
                if (bc.RemainingTime <= 0)
                    _barriers.RemoveAt(i);
            }
        }

        // ================= ACTION =================

        public void Attack(Entity attacker)
        {
            var ch = attacker.Get<CharacterComponent>();
            if (ch.IsBusy) return;

            int idx = _rng.Next(1, ch.AttackAnimCount + 1);
            ch.CurrentAttackAnim = $"Attack_{idx}";
            ch.IsAttacking = true;
            ch.ActionTimer = ch.ActionDuration;
            ch.AttackHitDone = false;
            ch.TriggeredAttackEffects.Clear();
            ch.TriggeredAttackFrames.Clear();

            var sp = attacker.Get<SpriteComponent>();
            sp.CurrentAnimation = ch.CurrentAttackAnim;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0;
            sp.AnimationFinished = false;

            System.Diagnostics.Debug.WriteLine($"[CombatSystem.Attack] Attack started - Anim={ch.CurrentAttackAnim}, ActionDuration={ch.ActionDuration}, AttackProjectile={ch.BaseStats.AttackProjectile}");
        }

        private bool ShouldTriggerAttack(EffectData e, SpriteComponent sp, CharacterComponent ch, int idx)
        {
            var trigger = (e.Trigger ?? string.Empty).Trim().ToLowerInvariant();

            switch (trigger)
            {
                case "onstart":
                    return !ch.TriggeredAttackEffects.Contains(idx);

                case "onend":
                    return sp.AnimationFinished && !ch.TriggeredAttackEffects.Contains(idx);

                case "onframe":
                    return e.Frames != null &&
                           e.Frames.Count > 0 &&
                           sp.CurrentFrame == e.Frames[0] &&
                           !ch.TriggeredAttackEffects.Contains(idx);

                case "onframes":
                    return e.Frames != null &&
                           e.Frames.Contains(sp.CurrentFrame) &&
                           !ch.TriggeredAttackFrames.Contains((idx, sp.CurrentFrame));

                case "onmiddle":
                    if (sp.CurrentAnimationFrameCount <= 0)
                        return false;

                    int middleFrame = sp.CurrentAnimationFrameCount / 2;
                    return sp.CurrentFrame >= middleFrame && !ch.TriggeredAttackEffects.Contains(idx);

                default:
                    return false;
            }
        }

        public void UseSkill(Entity caster, int slot)
        {
            var ch = caster.Get<CharacterComponent>();
            if (ch.IsBusy) return;

            var skill = slot == 1 ? ch.Skill1 : ch.Skill2;
            if (skill == null) return;

            float cd = slot == 1 ? ch.Skill1Cooldown : ch.Skill2Cooldown;
            if (cd > 0 || ch.Mana < skill.ManaCost) return;

            ch.Mana -= skill.ManaCost;
            ch.IsUsingSkill = true;
            ch.CurrentSkillSlot = slot;
            ch.CurrentSkillAnim = skill.Animation;
            ch.TriggeredEffects.Clear();
            ch.TriggeredFrames.Clear();

            var sp = caster.Get<SpriteComponent>();
            sp.CurrentAnimation = skill.Animation;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0;
            sp.AnimationFinished = false;

            if (slot == 1) ch.Skill1Cooldown = skill.Cooldown;
            else ch.Skill2Cooldown = skill.Cooldown;
        }

        // ================= TRIGGER =================

        private bool ShouldTrigger(EffectData e, SpriteComponent sp, CharacterComponent ch, int idx)
        {
            var trigger = (e.Trigger ?? string.Empty).Trim().ToLowerInvariant();

            switch (trigger)
            {
                case "onstart":
                    return !ch.TriggeredEffects.Contains(idx);

                case "onend":
                    bool triggered = sp.AnimationFinished && !ch.TriggeredEffects.Contains(idx);
                    if (triggered)
                        System.Diagnostics.Debug.WriteLine($"[ShouldTrigger] onEnd triggered! AnimFinished={sp.AnimationFinished}, NotInTriggered={!ch.TriggeredEffects.Contains(idx)}");
                    return triggered;

                case "onframe":
                    return e.Frames != null &&
                           e.Frames.Count > 0 &&
                           sp.CurrentFrame == e.Frames[0] &&
                           !ch.TriggeredEffects.Contains(idx);

                case "onframes":
                    bool shouldTriggerFrames = e.Frames != null &&
                           e.Frames.Contains(sp.CurrentFrame) &&
                           !ch.TriggeredFrames.Contains((idx, sp.CurrentFrame));
                    if (shouldTriggerFrames)
                        System.Diagnostics.Debug.WriteLine($"[ShouldTrigger] onFrames triggered! Frame={sp.CurrentFrame}, Frames={string.Join(",", e.Frames)}");
                    return shouldTriggerFrames;

                case "onmiddle":
                    if (sp.CurrentAnimationFrameCount <= 0)
                        return false;

                    int middleFrame = sp.CurrentAnimationFrameCount / 2;
                    return sp.CurrentFrame >= middleFrame && !ch.TriggeredEffects.Contains(idx);

                default:
                    return false;
            }
        }

        // ================= EXECUTE =================

        private void ExecuteEffect(Entity caster, EffectData e, CharacterComponent ch,
                                   SpriteComponent sp, int idx)
        {
            // Track đã trigger
            if (string.Equals(e.Trigger, "onFrames", StringComparison.OrdinalIgnoreCase))
                ch.TriggeredFrames.Add((idx, sp.CurrentFrame));
            else
                ch.TriggeredEffects.Add(idx);

            if (_target != null && (IsBlockedByBarrier(caster, _target, e.Type) || IsBlockedByProtection(caster, _target)))
                return;

            // Thực thi
            ApplyEffect(caster, e);
        }

        private void ExecuteAttackEffect(Entity caster, EffectData e, CharacterComponent ch,
                                         SpriteComponent sp, int idx)
        {
            if (string.Equals(e.Trigger, "onFrames", StringComparison.OrdinalIgnoreCase))
                ch.TriggeredAttackFrames.Add((idx, sp.CurrentFrame));
            else
                ch.TriggeredAttackEffects.Add(idx);

            if (_target != null && (IsBlockedByBarrier(caster, _target, e.Type) || IsBlockedByProtection(caster, _target)))
                return;

            ApplyEffect(caster, e);
        }

        private void ApplyEffect(Entity caster, EffectData e)
        {
            switch (e.Type)
            {
                case "melee":
                    if (_target != null)
                    {
                        var casterMv = caster.Get<MovementComponent>();
                        var targetMv = _target.Get<MovementComponent>();

                        System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Melee effect triggered. Caster at X={casterMv.X}, Target at X={targetMv.X}, Range={e.Range}, Distance={Math.Abs(casterMv.X - targetMv.X)}");

                        // Chỉ gây damage nếu còn trong phạm vi
                        if (Math.Abs(casterMv.X - targetMv.X) < e.Range)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Damage applied: {e.Damage}");
                            TakeDamage(_target, e.Damage, e.Stun);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Out of range or blocked, no damage!");
                        }
                    }
                    break;

                case "projectile":
                    System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Creating projectile: {e.ProjectileAnim}");
                    var proj = ProjectileFactory.Create(caster, _target, e);
                    _projectileSystem.Spawn(proj);
                    System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Projectile spawned at ({proj.Get<ProjectileComponent>().X}, {proj.Get<ProjectileComponent>().Y})");
                    break;

                case "barrier":
                    ExecuteBarrier(caster, e);
                    break;
            }
        }

        private void ExecuteBarrier(Entity caster, EffectData e)
        {
            var spawn = ResolveBarrierSpawnPosition(caster, _target, e);
            bool facingRight = ResolveBarrierFacing(caster, _target, e);

            // Tạo barrier entity
            var barrier = new Entity(Guid.NewGuid().GetHashCode());
            barrier.Add(new BarrierComponent
            {
                IsActive = true,
                RemainingTime = e.Duration,
                MaxDuration = e.Duration,
                X = spawn.X,
                Y = spawn.Y,
                Damage = e.Damage,
                HitFrames = e.Frames ?? new List<int>(),
                CollisionWidth = e.CollisionWidth,
                CollisionHeight = e.CollisionHeight,
                BlockEnemyAttack = e.BlockEnemyAttack,
                BlockEnemyProjectile = e.BlockEnemyProjectile,
                BlockEnemySkill = e.BlockEnemySkill,
                AnimationKey = string.IsNullOrWhiteSpace(e.ObjectAnim) ? e.ProjectileAnim : e.ObjectAnim,
                Owner = caster,
                FacingRight = facingRight,
                Render = e.Render
            });

            barrier.Add(new SpriteComponent());

            _barriers.Add(barrier);
            System.Diagnostics.Debug.WriteLine($"[ExecuteBarrier] Barrier created at X={barrier.Get<BarrierComponent>().X}, Y={barrier.Get<BarrierComponent>().Y}, Anim={barrier.Get<BarrierComponent>().AnimationKey}");
        }

        private static bool ResolveBarrierFacing(Entity caster, Entity? target, EffectData effect)
        {
            var source = (effect.Render.FacingSource ?? "owner").Trim().ToLowerInvariant();

            return source switch
            {
                "target" when target != null => target.Get<MovementComponent>().FacingRight,
                "fixed" => true,
                _ => caster.Get<MovementComponent>().FacingRight
            };
        }

        private static (float X, float Y) ResolveBarrierSpawnPosition(Entity caster, Entity? target, EffectData e)
        {
            var casterMv = caster.Get<MovementComponent>();
            if (target == null)
                return (casterMv.X + e.SpawnOffsetX, casterMv.Y + e.SpawnOffsetY);

            var targetMv = target.Get<MovementComponent>();
            var mode = (e.SpawnMode ?? "between").Trim().ToLowerInvariant();

            return mode switch
            {
                "targetfront" => (
                    targetMv.X + (casterMv.X < targetMv.X ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    targetMv.Y + e.SpawnOffsetY),
                "casterfront" => (
                    casterMv.X + (casterMv.X < targetMv.X ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    casterMv.Y + e.SpawnOffsetY),
                _ => (
                    ((casterMv.X + targetMv.X) * 0.5f) + (targetMv.X >= casterMv.X ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    targetMv.Y + e.SpawnOffsetY)
            };
        }

        // ================= DAMAGE =================

        public void TakeDamage(Entity target, int rawDamage, float stun = 0f)
        {
            var ch = target.Get<CharacterComponent>();
            if (ch.IsDead) return;

            int damage = Math.Max(0, rawDamage - ch.BaseStats.Def);
            ch.Hp = Math.Max(0, ch.Hp - damage);

            ch.IsHurt = true;
            ch.HurtTimer = ch.HurtDuration;

            if (stun > 0)
            {
                ch.IsStunned = true;
                ch.StunTimer = stun;
            }

            if (ch.Hp <= 0) ch.IsDead = true;
        }
    }
}
