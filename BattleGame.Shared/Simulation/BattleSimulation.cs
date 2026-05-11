using BattleGame.Shared.Config;
using BattleGame.Shared.Models;

namespace BattleGame.Shared.Simulation;

public class BattleSimulation
{
    private const float Gravity = 800f;
    private const float DashDuration = 0.22f;
    private const float DashMultiplier = 3f;
    private const float HurtDuration = 0.3f;
    private const float CharacterWidth = 100f;
    private const float CharacterHeight = 100f;
    private const float ProjectileLifetime = 3f;
    private readonly Dictionary<int, BattleInput> _latestInputs = new();
    private int _nextProjectileId = 1;
    private int _nextEffectId = 1;

    public BattleState State { get; }
    public float MapLeft { get; set; } = 50f;
    public float MapRight { get; set; } = 1230f;
    public float GroundY { get; set; } = 580f;

    public BattleSimulation(BattleState state)
    {
        State = state;
    }

    public static BattleSimulation Create(
        int player1Id,
        string player1CharacterId,
        int player2Id,
        string player2CharacterId,
        string? configRoot = null,
        float groundY = 580f,
        float mapRight = 1230f)
    {
        var player1Stats = LoadStats(player1CharacterId, configRoot);
        var player2Stats = LoadStats(player2CharacterId, configRoot);
        var state = new BattleState
        {
            Player1 = CreatePlayer(player1Id, player1CharacterId, player1Stats, 200f, groundY, true),
            Player2 = CreatePlayer(player2Id, player2CharacterId, player2Stats, Math.Min(mapRight - 300f, 500f), groundY, false)
        };

        return new BattleSimulation(state)
        {
            MapRight = mapRight,
            GroundY = groundY
        };
    }

    public void ApplyInput(BattleInput input)
    {
        if (input.PlayerId <= 0)
            return;

        _latestInputs[input.PlayerId] = input;
    }

    public void Update(float deltaTime)
    {
        if (State.IsGameOver)
            return;

        deltaTime = Math.Clamp(deltaTime, 0f, 0.05f);
        State.ServerTick++;

        UpdatePlayer(State.Player1, State.Player2, deltaTime);
        UpdatePlayer(State.Player2, State.Player1, deltaTime);
        UpdateProjectiles(deltaTime);
        UpdateEffects(deltaTime);
        CheckGameOver();
    }

    private static BattleCharacterStats LoadStats(string characterId, string? configRoot)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(configRoot))
            {
                var definition = BattleCharacterDefinitionLoader.LoadById(configRoot, characterId);
                definition.Stats.Skill1 = definition.Skill1;
                definition.Stats.Skill2 = definition.Skill2;
                definition.Stats.AttackEffects = definition.AttackEffects;
                return definition.Stats;
            }
        }
        catch
        {
        }

        return BattleCharacterCatalog.GetStats(characterId);
    }

    private static PlayerBattleState CreatePlayer(
        int playerId,
        string characterId,
        BattleCharacterStats stats,
        float x,
        float y,
        bool facingRight)
    {
        return new PlayerBattleState
        {
            PlayerId = playerId,
            CharacterId = characterId,
            Stats = stats,
            X = x,
            Y = y,
            Hp = stats.Hp,
            Mana = stats.Mana,
            FacingRight = facingRight,
            CurrentAnimation = "Idle",
            ActionDuration = stats.AttackDuration
        };
    }

    private void UpdatePlayer(PlayerBattleState player, PlayerBattleState opponent, float dt)
    {
        if (!_latestInputs.TryGetValue(player.PlayerId, out var input))
            input = new BattleInput { PlayerId = player.PlayerId, FacingRight = player.FacingRight };

        UpdateTimers(player, dt);
        StartActions(player, input);
        UpdateMovement(player, input, dt);
        ResolveActionEffects(player, opponent);
        UpdateAnimation(player);
    }

    private static void UpdateTimers(PlayerBattleState player, float dt)
    {
        if (player.HurtTimer > 0f)
        {
            player.HurtTimer -= dt;
            if (player.HurtTimer <= 0f)
                player.IsHurt = false;
        }

        if (player.StunTimer > 0f)
        {
            player.StunTimer -= dt;
            if (player.StunTimer <= 0f)
                player.IsStunned = false;
        }

        if (player.Skill1Cooldown > 0f)
            player.Skill1Cooldown -= dt;
        if (player.Skill2Cooldown > 0f)
            player.Skill2Cooldown -= dt;

        if (player.IsDashing)
        {
            player.DashTimer -= dt;
            if (player.DashTimer <= 0f)
            {
                player.IsDashing = false;
                player.VelocityX = 0f;
            }
        }

        if (player.IsAttacking || player.IsUsingSkill)
        {
            player.ActionTimer -= dt;
            if (player.ActionTimer <= 0f)
            {
                player.IsAttacking = false;
                player.IsUsingSkill = false;
                player.CurrentSkillSlot = 0;
                player.CurrentSkillAnimation = "";
                player.ActionHitDone = false;
                player.TriggeredEffects.Clear();
                player.TriggeredAttackEffects.Clear();
            }
        }
    }

    private void StartActions(PlayerBattleState player, BattleInput input)
    {
        if (player.IsDead)
            return;

        if (!player.IsBusy)
            player.IsProtecting = input.BlockHeld;
        else if (!input.BlockHeld)
            player.IsProtecting = false;

        if (player.IsBusy || player.IsProtecting)
            return;

        player.FacingRight = input.FacingRight;

        if (input.DashPressed)
        {
            player.IsDashing = true;
            player.DashTimer = DashDuration;
            StartAction(player, "Dash", DashDuration);
            return;
        }

        if (input.SkillSlot is 1 or 2)
        {
            var skill = input.SkillSlot == 1 ? player.Stats.Skill1 : player.Stats.Skill2;
            float cooldown = input.SkillSlot == 1 ? player.Skill1Cooldown : player.Skill2Cooldown;
            if (skill != null && cooldown <= 0f && player.Mana >= skill.ManaCost)
            {
                player.Mana -= skill.ManaCost;
                player.IsUsingSkill = true;
                player.CurrentSkillSlot = input.SkillSlot;
                player.CurrentSkillAnimation = skill.Animation;
                StartAction(player, string.IsNullOrWhiteSpace(skill.Animation) ? $"Skill{input.SkillSlot}" : skill.Animation, EstimateActionDuration(skill));

                if (input.SkillSlot == 1)
                    player.Skill1Cooldown = skill.Cooldown;
                else
                    player.Skill2Cooldown = skill.Cooldown;
            }
            return;
        }

        if (input.AttackPressed)
        {
            player.IsAttacking = true;
            StartAction(player, "Attack_1", player.Stats.AttackDuration);
        }
    }

    private void StartAction(PlayerBattleState player, string animation, float duration)
    {
        player.ActionTimer = Math.Max(0.05f, duration);
        player.ActionDuration = Math.Max(0.05f, duration);
        player.ActionHitDone = false;
        player.CurrentActionId++;
        player.CurrentActionTick = State.ServerTick;
        player.CurrentAnimation = animation;
        player.CurrentFrame = 0;
        player.TriggeredEffects.Clear();
        player.TriggeredAttackEffects.Clear();
    }

    private static float EstimateActionDuration(SkillData skill)
    {
        return skill.Effects.Count == 0 ? 0.7f : Math.Max(0.35f, skill.Cooldown > 0f ? Math.Min(skill.Cooldown, 1.2f) : 0.7f);
    }

    private void UpdateMovement(PlayerBattleState player, BattleInput input, float dt)
    {
        if (!player.IsBusy && !player.IsProtecting)
        {
            player.VelocityX = Math.Clamp(input.MoveX, -1f, 1f) * player.Stats.Speed;
            if (input.MoveX < 0f)
                player.FacingRight = false;
            else if (input.MoveX > 0f)
                player.FacingRight = true;
        }
        else if (player.IsDashing)
        {
            player.VelocityX = (player.FacingRight ? 1f : -1f) * player.Stats.Speed * DashMultiplier;
        }
        else
        {
            player.VelocityX = 0f;
        }

        if (!player.IsGrounded)
            player.VelocityY += Gravity * dt;

        player.X = Math.Clamp(player.X + player.VelocityX * dt, MapLeft, MapRight);
        player.Y += player.VelocityY * dt;

        if (player.Y >= GroundY)
        {
            player.Y = GroundY;
            player.VelocityY = 0f;
            player.IsGrounded = true;
        }
    }

    private void ResolveActionEffects(PlayerBattleState attacker, PlayerBattleState target)
    {
        if (target.IsDead)
            return;

        if (attacker.IsAttacking)
        {
            ResolveEffects(attacker, target, attacker.Stats.AttackEffects, attacker.TriggeredAttackEffects, fallbackToBaseAttack: true);
            return;
        }

        if (attacker.IsUsingSkill)
        {
            var skill = attacker.CurrentSkillSlot == 1 ? attacker.Stats.Skill1 : attacker.Stats.Skill2;
            if (skill != null)
                ResolveEffects(attacker, target, skill.Effects, attacker.TriggeredEffects, fallbackToBaseAttack: false);
        }
    }

    private void ResolveEffects(
        PlayerBattleState attacker,
        PlayerBattleState target,
        List<EffectData> effects,
        HashSet<int> triggered,
        bool fallbackToBaseAttack)
    {
        if (effects.Count == 0)
        {
            if (fallbackToBaseAttack)
                ResolveBaseAttack(attacker, target);
            return;
        }

        for (int i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            if (triggered.Contains(i) || !ShouldTrigger(attacker, effect))
                continue;

            triggered.Add(i);
            ApplyEffect(attacker, target, effect);
        }
    }

    private static bool ShouldTrigger(PlayerBattleState player, EffectData effect)
    {
        float elapsed = player.ActionDuration - player.ActionTimer;
        float progress = player.ActionDuration <= 0f ? 1f : elapsed / player.ActionDuration;
        string trigger = (effect.Trigger ?? "").Trim().ToLowerInvariant();

        return trigger switch
        {
            "onstart" => elapsed <= 0.08f,
            "onend" => progress >= 0.9f,
            "onframe" or "onframes" or "onmiddle" => progress >= 0.5f,
            _ => progress >= 0.5f
        };
    }

    private void ResolveBaseAttack(PlayerBattleState attacker, PlayerBattleState target)
    {
        if (attacker.ActionHitDone)
            return;

        float elapsed = attacker.ActionDuration - attacker.ActionTimer;
        if (elapsed < attacker.ActionDuration * 0.5f)
            return;

        attacker.ActionHitDone = true;

        if (!IsInMeleeRange(attacker, target, attacker.Stats.AttackRange))
            return;

        if (IsBlockedByBarrier(attacker, target, "melee") || IsBlockedByProtection(attacker, target))
            return;

        ApplyDamage(target, attacker.Stats.Atk, 0f);
    }

    private void ApplyEffect(PlayerBattleState attacker, PlayerBattleState target, EffectData effect)
    {
        if (IsBlockedByBarrier(attacker, target, effect.Type) || IsBlockedByProtection(attacker, target))
            return;

        switch ((effect.Type ?? "").Trim().ToLowerInvariant())
        {
            case "melee":
                if (IsInMeleeRange(attacker, target, effect.Range))
                    ApplyDamage(target, effect.Damage, effect.Stun);
                break;
            case "projectile":
                SpawnProjectile(attacker, effect);
                break;
            case "barrier":
                SpawnBarrier(attacker, target, effect);
                break;
        }
    }

    private void SpawnProjectile(PlayerBattleState owner, EffectData effect)
    {
        float direction = owner.FacingRight ? 1f : -1f;
        State.Projectiles.Add(new ProjectileState
        {
            ProjectileId = _nextProjectileId++,
            OwnerPlayerId = owner.PlayerId,
            X = owner.X + direction * 45f,
            Y = owner.Y + effect.SpawnOffsetY,
            VelocityX = direction * effect.Speed,
            VelocityY = 0f,
            Damage = effect.Damage,
            Stun = effect.Stun,
            Range = effect.Range,
            Lifetime = ProjectileLifetime,
            AnimationKey = effect.ProjectileAnim,
            FacingRight = owner.FacingRight,
            RenderOffsetX = effect.Render.OffsetX,
            RenderOffsetY = effect.Render.OffsetY,
            Render = effect.Render
        });
    }

    private void SpawnBarrier(PlayerBattleState owner, PlayerBattleState target, EffectData effect)
    {
        var (x, y) = ResolveEffectSpawn(owner, target, effect);
        State.Effects.Add(new EffectState
        {
            EffectId = _nextEffectId++,
            OwnerPlayerId = owner.PlayerId,
            EffectType = "barrier",
            AnimationKey = string.IsNullOrWhiteSpace(effect.ObjectAnim) ? effect.ProjectileAnim : effect.ObjectAnim,
            X = x,
            Y = y,
            Damage = effect.Damage,
            Stun = effect.Stun,
            CollisionWidth = effect.CollisionWidth,
            CollisionHeight = effect.CollisionHeight,
            BlockEnemyAttack = effect.BlockEnemyAttack,
            BlockEnemyProjectile = effect.BlockEnemyProjectile,
            BlockEnemySkill = effect.BlockEnemySkill,
            RemainingTime = effect.Duration,
            FacingRight = ResolveEffectFacing(owner, target, effect),
            Render = effect.Render
        });
    }

    private void UpdateProjectiles(float dt)
    {
        for (int i = State.Projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = State.Projectiles[i];
            projectile.X += projectile.VelocityX * dt;
            projectile.Y += projectile.VelocityY * dt;
            projectile.Timer += dt;

            if (projectile.Timer >= projectile.Lifetime || IsProjectileBlockedByBarrier(projectile))
            {
                State.Projectiles.RemoveAt(i);
                continue;
            }

            PlayerBattleState target = projectile.OwnerPlayerId == State.Player1.PlayerId ? State.Player2 : State.Player1;
            if (!target.IsDead && ContainsPoint(target, projectile.X + projectile.RenderOffsetX, projectile.Y + projectile.RenderOffsetY))
            {
                if (!IsProjectileBlockedByProtection(projectile, target))
                    ApplyDamage(target, projectile.Damage, projectile.Stun);

                State.Projectiles.RemoveAt(i);
            }
        }
    }

    private void UpdateEffects(float dt)
    {
        for (int i = State.Effects.Count - 1; i >= 0; i--)
        {
            var effect = State.Effects[i];
            effect.RemainingTime -= dt;
            if (effect.RemainingTime <= 0f)
            {
                State.Effects.RemoveAt(i);
                continue;
            }

            PlayerBattleState target = effect.OwnerPlayerId == State.Player1.PlayerId ? State.Player2 : State.Player1;
            if (effect.Damage > 0 && effect.LastDamageTick != State.ServerTick && IntersectsRectangle(target, effect.X, effect.Y, effect.CollisionWidth, effect.CollisionHeight))
            {
                ApplyDamage(target, effect.Damage, effect.Stun);
                effect.LastDamageTick = State.ServerTick;
            }
        }
    }

    private bool IsBlockedByBarrier(PlayerBattleState attacker, PlayerBattleState target, string effectType)
    {
        foreach (var barrier in State.Effects)
        {
            if (barrier.EffectType != "barrier" || barrier.OwnerPlayerId != target.PlayerId || barrier.RemainingTime <= 0f)
                continue;

            bool blocks = effectType.ToLowerInvariant() switch
            {
                "projectile" => barrier.BlockEnemyProjectile,
                "melee" => barrier.BlockEnemyAttack,
                _ => barrier.BlockEnemySkill
            };

            if (blocks && (IntersectsRectangle(attacker, barrier.X, barrier.Y, barrier.CollisionWidth, barrier.CollisionHeight)
                || IntersectsRectangle(target, barrier.X, barrier.Y, barrier.CollisionWidth, barrier.CollisionHeight)))
                return true;
        }

        return false;
    }

    private bool IsProjectileBlockedByBarrier(ProjectileState projectile)
    {
        foreach (var barrier in State.Effects)
        {
            if (barrier.EffectType != "barrier" || barrier.OwnerPlayerId == projectile.OwnerPlayerId || !barrier.BlockEnemyProjectile)
                continue;

            if (IsPointInRectangle(projectile.X + projectile.RenderOffsetX, projectile.Y + projectile.RenderOffsetY, barrier.X, barrier.Y, barrier.CollisionWidth, barrier.CollisionHeight))
                return true;
        }

        return false;
    }

    private static bool IsBlockedByProtection(PlayerBattleState attacker, PlayerBattleState target)
    {
        if (!target.IsProtecting || target.IsDead)
            return false;

        if (target.Stats.ProtectionBlocksAllDirections)
            return true;

        return target.FacingRight ? attacker.X >= target.X : attacker.X <= target.X;
    }

    private static bool IsProjectileBlockedByProtection(ProjectileState projectile, PlayerBattleState target)
    {
        if (!target.IsProtecting || target.IsDead)
            return false;

        if (target.Stats.ProtectionBlocksAllDirections)
            return true;

        float hitX = projectile.X + projectile.RenderOffsetX;
        return target.FacingRight ? hitX >= target.X : hitX <= target.X;
    }

    private static bool IsInMeleeRange(PlayerBattleState attacker, PlayerBattleState target, float range)
    {
        float centerDistance = Math.Abs(attacker.X - target.X);
        float horizontalGap = Math.Max(0f, centerDistance - CharacterWidth);
        return horizontalGap < range;
    }

    private static bool ContainsPoint(PlayerBattleState target, float x, float y)
    {
        return x >= target.X - CharacterWidth / 2f
            && x <= target.X + CharacterWidth / 2f
            && y >= target.Y - CharacterHeight / 2f
            && y <= target.Y + CharacterHeight / 2f;
    }

    private static bool IntersectsRectangle(PlayerBattleState target, float centerX, float centerY, float width, float height)
    {
        return Math.Abs(target.X - centerX) <= CharacterWidth / 2f + width / 2f
            && Math.Abs(target.Y - centerY) <= CharacterHeight / 2f + height / 2f;
    }

    private static bool IsPointInRectangle(float x, float y, float centerX, float centerY, float width, float height)
    {
        return x >= centerX - width / 2f
            && x <= centerX + width / 2f
            && y >= centerY - height / 2f
            && y <= centerY + height / 2f;
    }

    private static (float X, float Y) ResolveEffectSpawn(PlayerBattleState owner, PlayerBattleState target, EffectData effect)
    {
        string mode = (effect.SpawnMode ?? "between").Trim().ToLowerInvariant();
        return mode switch
        {
            "targetfront" => (
                target.X + (owner.X < target.X ? effect.SpawnOffsetX : -effect.SpawnOffsetX),
                target.Y + effect.SpawnOffsetY),
            "casterfront" => (
                owner.X + (owner.FacingRight ? effect.SpawnOffsetX : -effect.SpawnOffsetX),
                owner.Y + effect.SpawnOffsetY),
            _ => (
                ((owner.X + target.X) * 0.5f) + (target.X >= owner.X ? effect.SpawnOffsetX : -effect.SpawnOffsetX),
                target.Y + effect.SpawnOffsetY)
        };
    }

    private static bool ResolveEffectFacing(PlayerBattleState owner, PlayerBattleState target, EffectData effect)
    {
        return (effect.Render.FacingSource ?? "owner").Trim().ToLowerInvariant() switch
        {
            "target" => target.FacingRight,
            "fixed" => true,
            _ => owner.FacingRight
        };
    }

    private static void ApplyDamage(PlayerBattleState target, int rawDamage, float stun)
    {
        int damage = Math.Max(0, rawDamage - target.Stats.Def);
        if (damage <= 0 && stun <= 0f)
            return;

        target.Hp = Math.Max(0, target.Hp - damage);
        target.IsHurt = damage > 0;
        target.HurtTimer = damage > 0 ? HurtDuration : target.HurtTimer;

        if (stun > 0f)
        {
            target.IsStunned = true;
            target.StunTimer = stun;
        }

        if (target.Hp <= 0)
        {
            target.IsDead = true;
            target.CurrentAnimation = "Dead";
        }
    }

    private static void UpdateAnimation(PlayerBattleState player)
    {
        if (player.IsDead)
        {
            player.CurrentAnimation = "Dead";
            return;
        }

        if (player.IsHurt)
        {
            player.CurrentAnimation = "Hurt";
            return;
        }

        if (player.IsStunned)
            return;

        if (player.IsAttacking || player.IsUsingSkill || player.IsDashing)
            return;

        if (player.IsProtecting)
            player.CurrentAnimation = "Protection";
        else if (Math.Abs(player.VelocityX) > 1f)
            player.CurrentAnimation = "Run";
        else
            player.CurrentAnimation = "Idle";
    }

    private void CheckGameOver()
    {
        if (State.Player1.IsDead || State.Player2.IsDead)
        {
            State.IsGameOver = true;
            State.WinnerPlayerId = State.Player1.IsDead
                ? State.Player2.PlayerId
                : State.Player1.PlayerId;
        }
    }
}
