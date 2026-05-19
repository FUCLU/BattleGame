using System;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Systems;

namespace BattleGame.Client.Game.AI;

public sealed class BossAiController
{
    private readonly BossAiProfile _profile;
    private float _actionCooldown;
    private float _skill1Cooldown;
    private float _dashCooldown;

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

        _actionCooldown = Math.Max(0f, _actionCooldown - dt);
        _skill1Cooldown = Math.Max(0f, _skill1Cooldown - dt);
        _dashCooldown = Math.Max(0f, _dashCooldown - dt);

        float dx = targetMv.X - bossMv.X;
        float absDx = Math.Abs(dx);
        bossMv.FacingRight = dx >= 0f;

        if (bossCh.IsBusy)
        {
            bossMv.VelocityX = 0f;
            return;
        }

        float engageRange = bossCh.BaseStats.AttackRange + _profile.EngageRangeBonus;
        if (absDx > engageRange)
        {
            TryDashTowardTarget(bossCh, bossMv, absDx);
            if (bossCh.IsDashing)
                return;

            float dir = dx >= 0f ? 1f : -1f;
            bossMv.VelocityX = dir * bossMv.Speed * _profile.ChaseSpeedMultiplier;
            return;
        }

        bossMv.VelocityX = 0f;

        if (_skill1Cooldown <= 0f && bossCh.Skill1 != null && absDx <= _profile.Skill1Range)
        {
            combat.UseSkill(boss, 1);
            _skill1Cooldown = _profile.Skill1Cooldown;
            _actionCooldown = _profile.PostSkillActionCooldown;
            return;
        }

        if (_actionCooldown <= 0f)
        {
            combat.Attack(boss);
            _actionCooldown = _profile.BasicAttackCooldown;
        }
    }

    private void TryDashTowardTarget(CharacterComponent bossCh, MovementComponent bossMv, float distance)
    {
        if (!_profile.CanDash || _dashCooldown > 0f || distance < _profile.DashMinRange)
            return;

        string dashAnimation = ResolveDashAnimation(bossCh);
        if (string.IsNullOrWhiteSpace(dashAnimation))
            return;

        float duration = bossCh.GetAnimationDuration(dashAnimation, _profile.DashDuration);
        bossCh.IsDashing = true;
        bossCh.DashTimer = _profile.DashDuration;
        bossCh.DashDuration = _profile.DashDuration;
        bossCh.ActionTimer = duration;
        bossCh.ActionDuration = duration;
        bossCh.DashSpeedMultiplier = _profile.DashSpeedMultiplier;
        bossMv.VelocityX = (bossMv.FacingRight ? 1f : -1f) * bossMv.Speed * bossCh.DashSpeedMultiplier;
        _dashCooldown = _profile.DashCooldown;
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
