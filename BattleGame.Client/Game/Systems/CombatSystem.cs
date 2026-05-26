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
            return CharacterHitbox.IntersectsRectangle(targetMv, bc.X, bc.Y, bc.CollisionWidth, bc.CollisionHeight);
        }

        private static float GetHorizontalGap(Entity a, Entity b)
        {
            var aMv = a.Get<MovementComponent>();
            var bMv = b.Get<MovementComponent>();

            return CharacterHitbox.GetHorizontalGap(aMv, bMv);
        }

        private static bool IsBaseAttackHit(Entity attacker, Entity target, float attackRange)
        {
            var attackerMv = attacker.Get<MovementComponent>();
            var targetMv = target.Get<MovementComponent>();
            float width = Math.Max(1f, attackRange);
            float centerX = attackerMv.X + (attackerMv.FacingRight ? 1f : -1f) * (CharacterHitbox.Width / 2f + width / 2f);

            return CharacterHitbox.IntersectsRectangle(targetMv, centerX, attackerMv.Y, width, CharacterHitbox.Height);
        }

        private static bool IsMeleeEffectHit(Entity caster, Entity target, EffectData effect)
        {
            var targetMv = target.Get<MovementComponent>();
            if (IsCasterBothSpawn(effect))
            {
                var casterMv = caster.Get<MovementComponent>();
                float offsetX = MathF.Abs(effect.SpawnOffsetX);
                return CharacterHitbox.IntersectsRectangle(targetMv, casterMv.X + offsetX, casterMv.Y + effect.SpawnOffsetY, effect.CollisionWidth, effect.CollisionHeight)
                    || CharacterHitbox.IntersectsRectangle(targetMv, casterMv.X - offsetX, casterMv.Y + effect.SpawnOffsetY, effect.CollisionWidth, effect.CollisionHeight);
            }

            var spawn = ResolveBarrierSpawnPosition(caster, target, effect);
            return CharacterHitbox.IntersectsRectangle(targetMv, spawn.X, spawn.Y, effect.CollisionWidth, effect.CollisionHeight);
        }

        private static bool IsCasterBothSpawn(EffectData effect)
        {
            string mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();
            return mode is "casterboth" or "casteraround";
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
                ? attackerMv.X > targetMv.X
                : attackerMv.X < targetMv.X;
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
                        // Chỉ gây damage nếu còn trong phạm vi và không bị barrier chặn
                        if (IsBaseAttackHit(entity, _target, ch.BaseStats.AttackRange)
                            && !IsBlockedByBarrier(entity, _target, "melee")
                            && !IsBlockedByProtection(entity, _target))
                            TakeDamage(_target, ch.BaseStats.Atk, armorPen: ch.BaseStats.ArmorPen);
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
                            ch.BaseStats.ArmorPen,
                            ch.BaseStats.AttackProjectileSpeed,
                            ch.BaseStats.AttackProjectileSpawnOffsetX,
                            ch.BaseStats.AttackProjectileSpawnOffsetY,
                            ch.BaseStats.AttackProjectileScale
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
            if (ch.Skill1Cooldown > 0) ch.Skill1Cooldown = Math.Max(0f, ch.Skill1Cooldown - deltaTime);
            if (ch.Skill2Cooldown > 0) ch.Skill2Cooldown = Math.Max(0f, ch.Skill2Cooldown - deltaTime);
            RegenerateMana(ch, deltaTime);

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
                        bool canApplyDamage = bc.HitFrames.Count == 0
                            ? bc.LastDamageFrame < 0
                            : bc.HitFrames.Contains(currentFrame) && bc.LastDamageFrame != currentFrame;

                        if (canApplyDamage)
                        {
                            if (!IsBlockedByProtection(bc.Owner, _target))
                                TakeDamage(_target, bc.Damage, bc.Stun, bc.ArmorPen);

                            bc.LastDamageFrame = currentFrame;
                        }
                    }
                }

                bc.RemainingTime -= deltaTime;
                if (bc.RemainingTime <= 0)
                    _barriers.RemoveAt(i);
            }
        }

        private static void RegenerateMana(CharacterComponent ch, float deltaTime)
        {
            int maxMana = Math.Max(0, ch.BaseStats.Mana);
            float regen = Math.Max(0f, ch.BaseStats.ManaRegen);
            if (ch.IsDead || maxMana == 0 || regen <= 0f || ch.Mana >= maxMana)
            {
                ch.ManaRegenAccumulator = 0f;
                return;
            }

            ch.ManaRegenAccumulator += regen * deltaTime;
            int gained = (int)Math.Floor(ch.ManaRegenAccumulator);
            if (gained <= 0)
                return;

            ch.Mana = Math.Min(maxMana, ch.Mana + gained);
            ch.ManaRegenAccumulator -= gained;
            if (ch.Mana >= maxMana)
                ch.ManaRegenAccumulator = 0f;
        }

        // ================= ACTION =================

        public void Attack(Entity attacker)
        {
            var ch = attacker.Get<CharacterComponent>();
            if (ch.IsBusy) return;

            string attackAnimation = ResolveAttackAnimation(ch);
            if (string.IsNullOrWhiteSpace(attackAnimation))
                return;

            float duration = ch.GetAnimationDuration(attackAnimation, ch.ActionDuration);
            ch.CurrentAttackAnim = attackAnimation;
            ch.IsAttacking = true;
            ch.ActionTimer = duration;
            ch.ActionDuration = duration;
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
            if (!string.IsNullOrWhiteSpace(e.Animation) &&
                !string.Equals(e.Animation, ch.CurrentAttackAnim, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var trigger = (e.Trigger ?? string.Empty).Trim().ToLowerInvariant();

            switch (trigger)
            {
                case "onstart":
                    return !ch.TriggeredAttackEffects.Contains(idx);

                case "onend":
                    return sp.AnimationFinished && !ch.TriggeredAttackEffects.Contains(idx);

                case "onframe":
                    var onFrameFrames = e.TriggerFrames ?? e.Frames;
                    return onFrameFrames != null &&
                           onFrameFrames.Count > 0 &&
                           sp.CurrentFrame == onFrameFrames[0] &&
                           !ch.TriggeredAttackEffects.Contains(idx);

                case "onframes":
                    var onFramesFrames = e.TriggerFrames ?? e.Frames;
                    return onFramesFrames != null &&
                           onFramesFrames.Contains(sp.CurrentFrame) &&
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

        public bool UseSkill(Entity caster, int slot)
        {
            var ch = caster.Get<CharacterComponent>();
            if (ch.IsBusy) return false;

            var skill = slot == 1 ? ch.Skill1 : ch.Skill2;
            if (skill == null) return false;

            float cd = slot == 1 ? ch.Skill1Cooldown : ch.Skill2Cooldown;
            if (cd > 0 || ch.Mana < skill.ManaCost) return false;

            string animation = string.IsNullOrWhiteSpace(skill.Animation)
                ? $"Skill{slot}"
                : skill.Animation;
            if (!ch.AvailableAnimations.Contains(animation))
                return false;

            float duration = ch.GetAnimationDuration(animation, 0.7f);
            ch.Mana -= skill.ManaCost;
            ch.IsUsingSkill = true;
            ch.CurrentSkillSlot = slot;
            ch.CurrentSkillAnim = animation;
            ch.ActionTimer = duration;
            ch.ActionDuration = duration;
            ch.TriggeredEffects.Clear();
            ch.TriggeredFrames.Clear();

            var sp = caster.Get<SpriteComponent>();
            sp.CurrentAnimation = animation;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0;
            sp.AnimationFinished = false;

            if (slot == 1) ch.Skill1Cooldown = skill.Cooldown;
            else ch.Skill2Cooldown = skill.Cooldown;
            return true;
        }

        private static string ResolveAttackAnimation(CharacterComponent ch)
        {
            var attacks = ch.AvailableAnimations
                .Where(animation => animation.StartsWith("Attack_", StringComparison.OrdinalIgnoreCase))
                .OrderBy(animation => animation, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return attacks.Length == 0
                ? string.Empty
                : attacks[_rng.Next(attacks.Length)];
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
                    var onFrameFrames = e.TriggerFrames ?? e.Frames;
                    return onFrameFrames != null &&
                           onFrameFrames.Count > 0 &&
                           sp.CurrentFrame == onFrameFrames[0] &&
                           !ch.TriggeredEffects.Contains(idx);

                case "onframes":
                    var onFramesFrames = e.TriggerFrames ?? e.Frames;
                    bool shouldTriggerFrames = onFramesFrames != null &&
                           onFramesFrames.Contains(sp.CurrentFrame) &&
                           !ch.TriggeredFrames.Contains((idx, sp.CurrentFrame));
                    if (shouldTriggerFrames)
                        System.Diagnostics.Debug.WriteLine($"[ShouldTrigger] onFrames triggered! Frame={sp.CurrentFrame}, TriggerFrames={string.Join(",", onFramesFrames!)}");
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

            if (_target != null && !IsProjectileEffect(e) && ShouldBlockImmediateEffect(caster, _target, e.Type))
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

            if (_target != null && !IsProjectileEffect(e) && ShouldBlockImmediateEffect(caster, _target, e.Type))
                return;

            ApplyEffect(caster, e);
        }

        private bool ShouldBlockImmediateEffect(Entity caster, Entity target, string effectType)
        {
            if (IsBlockedByBarrier(caster, target, effectType))
                return true;

            if (string.Equals(effectType, "barrier", StringComparison.OrdinalIgnoreCase))
                return false;

            return IsBlockedByProtection(caster, target);
        }

        private static bool IsProjectileEffect(EffectData effect)
            => string.Equals(effect.Type, "projectile", StringComparison.OrdinalIgnoreCase);

        private void ApplyEffect(Entity caster, EffectData e)
        {
            switch (e.Type)
            {
                case "melee":
                    if (_target != null)
                    {
                        var casterMv = caster.Get<MovementComponent>();
                        var targetMv = _target.Get<MovementComponent>();

                        float horizontalGap = GetHorizontalGap(caster, _target);

                        System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Melee effect triggered. Caster at X={casterMv.X}, Target at X={targetMv.X}, Range={e.Range}, Gap={horizontalGap}");

                        // Chỉ gây damage nếu còn trong phạm vi
                        if (IsMeleeEffectHit(caster, _target, e))
                        {
                            System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Damage applied: {e.Damage}");
                            var armorPen = e.ArmorPen ?? caster.Get<CharacterComponent>().BaseStats.ArmorPen;
                            TakeDamage(_target, e.Damage, e.Stun, armorPen);
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
                ArmorPen = e.ArmorPen ?? caster.Get<CharacterComponent>().BaseStats.ArmorPen,
                Stun = e.Stun,
                HitFrames = e.HitFrames ?? e.Frames ?? new List<int>(),
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
            var mode = (e.SpawnMode ?? "between").Trim().ToLowerInvariant();

            if (target == null)
            {
                if (mode == "casterself")
                    return (casterMv.X + e.SpawnOffsetX, casterMv.Y + e.SpawnOffsetY);

                // For front-based spawns, follow caster facing even when no target exists (e.g. dungeon solo).
                if (mode == "casterfront" || mode == "targetfront")
                {
                    float dir = casterMv.FacingRight ? 1f : -1f;
                    return (casterMv.X + dir * e.SpawnOffsetX, casterMv.Y + e.SpawnOffsetY);
                }

                return (casterMv.X + e.SpawnOffsetX, casterMv.Y + e.SpawnOffsetY);
            }
            var targetMv = target.Get<MovementComponent>();

            return mode switch
            {
                "casterself" => (
                    casterMv.X + e.SpawnOffsetX,
                    casterMv.Y + e.SpawnOffsetY),
                "targetfront" => (
                    targetMv.X + (casterMv.X < targetMv.X ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    targetMv.Y + e.SpawnOffsetY),
                "casterfront" => (
                    casterMv.X + (casterMv.FacingRight ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    casterMv.Y + e.SpawnOffsetY),
                _ => (
                    ((casterMv.X + targetMv.X) * 0.5f) + (targetMv.X >= casterMv.X ? e.SpawnOffsetX : -e.SpawnOffsetX),
                    targetMv.Y + e.SpawnOffsetY)
            };
        }

        // ================= DAMAGE =================

        public void TakeDamage(Entity target, float rawDamage, float stun = 0f, int armorPen = 0)
        {
            var ch = target.Get<CharacterComponent>();
            if (ch.IsDead || ch.IsInvulnerable) return;

            int damage = CalculateDamage(rawDamage, ch.BaseStats.Def, armorPen);
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

        private static int CalculateDamage(float rawDamage, int targetDef, int armorPen)
        {
            if (rawDamage <= 0)
                return 0;

            int effectiveDef = Math.Max(0, targetDef - Math.Max(0, armorPen));
            return Math.Max(1, (int)MathF.Round(rawDamage - effectiveDef, MidpointRounding.AwayFromZero));
        }
    }
}
