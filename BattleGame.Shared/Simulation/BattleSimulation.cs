using BattleGame.Shared.Config;
using BattleGame.Shared.Models;

namespace BattleGame.Shared.Simulation;

public class BattleSimulation
{
    private const float Gravity = 800f;
    private const float DashDuration = 0.22f;
    private const float DashMultiplier = 3f;
    private const float HurtDuration = 0.3f;
    private const float ProjectileLifetime = 3f;
    private const float ProjectileCollisionDelay = 0.08f;
    private const int InputStaleTicks = 12;
    private const int ActionInputBufferTicks = 15;
    private readonly Dictionary<int, BattleInput> _latestInputs = new();
    private readonly Dictionary<int, int> _latestInputTicks = new();
    private readonly Dictionary<int, int> _latestInputSequences = new();
    private readonly Dictionary<int, BufferedActionInput> _bufferedActionInputs = new();
    private readonly Dictionary<int, float> _manaRegenRemainders = new();
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
        const float leftRatio = 0.15f;
        const float rightRatio = 0.85f;
        const float minGap = 200f;
        float leftSpawnX = Math.Clamp(mapRight * leftRatio, 80f, mapRight - 240f);
        float rightSpawnX = Math.Max(mapRight * rightRatio, leftSpawnX + minGap);
        rightSpawnX = Math.Clamp(rightSpawnX, leftSpawnX + 150f, mapRight - 40f);
        var player1Stats = LoadStats(player1CharacterId, configRoot);
        var player2Stats = LoadStats(player2CharacterId, configRoot);
        var state = new BattleState
        {
            Player1 = CreatePlayer(player1Id, player1CharacterId, player1Stats, leftSpawnX, groundY, true),
            Player2 = CreatePlayer(player2Id, player2CharacterId, player2Stats, rightSpawnX, groundY, false)
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

        if (_latestInputSequences.TryGetValue(input.PlayerId, out int latestSequence) &&
            input.Sequence <= latestSequence)
            return;

        _latestInputs[input.PlayerId] = CreateContinuousInput(input);
        _latestInputTicks[input.PlayerId] = State.ServerTick;
        _latestInputSequences[input.PlayerId] = input.Sequence;

        if (HasOneShotInput(input))
            _bufferedActionInputs[input.PlayerId] = new BufferedActionInput(CloneInput(input), State.ServerTick);
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
        if (string.IsNullOrWhiteSpace(configRoot))
        {
            throw new InvalidOperationException(
                $"Missing config root when loading character '{characterId}'. Fallback catalog is disabled.");
        }

        try
        {
            var definition = BattleCharacterDefinitionLoader.LoadById(configRoot, characterId);
            definition.Stats.Skill1 = definition.Skill1;
            definition.Stats.Skill2 = definition.Skill2;
            definition.Stats.AttackEffects = definition.AttackEffects;
            return definition.Stats;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load battle config for '{characterId}' from '{configRoot}'.", ex);
        }
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
        BattleInput input;
        bool hasFreshInput = _latestInputs.TryGetValue(player.PlayerId, out var latestInput);
        if (!hasFreshInput)
        {
            input = new BattleInput { PlayerId = player.PlayerId, FacingRight = player.FacingRight };
        }
        else if (_latestInputTicks.TryGetValue(player.PlayerId, out int lastTick) &&
                 State.ServerTick - lastTick > InputStaleTicks)
        {
            input = new BattleInput { PlayerId = player.PlayerId, FacingRight = player.FacingRight };
            hasFreshInput = false;
        }
        else
        {
            input = latestInput!;
        }

        UpdateTimers(player, dt);
        BattleInput actionInput = TryGetBufferedActionInput(player.PlayerId, input, out var bufferedAction)
            ? CreateActionInput(input, bufferedAction)
            : input;
        bool consumedOneShot = StartActions(player, actionInput);
        if (consumedOneShot)
            ConsumeOneShotInput(player.PlayerId);
        UpdateMovement(player, input, dt);
        ResolveActionEffects(player, opponent);
        UpdateAnimation(player);
    }

    private static bool HasOneShotInput(BattleInput input)
        => input.JumpPressed || input.AttackPressed || input.SkillSlot > 0 || input.DashPressed;

    private void ConsumeOneShotInput(int playerId)
    {
        _bufferedActionInputs.Remove(playerId);

        if (!_latestInputs.TryGetValue(playerId, out var input))
            return;

        input.JumpPressed = false;
        input.AttackPressed = false;
        input.SkillSlot = 0;
        input.DashPressed = false;
    }

    private void UpdateTimers(PlayerBattleState player, float dt)
    {
        RegenerateMana(player, dt);

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
            player.Skill1Cooldown = Math.Max(0f, player.Skill1Cooldown - dt);
        if (player.Skill2Cooldown > 0f)
            player.Skill2Cooldown = Math.Max(0f, player.Skill2Cooldown - dt);

        if (player.IsDashing)
        {
            player.DashTimer = Math.Max(0f, player.DashTimer - dt);
            if (player.DashTimer <= 0f)
                player.VelocityX = 0f;
        }

        if (player.IsAttacking || player.IsUsingSkill || player.IsDashing)
        {
            player.ActionTimer -= dt;
            UpdateActionFrame(player);
            if (player.ActionTimer <= 0f)
            {
                player.IsAttacking = false;
                player.IsUsingSkill = false;
                player.IsDashing = false;
                player.VelocityX = 0f;
                player.CurrentSkillSlot = 0;
                player.CurrentSkillAnimation = "";
                player.ActionHitDone = false;
                player.TriggeredEffects.Clear();
                player.TriggeredAttackEffects.Clear();
                player.TriggeredEffectFrames.Clear();
                player.TriggeredAttackEffectFrames.Clear();
            }
        }
    }

    private void RegenerateMana(PlayerBattleState player, float dt)
    {
        int maxMana = Math.Max(0, player.Stats.Mana);
        float regen = Math.Max(0f, player.Stats.ManaRegen);
        if (player.IsDead || maxMana == 0 || regen <= 0f || player.Mana >= maxMana)
        {
            _manaRegenRemainders[player.PlayerId] = 0f;
            return;
        }

        _manaRegenRemainders.TryGetValue(player.PlayerId, out float remainder);
        remainder += regen * dt;

        int gained = (int)MathF.Floor(remainder);
        if (gained > 0)
        {
            player.Mana = Math.Min(maxMana, player.Mana + gained);
            remainder -= gained;
        }

        if (player.Mana >= maxMana)
            remainder = 0f;

        _manaRegenRemainders[player.PlayerId] = remainder;
    }

    private bool StartActions(PlayerBattleState player, BattleInput input)
    {
        if (player.IsDead)
            return false;

        if (!player.IsBusy)
            player.IsProtecting = input.BlockHeld;
        else if (!input.BlockHeld)
            player.IsProtecting = false;

        if (player.IsBusy || player.IsProtecting)
            return false;

        player.FacingRight = input.FacingRight;

        if (input.DashPressed)
        {
            string dashAnimation = ResolveDashAnimation(player);
            if (string.IsNullOrWhiteSpace(dashAnimation))
                return false;

            player.IsDashing = true;
            player.DashTimer = DashDuration;
            StartAction(player, dashAnimation, DashDuration);
            return true;
        }

        if (input.SkillSlot is 1 or 2)
        {
            var skill = input.SkillSlot == 1 ? player.Stats.Skill1 : player.Stats.Skill2;
            float cooldown = input.SkillSlot == 1 ? player.Skill1Cooldown : player.Skill2Cooldown;
            if (skill != null && cooldown <= 0f && player.Mana >= skill.ManaCost)
            {
                string actionAnimation = string.IsNullOrWhiteSpace(skill.Animation) ? $"Skill{input.SkillSlot}" : skill.Animation;
                if (!player.Stats.Animations.ContainsKey(actionAnimation))
                    return false;

                player.Mana -= skill.ManaCost;
                player.IsUsingSkill = true;
                player.CurrentSkillSlot = input.SkillSlot;
                player.CurrentSkillAnimation = skill.Animation;
                StartAction(player, actionAnimation, EstimateActionDuration(player, skill, actionAnimation));

                if (input.SkillSlot == 1)
                    player.Skill1Cooldown = skill.Cooldown;
                else
                    player.Skill2Cooldown = skill.Cooldown;

                return true;
            }
            return false;
        }

        if (input.AttackPressed)
        {
            string attackAnimation = ResolveAttackAnimation(player);
            if (string.IsNullOrWhiteSpace(attackAnimation))
                return false;

            player.IsAttacking = true;
            StartAction(player, attackAnimation, EstimateActionDuration(player, null, attackAnimation, player.Stats.AttackDuration));
            return true;
        }

        return false;
    }

    private static string ResolveAttackAnimation(PlayerBattleState player)
    {
        int attackCount = Math.Max(1, player.Stats.AttackAnimCount);
        for (int attempt = 0; attempt < attackCount; attempt++)
        {
            int idx = ((player.CurrentActionId + attempt) % attackCount) + 1;
            string animation = $"Attack_{idx}";
            if (player.Stats.Animations.ContainsKey(animation))
                return animation;
        }

        return string.Empty;
    }

    private static string ResolveDashAnimation(PlayerBattleState player)
        => ResolveFirstAvailableAnimation(player, "Dash", "Run", "Walk");

    private static string ResolveFirstAvailableAnimation(PlayerBattleState player, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (player.Stats.Animations.ContainsKey(candidate))
                return candidate;
        }

        return string.Empty;
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
        player.TriggeredEffectFrames.Clear();
        player.TriggeredAttackEffectFrames.Clear();
    }

    private static float EstimateActionDuration(PlayerBattleState player, SkillData? skill, string actionAnimation, float fallbackDuration = 0.7f)
    {
        if (player.Stats.Animations.TryGetValue(actionAnimation, out var meta))
        {
            float fps = Math.Max(1f, meta.Fps);
            int frameCount = Math.Max(1, meta.FrameCount);
            return Math.Max(0.05f, frameCount / fps);
        }

        if (skill == null)
            return Math.Max(0.05f, fallbackDuration);

        return skill.Effects.Count == 0 ? Math.Max(0.05f, fallbackDuration) : Math.Max(0.35f, skill.Cooldown > 0f ? Math.Min(skill.Cooldown, 1.2f) : fallbackDuration);
    }

    private static void UpdateActionFrame(PlayerBattleState player)
    {
        if (string.IsNullOrWhiteSpace(player.CurrentAnimation))
            return;

        if (!player.Stats.Animations.TryGetValue(player.CurrentAnimation, out var meta))
            return;

        int frameCount = Math.Max(1, meta.FrameCount);
        float elapsed = Math.Max(0f, player.ActionDuration - player.ActionTimer);
        int frameIndex = Math.Clamp((int)MathF.Floor(elapsed * Math.Max(1f, meta.Fps)), 0, frameCount - 1);
        player.CurrentFrame = frameIndex;
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
        else if (player.IsDashing && player.DashTimer > 0f)
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
            ResolveEffects(
                attacker,
                target,
                attacker.Stats.AttackEffects,
                attacker.TriggeredAttackEffects,
                attacker.TriggeredAttackEffectFrames,
                fallbackToBaseAttack: attacker.Stats.AttackEffects.Count == 0);
            return;
        }

        if (attacker.IsUsingSkill)
        {
            var skill = attacker.CurrentSkillSlot == 1 ? attacker.Stats.Skill1 : attacker.Stats.Skill2;
            if (skill != null)
                ResolveEffects(
                    attacker,
                    target,
                    skill.Effects,
                    attacker.TriggeredEffects,
                    attacker.TriggeredEffectFrames,
                    fallbackToBaseAttack: false);
        }
    }

    private void ResolveEffects(
        PlayerBattleState attacker,
        PlayerBattleState target,
        List<EffectData> effects,
        HashSet<int> triggered,
        HashSet<string> triggeredFrames,
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
            if (!string.IsNullOrWhiteSpace(effect.Animation) &&
                !string.Equals(effect.Animation, attacker.CurrentAnimation, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!ShouldTrigger(attacker, i, effect, triggered, triggeredFrames, out string? frameMarker))
                continue;

            if (frameMarker == null)
                triggered.Add(i);
            else
                triggeredFrames.Add(frameMarker);
            ApplyEffect(attacker, target, effect);
        }
    }

    private static bool ShouldTrigger(
        PlayerBattleState player,
        int effectIndex,
        EffectData effect,
        HashSet<int> triggered,
        HashSet<string> triggeredFrames,
        out string? frameMarker)
    {
        frameMarker = null;
        float elapsed = player.ActionDuration - player.ActionTimer;
        int frameCount = GetCurrentAnimationFrameCount(player);
        int currentFrame = Math.Clamp(player.CurrentFrame, 0, frameCount - 1);
        string trigger = (effect.Trigger ?? "").Trim().ToLowerInvariant();

        switch (trigger)
        {
            case "onstart":
                return elapsed <= 0.08f && !triggered.Contains(effectIndex);
            case "onend":
                return currentFrame >= frameCount - 1 && !triggered.Contains(effectIndex);
            case "onframe":
                var onFrameFrames = effect.TriggerFrames ?? effect.Frames;
                if (onFrameFrames == null || onFrameFrames.Count == 0)
                    return currentFrame >= frameCount / 2 && !triggered.Contains(effectIndex);

                return onFrameFrames[0] == currentFrame && !triggered.Contains(effectIndex);
            case "onframes":
                var onFramesFrames = effect.TriggerFrames ?? effect.Frames;
                if (onFramesFrames == null || onFramesFrames.Count == 0)
                    return currentFrame >= frameCount / 2 && !triggered.Contains(effectIndex);

                bool matched = onFramesFrames.Contains(currentFrame);
                if (!matched)
                    return false;

                frameMarker = $"{effectIndex}:{currentFrame}";
                return !triggeredFrames.Contains(frameMarker);
            case "onmiddle":
            default:
                return currentFrame >= frameCount / 2 && !triggered.Contains(effectIndex);
        }
    }

    private static int GetCurrentAnimationFrameCount(PlayerBattleState player)
    {
        if (!string.IsNullOrWhiteSpace(player.CurrentAnimation) &&
            player.Stats.Animations.TryGetValue(player.CurrentAnimation, out var meta))
        {
            return Math.Max(1, meta.FrameCount);
        }

        return 1;
    }

    private void ResolveBaseAttack(PlayerBattleState attacker, PlayerBattleState target)
    {
        if (attacker.ActionHitDone)
            return;

        float elapsed = attacker.ActionDuration - attacker.ActionTimer;
        if (elapsed < attacker.ActionDuration * 0.5f)
            return;

        attacker.ActionHitDone = true;

        if (!string.IsNullOrWhiteSpace(attacker.Stats.AttackProjectile)
            && attacker.Stats.AttackProjectileSpeed > 0f)
        {
            SpawnBasicAttackProjectile(attacker);
            return;
        }

        if (!IsBaseAttackHit(attacker, target))
            return;

        if (IsBlockedByBarrier(attacker, target, "melee") || IsBlockedByProtection(attacker, target))
            return;

        ApplyDamage(target, attacker.Stats.Atk, 0f, attacker.Stats.ArmorPen);
    }

    private void SpawnBasicAttackProjectile(PlayerBattleState owner)
    {
        float direction = owner.FacingRight ? 1f : -1f;
        State.Projectiles.Add(new ProjectileState
        {
            ProjectileId = _nextProjectileId++,
            OwnerPlayerId = owner.PlayerId,
            X = owner.X + (owner.FacingRight ? owner.Stats.AttackProjectileSpawnOffsetX : -owner.Stats.AttackProjectileSpawnOffsetX),
            Y = owner.Y + owner.Stats.AttackProjectileSpawnOffsetY,
            VelocityX = direction * owner.Stats.AttackProjectileSpeed,
            VelocityY = 0f,
            Damage = owner.Stats.Atk,
            ArmorPen = owner.Stats.ArmorPen,
            Stun = 0f,
            Range = 45f,
            Lifetime = ProjectileLifetime,
            AnimationKey = owner.Stats.AttackProjectile ?? string.Empty,
            FacingRight = owner.FacingRight,
            RenderOffsetX = 0f,
            RenderOffsetY = 0f,
            HitFrames = new List<int>(),
            Render = new EffectRenderData
            {
                Scale = owner.Stats.AttackProjectileScale,
                UseSpriteSize = true
            }
        });
    }

    private void ApplyEffect(PlayerBattleState attacker, PlayerBattleState target, EffectData effect)
    {
        string effectType = (effect.Type ?? "").Trim().ToLowerInvariant();
        if (effectType == "projectile")
        {
            SpawnProjectile(attacker, target, effect);
            return;
        }

        if (IsBlockedByBarrier(attacker, target, effectType))
            return;

        if (effectType != "barrier" && IsBlockedByProtection(attacker, target))
            return;

        switch (effectType)
        {
            case "melee":
                if (IsMeleeEffectHit(attacker, target, effect))
                    ApplyDamage(target, effect.Damage, effect.Stun, effect.ArmorPen ?? attacker.Stats.ArmorPen);
                break;
            case "barrier":
                SpawnBarrier(attacker, target, effect);
                break;
        }
    }

    private void SpawnProjectile(PlayerBattleState owner, PlayerBattleState target, EffectData effect)
    {
        var spawn = ResolveProjectileSpawn(owner, target, effect);
        var velocity = ResolveProjectileVelocity(owner, effect);
        State.Projectiles.Add(new ProjectileState
        {
            ProjectileId = _nextProjectileId++,
            OwnerPlayerId = owner.PlayerId,
            X = spawn.X,
            Y = spawn.Y,
            VelocityX = velocity.X,
            VelocityY = velocity.Y,
            Damage = effect.Damage,
            ArmorPen = effect.ArmorPen ?? owner.Stats.ArmorPen,
            Stun = effect.Stun,
            Range = effect.Range,
            CollisionWidth = Math.Max(effect.CollisionWidth, (int)MathF.Round(effect.Range * 2f)),
            CollisionHeight = Math.Max(effect.CollisionHeight, (int)MathF.Round(effect.Range * 2f)),
            Lifetime = effect.Duration > 0f ? effect.Duration : ProjectileLifetime,
            AnimationKey = effect.ProjectileAnim,
            FacingRight = owner.FacingRight,
            HitFrames = effect.HitFrames?.ToList() ?? new List<int>(),
            RenderOffsetX = effect.Render.OffsetX,
            RenderOffsetY = effect.Render.OffsetY,
            Render = effect.Render
        });
    }

    private static (float X, float Y) ResolveProjectileSpawn(PlayerBattleState owner, PlayerBattleState target, EffectData effect)
    {
        string mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();
        if (mode is "targettop" or "targetabove" or "targettopdown")
            return (target.X + effect.SpawnOffsetX, target.Y + effect.SpawnOffsetY);

        if (mode is "casterfront" or "ownerfront")
            return (owner.X + (owner.FacingRight ? effect.SpawnOffsetX : -effect.SpawnOffsetX), owner.Y + effect.SpawnOffsetY);

        if (mode is "casterself" or "ownerself")
            return (owner.X + effect.SpawnOffsetX, owner.Y + effect.SpawnOffsetY);

        if (mode is "targetfront")
            return (target.X + (owner.X < target.X ? effect.SpawnOffsetX : -effect.SpawnOffsetX), target.Y + effect.SpawnOffsetY);

        return (owner.X + (owner.FacingRight ? 80f : -80f), owner.Y - 50f);
    }

    private static (float X, float Y) ResolveProjectileVelocity(PlayerBattleState owner, EffectData effect)
    {
        string mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();
        if (mode is "targettop" or "targetabove" or "targettopdown")
            return (0f, MathF.Abs(effect.Speed));

        return (owner.FacingRight ? effect.Speed : -effect.Speed, 0f);
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
            ArmorPen = effect.ArmorPen ?? owner.Stats.ArmorPen,
            Stun = effect.Stun,
            CollisionWidth = effect.CollisionWidth,
            CollisionHeight = effect.CollisionHeight,
            BlockEnemyAttack = effect.BlockEnemyAttack,
            BlockEnemyProjectile = effect.BlockEnemyProjectile,
            BlockEnemySkill = effect.BlockEnemySkill,
            RemainingTime = effect.Duration,
            Duration = effect.Duration,
            FacingRight = ResolveEffectFacing(owner, target, effect),
            HitFrames = effect.HitFrames?.ToList() ?? effect.Frames?.ToList() ?? new List<int>(),
            Render = effect.Render
        });
    }

    private void UpdateProjectiles(float dt)
    {
        for (int i = State.Projectiles.Count - 1; i >= 0; i--)
        {
            var projectile = State.Projectiles[i];
            float previousX = projectile.X;
            float previousY = projectile.Y;
            projectile.X += projectile.VelocityX * dt;
            projectile.Y += projectile.VelocityY * dt;
            projectile.Timer += dt;
            UpdateProjectileFrame(projectile);

            if (projectile.Timer >= projectile.Lifetime ||
                (projectile.Timer >= ProjectileCollisionDelay && IsProjectileBlockedByBarrier(projectile)))
            {
                State.Projectiles.RemoveAt(i);
                continue;
            }

            PlayerBattleState target = projectile.OwnerPlayerId == State.Player1.PlayerId ? State.Player2 : State.Player1;
            if (projectile.Timer >= ProjectileCollisionDelay
                && !target.IsDead
                && CanProjectileDamageOnCurrentFrame(projectile)
                && ProjectileIntersectsTarget(projectile, target, previousX, previousY))
            {
                if (!IsProjectileBlockedByProtection(projectile, target))
                    ApplyDamage(target, projectile.Damage, projectile.Stun, projectile.ArmorPen);

                State.Projectiles.RemoveAt(i);
            }
        }
    }

    private void UpdateProjectileFrame(ProjectileState projectile)
    {
        PlayerBattleState owner = projectile.OwnerPlayerId == State.Player1.PlayerId ? State.Player1 : State.Player2;
        if (string.IsNullOrWhiteSpace(projectile.AnimationKey) ||
            !owner.Stats.Animations.TryGetValue(projectile.AnimationKey, out var meta))
        {
            return;
        }

        int frameCount = Math.Max(1, meta.FrameCount);
        int frame = (int)MathF.Floor(projectile.Timer * Math.Max(1f, meta.Fps));
        projectile.CurrentFrame = Math.Clamp(frame, 0, frameCount - 1);
    }

    private static bool CanProjectileDamageOnCurrentFrame(ProjectileState projectile)
    {
        // Moving projectiles must damage on contact. Gating them by animation
        // hit frames makes long-range shots pass through when they reach the
        // target on a non-hit visual frame.
        return true;
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

            PlayerBattleState owner = effect.OwnerPlayerId == State.Player1.PlayerId ? State.Player1 : State.Player2;
            UpdateEffectFrame(effect, owner);
            PlayerBattleState target = effect.OwnerPlayerId == State.Player1.PlayerId ? State.Player2 : State.Player1;
            if (effect.Damage > 0
                && ShouldApplyEffectDamage(effect)
                && IntersectsRectangle(target, effect.X, effect.Y, effect.CollisionWidth, effect.CollisionHeight))
            {
                if (!IsBlockedByProtection(owner, target))
                    ApplyDamage(target, effect.Damage, effect.Stun, effect.ArmorPen);

                MarkEffectDamageApplied(effect);
            }
        }
    }

    private static void UpdateEffectFrame(EffectState effect, PlayerBattleState owner)
    {
        if (effect.Duration <= 0f ||
            !owner.Stats.Animations.TryGetValue(effect.AnimationKey, out var meta))
        {
            return;
        }

        float elapsed = Math.Max(0f, effect.Duration - effect.RemainingTime);
        int frameCount = Math.Max(1, meta.FrameCount);
        int frame = (int)MathF.Floor(elapsed * Math.Max(1f, meta.Fps));
        effect.CurrentFrame = Math.Clamp(frame, 0, frameCount - 1);
    }

    private static bool ShouldApplyEffectDamage(EffectState effect)
    {
        if (effect.LastDamageTick >= 0 && effect.HitFrames.Count == 0)
            return false;

        if (effect.HitFrames.Count == 0)
            return true;

        int hitFrame = effect.CurrentFrame + 1;
        return effect.HitFrames.Contains(hitFrame)
            && !effect.DamagedFrames.Contains(hitFrame);
    }

    private void MarkEffectDamageApplied(EffectState effect)
    {
        effect.LastDamageTick = State.ServerTick;
        if (effect.HitFrames.Count > 0)
            effect.DamagedFrames.Add(effect.CurrentFrame + 1);
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

    private static bool IsBaseAttackHit(PlayerBattleState attacker, PlayerBattleState target)
    {
        float width = Math.Max(1f, attacker.Stats.AttackRange);
        float centerX = attacker.X + (attacker.FacingRight ? 1f : -1f) * (BattleHitbox.CharacterWidth / 2f + width / 2f);
        return BattleHitbox.IntersectsRectangle(
            target.X,
            target.Y,
            centerX,
            attacker.Y,
            width,
            BattleHitbox.CharacterHeight);
    }

    private static bool IsMeleeEffectHit(PlayerBattleState attacker, PlayerBattleState target, EffectData effect)
    {
        if (IsCasterBothSpawn(effect))
        {
            float offsetX = MathF.Abs(effect.SpawnOffsetX);
            return IntersectsRectangle(target, attacker.X + offsetX, attacker.Y + effect.SpawnOffsetY, effect.CollisionWidth, effect.CollisionHeight)
                || IntersectsRectangle(target, attacker.X - offsetX, attacker.Y + effect.SpawnOffsetY, effect.CollisionWidth, effect.CollisionHeight);
        }

        var (x, y) = ResolveEffectSpawn(attacker, target, effect);
        return IntersectsRectangle(target, x, y, effect.CollisionWidth, effect.CollisionHeight);
    }

    private static bool IsCasterBothSpawn(EffectData effect)
    {
        string mode = (effect.SpawnMode ?? string.Empty).Trim().ToLowerInvariant();
        return mode is "casterboth" or "casteraround";
    }

    private static bool ProjectileIntersectsTarget(ProjectileState projectile, PlayerBattleState target, float previousX, float previousY)
    {
        float fallbackSize = Math.Max(1f, projectile.Range * 2f);
        float collisionWidth = projectile.CollisionWidth > 0 ? projectile.CollisionWidth : fallbackSize;
        float collisionHeight = projectile.CollisionHeight > 0 ? projectile.CollisionHeight : fallbackSize;

        float currentCenterX = projectile.X + projectile.RenderOffsetX;
        float currentCenterY = projectile.Y + projectile.RenderOffsetY;
        float previousCenterX = previousX + projectile.RenderOffsetX;
        float previousCenterY = previousY + projectile.RenderOffsetY;

        float sweptCenterX = (currentCenterX + previousCenterX) * 0.5f;
        float sweptCenterY = (currentCenterY + previousCenterY) * 0.5f;
        float sweptWidth = collisionWidth + MathF.Abs(currentCenterX - previousCenterX);
        float sweptHeight = collisionHeight + MathF.Abs(currentCenterY - previousCenterY);

        return BattleHitbox.IntersectsRectangle(
            target.X,
            target.Y,
            sweptCenterX,
            sweptCenterY,
            sweptWidth,
            sweptHeight);
    }

    private static bool IntersectsRectangle(PlayerBattleState target, float centerX, float centerY, float width, float height)
    {
        return BattleHitbox.IntersectsRectangle(target.X, target.Y, centerX, centerY, width, height);
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
            "casterself" => (
                owner.X + effect.SpawnOffsetX,
                owner.Y + effect.SpawnOffsetY),
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

    private static void ApplyDamage(PlayerBattleState target, float rawDamage, float stun, int armorPen = 0)
    {
        if (target.IsDead || target.IsInvulnerable)
            return;

        int damage = CalculateDamage(rawDamage, target.Stats.Def, armorPen);
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

    private static int CalculateDamage(float rawDamage, int targetDef, int armorPen)
    {
        if (rawDamage <= 0)
            return 0;

        int effectiveDef = Math.Max(0, targetDef - Math.Max(0, armorPen));
        return Math.Max(1, (int)MathF.Round(rawDamage - effectiveDef, MidpointRounding.AwayFromZero));
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
        {
            player.CurrentAnimation = ResolveAnimation(player, "Protection", "Idle");
        }
        else if (!player.IsGrounded)
        {
            player.CurrentAnimation = ResolveAnimation(player, "Jump", "Idle");
        }
        else if (Math.Abs(player.VelocityX) > 150f)
        {
            player.CurrentAnimation = ResolveAnimation(player, "Run", "Walk", "Idle");
        }
        else if (Math.Abs(player.VelocityX) > 1f)
        {
            player.CurrentAnimation = ResolveAnimation(player, "Walk", "Run", "Idle");
        }
        else
        {
            player.CurrentAnimation = "Idle";
        }
    }

    private static string ResolveAnimation(PlayerBattleState player, params string[] candidates)
    {
        foreach (string candidate in candidates)
        {
            if (player.Stats.Animations.ContainsKey(candidate))
                return candidate;
        }

        return "Idle";
    }

    private bool TryGetBufferedActionInput(int playerId, BattleInput continuousInput, out BattleInput actionInput)
    {
        actionInput = continuousInput;
        if (!_bufferedActionInputs.TryGetValue(playerId, out var buffered))
            return false;

        if (State.ServerTick - buffered.ServerTick > ActionInputBufferTicks)
        {
            _bufferedActionInputs.Remove(playerId);
            return false;
        }

        actionInput = buffered.Input;
        return true;
    }

    private static BattleInput CreateActionInput(BattleInput continuousInput, BattleInput bufferedInput)
    {
        return new BattleInput
        {
            PlayerId = continuousInput.PlayerId,
            Sequence = continuousInput.Sequence,
            ClientTick = bufferedInput.ClientTick,
            MoveX = bufferedInput.MoveX,
            JumpPressed = bufferedInput.JumpPressed,
            BlockHeld = bufferedInput.BlockHeld,
            AttackPressed = bufferedInput.AttackPressed,
            SkillSlot = bufferedInput.SkillSlot,
            DashPressed = bufferedInput.DashPressed,
            FacingRight = bufferedInput.FacingRight
        };
    }

    private static BattleInput CreateContinuousInput(BattleInput input)
    {
        var continuous = CloneInput(input);
        continuous.JumpPressed = false;
        continuous.AttackPressed = false;
        continuous.SkillSlot = 0;
        continuous.DashPressed = false;
        return continuous;
    }

    private static BattleInput CloneInput(BattleInput input)
    {
        return new BattleInput
        {
            PlayerId = input.PlayerId,
            Sequence = input.Sequence,
            ClientTick = input.ClientTick,
            MoveX = input.MoveX,
            JumpPressed = input.JumpPressed,
            BlockHeld = input.BlockHeld,
            AttackPressed = input.AttackPressed,
            SkillSlot = input.SkillSlot,
            DashPressed = input.DashPressed,
            FacingRight = input.FacingRight
        };
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

    private sealed class BufferedActionInput
    {
        public BufferedActionInput(BattleInput input, int serverTick)
        {
            Input = input;
            ServerTick = serverTick;
        }

        public BattleInput Input { get; }
        public int ServerTick { get; }
    }
}
