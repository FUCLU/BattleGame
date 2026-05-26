using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using BattleGame.Client.Config;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.AI;
using BattleGame.Client.Game.Dungeon;
using BattleGame.Client.Game.Gameplay;
using BattleGame.Client.Game.Input;
using BattleGame.Client.Game.Rendering;
using BattleGame.Client.Game.Systems;
using BattleGame.Shared.Models;
using BattleGame.Shared.Simulation;

namespace BattleGame.Client.Game
{
    public class GameEngine
    {
        private const float GroundBottomMargin = 140f;
        private const float DefaultMapPadding = 50f;
        private const float MinPlayableMapWidth = 420f;
        private const float DefaultSpawnLeftRatio = 0.15f;
        private const float DefaultSpawnRightRatio = 0.85f;
        private const float DungeonGroundOffsetY = 60f;
        private const float CameraDeadZoneWidthRatio = 0.40f;
        private Entity _player = null!;
        private Entity? _enemy;

        private readonly AnimationSystem _animSystem = new();
        private readonly MovementSystem _moveSystem = new();
        private ProjectileSystem _projectileSystem = null!;
        private CombatSystem _playerCombatSystem = null!;
        private CombatSystem _enemyCombatSystem = null!;

        private CharacterRenderer _renderer = null!;
        private BarrierRenderer _barrierRenderer = null!;
        private PlayerController _controller = null!;
        private PlayerController? _localEnemyController;
        private readonly Dictionary<string, SpriteAnimation> _onlineEffectAnimations = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<ProjectileState> _onlineProjectiles = new();
        private readonly List<EffectState> _onlineEffects = new();
        private readonly Dictionary<int, OnlineProjectileVisual> _onlineProjectileVisuals = new();
        private readonly Dictionary<int, OnlineEffectVisual> _onlineEffectVisuals = new();
        private readonly Dictionary<int, VisualFrameState> _projectileFrames = new();
        private readonly Dictionary<int, VisualFrameState> _effectFrames = new();
        private readonly Dictionary<string, SpriteAnimation> _playerAnimations = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, SpriteAnimation> _enemyAnimations = new(StringComparer.OrdinalIgnoreCase);
        private int _localOnlinePlayerId;
        private int _remoteOnlinePlayerId;
        private int _onlineVisualRoundNumber = -1;
        private LocalActionPrediction? _localActionPrediction;
        private Image? _mapBackground;
        private readonly List<ParallaxLayer> _parallaxLayers = new();
        private ParallaxLayer? _foregroundLayer;
        private readonly Dictionary<string, List<MapObjectRenderItem>> _mapObjectsByLayer = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Image> _mapObjectImageCache = new(StringComparer.OrdinalIgnoreCase);
        private DungeonRunController? _dungeonRun;
        private readonly bool _hideDefaultEnemyInDungeon;
        private BossAiController? _bossAiController;
        private string? _activeDungeonSpawnToken;
        private bool _activeDungeonSpawnDefeated;
        private int _dungeonDefeatedCount;

        private DateTime _lastTime;
        private float _groundY;
        private float _mapWidth;
        private float _cameraX;
        private int _formWidth;
        private int _formHeight;
        private readonly string _mapId;
        private readonly string _clientRoot;

        public Entity Player => _player;
        public Entity? Enemy => _enemy;
        public bool IsDungeonCompleted => _dungeonRun?.IsCompleted == true;
        public int DungeonDefeatedCount => _dungeonDefeatedCount;

        public GameEngine(string characterId, string mapId, int formWidth, int formHeight, string? enemyCharacterId = null, bool localTwoPlayer = false)
        {
            _mapId = mapId;
            _clientRoot = ResolveClientRoot();
            _formWidth = formWidth;
            _formHeight = formHeight;
            _groundY = GetGroundY(mapId, formHeight);
            _mapWidth = GetWorldWidth(mapId, formWidth);
            _hideDefaultEnemyInDungeon = IsDungeonParallaxMapId(mapId);

            RecomputeMapBounds();

            // Load map background directly
            LoadMapBackground(mapId);
            InitializeDungeonRun(mapId);

            // Load animations trước — ProjectileSystem cần để render
            var loader = new AnimationLoader("Assets");
            var animations = loader.Load(characterId);
            foreach (var kv in animations)
                _playerAnimations[kv.Key] = kv.Value;

            var animKeys = new Dictionary<string, object>();
            foreach (var kv in _playerAnimations)
                animKeys[kv.Key] = kv.Value;

            // Khởi tạo theo thứ tự dependency
            _projectileSystem = new ProjectileSystem(_onlineEffectAnimations);
            _playerCombatSystem = new CombatSystem(_projectileSystem);
            _enemyCombatSystem = new CombatSystem(_projectileSystem);

            // Tạo nhân vật
            float leftSpawnX = ResolveDefaultSpawnLeft();
            float rightSpawnX = ResolveDefaultSpawnRight(leftSpawnX);
            _player = CharacterFactory.Create(characterId, leftSpawnX, _groundY, animKeys);

            // Enemy theo character đối thủ đã chọn từ RoomForm/MatchFound.
            if (!_hideDefaultEnemyInDungeon)
            {
                string resolvedEnemyCharacterId = string.IsNullOrWhiteSpace(enemyCharacterId)
                    ? "samurai"
                    : enemyCharacterId.Trim().ToLowerInvariant();

                var enemyLoader = new AnimationLoader("Assets");
                foreach (var kv in enemyLoader.Load(resolvedEnemyCharacterId))
                    _enemyAnimations[kv.Key] = kv.Value;
                var enemyAnimKeys = new Dictionary<string, object>();
                foreach (var kv in _enemyAnimations)
                    enemyAnimKeys[kv.Key] = kv.Value;
                float enemyStartX = IsDungeonParallaxMap
                    ? Math.Min(_mapWidth - 300f, 7600f)
                    : rightSpawnX;
                _enemy = CharacterFactory.Create(resolvedEnemyCharacterId, enemyStartX, _groundY, enemyAnimKeys);
                _enemy.Get<MovementComponent>().FacingRight = _player.Get<MovementComponent>().X >= enemyStartX;
            }
            RefreshCombinedAnimations();

            // Đăng ký target cho projectile collision
            _projectileSystem.RegisterTarget(_player);
            if (_enemy != null)
                _projectileSystem.RegisterTarget(_enemy);

            // Chia sẻ barrier giữa cả hai phía
            _playerCombatSystem.SetBarrierProvider(GetAllBarriers);
            _enemyCombatSystem.SetBarrierProvider(GetAllBarriers);
            _projectileSystem.SetBarrierProvider(GetAllBarriers);

            // Player đánh enemy, Enemy đánh player
            if (_enemy != null)
            {
                _playerCombatSystem.SetTarget(_enemy);
                _enemyCombatSystem.SetTarget(_player);
            }
            _renderer = new CharacterRenderer(_player.Id, _playerAnimations, _enemyAnimations);
            _barrierRenderer = new BarrierRenderer(_onlineEffectAnimations);
            _controller = new PlayerController(_player, _enemy ?? _player, _playerCombatSystem);
            if (localTwoPlayer && _enemy != null)
            {
                _localEnemyController = new PlayerController(
                    _enemy,
                    _player,
                    _enemyCombatSystem,
                    Keys.Left,
                    Keys.Right,
                    Keys.Down,
                    Keys.NumPad1,
                    Keys.NumPad4,
                    Keys.NumPad5,
                    Keys.NumPad2);
            }
            _lastTime = DateTime.Now;
            UpdateCamera();
        }

        private float ResolveDefaultSpawnLeft()
        {
            float leftSpawnX = _mapWidth * DefaultSpawnLeftRatio;
            float minX = _moveSystem.MapLeft + 80f;
            float maxX = _moveSystem.MapRight - 200f;
            return ClampSafe(leftSpawnX, minX, maxX, (_moveSystem.MapLeft + _moveSystem.MapRight) * 0.5f);
        }

        private float ResolveDefaultSpawnRight(float leftSpawnX)
        {
            float rightSpawnX = _mapWidth * DefaultSpawnRightRatio;
            rightSpawnX = Math.Max(rightSpawnX, leftSpawnX + 200f);
            float minX = leftSpawnX + 150f;
            float maxX = _moveSystem.MapRight - 40f;
            return ClampSafe(rightSpawnX, minX, maxX, _moveSystem.MapRight - 60f);
        }

        public void ApplyOnlineWorldState(BattleState state, int localPlayerId, bool mirrorView = false)
        {
            PlayerBattleState local = state.Player1.PlayerId == localPlayerId ? state.Player1 : state.Player2;
            PlayerBattleState remote = state.Player1.PlayerId == localPlayerId ? state.Player2 : state.Player1;
            int incomingRound = state.RoundNumber <= 0 ? 1 : state.RoundNumber;
            if (_onlineVisualRoundNumber != incomingRound ||
                (_localOnlinePlayerId != 0 && _localOnlinePlayerId != local.PlayerId) ||
                (_remoteOnlinePlayerId != 0 && _remoteOnlinePlayerId != remote.PlayerId))
            {
                ClearOnlineVisuals();
                _onlineVisualRoundNumber = incomingRound;
            }

            _localOnlinePlayerId = local.PlayerId;
            _remoteOnlinePlayerId = remote.PlayerId;

            EnsureOnlineCharacterAssets(_player, local.CharacterId, _playerAnimations);
            if (_enemy != null)
                EnsureOnlineCharacterAssets(_enemy, remote.CharacterId, _enemyAnimations);
            RefreshCombinedAnimations();

            if (mirrorView)
            {
                local = MirrorSnapshot(local);
                remote = MirrorSnapshot(remote);
            }

            ApplySnapshot(_player, local, isLocal: true);
            if (_enemy != null)
                ApplySnapshot(_enemy, remote, isLocal: false);

            _onlineProjectiles.Clear();
            if (mirrorView)
            {
                foreach (var projectile in state.Projectiles)
                {
                    _onlineProjectiles.Add(new ProjectileState
                    {
                        ProjectileId = projectile.ProjectileId,
                        OwnerPlayerId = projectile.OwnerPlayerId,
                        X = MirrorX(projectile.X),
                        Y = projectile.Y,
                        VelocityX = -projectile.VelocityX,
                        VelocityY = projectile.VelocityY,
                        Damage = projectile.Damage,
                        ArmorPen = projectile.ArmorPen,
                        Stun = projectile.Stun,
                        Range = projectile.Range,
                        CollisionWidth = projectile.CollisionWidth,
                        CollisionHeight = projectile.CollisionHeight,
                        Lifetime = projectile.Lifetime,
                        Timer = projectile.Timer,
                        AnimationKey = projectile.AnimationKey,
                        CurrentFrame = projectile.CurrentFrame,
                        HitFrames = new List<int>(projectile.HitFrames),
                        FacingRight = !projectile.FacingRight,
                        RenderOffsetX = -projectile.RenderOffsetX,
                        RenderOffsetY = projectile.RenderOffsetY,
                        Render = MirrorRender(projectile.Render)
                    });
                }
            }
            else
            {
                _onlineProjectiles.AddRange(state.Projectiles);
            }
            _onlineEffects.Clear();
            if (mirrorView)
            {
                foreach (var effect in state.Effects)
                {
                    _onlineEffects.Add(new EffectState
                    {
                        EffectId = effect.EffectId,
                        OwnerPlayerId = effect.OwnerPlayerId,
                        EffectType = effect.EffectType,
                        AnimationKey = effect.AnimationKey,
                        X = MirrorX(effect.X),
                        Y = effect.Y,
                        Damage = effect.Damage,
                        ArmorPen = effect.ArmorPen,
                        Stun = effect.Stun,
                        CollisionWidth = effect.CollisionWidth,
                        CollisionHeight = effect.CollisionHeight,
                        BlockEnemyAttack = effect.BlockEnemyAttack,
                        BlockEnemyProjectile = effect.BlockEnemyProjectile,
                        BlockEnemySkill = effect.BlockEnemySkill,
                        CurrentFrame = effect.CurrentFrame,
                        HitFrames = new List<int>(effect.HitFrames),
                        DamagedFrames = new HashSet<int>(effect.DamagedFrames),
                        Duration = effect.Duration,
                        RemainingTime = effect.RemainingTime,
                        FacingRight = !effect.FacingRight,
                        LastDamageTick = effect.LastDamageTick,
                        Render = MirrorRender(effect.Render)
                    });
                }
            }
            else
            {
                _onlineEffects.AddRange(state.Effects);
            }

            SyncOnlineProjectileVisuals(_onlineProjectiles);
            SyncOnlineEffectVisuals(_onlineEffects);
            UpdateCamera();
        }

        private void EnsureOnlineCharacterAssets(
            Entity entity,
            string characterId,
            Dictionary<string, SpriteAnimation> animationSet)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return;

            var ch = entity.Get<CharacterComponent>();
            if (string.Equals(ch.CharacterId, characterId, StringComparison.OrdinalIgnoreCase) &&
                animationSet.Count > 0)
            {
                return;
            }

            var loader = new AnimationLoader("Assets");
            var loadedAnimations = loader.Load(characterId);
            if (loadedAnimations.Count > 0)
            {
                animationSet.Clear();
                foreach (var kv in loadedAnimations)
                    animationSet[kv.Key] = kv.Value;
            }

            try
            {
                string path = CharacterDefinitionLoader.ResolveConfigPath(_clientRoot, characterId);
                var definition = CharacterDefinitionLoader.Load(path);
                ch.CharacterId = definition.Id;
                ch.BaseStats = definition.Stats;
                ch.Render = definition.Render;
                ch.AvailableAnimations = new HashSet<string>(animationSet.Keys, StringComparer.OrdinalIgnoreCase);
                ch.AnimationDurations = BuildAnimationDurations(animationSet);
                ch.Skill1 = definition.Skill1;
                ch.Skill2 = definition.Skill2;
                ch.AttackEffects = definition.AttackEffects;
                ch.AttackAnimCount = Math.Max(1, animationSet.Keys.Count(k => k.StartsWith("Attack_", StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GameEngine] Failed to refresh online character assets for '{characterId}': {ex}");
            }
        }

        private PlayerBattleState MirrorSnapshot(PlayerBattleState snapshot)
        {
            return new PlayerBattleState
            {
                PlayerId = snapshot.PlayerId,
                CharacterId = snapshot.CharacterId,
                Stats = snapshot.Stats,
                X = MirrorX(snapshot.X),
                Y = snapshot.Y,
                VelocityX = -snapshot.VelocityX,
                VelocityY = snapshot.VelocityY,
                FacingRight = !snapshot.FacingRight,
                IsGrounded = snapshot.IsGrounded,
                Hp = snapshot.Hp,
                Mana = snapshot.Mana,
                IsProtecting = snapshot.IsProtecting,
                IsAttacking = snapshot.IsAttacking,
                IsUsingSkill = snapshot.IsUsingSkill,
                IsDashing = snapshot.IsDashing,
                IsHurt = snapshot.IsHurt,
                IsStunned = snapshot.IsStunned,
                IsDead = snapshot.IsDead,
                ActionTimer = snapshot.ActionTimer,
                ActionDuration = snapshot.ActionDuration,
                ActionHitDone = snapshot.ActionHitDone,
                CurrentSkillSlot = snapshot.CurrentSkillSlot,
                CurrentSkillAnimation = snapshot.CurrentSkillAnimation,
                HurtTimer = snapshot.HurtTimer,
                StunTimer = snapshot.StunTimer,
                DashTimer = snapshot.DashTimer,
                Skill1Cooldown = snapshot.Skill1Cooldown,
                Skill2Cooldown = snapshot.Skill2Cooldown,
                CurrentAnimation = snapshot.CurrentAnimation,
                CurrentFrame = snapshot.CurrentFrame,
                CurrentActionId = snapshot.CurrentActionId,
                CurrentActionTick = snapshot.CurrentActionTick
            };
        }

        private float MirrorX(float x) => _mapWidth - x;

        private static Dictionary<string, float> BuildAnimationDurations(Dictionary<string, SpriteAnimation> animations)
        {
            var durations = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in animations)
            {
                if (kv.Value.Frames.Length == 0)
                    continue;

                durations[kv.Key] = kv.Value.Frames.Length / Math.Max(1f, kv.Value.Fps);
            }

            return durations;
        }

    public void UpdateOnlineVisuals(float dt)
    {
        dt = Math.Min(dt, 0.05f);
        UpdateLocalActionPrediction(dt);

        _renderer.Update(_player, dt);
        if (_enemy != null)
            _renderer.Update(_enemy, dt);

        foreach (var item in _onlineProjectileVisuals.ToList())
        {
            var visual = item.Value;
            var projectile = visual.State;
            if (!TryResolveOnlineAnimation(projectile.OwnerPlayerId, projectile.AnimationKey, out var anim))
            {
                if (!visual.IsServerActive)
                    RemoveOnlineProjectileVisual(item.Key);
                continue;
            }

            visual.AgeSeconds += dt;
            if (visual.IsServerActive)
            {
                projectile.X += projectile.VelocityX * dt;
                projectile.Y += projectile.VelocityY * dt;
                projectile.Timer += dt;
            }
            else
            {
                visual.MissingSeconds += dt;
            }

            AdvanceVisualFrame(
                _projectileFrames,
                projectile.ProjectileId,
                anim,
                dt);

            if (!visual.IsServerActive && ShouldRemoveOnlineVisual(_projectileFrames[item.Key], anim, visual))
                RemoveOnlineProjectileVisual(item.Key);
            }

            foreach (var item in _onlineEffectVisuals.ToList())
            {
                var visual = item.Value;
                var effect = visual.State;
                if (!TryResolveOnlineAnimation(effect.OwnerPlayerId, effect.AnimationKey, out var anim))
                {
                    if (!visual.IsServerActive)
                        RemoveOnlineEffectVisual(item.Key);
                    continue;
                }

                visual.AgeSeconds += dt;
                if (!visual.IsServerActive)
                    visual.MissingSeconds += dt;

                AdvanceVisualFrame(
                    _effectFrames,
                    effect.EffectId,
                    anim,
                    dt);

                if (!visual.IsServerActive && ShouldRemoveOnlineVisual(_effectFrames[item.Key], anim, visual))
                    RemoveOnlineEffectVisual(item.Key);
            }
        }

        public bool TryPredictLocalAction(BattleInput input)
        {
            if (input.PlayerId <= 0)
                return false;

            if (_localOnlinePlayerId != 0 && input.PlayerId != _localOnlinePlayerId)
                return false;

            var ch = _player.Get<CharacterComponent>();
            if (!input.BlockHeld)
                ch.IsProtecting = false;

            if (ch.IsDead || ch.IsBusy || ch.IsProtecting)
                return false;

            if (input.DashPressed)
                return TryPredictLocalDash(ch, _player.Get<MovementComponent>(), _player.Get<SpriteComponent>());

            if (input.SkillSlot is 1 or 2)
                return TryPredictLocalSkill(ch, _player.Get<SpriteComponent>(), input.SkillSlot);

            if (input.AttackPressed)
                return TryPredictLocalAttack(ch, _player.Get<SpriteComponent>());

            return false;
        }

        public void ClearLocalActionPrediction()
        {
            _localActionPrediction = null;
        }

        private bool TryPredictLocalSkill(CharacterComponent ch, SpriteComponent sp, int slot)
        {
            SkillData? skill = slot == 1 ? ch.Skill1 : ch.Skill2;
            if (skill == null)
                return false;

            float cooldown = slot == 1 ? ch.Skill1Cooldown : ch.Skill2Cooldown;
            if (cooldown > 0.05f || ch.Mana < skill.ManaCost)
                return false;

            string requestedAnimation = string.IsNullOrWhiteSpace(skill.Animation)
                ? $"Skill{slot}"
                : skill.Animation;
            string animation = ResolveActionAnimation(requestedAnimation, $"Skill{slot}");
            if (string.IsNullOrWhiteSpace(animation))
                return false;

            float duration = EstimateLocalAnimationDuration(animation, 0.7f);

            ch.IsAttacking = false;
            ch.IsDashing = false;
            ch.IsUsingSkill = true;
            ch.CurrentSkillSlot = slot;
            ch.CurrentSkillAnim = animation;
            ch.ActionTimer = duration;
            ch.ActionDuration = duration;
            ch.Mana = Math.Max(0, ch.Mana - skill.ManaCost);
            if (slot == 1)
                ch.Skill1Cooldown = skill.Cooldown;
            else
                ch.Skill2Cooldown = skill.Cooldown;
            ch.TriggeredEffects.Clear();
            ch.TriggeredFrames.Clear();

            BeginPredictedAnimation(sp, animation);
            StartLocalActionPrediction(PredictedActionKind.Skill, animation, slot, duration);
            return true;
        }

        private bool TryPredictLocalAttack(CharacterComponent ch, SpriteComponent sp)
        {
            string animation = ResolvePredictedAttackAnimation(ch, sp);
            if (string.IsNullOrWhiteSpace(animation))
                return false;

            float duration = EstimateLocalAnimationDuration(animation, ch.ActionDuration > 0f ? ch.ActionDuration : 0.7f);

            ch.IsUsingSkill = false;
            ch.IsDashing = false;
            ch.IsAttacking = true;
            ch.CurrentAttackAnim = animation;
            ch.ActionTimer = duration;
            ch.ActionDuration = duration;
            ch.AttackHitDone = false;
            ch.TriggeredAttackEffects.Clear();
            ch.TriggeredAttackFrames.Clear();

            BeginPredictedAnimation(sp, animation);
            StartLocalActionPrediction(PredictedActionKind.Attack, animation, 0, duration);
            return true;
        }

        private bool TryPredictLocalDash(CharacterComponent ch, MovementComponent mv, SpriteComponent sp)
        {
            string animation = ResolveActionAnimation("Dash", "Run", "Walk");
            if (string.IsNullOrWhiteSpace(animation))
                return false;

            float duration = ch.DashDuration > 0f ? ch.DashDuration : 0.22f;

            ch.IsUsingSkill = false;
            ch.IsAttacking = false;
            ch.IsDashing = true;
            ch.DashTimer = duration;
            ch.ActionTimer = duration;
            ch.ActionDuration = duration;
            mv.VelocityX = (mv.FacingRight ? 1f : -1f) * mv.Speed * ch.DashSpeedMultiplier;

            BeginPredictedAnimation(sp, animation);
            StartLocalActionPrediction(PredictedActionKind.Dash, animation, 0, duration);
            return true;
        }

        private void UpdateLocalActionPrediction(float dt)
        {
            if (_localActionPrediction == null)
                return;

            var ch = _player.Get<CharacterComponent>();
            ch.ActionTimer = Math.Max(0f, ch.ActionTimer - dt);
            if (_localActionPrediction.Kind == PredictedActionKind.Dash)
                ch.DashTimer = Math.Max(0f, ch.DashTimer - dt);

            _localActionPrediction.HoldSeconds -= dt;
            if (_localActionPrediction.HoldSeconds <= 0f)
            {
                EndLocalPredictedAction(ch, _player.Get<SpriteComponent>());
                _localActionPrediction = null;
            }
        }

        private bool ShouldKeepLocalPrediction(PlayerBattleState snapshot)
        {
            if (_localActionPrediction == null || _localActionPrediction.HoldSeconds <= 0f)
                return false;

            if (snapshot.IsDead || snapshot.IsHurt || snapshot.IsStunned)
                return false;

            return !IsServerDrivenAction(snapshot);
        }

        private void StartLocalActionPrediction(PredictedActionKind kind, string animation, int skillSlot, float duration)
        {
            _localActionPrediction = new LocalActionPrediction
            {
                Kind = kind,
                Animation = animation,
                SkillSlot = skillSlot,
                HoldSeconds = Math.Clamp(duration, 0.12f, 2.0f)
            };
        }

        private static void EndLocalPredictedAction(CharacterComponent ch, SpriteComponent sp)
        {
            ch.IsAttacking = false;
            ch.IsUsingSkill = false;
            ch.IsDashing = false;
            ch.CurrentSkillSlot = 0;
            ch.ActionTimer = 0f;
            ch.DashTimer = 0f;

            if (!ch.IsDead && !ch.IsHurt && !ch.IsStunned &&
                !string.Equals(sp.CurrentAnimation, "Idle", StringComparison.OrdinalIgnoreCase))
            {
                sp.CurrentAnimation = "Idle";
                sp.CurrentFrame = 0;
                sp.FrameTimer = 0f;
                sp.AnimationFinished = false;
            }
        }

        private static void BeginPredictedAnimation(SpriteComponent sp, string animation)
        {
            sp.CurrentAnimation = animation;
            sp.CurrentFrame = 0;
            sp.FrameTimer = 0f;
            sp.AnimationFinished = false;
        }

        private string ResolvePredictedAttackAnimation(CharacterComponent ch, SpriteComponent sp)
        {
            int attackCount = Math.Max(1, ch.AttackAnimCount);
            int actionId = Math.Max(0, sp.SyncedActionId);

            for (int attempt = 0; attempt < attackCount; attempt++)
            {
                int idx = ((actionId + attempt) % attackCount) + 1;
                string animation = $"Attack_{idx}";
                if (_playerAnimations.ContainsKey(animation))
                    return animation;
            }

            return string.Empty;
        }

        private string ResolveActionAnimation(params string[] candidates)
        {
            foreach (string candidate in candidates)
            {
                if (!string.IsNullOrWhiteSpace(candidate) && _playerAnimations.ContainsKey(candidate))
                    return candidate;
            }

            return string.Empty;
        }

        private float EstimateLocalAnimationDuration(string animation, float fallbackDuration)
        {
            if (_playerAnimations.TryGetValue(animation, out var anim) &&
                anim.Frames.Length > 0 &&
                anim.Fps > 0f)
            {
                return Math.Max(0.05f, anim.Frames.Length / Math.Max(1f, anim.Fps));
            }

            return Math.Max(0.05f, fallbackDuration);
        }

        private bool IsDungeonParallaxMap => DungeonMapRegistry.IsDungeonMap(_mapId);

        private static float GetWorldWidth(string mapId, int formWidth)
            => DungeonMapRegistry.TryGet(mapId, out var dungeonMap)
                ? dungeonMap.WorldWidth
                : Math.Max(MinPlayableMapWidth, formWidth);

        private static float GetGroundY(string mapId, int formHeight)
            => formHeight - GroundBottomMargin +
               (IsDungeonParallaxMapId(mapId) ? DungeonGroundOffsetY : 0f);

        private static bool IsDungeonParallaxMapId(string mapId)
            => DungeonMapRegistry.IsDungeonMap(mapId);

        private static string ResolveClientRoot()
        {
            string startDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? AppDomain.CurrentDomain.BaseDirectory;

            return ClientContentRoot.Resolve(startDirectory);
        }

        public void Resize(int formWidth, int formHeight)
        {
            int width = Math.Max(1, formWidth);
            int height = Math.Max(1, formHeight);

            float oldGroundY = _groundY;
            _formWidth = width;
            _formHeight = height;
            _groundY = GetGroundY(_mapId, height);
            _mapWidth = GetWorldWidth(_mapId, width);

            RecomputeMapBounds();

            ResizeEntity(_player, oldGroundY, _groundY);
            if (_enemy != null)
                ResizeEntity(_enemy, oldGroundY, _groundY);
            UpdateCamera();
        }

        private void ResizeEntity(Entity entity, float oldGroundY, float newGroundY)
        {
            var mv = entity.Get<MovementComponent>();
            float groundDelta = newGroundY - oldGroundY;

            mv.GroundY = newGroundY;
            mv.Y = mv.IsGrounded || mv.Y >= oldGroundY - 1f
                ? newGroundY
                : mv.Y + groundDelta;

            mv.X = ClampSafe(mv.X, _moveSystem.MapLeft, _moveSystem.MapRight, _moveSystem.MapLeft);
        }

        private void RecomputeMapBounds()
        {
            _moveSystem.MapLeft = DefaultMapPadding;
            _moveSystem.MapRight = Math.Max(_moveSystem.MapLeft + 1f, _mapWidth - DefaultMapPadding);
        }

        private static float ClampSafe(float value, float min, float max, float fallback)
        {
            if (max < min)
                return fallback;
            return Math.Clamp(value, min, max);
        }

        private IEnumerable<Entity> GetAllBarriers()
        {
            return _playerCombatSystem.GetBarriers().Concat(_enemyCombatSystem.GetBarriers());
        }

        public void Update(float dt)
        {
            dt = Math.Min(dt, 0.05f);

            _controller.Update();
            _localEnemyController?.Update();
            if (_enemy != null)
                _bossAiController?.Update(dt, _enemy, _player, _enemyCombatSystem);

            _renderer.Update(_player, dt);
            if (_enemy != null)
                _renderer.Update(_enemy, dt);

            _playerCombatSystem.Update(_player, dt);
            if (_enemy != null)
                _enemyCombatSystem.Update(_enemy, dt);

            _moveSystem.Update(_player, dt);
            if (_enemy != null)
                _moveSystem.Update(_enemy, dt);

            _animSystem.Update(_player, dt);
            if (_enemy != null)
                _animSystem.Update(_enemy, dt);

            _projectileSystem.Update(dt);
            UpdateDungeonRun();

            foreach (var barrier in _playerCombatSystem.GetBarriers())
            {
                _barrierRenderer.Update(barrier, dt);
            }
            if (_enemy != null)
            {
                foreach (var barrier in _enemyCombatSystem.GetBarriers())
                {
                    _barrierRenderer.Update(barrier, dt);
                }
            }

            UpdateCamera();
        }

        public void UpdatePresentation(float dt)
        {
            dt = Math.Min(dt, 0.05f);

            _renderer.Update(_player, dt);
            if (_enemy != null)
                _renderer.Update(_enemy, dt);
        }

        public void Draw(Graphics g)
        {
            bool drawEnemyHealthOverForeground = false;

            if (IsDungeonParallaxMap && _parallaxLayers.Count > 0)
            {
                DrawParallaxBackground(g);
            }
            else if (_mapBackground != null)
            {
                g.DrawImage(_mapBackground, 0, 0, _formWidth, _formHeight);
            }

            var state = g.Save();
            g.TranslateTransform(-_cameraX, 0f);

            _renderer.Draw(g, _player);
            if (_enemy != null)
            {
                _renderer.Draw(g, _enemy);
            }
            _projectileSystem.Draw(g);
            DrawOnlineProjectiles(g);

            g.Restore(state);

            if (IsDungeonParallaxMap && _foregroundLayer != null)
            {
                DrawParallaxLayer(g, _foregroundLayer);
                DrawLayerObjects(g, _foregroundLayer);
            }

            var effectState = g.Save();
            g.TranslateTransform(-_cameraX, 0f);
            DrawBarrierEffects(g);
            DrawOnlineEffects(g);
            g.Restore(effectState);

            if (drawEnemyHealthOverForeground && _enemy != null)
            {
                var enemyState = g.Save();
                g.TranslateTransform(-_cameraX, 0f);
                _renderer.DrawHealthBar(g, _enemy);
                g.Restore(enemyState);
            }
        }

        private void DrawBarrierEffects(Graphics g)
        {
            foreach (var barrier in _playerCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }

            if (_enemy == null)
                return;

            foreach (var barrier in _enemyCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }
        }

        private void RefreshCombinedAnimations()
        {
            _onlineEffectAnimations.Clear();

            foreach (var kv in _playerAnimations)
                _onlineEffectAnimations[kv.Key] = kv.Value;

            foreach (var kv in _enemyAnimations)
                _onlineEffectAnimations.TryAdd(kv.Key, kv.Value);
        }

        private static EffectRenderData MirrorRender(EffectRenderData render)
            => new()
            {
                Scale = render.Scale,
                OffsetX = render.OffsetX,
                OffsetY = render.OffsetY,
                UseSpriteSize = render.UseSpriteSize,
                AlignY = render.AlignY,
                FacingSource = render.FacingSource
            };

        private void ApplySnapshot(Entity entity, PlayerBattleState snapshot, bool isLocal)
        {
            var mv = entity.Get<MovementComponent>();
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();
            bool serverAction = IsServerDrivenAction(snapshot);
            if (isLocal &&
                _localActionPrediction != null &&
                (serverAction || snapshot.IsDead || snapshot.IsHurt || snapshot.IsStunned))
            {
                _localActionPrediction = null;
            }

            bool keepPrediction = isLocal && ShouldKeepLocalPrediction(snapshot);

            mv.X = snapshot.X;
            mv.Y = snapshot.Y;
            mv.VelocityX = snapshot.VelocityX;
            mv.VelocityY = snapshot.VelocityY;
            mv.FacingRight = snapshot.FacingRight;
            mv.IsGrounded = snapshot.IsGrounded;

            ch.Hp = Math.Clamp(snapshot.Hp, 0, ch.BaseStats.Hp);
            int snapshotMana = Math.Clamp(snapshot.Mana, 0, ch.BaseStats.Mana);
            if (keepPrediction && _localActionPrediction?.Kind == PredictedActionKind.Skill)
                ch.Mana = Math.Min(ch.Mana, snapshotMana);
            else
                ch.Mana = snapshotMana;

            if (!keepPrediction)
            {
                ch.IsProtecting = snapshot.IsProtecting;
                ch.IsAttacking = snapshot.IsAttacking;
                ch.IsUsingSkill = snapshot.IsUsingSkill;
                ch.IsDashing = snapshot.IsDashing;
                ch.IsHurt = snapshot.IsHurt;
                ch.IsStunned = snapshot.IsStunned;
                ch.StunTimer = snapshot.StunTimer;
                ch.HurtTimer = snapshot.HurtTimer;
                ch.DashTimer = snapshot.DashTimer;
                ch.ActionTimer = snapshot.ActionTimer;
                ch.ActionDuration = snapshot.ActionDuration;
                ch.AttackHitDone = snapshot.ActionHitDone;
                ch.CurrentSkillSlot = snapshot.CurrentSkillSlot;
                if (!string.IsNullOrWhiteSpace(snapshot.CurrentSkillAnimation))
                    ch.CurrentSkillAnim = snapshot.CurrentSkillAnimation;
                if (snapshot.IsAttacking && !string.IsNullOrWhiteSpace(snapshot.CurrentAnimation))
                    ch.CurrentAttackAnim = snapshot.CurrentAnimation;
                ch.IsDead = snapshot.IsDead;
            }

            if (keepPrediction && _localActionPrediction?.Kind == PredictedActionKind.Skill)
                ApplyPredictedCooldownSnapshot(ch, snapshot);
            else
                ApplyCooldownSnapshot(ch, snapshot);

            bool animationChanged = !string.IsNullOrWhiteSpace(snapshot.CurrentAnimation) &&
                !string.Equals(sp.CurrentAnimation, snapshot.CurrentAnimation, StringComparison.OrdinalIgnoreCase);
            bool actionStarted = serverAction &&
                snapshot.CurrentActionId > 0 &&
                snapshot.CurrentActionId != sp.SyncedActionId;

            if (!keepPrediction && animationChanged)
            {
                sp.CurrentAnimation = snapshot.CurrentAnimation;
                sp.CurrentFrame = 0;
                sp.FrameTimer = 0f;
                sp.AnimationFinished = false;
            }

            if (!keepPrediction && actionStarted)
            {
                int serverFrame = Math.Max(0, snapshot.CurrentFrame);
                bool keepLocalFrame = isLocal &&
                    !string.IsNullOrWhiteSpace(snapshot.CurrentAnimation) &&
                    string.Equals(sp.CurrentAnimation, snapshot.CurrentAnimation, StringComparison.OrdinalIgnoreCase) &&
                    sp.CurrentFrame >= serverFrame;

                if (!keepLocalFrame)
                {
                    sp.CurrentFrame = serverFrame;
                    sp.FrameTimer = 0f;
                    sp.AnimationFinished = false;
                }

                sp.SyncedActionId = snapshot.CurrentActionId;
            }
        }

        private static void ApplyCooldownSnapshot(CharacterComponent ch, PlayerBattleState snapshot)
        {
            ch.Skill1Cooldown = Math.Max(0f, snapshot.Skill1Cooldown);
            ch.Skill2Cooldown = Math.Max(0f, snapshot.Skill2Cooldown);
        }

        private static void ApplyPredictedCooldownSnapshot(CharacterComponent ch, PlayerBattleState snapshot)
        {
            ch.Skill1Cooldown = Math.Max(ch.Skill1Cooldown, Math.Max(0f, snapshot.Skill1Cooldown));
            ch.Skill2Cooldown = Math.Max(ch.Skill2Cooldown, Math.Max(0f, snapshot.Skill2Cooldown));
        }

        private static bool IsServerDrivenAction(PlayerBattleState snapshot)
            => snapshot.IsAttacking || snapshot.IsUsingSkill || snapshot.IsDashing;

        private void ClearOnlineVisuals()
        {
            _onlineProjectileVisuals.Clear();
            _onlineEffectVisuals.Clear();
            _projectileFrames.Clear();
            _effectFrames.Clear();
            _player.Get<SpriteComponent>().SyncedActionId = -1;
            if (_enemy != null)
                _enemy.Get<SpriteComponent>().SyncedActionId = -1;
        }

        private void SyncOnlineProjectileVisuals(IEnumerable<ProjectileState> projectiles)
        {
            var liveIds = new HashSet<int>();

            foreach (var projectile in projectiles)
            {
                liveIds.Add(projectile.ProjectileId);

                if (!_onlineProjectileVisuals.TryGetValue(projectile.ProjectileId, out var visual))
                    _onlineProjectileVisuals[projectile.ProjectileId] = visual = new OnlineProjectileVisual();

                visual.State = CloneProjectileState(projectile);
                visual.IsServerActive = true;
                visual.MissingSeconds = 0f;
                visual.AgeSeconds = Math.Max(visual.AgeSeconds, projectile.Timer);
                EnsureVisualFrame(_projectileFrames, projectile.ProjectileId, projectile.CurrentFrame);
            }

            foreach (var item in _onlineProjectileVisuals)
            {
                if (!liveIds.Contains(item.Key))
                    item.Value.IsServerActive = false;
            }
        }

        private void SyncOnlineEffectVisuals(IEnumerable<EffectState> effects)
        {
            var liveIds = new HashSet<int>();

            foreach (var effect in effects)
            {
                liveIds.Add(effect.EffectId);

                if (!_onlineEffectVisuals.TryGetValue(effect.EffectId, out var visual))
                    _onlineEffectVisuals[effect.EffectId] = visual = new OnlineEffectVisual();

                visual.State = CloneEffectState(effect);
                visual.IsServerActive = true;
                visual.MissingSeconds = 0f;
                float elapsed = Math.Max(0f, effect.Duration - effect.RemainingTime);
                visual.AgeSeconds = Math.Max(visual.AgeSeconds, elapsed);
                EnsureVisualFrame(_effectFrames, effect.EffectId, effect.CurrentFrame);
            }

            foreach (var item in _onlineEffectVisuals)
            {
                if (!liveIds.Contains(item.Key))
                    item.Value.IsServerActive = false;
            }
        }

        private static void EnsureVisualFrame(Dictionary<int, VisualFrameState> frames, int id, int frame)
        {
            int frameIndex = Math.Max(0, frame);
            if (frames.TryGetValue(id, out var state))
            {
                if (frameIndex > state.Frame)
                {
                    state.Frame = frameIndex;
                    state.Timer = 0f;
                }
                return;
            }

            frames[id] = new VisualFrameState { Frame = frameIndex };
        }

        private static ProjectileState CloneProjectileState(ProjectileState projectile)
            => new()
            {
                ProjectileId = projectile.ProjectileId,
                OwnerPlayerId = projectile.OwnerPlayerId,
                X = projectile.X,
                Y = projectile.Y,
                VelocityX = projectile.VelocityX,
                VelocityY = projectile.VelocityY,
                Damage = projectile.Damage,
                ArmorPen = projectile.ArmorPen,
                Stun = projectile.Stun,
                Range = projectile.Range,
                CollisionWidth = projectile.CollisionWidth,
                CollisionHeight = projectile.CollisionHeight,
                Lifetime = projectile.Lifetime,
                Timer = projectile.Timer,
                AnimationKey = projectile.AnimationKey,
                CurrentFrame = projectile.CurrentFrame,
                HitFrames = new List<int>(projectile.HitFrames),
                FacingRight = projectile.FacingRight,
                RenderOffsetX = projectile.RenderOffsetX,
                RenderOffsetY = projectile.RenderOffsetY,
                Render = CloneRender(projectile.Render)
            };

        private static EffectState CloneEffectState(EffectState effect)
            => new()
            {
                EffectId = effect.EffectId,
                OwnerPlayerId = effect.OwnerPlayerId,
                EffectType = effect.EffectType,
                AnimationKey = effect.AnimationKey,
                X = effect.X,
                Y = effect.Y,
                Damage = effect.Damage,
                ArmorPen = effect.ArmorPen,
                Stun = effect.Stun,
                CollisionWidth = effect.CollisionWidth,
                CollisionHeight = effect.CollisionHeight,
                BlockEnemyAttack = effect.BlockEnemyAttack,
                BlockEnemyProjectile = effect.BlockEnemyProjectile,
                BlockEnemySkill = effect.BlockEnemySkill,
                CurrentFrame = effect.CurrentFrame,
                HitFrames = new List<int>(effect.HitFrames),
                DamagedFrames = new HashSet<int>(effect.DamagedFrames),
                Duration = effect.Duration,
                RemainingTime = effect.RemainingTime,
                FacingRight = effect.FacingRight,
                LastDamageTick = effect.LastDamageTick,
                Render = CloneRender(effect.Render)
            };

        private static EffectRenderData CloneRender(EffectRenderData render)
            => new()
            {
                Scale = render.Scale,
                OffsetX = render.OffsetX,
                OffsetY = render.OffsetY,
                UseSpriteSize = render.UseSpriteSize,
                AlignY = render.AlignY,
                FacingSource = render.FacingSource
            };

        private static void AdvanceVisualFrame(
            Dictionary<int, VisualFrameState> frameStates,
            int id,
            SpriteAnimation anim,
            float dt)
        {
            if (!frameStates.TryGetValue(id, out var state) ||
                anim.Frames.Length == 0)
            {
                return;
            }

            state.Timer += dt;
            while (state.Timer >= anim.FrameDuration)
            {
                state.Timer -= anim.FrameDuration;
                if (state.Frame < anim.Frames.Length - 1)
                    state.Frame++;
                else if (anim.Loop)
                    state.Frame = 0;
            }
        }

        private void RemoveOnlineProjectileVisual(int id)
        {
            _onlineProjectileVisuals.Remove(id);
            _projectileFrames.Remove(id);
        }

        private void RemoveOnlineEffectVisual(int id)
        {
            _onlineEffectVisuals.Remove(id);
            _effectFrames.Remove(id);
        }

        private static bool ShouldRemoveOnlineVisual(VisualFrameState frame, SpriteAnimation anim, OnlineVisualBase visual)
        {
            if (visual.MissingSeconds < 0.05f)
                return false;

            float frameDuration = Math.Max(0.01f, anim.FrameDuration);
            float animationDuration = Math.Clamp(anim.Frames.Length * frameDuration, 0.12f, 2.0f);
            if (visual.AgeSeconds < animationDuration)
                return false;

            return anim.Loop || frame.Frame >= anim.Frames.Length - 1;
        }

        private bool TryResolveOnlineAnimation(int ownerPlayerId, string animationKey, out SpriteAnimation anim)
        {
            anim = null!;
            if (string.IsNullOrWhiteSpace(animationKey))
                return false;

            Dictionary<string, SpriteAnimation> ownerAnimations =
                ownerPlayerId == _localOnlinePlayerId || _remoteOnlinePlayerId == 0
                    ? _playerAnimations
                    : _enemyAnimations;

            if (ownerAnimations.TryGetValue(animationKey, out var ownerAnim) && ownerAnim != null)
            {
                anim = ownerAnim;
                return true;
            }

            if (_onlineEffectAnimations.TryGetValue(animationKey, out var effectAnim) && effectAnim != null)
            {
                anim = effectAnim;
                return true;
            }

            return false;
        }

        private void DrawOnlineProjectiles(Graphics g)
        {
            foreach (var onlineVisual in _onlineProjectileVisuals.Values)
            {
                var projectile = onlineVisual.State;
                if (!TryResolveOnlineAnimation(projectile.OwnerPlayerId, projectile.AnimationKey, out var anim) ||
                    anim.Frames.Length == 0)
                {
                    continue;
                }

                int frameIndex = _projectileFrames.TryGetValue(projectile.ProjectileId, out var frameState)
                    ? Math.Min(frameState.Frame, anim.Frames.Length - 1)
                    : Math.Min(projectile.CurrentFrame, anim.Frames.Length - 1);
                DrawEffectFrame(g, anim.Frames[frameIndex], projectile.X, projectile.Y, projectile.FacingRight, projectile.Render);
            }
        }

        private void DrawOnlineEffects(Graphics g)
        {
            foreach (var onlineVisual in _onlineEffectVisuals.Values)
            {
                var effect = onlineVisual.State;
                if (!TryResolveOnlineAnimation(effect.OwnerPlayerId, effect.AnimationKey, out var anim) ||
                    anim.Frames.Length == 0)
                {
                    continue;
                }

                int frameIndex = _effectFrames.TryGetValue(effect.EffectId, out var frameState)
                    ? Math.Min(frameState.Frame, anim.Frames.Length - 1)
                    : Math.Min(effect.CurrentFrame, anim.Frames.Length - 1);
                DrawEffectFrame(g, anim.Frames[frameIndex], effect.X, effect.Y, effect.FacingRight, effect.Render);
            }
        }

        private static void DrawEffectFrame(Graphics g, Image frame, float centerX, float centerY, bool facingRight, EffectRenderData render)
        {
            const int defaultSize = 80;
            int baseWidth = render.UseSpriteSize ? frame.Width : defaultSize;
            int baseHeight = render.UseSpriteSize ? frame.Height : defaultSize;
            int drawWidth = Math.Max(1, (int)MathF.Round(baseWidth * render.Scale));
            int drawHeight = Math.Max(1, (int)MathF.Round(baseHeight * render.Scale));
            float directionalOffsetX = facingRight ? render.OffsetX : -render.OffsetX;
            int x = (int)MathF.Round(centerX + directionalOffsetX - drawWidth / 2f);
            int y = ResolveEffectDrawY(centerY, drawHeight, render);

            var state = g.Save();
            if (facingRight)
            {
                g.DrawImage(frame, x, y, drawWidth, drawHeight);
            }
            else
            {
                g.TranslateTransform(x + drawWidth / 2f, y + drawHeight / 2f);
                g.ScaleTransform(-1, 1);
                g.DrawImage(frame, -drawWidth / 2, -drawHeight / 2, drawWidth, drawHeight);
            }
            g.Restore(state);
        }

        private static int ResolveEffectDrawY(float y, int drawHeight, EffectRenderData render)
        {
            float finalY = (render.AlignY ?? "center").Trim().ToLowerInvariant() switch
            {
                "bottom" => y + render.OffsetY - drawHeight,
                "top" => y + render.OffsetY,
                _ => y + render.OffsetY - drawHeight / 2f
            };

            return (int)MathF.Round(finalY);
        }

        private void UpdateCamera()
        {
            var playerMovement = _player.Get<MovementComponent>();
            float maxCameraX = Math.Max(0f, _mapWidth - _formWidth);
            if (maxCameraX <= 0f)
            {
                _cameraX = 0f;
                return;
            }

            float deadZoneWidth = _formWidth * CameraDeadZoneWidthRatio;
            float deadZoneLeft = (_formWidth - deadZoneWidth) / 2f;
            float deadZoneRight = deadZoneLeft + deadZoneWidth;
            float playerScreenX = playerMovement.X - _cameraX;

            if (playerScreenX < deadZoneLeft)
            {
                _cameraX -= deadZoneLeft - playerScreenX;
            }
            else if (playerScreenX > deadZoneRight)
            {
                _cameraX += playerScreenX - deadZoneRight;
            }

            _cameraX = Math.Clamp(_cameraX, 0f, maxCameraX);
        }

        private void DrawParallaxBackground(Graphics g)
        {
            foreach (var layer in _parallaxLayers)
            {
                DrawParallaxLayer(g, layer);
                DrawLayerObjects(g, layer);
            }
        }

        private void DrawParallaxLayer(Graphics g, ParallaxLayer layer)
        {
            int scaledWidth = Math.Max(1, (int)MathF.Ceiling(layer.Image.Width * layer.Scale));
            int scaledHeight = Math.Max(1, (int)MathF.Ceiling(layer.Image.Height * layer.Scale));
            int overlap = Math.Max(1, (int)MathF.Ceiling(layer.Scale));
            float offsetX = -(_cameraX * layer.Speed) % scaledWidth;
            if (offsetX > 0)
                offsetX -= scaledWidth;

            int startX = (int)MathF.Floor(offsetX);
            int step = Math.Max(1, scaledWidth - overlap);

            for (int x = startX; x < _formWidth; x += step)
            {
                g.DrawImage(layer.Image, x, 0, scaledWidth + overlap, scaledHeight);
            }
        }

        private void DrawLayerObjects(Graphics g, ParallaxLayer layer)
        {
            if (!_mapObjectsByLayer.TryGetValue(layer.LayerId, out var objects) || objects.Count == 0)
                return;

            foreach (var obj in objects)
            {
                int drawWidth = obj.Width > 0 ? obj.Width : Math.Max(1, (int)MathF.Round(obj.Image.Width * obj.Scale));
                int drawHeight = obj.Height > 0 ? obj.Height : Math.Max(1, (int)MathF.Round(obj.Image.Height * obj.Scale));
                float screenX = obj.WorldX - (_cameraX * layer.Speed);
                float screenY = obj.WorldY;

                g.DrawImage(
                    obj.Image,
                    (int)MathF.Round(screenX),
                    (int)MathF.Round(screenY),
                    drawWidth,
                    drawHeight);
            }
        }

        private void LoadMapBackground(string mapId)
        {
            _mapBackground?.Dispose();
            _mapBackground = null;
            foreach (var layer in _parallaxLayers)
                layer.Image.Dispose();
            _parallaxLayers.Clear();
            _foregroundLayer?.Image.Dispose();
            _foregroundLayer = null;
            ClearMapObjects();

            if (DungeonMapRegistry.TryGet(mapId, out var dungeonMap))
            {
                LoadDungeonParallax(dungeonMap);
                return;
            }

            var mapNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "terrace", "terrace.png" },
                { "castle", "castle.png" },
                { "forest", "BackgroundForest.png" },
                { "throneroom", "throneroom.png" }
            };

            if (!mapNames.TryGetValue(mapId, out var imageName))
            {
                imageName = mapNames["terrace"];
            }

            string imagePath = Path.Combine(_clientRoot, "Assets", "Background", imageName);

            if (File.Exists(imagePath))
            {
                try
                {
                    _mapBackground = Image.FromFile(imagePath);
                    Console.WriteLine($"[GameEngine] Loaded map background: {imagePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GameEngine] Error loading map background: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[GameEngine] Map background not found: {imagePath}");
            }
        }

        private void InitializeDungeonRun(string mapId)
        {
            _dungeonRun = null;
            if (!DungeonMapRegistry.TryGet(mapId, out var dungeonMap))
            {
                return;
            }

            DungeonDefinition? definition = DungeonContentLoader.TryLoadDefinition(_clientRoot, dungeonMap.FolderName);
            if (definition == null)
                return;

            DungeonMonsterCatalog? catalog = DungeonContentLoader.TryLoadMonsterCatalog(_clientRoot);
            if (catalog == null || catalog.Monsters.Count == 0)
                return;

            _dungeonRun = new DungeonRunController(definition, catalog.Monsters);
        }

        private void UpdateDungeonRun()
        {
            if (_dungeonRun == null)
                return;

            CompleteActiveDungeonSpawnIfDefeated();

            float playerX = _player.Get<MovementComponent>().X;
            _dungeonRun.Update(playerX);
            while (_dungeonRun.TryDequeueSpawn(out DungeonSpawnRequest request))
            {
                Console.WriteLine($"[Dungeon] Spawn request wave={request.WaveId}, prefab={request.PrefabId}, character={request.CharacterId}, x={request.X}, y={request.Y}, boss={request.IsBoss}");
                SpawnDungeonEnemy(request);
            }
        }

        private void CompleteActiveDungeonSpawnIfDefeated()
        {
            if (_dungeonRun == null || _activeDungeonSpawnToken == null || _enemy == null)
                return;

            var enemyCharacter = _enemy.Get<CharacterComponent>();
            if (!enemyCharacter.IsDead)
                return;

            if (!_activeDungeonSpawnDefeated)
            {
                _activeDungeonSpawnDefeated = true;
                _bossAiController = null;
                Console.WriteLine($"[Dungeon] Boss defeated token={_activeDungeonSpawnToken}");
            }

            var enemySprite = _enemy.Get<SpriteComponent>();
            if (!string.Equals(enemySprite.CurrentAnimation, "Dead", StringComparison.OrdinalIgnoreCase) ||
                !enemySprite.AnimationFinished)
            {
                return;
            }

            _dungeonRun.MarkSpawnDefeated(_activeDungeonSpawnToken);
            _activeDungeonSpawnToken = null;
            _activeDungeonSpawnDefeated = false;
            _enemy = null;
            _dungeonDefeatedCount++;
        }

        private void SpawnDungeonEnemy(DungeonSpawnRequest request)
        {
            if (_enemy != null && !_enemy.Get<CharacterComponent>().IsDead)
                return;

            var enemyLoader = new AnimationLoader("Assets");
            _enemyAnimations.Clear();
            foreach (var kv in enemyLoader.Load(request.CharacterId))
                _enemyAnimations[kv.Key] = kv.Value;

            RefreshCombinedAnimations();

            var enemyAnimKeys = new Dictionary<string, object>();
            foreach (var kv in _enemyAnimations)
                enemyAnimKeys[kv.Key] = kv.Value;

            _enemy = CharacterFactory.Create(request.CharacterId, request.X, _groundY, enemyAnimKeys);
            var enemyMovement = _enemy.Get<MovementComponent>();
            enemyMovement.GroundY = _groundY;
            enemyMovement.Y = request.Y > 0f ? request.Y : enemyMovement.GroundY;
            enemyMovement.FacingRight = _player.Get<MovementComponent>().X >= enemyMovement.X;

            _projectileSystem.RegisterTarget(_enemy);
            _playerCombatSystem.SetTarget(_enemy);
            _enemyCombatSystem.SetTarget(_player);
            _bossAiController = new BossAiController(BossAiProfileLoader.Load(_clientRoot, request.CharacterId));
            _activeDungeonSpawnToken = request.SpawnToken;
            _activeDungeonSpawnDefeated = false;
        }

        private void LoadDungeonParallax(DungeonMapDefinition dungeonMap)
        {
            string mapPath = Path.Combine(_clientRoot, "Assets", "dungeon", dungeonMap.FolderName);
            string previewPath = Path.Combine(mapPath, dungeonMap.PreviewFileName);
            if (File.Exists(previewPath))
            {
                try
                {
                    _mapBackground = Image.FromFile(previewPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GameEngine] Error loading {dungeonMap.MapId} preview {previewPath}: {ex.Message}");
                }
            }

            foreach (var layer in dungeonMap.Layers)
            {
                LoadParallaxLayer(mapPath, dungeonMap.MapId, layer.FileName, layer.FileName, layer.Speed, _parallaxLayers);
            }

            if (!string.IsNullOrWhiteSpace(dungeonMap.ForegroundFileName))
            {
                LoadParallaxLayer(mapPath, dungeonMap.MapId, "foreground", dungeonMap.ForegroundFileName, 1.00f, null);
            }

            LoadMapObjects(mapPath);
            Console.WriteLine($"[GameEngine] Loaded {dungeonMap.MapId} parallax layers: {_parallaxLayers.Count} from {mapPath}");
        }

        private void LoadParallaxLayer(
            string mapPath,
            string mapLabel,
            string layerId,
            string fileName,
            float speed,
            List<ParallaxLayer>? targetLayers)
        {
            string imagePath = Path.Combine(mapPath, fileName);
            if (!File.Exists(imagePath))
                return;

            try
            {
                var image = Image.FromFile(imagePath);
                float scale = Math.Max(
                    _formHeight / (float)image.Height,
                    _formWidth / (float)image.Width);
                var parallaxLayer = new ParallaxLayer(layerId, image, speed, scale);
                if (targetLayers == null)
                    _foregroundLayer = parallaxLayer;
                else
                    targetLayers.Add(parallaxLayer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Error loading {mapLabel} layer {imagePath}: {ex.Message}");
            }
        }

        private void LoadMapObjects(string mapPath)
        {
            string objectPath = Path.Combine(mapPath, "objects.json");
            if (!File.Exists(objectPath))
                return;

            try
            {
                string json = File.ReadAllText(objectPath);
                var data = JsonSerializer.Deserialize<List<MapObjectConfig>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null)
                    return;

                foreach (var item in data)
                {
                    if (string.IsNullOrWhiteSpace(item.Layer) || string.IsNullOrWhiteSpace(item.Sprite))
                        continue;

                    Image? image = TryLoadMapObjectImage(item.Sprite);
                    if (image == null)
                        continue;

                    if (!_mapObjectsByLayer.TryGetValue(item.Layer, out var list))
                    {
                        list = new List<MapObjectRenderItem>();
                        _mapObjectsByLayer[item.Layer] = list;
                    }

                    list.Add(new MapObjectRenderItem(
                        item.Layer,
                        image,
                        item.X,
                        item.Y,
                        item.Scale <= 0f ? 1f : item.Scale,
                        item.Width,
                        item.Height));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Error loading map objects {objectPath}: {ex.Message}");
            }
        }

        private Image? TryLoadMapObjectImage(string spritePath)
        {
            string normalized = spritePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            string fullPath = Path.IsPathRooted(normalized)
                ? normalized
                : Path.Combine(_clientRoot, normalized);

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[GameEngine] Map object sprite not found: {fullPath}");
                return null;
            }

            if (_mapObjectImageCache.TryGetValue(fullPath, out var cached))
                return cached;

            try
            {
                var image = Image.FromFile(fullPath);
                _mapObjectImageCache[fullPath] = image;
                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Error loading map object sprite {fullPath}: {ex.Message}");
                return null;
            }
        }

        private void ClearMapObjects()
        {
            _mapObjectsByLayer.Clear();
            foreach (var image in _mapObjectImageCache.Values)
            {
                image.Dispose();
            }
            _mapObjectImageCache.Clear();
        }

        private sealed class ParallaxLayer
        {
            public ParallaxLayer(string layerId, Image image, float speed, float scale)
            {
                LayerId = layerId;
                Image = image;
                Speed = speed;
                Scale = scale;
            }

            public string LayerId { get; }
            public Image Image { get; }
            public float Speed { get; }
            public float Scale { get; }
        }

        private sealed class MapObjectRenderItem
        {
            public MapObjectRenderItem(string layer, Image image, float worldX, float worldY, float scale, int width, int height)
            {
                Layer = layer;
                Image = image;
                WorldX = worldX;
                WorldY = worldY;
                Scale = scale;
                Width = width;
                Height = height;
            }

            public string Layer { get; }
            public Image Image { get; }
            public float WorldX { get; }
            public float WorldY { get; }
            public float Scale { get; }
            public int Width { get; }
            public int Height { get; }
        }

        private sealed class MapObjectConfig
        {
            public string Layer { get; set; } = string.Empty;
            public string Sprite { get; set; } = string.Empty;
            public float X { get; set; }
            public float Y { get; set; }
            public float Scale { get; set; } = 1f;
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private enum PredictedActionKind
        {
            Attack,
            Skill,
            Dash
        }

        private sealed class LocalActionPrediction
        {
            public PredictedActionKind Kind { get; init; }
            public string Animation { get; init; } = string.Empty;
            public int SkillSlot { get; init; }
            public float HoldSeconds { get; set; }
        }

        private abstract class OnlineVisualBase
        {
            public bool IsServerActive { get; set; }
            public float AgeSeconds { get; set; }
            public float MissingSeconds { get; set; }
        }

        private sealed class OnlineProjectileVisual : OnlineVisualBase
        {
            public ProjectileState State { get; set; } = new();
        }

        private sealed class OnlineEffectVisual : OnlineVisualBase
        {
            public EffectState State { get; set; } = new();
        }

        private sealed class VisualFrameState
        {
            public int Frame { get; set; }
            public float Timer { get; set; }
        }
    }
}
