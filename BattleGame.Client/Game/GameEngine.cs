using System;
using System.Collections.Generic;
using System.Drawing;
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
        private CombatSystem _combatSystem = null!;

        private CharacterRenderer _renderer = null!;
        private PlayerController _controller = null!;

        private DateTime _lastTime;
        private const float GroundY = 400f;

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
            _combatSystem = new CombatSystem(_projectileSystem);

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

            // Player đánh enemy
            _combatSystem.SetTarget(_enemy);

            _renderer = new CharacterRenderer(_player.Id, animations, enemyAnimations);
            _controller = new PlayerController(_player, _enemy, _combatSystem);
            _lastTime = DateTime.Now;
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
            _combatSystem.Update(_player, dt);
            _combatSystem.Update(_enemy, dt);

            _moveSystem.Update(_player, dt);
            _moveSystem.Update(_enemy, dt);

            _animSystem.Update(_player, dt);
            _animSystem.Update(_enemy, dt);

            _projectileSystem.Update(dt);
        }

        public void Draw(Graphics g)
        {
            _renderer.Draw(g, _player);
            _renderer.Draw(g, _enemy);
            _projectileSystem.Draw(g);
        }
    }
}