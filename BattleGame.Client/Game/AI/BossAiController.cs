using System;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Systems;

namespace BattleGame.Client.Game.AI;

public sealed class BossAiController
{
    private const float StopChaseRangeBuffer = 30f;

    private readonly BossAiProfile _profile;
    private float _actionCooldown;
    private float _dashCooldown;
    private int _dashComboNextSkill;
    private bool _wasBusy;

    public BossAiController(BossAiProfile profile)
    {
        _profile = profile;
    }

    public void Update(float dt, Entity boss, Entity target, CombatSystem combat)
    {
        var bossMv = boss.Get<MovementComponent>();
        var bossCh = boss.Get<CharacterComponent>();
        var targetMv = target.Get<MovementComponent>();
        var targetCh = target.Get<CharacterComponent>();

        if (bossCh.IsDead || targetCh.IsDead)
            return;

        _dashCooldown = Math.Max(0f, _dashCooldown - dt);

        float dx = targetMv.X - bossMv.X;
        float absDx = Math.Abs(dx);
        bossMv.FacingRight = dx >= 0f;

        if (bossCh.IsBusy)
        {
            _wasBusy = true;
            bossMv.VelocityX = 0f;
            return;
        }

        if (_wasBusy)
        {
            _wasBusy = false;
        }
        else
        {
            _actionCooldown = Math.Max(0f, _actionCooldown - dt);
        }

        float stopChaseRange = ResolveStopChaseRange(bossCh);
        float chaseStopDistance = Math.Max(0f, stopChaseRange - StopChaseRangeBuffer);
        float basicAttackRange = ResolveBasicAttackRange(bossCh, stopChaseRange);

        if (TryContinueDashCombo(boss, bossMv, combat, bossCh, absDx))
            return;

        if (TrySkill(boss, bossMv, combat, bossCh, absDx, slot: 2, _profile.Skill2Range))
            return;

        if (TrySkill(boss, bossMv, combat, bossCh, absDx, slot: 1, _profile.Skill1Range))
            return;

        if (absDx <= basicAttackRange && TryBasicAttack(boss, bossMv, combat))
            return;

        if (absDx > chaseStopDistance)
        {
            bool canQueueDashCombo = CanQueueDashCombo(bossCh, absDx);
            if (TryDashTowardTarget(bossCh, bossMv, absDx, chaseStopDistance))
            {
                if (canQueueDashCombo)
                    _dashComboNextSkill = _profile.DashComboFirstSkill;

                return;
            }

            float dir = dx >= 0f ? 1f : -1f;
            bossMv.VelocityX = dir * bossMv.Speed * _profile.ChaseSpeedMultiplier;
            return;
        }

        bossMv.VelocityX = 0f;

        TryBasicAttack(boss, bossMv, combat);
    }

    private bool TryContinueDashCombo(Entity boss, MovementComponent bossMv, CombatSystem combat, CharacterComponent bossCh, float distance)
    {
        if (_dashComboNextSkill == 0)
            return false;

        int slot = _dashComboNextSkill;
        float range = slot == 2 ? _profile.Skill2Range : _profile.Skill1Range;
        bool used = slot == 2
            ? TrySkill(boss, bossMv, combat, bossCh, distance, slot, range)
            : TrySkill(boss, bossMv, combat, bossCh, distance, slot, range);

        if (!used)
        {
            if (distance <= range)
                _dashComboNextSkill = 0;

            return false;
        }

        _dashComboNextSkill = slot == _profile.DashComboFirstSkill
            ? _profile.DashComboSecondSkill
            : 0;
        return true;
    }

    private bool TryDashTowardTarget(CharacterComponent bossCh, MovementComponent bossMv, float distance, float stopChaseRange)
    {
        if (!_profile.CanDash || _dashCooldown > 0f || distance < _profile.DashMinRange)
            return false;

        if (_profile.DashMaxRange > 0f && distance > _profile.DashMaxRange)
            return false;

        string dashAnimation = ResolveDashAnimation(bossCh);
        if (string.IsNullOrWhiteSpace(dashAnimation))
            return false;

        float dashStopRange = _profile.DashStopRange > 0f
            ? _profile.DashStopRange
            : stopChaseRange;
        float dashSpeed = Math.Max(1f, bossMv.Speed * _profile.DashSpeedMultiplier);
        float smartDuration = Math.Max(0.05f, (distance - dashStopRange) / dashSpeed);
        float dashDuration = Math.Clamp(smartDuration, 0.05f, _profile.DashDuration);
        float duration = bossCh.GetAnimationDuration(dashAnimation, dashDuration);
        bossCh.IsDashing = true;
        bossCh.DashTimer = dashDuration;
        bossCh.DashDuration = dashDuration;
        bossCh.ActionTimer = duration;
        bossCh.ActionDuration = duration;
        bossCh.DashSpeedMultiplier = _profile.DashSpeedMultiplier;
        bossMv.VelocityX = (bossMv.FacingRight ? 1f : -1f) * bossMv.Speed * bossCh.DashSpeedMultiplier;
        _dashCooldown = _profile.DashCooldown;
        return true;
    }

    private bool CanQueueDashCombo(CharacterComponent bossCh, float distance)
    {
        if (!_profile.DashComboOnDash)
            return false;

        return IsSkillReady(bossCh, _profile.DashComboFirstSkill, distance)
            && IsSkillReady(bossCh, _profile.DashComboSecondSkill, distance);
    }

    private bool IsSkillReady(CharacterComponent bossCh, int slot, float distance)
    {
        return slot switch
        {
            1 => bossCh.Skill1 != null && bossCh.Skill1Cooldown <= 0f && distance <= _profile.DashMaxRange,
            2 => bossCh.Skill2 != null && bossCh.Skill2Cooldown <= 0f && distance <= _profile.DashMaxRange,
            _ => false
        };
    }

    private bool TryBasicAttack(Entity boss, MovementComponent bossMv, CombatSystem combat)
    {
        if (_actionCooldown > 0f)
            return false;

        bossMv.VelocityX = 0f;
        combat.Attack(boss);
        _actionCooldown = _profile.BasicAttackCooldown;
        _wasBusy = true;
        return true;
    }

    private bool TrySkill(
        Entity boss,
        MovementComponent bossMv,
        CombatSystem combat,
        CharacterComponent bossCh,
        float distance,
        int slot,
        float range)
    {
        if (range <= 0f || distance > range)
            return false;

        if (slot == 1 && bossCh.Skill1 == null)
            return false;

        if (slot == 2 && bossCh.Skill2 == null)
            return false;

        bossMv.VelocityX = 0f;
        if (!combat.UseSkill(boss, slot))
            return false;

        _actionCooldown = _profile.PostSkillActionCooldown;
        _wasBusy = true;
        return true;
    }

    private float ResolveStopChaseRange(CharacterComponent bossCh)
    {
        if (_profile.StopChaseRange > 0f)
            return _profile.StopChaseRange;

        return bossCh.BaseStats.AttackRange + _profile.EngageRangeBonus;
    }

    private float ResolveBasicAttackRange(CharacterComponent bossCh, float stopChaseRange)
    {
        if (_profile.BasicAttackRange > 0f)
            return _profile.BasicAttackRange;

        if (_profile.ChaseAttackRange > 0f)
            return _profile.ChaseAttackRange;

        return Math.Max(stopChaseRange, bossCh.BaseStats.AttackRange);
    }

    private static string ResolveDashAnimation(CharacterComponent ch)
    {
        if (ch.AvailableAnimations.Contains("Dash"))
            return "Dash";
        if (ch.AvailableAnimations.Contains("Run"))
            return "Run";
        if (ch.AvailableAnimations.Contains("Walk"))
            return "Walk";

        return string.Empty;
    }
}
