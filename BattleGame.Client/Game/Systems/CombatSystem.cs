using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Gameplay;
using BattleGame.Shared.Models;
using System;

namespace BattleGame.Client.Game.Systems
{
    public class CombatSystem
    {
        private static readonly Random _rng = new();

        private readonly ProjectileSystem _projectileSystem;
        private Entity? _target;

        public CombatSystem(ProjectileSystem projectileSystem)
        {
            _projectileSystem = projectileSystem;
        }

        public void SetTarget(Entity target) => _target = target;

        // ===== RANGE CHECK =====
        private bool IsInRange(MovementComponent attacker, MovementComponent target, float range)
        {
            float distance = Math.Abs(attacker.X - target.X);
            return distance < range;
        }

        public void Update(Entity entity, float deltaTime)
        {
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();

            // ===== ATTACK =====
            if (ch.IsAttacking)
            {
                ch.ActionTimer -= deltaTime;

                if (!ch.AttackHitDone && sp.CurrentFrame >= ch.AttackHitFrame)
                {
                    if (_target != null)
                    {
                        var attMv = entity.Get<MovementComponent>();
                        var tgtMv = _target.Get<MovementComponent>();

                        // Chỉ gây damage nếu còn trong phạm vi
                        if (IsInRange(attMv, tgtMv, ch.BaseStats.AttackRange))
                            TakeDamage(_target, ch.BaseStats.Atk);
                    }
                    ch.AttackHitDone = true;
                }

                if (ch.ActionTimer <= 0 || sp.AnimationFinished)
                    ch.IsAttacking = false;
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

            var sp = attacker.Get<SpriteComponent>();
            sp.CurrentAnimation = ch.CurrentAttackAnim;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0;
            sp.AnimationFinished = false;
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
            switch (e.Trigger)
            {
                case "onStart":
                    return !ch.TriggeredEffects.Contains(idx);

                case "onEnd":
                    bool triggered = sp.AnimationFinished && !ch.TriggeredEffects.Contains(idx);
                    if (triggered)
                        System.Diagnostics.Debug.WriteLine($"[ShouldTrigger] onEnd triggered! AnimFinished={sp.AnimationFinished}, NotInTriggered={!ch.TriggeredEffects.Contains(idx)}");
                    return triggered;

                case "onFrame":
                    return e.Frames != null &&
                           e.Frames.Count > 0 &&
                           sp.CurrentFrame == e.Frames[0] &&
                           !ch.TriggeredEffects.Contains(idx);

                case "onFrames":
                    return e.Frames != null &&
                           e.Frames.Contains(sp.CurrentFrame) &&
                           !ch.TriggeredFrames.Contains((idx, sp.CurrentFrame));

                default:
                    return false;
            }
        }

        // ================= EXECUTE =================

        private void ExecuteEffect(Entity caster, EffectData e, CharacterComponent ch,
                                   SpriteComponent sp, int idx)
        {
            // Track đã trigger
            if (e.Trigger == "onFrames")
                ch.TriggeredFrames.Add((idx, sp.CurrentFrame));
            else
                ch.TriggeredEffects.Add(idx);

            // Thực thi
            switch (e.Type)
            {
                case "melee":
                    if (_target != null)
                    {
                        var casterMv = caster.Get<MovementComponent>();
                        var targetMv = _target.Get<MovementComponent>();

                        // Chỉ gây damage nếu còn trong phạm vi
                        if (IsInRange(casterMv, targetMv, e.Range))
                            TakeDamage(_target, e.Damage, e.Stun);
                    }
                    break;

                case "projectile":
                    System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Creating projectile: {e.ProjectileAnim}");
                    var proj = ProjectileFactory.Create(caster, e);
                    _projectileSystem.Spawn(proj);
                    System.Diagnostics.Debug.WriteLine($"[ExecuteEffect] Projectile spawned at ({proj.Get<ProjectileComponent>().X}, {proj.Get<ProjectileComponent>().Y})");
                    break;
            }
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