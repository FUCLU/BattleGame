using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
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
        private const string CaveMapId = "cave";
        private const string Stage2MapId = "stage2";
        private const float DungeonWorldWidth = 8000f;
        private const float DungeonGroundOffsetY = 30f;
        private const float CameraDeadZoneWidthRatio = 0.40f;

        private Entity _player = null!;
        private Entity _enemy = null!;

        private readonly AnimationSystem _animSystem = new();
        private readonly MovementSystem _moveSystem = new();
        private ProjectileSystem _projectileSystem = null!;
        private CombatSystem _playerCombatSystem = null!;
        private CombatSystem _enemyCombatSystem = null!;

        private CharacterRenderer _renderer = null!;
        private BarrierRenderer _barrierRenderer = null!;
        private PlayerController _controller = null!;
        private Dictionary<string, SpriteAnimation> _onlineEffectAnimations = new();
        private readonly List<ProjectileState> _onlineProjectiles = new();
        private readonly List<EffectState> _onlineEffects = new();
        private readonly Dictionary<int, VisualFrameState> _projectileFrames = new();
        private readonly Dictionary<int, VisualFrameState> _effectFrames = new();
        private Image? _mapBackground;
        private readonly List<ParallaxLayer> _parallaxLayers = new();
        private ParallaxLayer? _foregroundLayer;

        private DateTime _lastTime;
        private float _groundY;
        private float _mapWidth;
        private float _cameraX;
        private int _formWidth;
        private int _formHeight;
        private readonly string _mapId;
        private readonly string _clientRoot;

        public Entity Player => _player;
        public Entity Enemy => _enemy;

        public GameEngine(string characterId, string mapId, int formWidth, int formHeight, string? enemyCharacterId = null)
        {
            _mapId = mapId;
            _clientRoot = ResolveClientRoot();
            _formWidth = formWidth;
            _formHeight = formHeight;
            _groundY = GetGroundY(mapId, formHeight);
            _mapWidth = GetWorldWidth(mapId, formWidth);

            _moveSystem.MapLeft = 50f;
            _moveSystem.MapRight = _mapWidth - 50f;

            // Load map background directly
            LoadMapBackground(mapId);

            // Load animations trước — ProjectileSystem cần để render
            var loader = new AnimationLoader("Assets");
            var animations = loader.Load(characterId);

            var animKeys = new Dictionary<string, object>();
            foreach (var kv in animations)
                animKeys[kv.Key] = kv.Value;

            // Khởi tạo theo thứ tự dependency
            _projectileSystem = new ProjectileSystem(animations);
            _playerCombatSystem = new CombatSystem(_projectileSystem);
            _enemyCombatSystem = new CombatSystem(_projectileSystem);

            // Tạo nhân vật
            _player = CharacterFactory.Create(characterId, 200f, _groundY, animKeys);

            // Enemy theo character đối thủ đã chọn từ RoomForm/MatchFound.
            string resolvedEnemyCharacterId = string.IsNullOrWhiteSpace(enemyCharacterId)
                ? "samurai"
                : enemyCharacterId.Trim().ToLowerInvariant();

            var enemyLoader = new AnimationLoader("Assets");
            var enemyAnimations = enemyLoader.Load(resolvedEnemyCharacterId);
            _onlineEffectAnimations = MergeAnimations(animations, enemyAnimations);
            var enemyAnimKeys = new Dictionary<string, object>();
            foreach (var kv in enemyAnimations)
                enemyAnimKeys[kv.Key] = kv.Value;
            float enemyStartX = IsDungeonParallaxMap ? Math.Min(_mapWidth - 300f, 7600f) : 500f;
            _enemy = CharacterFactory.Create(resolvedEnemyCharacterId, enemyStartX, _groundY, enemyAnimKeys);

            // Đăng ký target cho projectile collision
            _projectileSystem.RegisterTarget(_player);
            _projectileSystem.RegisterTarget(_enemy);

            // Chia sẻ barrier giữa cả hai phía
            _playerCombatSystem.SetBarrierProvider(GetAllBarriers);
            _enemyCombatSystem.SetBarrierProvider(GetAllBarriers);
            _projectileSystem.SetBarrierProvider(GetAllBarriers);

            // Player đánh enemy, Enemy đánh player
            _playerCombatSystem.SetTarget(_enemy);
            _enemyCombatSystem.SetTarget(_player);

            _renderer = new CharacterRenderer(_player.Id, animations, enemyAnimations);
            _barrierRenderer = new BarrierRenderer(animations);
            _controller = new PlayerController(_player, _enemy, _playerCombatSystem);
            _lastTime = DateTime.Now;
            UpdateCamera();
        }

        public void ApplyOnlineWorldState(BattleState state, int localPlayerId)
        {
            PlayerBattleState local = state.Player1.PlayerId == localPlayerId ? state.Player1 : state.Player2;
            PlayerBattleState remote = state.Player1.PlayerId == localPlayerId ? state.Player2 : state.Player1;

            ApplySnapshot(_player, local);
            ApplySnapshot(_enemy, remote);

            _onlineProjectiles.Clear();
            _onlineProjectiles.AddRange(state.Projectiles);
            _onlineEffects.Clear();
            _onlineEffects.AddRange(state.Effects);

            SyncVisualFrames(_projectileFrames, _onlineProjectiles.Select(p => p.ProjectileId));
            SyncVisualFrames(_effectFrames, _onlineEffects.Select(e => e.EffectId));
            UpdateCamera();
        }

        public void UpdateOnlineVisuals(float dt)
        {
            dt = Math.Min(dt, 0.05f);
            foreach (var projectile in _onlineProjectiles)
            {
                AdvanceVisualFrame(
                    _projectileFrames,
                    projectile.ProjectileId,
                    projectile.AnimationKey,
                    _onlineEffectAnimations,
                    dt);
            }

            foreach (var effect in _onlineEffects)
            {
                AdvanceVisualFrame(
                    _effectFrames,
                    effect.EffectId,
                    effect.AnimationKey,
                    _onlineEffectAnimations,
                    dt);
            }
        }

        private bool IsCaveMap => string.Equals(_mapId, CaveMapId, StringComparison.OrdinalIgnoreCase);
        private bool IsStage2Map => string.Equals(_mapId, Stage2MapId, StringComparison.OrdinalIgnoreCase);
        private bool IsDungeonParallaxMap => IsCaveMap || IsStage2Map;

        private static float GetWorldWidth(string mapId, int formWidth)
            => IsDungeonParallaxMapId(mapId)
                ? DungeonWorldWidth
                : formWidth;

        private static float GetGroundY(string mapId, int formHeight)
            => formHeight - GroundBottomMargin +
               (IsDungeonParallaxMapId(mapId) ? DungeonGroundOffsetY : 0f);

        private static bool IsDungeonParallaxMapId(string mapId)
            => string.Equals(mapId, CaveMapId, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(mapId, Stage2MapId, StringComparison.OrdinalIgnoreCase);

        private static string ResolveClientRoot()
        {
            string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? AppDomain.CurrentDomain.BaseDirectory;

            while (!string.IsNullOrWhiteSpace(current))
            {
                if (Directory.Exists(Path.Combine(current, "Assets")) &&
                    Directory.Exists(Path.Combine(current, "Config")))
                {
                    return current;
                }

                var parent = Directory.GetParent(current);
                if (parent == null)
                    break;

                current = parent.FullName;
            }

            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
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

            _moveSystem.MapLeft = 50f;
            _moveSystem.MapRight = _mapWidth - 50f;

            ResizeEntity(_player, oldGroundY, _groundY);
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

            mv.X = Math.Clamp(mv.X, _moveSystem.MapLeft, _moveSystem.MapRight);
        }

        private IEnumerable<Entity> GetAllBarriers()
        {
            return _playerCombatSystem.GetBarriers().Concat(_enemyCombatSystem.GetBarriers());
        }

        public void Update(float dt)
        {
            dt = Math.Min(dt, 0.05f);

            _controller.Update();

            // ===== UPDATE ANIMATION FIRST (before combat check) =====
            _renderer.Update(_player, dt);
            _renderer.Update(_enemy, dt);

            // ===== COMBAT (now AnimationFinished is up-to-date) =====
            _playerCombatSystem.Update(_player, dt);
            _enemyCombatSystem.Update(_enemy, dt);

            _moveSystem.Update(_player, dt);
            _moveSystem.Update(_enemy, dt);

            _animSystem.Update(_player, dt);
            _animSystem.Update(_enemy, dt);

            _projectileSystem.Update(dt);

            // ===== UPDATE BARRIERS =====
            foreach (var barrier in _playerCombatSystem.GetBarriers())
            {
                _barrierRenderer.Update(barrier, dt);
            }
            foreach (var barrier in _enemyCombatSystem.GetBarriers())
            {
                _barrierRenderer.Update(barrier, dt);
            }

            UpdateCamera();
        }

        public void Draw(Graphics g)
        {
            // Draw map background first
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
            _renderer.Draw(g, _enemy);
            _projectileSystem.Draw(g);
            DrawOnlineProjectiles(g);

            // ===== DRAW BARRIERS =====
            foreach (var barrier in _playerCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }
            foreach (var barrier in _enemyCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }
            DrawOnlineEffects(g);

            g.Restore(state);

            if (IsDungeonParallaxMap && _foregroundLayer != null)
            {
                DrawParallaxLayer(g, _foregroundLayer);
            }
        }

        private static Dictionary<string, SpriteAnimation> MergeAnimations(
            Dictionary<string, SpriteAnimation> playerAnimations,
            Dictionary<string, SpriteAnimation> enemyAnimations)
        {
            var merged = new Dictionary<string, SpriteAnimation>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in playerAnimations)
                merged[kv.Key] = kv.Value;
            foreach (var kv in enemyAnimations)
                merged.TryAdd(kv.Key, kv.Value);
            return merged;
        }

        private static void ApplySnapshot(Entity entity, PlayerBattleState snapshot)
        {
            var mv = entity.Get<MovementComponent>();
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();

            mv.X = snapshot.X;
            mv.Y = snapshot.Y;
            mv.VelocityX = snapshot.VelocityX;
            mv.VelocityY = snapshot.VelocityY;
            mv.FacingRight = snapshot.FacingRight;
            mv.IsGrounded = snapshot.IsGrounded;

            ch.Hp = Math.Clamp(snapshot.Hp, 0, ch.BaseStats.Hp);
            ch.Mana = Math.Clamp(snapshot.Mana, 0, ch.BaseStats.Mana);
            ch.IsProtecting = snapshot.IsProtecting;
            ch.IsAttacking = snapshot.IsAttacking;
            ch.IsUsingSkill = snapshot.IsUsingSkill;
            ch.IsHurt = snapshot.IsHurt;
            ch.IsStunned = snapshot.IsStunned;
            ch.StunTimer = snapshot.StunTimer;
            ch.HurtTimer = snapshot.HurtTimer;
            ch.IsDead = snapshot.IsDead;

            if (!string.IsNullOrWhiteSpace(snapshot.CurrentAnimation))
                sp.CurrentAnimation = snapshot.CurrentAnimation;

            sp.CurrentFrame = Math.Max(0, snapshot.CurrentFrame);
        }

        private static void SyncVisualFrames(Dictionary<int, VisualFrameState> frames, IEnumerable<int> liveIds)
        {
            var live = liveIds.ToHashSet();
            foreach (int id in frames.Keys.Where(id => !live.Contains(id)).ToList())
                frames.Remove(id);

            foreach (int id in live)
                frames.TryAdd(id, new VisualFrameState());
        }

        private static void AdvanceVisualFrame(
            Dictionary<int, VisualFrameState> frameStates,
            int id,
            string animationKey,
            Dictionary<string, SpriteAnimation> animations,
            float dt)
        {
            if (!frameStates.TryGetValue(id, out var state) ||
                string.IsNullOrWhiteSpace(animationKey) ||
                !animations.TryGetValue(animationKey, out var anim) ||
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

        private void DrawOnlineProjectiles(Graphics g)
        {
            foreach (var projectile in _onlineProjectiles)
            {
                if (string.IsNullOrWhiteSpace(projectile.AnimationKey) ||
                    !_onlineEffectAnimations.TryGetValue(projectile.AnimationKey, out var anim) ||
                    anim.Frames.Length == 0)
                {
                    continue;
                }

                int frameIndex = _projectileFrames.TryGetValue(projectile.ProjectileId, out var visual)
                    ? Math.Min(visual.Frame, anim.Frames.Length - 1)
                    : Math.Min(projectile.CurrentFrame, anim.Frames.Length - 1);
                DrawEffectFrame(g, anim.Frames[frameIndex], projectile.X, projectile.Y, projectile.FacingRight, projectile.Render);
            }
        }

        private void DrawOnlineEffects(Graphics g)
        {
            foreach (var effect in _onlineEffects)
            {
                if (string.IsNullOrWhiteSpace(effect.AnimationKey) ||
                    !_onlineEffectAnimations.TryGetValue(effect.AnimationKey, out var anim) ||
                    anim.Frames.Length == 0)
                {
                    continue;
                }

                int frameIndex = _effectFrames.TryGetValue(effect.EffectId, out var visual)
                    ? Math.Min(visual.Frame, anim.Frames.Length - 1)
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
            int x = (int)MathF.Round(centerX + render.OffsetX - drawWidth / 2f);
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

        private void LoadMapBackground(string mapId)
        {
            _mapBackground?.Dispose();
            _mapBackground = null;
            foreach (var layer in _parallaxLayers)
                layer.Image.Dispose();
            _parallaxLayers.Clear();
            _foregroundLayer?.Image.Dispose();
            _foregroundLayer = null;

            if (string.Equals(mapId, CaveMapId, StringComparison.OrdinalIgnoreCase))
            {
                LoadCaveParallax();
                return;
            }

            if (string.Equals(mapId, Stage2MapId, StringComparison.OrdinalIgnoreCase))
            {
                LoadStage2Parallax();
                return;
            }

            var mapNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "terrace", "Background.png" },
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

        private void LoadCaveParallax()
        {
            LoadDungeonParallax(
                folderName: "map1",
                mapLabel: "cave",
                previewFileName: "0.png",
                layers: new (string FileName, float Speed)[]
                {
                    ("7.png", 0.00f),
                    ("6.png", 0.12f),
                    ("5.png", 0.22f),
                    ("4.png", 0.36f),
                    ("3.png", 0.52f),
                    ("2.png", 1.00f)
                },
                foregroundFileName: "1.png");
        }

        private void LoadStage2Parallax()
        {
            LoadDungeonParallax(
                folderName: "map2",
                mapLabel: "stage2",
                previewFileName: "preview.png",
                layers: new (string FileName, float Speed)[]
                {
                    ("back.png", 0.00f),
                    ("middle.png", 0.35f)
                },
                foregroundFileName: "front.png");
        }

        private void LoadDungeonParallax(
            string folderName,
            string mapLabel,
            string previewFileName,
            IReadOnlyList<(string FileName, float Speed)> layers,
            string? foregroundFileName)
        {
            string mapPath = Path.Combine(_clientRoot, "Assets", "dungeon", folderName);
            string previewPath = Path.Combine(mapPath, previewFileName);
            if (File.Exists(previewPath))
            {
                try
                {
                    _mapBackground = Image.FromFile(previewPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GameEngine] Error loading {mapLabel} preview {previewPath}: {ex.Message}");
                }
            }

            foreach (var layer in layers)
            {
                LoadParallaxLayer(mapPath, mapLabel, layer.FileName, layer.Speed, _parallaxLayers);
            }

            if (!string.IsNullOrWhiteSpace(foregroundFileName))
            {
                LoadParallaxLayer(mapPath, mapLabel, foregroundFileName, 1.00f, null);
            }

            Console.WriteLine($"[GameEngine] Loaded {mapLabel} parallax layers: {_parallaxLayers.Count} from {mapPath}");
        }

        private void LoadParallaxLayer(
            string mapPath,
            string mapLabel,
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
                var parallaxLayer = new ParallaxLayer(image, speed, scale);
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

        private sealed class ParallaxLayer
        {
            public ParallaxLayer(Image image, float speed, float scale)
            {
                Image = image;
                Speed = speed;
                Scale = scale;
            }

            public Image Image { get; }
            public float Speed { get; }
            public float Scale { get; }
        }

        private sealed class VisualFrameState
        {
            public int Frame { get; set; }
            public float Timer { get; set; }
        }
    }
}
