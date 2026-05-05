using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private DateTime _lastTime;
        private float _groundY;
        private float _mapWidth;
        private int _formWidth;
        private int _formHeight;

        public Entity Player => _player;
        public Entity Enemy => _enemy;

        public GameEngine(string characterId, string mapId, int formWidth, int formHeight, string? enemyCharacterId = null)
        {
            _formWidth = formWidth;
            _formHeight = formHeight;
            _groundY = formHeight - 120f;
            _mapWidth = formWidth;

            _moveSystem.MapLeft = 50f;
            _moveSystem.MapRight = formWidth - 50f;

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
            _enemy = CharacterFactory.Create(resolvedEnemyCharacterId, 500f, _groundY, enemyAnimKeys);

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
        }

        public void Draw(Graphics g)
        {
            // Draw map background first
            if (_mapBackground != null)
            {
                g.DrawImage(_mapBackground, 0, 0, _formWidth, _formHeight);
            }

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
        }

        private void LoadMapBackground(string mapId)
        {
            _mapBackground?.Dispose();
            _mapBackground = null;

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

            string imagePath = Path.Combine("Assets", "Background", imageName);

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
    }
}
