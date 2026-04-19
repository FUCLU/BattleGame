using System;
using System.Collections.Generic;
using System.Drawing;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Gameplay;
using BattleGame.Client.Game.Input;
using BattleGame.Client.Game.Rendering;
using BattleGame.Client.Game.Systems;

namespace BattleGame.Client.Game
{
    public class GameEngine
    {
        private Entity _player = null!;

        private readonly AnimationSystem _animSystem = new();
        private readonly MovementSystem _moveSystem = new();
        private readonly CombatSystem _combatSystem = new();
        private CharacterRenderer _renderer = null!;
        private PlayerController _controller = null!;

        private DateTime _lastTime;
        private const float GroundY = 400f;

        public GameEngine(string characterId)
        {
            var loader = new AnimationLoader("Assets");
            var animations = loader.Load(characterId);

            var animKeys = new Dictionary<string, object>();
            foreach (var kv in animations)
                animKeys[kv.Key] = kv.Value;

            _player = CharacterFactory.Create(characterId, 200f, GroundY, animKeys);
            _renderer = new CharacterRenderer(animations);
            _controller = new PlayerController(_player, _combatSystem);
            _lastTime = DateTime.Now;
        }

        public void Update()
        {
            var now = DateTime.Now;
            float dt = (float)(now - _lastTime).TotalSeconds;
            _lastTime = now;
            dt = Math.Min(dt, 0.05f);

            _controller.Update();
            _combatSystem.Update(_player, dt);
            _moveSystem.Update(_player, dt);
            _animSystem.Update(_player, dt);
            _renderer.Update(_player, dt);
        }

        public void Draw(Graphics g) => _renderer.Draw(g, _player);
    }
}