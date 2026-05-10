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

namespace BattleGame.Client.Game
{
    public class GameEngine
    {
        private const float GroundBottomMargin = 140f;
        private const string CaveMapId = "cave";
        private const float CaveWorldWidth = 8000f;

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
            _groundY = formHeight - GroundBottomMargin;
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
            var enemyAnimKeys = new Dictionary<string, object>();
            foreach (var kv in enemyAnimations)
                enemyAnimKeys[kv.Key] = kv.Value;
            float enemyStartX = IsCaveMap ? Math.Min(_mapWidth - 300f, 7600f) : 500f;
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

        private bool IsCaveMap => string.Equals(_mapId, CaveMapId, StringComparison.OrdinalIgnoreCase);

        private static float GetWorldWidth(string mapId, int formWidth)
            => string.Equals(mapId, CaveMapId, StringComparison.OrdinalIgnoreCase)
                ? CaveWorldWidth
                : formWidth;

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
            _groundY = height - GroundBottomMargin;
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
            if (IsCaveMap && _parallaxLayers.Count > 0)
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

            // ===== DRAW BARRIERS =====
            foreach (var barrier in _playerCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }
            foreach (var barrier in _enemyCombatSystem.GetBarriers())
            {
                _barrierRenderer.Draw(g, barrier);
            }

            g.Restore(state);

            if (IsCaveMap && _foregroundLayer != null)
            {
                DrawParallaxLayer(g, _foregroundLayer);
            }
        }

        private void UpdateCamera()
        {
            var playerMovement = _player.Get<MovementComponent>();
            float idealCameraX = playerMovement.X - _formWidth / 2f;
            float maxCameraX = Math.Max(0f, _mapWidth - _formWidth);
            _cameraX = Math.Clamp(idealCameraX, 0f, maxCameraX);
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
            string cavePath = Path.Combine(_clientRoot, "Assets", "dungeon", "map1");
            string previewPath = Path.Combine(cavePath, "0.png");
            if (File.Exists(previewPath))
            {
                try
                {
                    _mapBackground = Image.FromFile(previewPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GameEngine] Error loading cave preview {previewPath}: {ex.Message}");
                }
            }

            var layers = new (string FileName, float Speed)[]
            {
                ("7.png", 0.00f),
                ("6.png", 0.12f),
                ("5.png", 0.22f),
                ("4.png", 0.36f),
                ("3.png", 0.52f),
                ("2.png", 1.00f)
            };

            foreach (var layer in layers)
            {
                string imagePath = Path.Combine(cavePath, layer.FileName);
                if (!File.Exists(imagePath))
                    continue;

                try
                {
                    var image = Image.FromFile(imagePath);
                    float scale = Math.Max(
                        _formHeight / (float)image.Height,
                        _formWidth / (float)image.Width);
                    _parallaxLayers.Add(new ParallaxLayer(image, layer.Speed, scale));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GameEngine] Error loading cave layer {imagePath}: {ex.Message}");
                }
            }

            LoadCaveForeground(cavePath);

            Console.WriteLine($"[GameEngine] Loaded cave parallax layers: {_parallaxLayers.Count} from {cavePath}");
        }

        private void LoadCaveForeground(string cavePath)
        {
            string imagePath = Path.Combine(cavePath, "1.png");
            if (!File.Exists(imagePath))
                return;

            try
            {
                var image = Image.FromFile(imagePath);
                float scale = Math.Max(
                    _formHeight / (float)image.Height,
                    _formWidth / (float)image.Width);
                _foregroundLayer = new ParallaxLayer(image, 1.00f, scale);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] Error loading cave foreground {imagePath}: {ex.Message}");
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
    }
}
