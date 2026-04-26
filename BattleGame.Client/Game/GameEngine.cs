using System;
using System.Collections.Generic;
using System.Drawing;
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

        private DateTime _lastTime;
        private const float GroundY = 400f;

        public Entity Player => _player;
        public Entity Enemy => _enemy;

        public GameEngine(string characterId)
        {
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
            _player = CharacterFactory.Create(characterId, 200f, GroundY, animKeys);

            // Enemy luôn là Samurai (để test)
            var enemyLoader = new AnimationLoader("Assets");
            var enemyAnimations = enemyLoader.Load("samurai");
            var enemyAnimKeys = new Dictionary<string, object>();
            foreach (var kv in enemyAnimations)
                enemyAnimKeys[kv.Key] = kv.Value;
            _enemy = CharacterFactory.Create("samurai", 500f, GroundY, enemyAnimKeys);

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

        public void Update()
        {
            var now = DateTime.Now;
            float dt = (float)(now - _lastTime).TotalSeconds;
            _lastTime = now;
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
    }
}