using BattleGame.Client.Game.Characters;
using BattleGame.Client.Game.Skills;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BattleGame.Client.Game
{
    public class GameEngine
    {
        // ── Core objects ──────────────────────────
        private readonly Soldier _player;
        private readonly PlayerController _controller;
        private readonly AnimationManager _animationManager;
        private readonly ProjectileManager _projectileManager;

        // ── Input ─────────────────────────────────
        private readonly HashSet<Keys> _keys = new();

        public GameEngine()
        {
            _player = new Soldier();
            _projectileManager = new ProjectileManager();
            _controller = new PlayerController(_player);
            _animationManager = new AnimationManager(_player);

            // Inject ProjectileManager vào các skill cần spawn projectile
            foreach (var skill in _player.Skills)
            {
                if (skill is ShootSkill ss) ss.Init(_projectileManager);
                if (skill is GrenadeSkill gs) gs.Init(_projectileManager);
            }
        }

        // ═══════════════════════════════════════
        //  INPUT
        // ═══════════════════════════════════════
        public void KeyDown(Keys key) => _keys.Add(key);
        public void KeyUp(Keys key) => _keys.Remove(key);

        // ═══════════════════════════════════════
        //  UPDATE
        // ═══════════════════════════════════════
        public void Update()
        {
            _controller.Update(_keys);
            _player.Update();
            _animationManager.Update();

            // Tick delay grenade
            foreach (var skill in _player.Skills)
                if (skill is GrenadeSkill gs) gs.UpdateDelay();

            _projectileManager.Update();
        }

        // ═══════════════════════════════════════
        //  DRAW
        // ═══════════════════════════════════════
        public void Draw(Graphics g)
        {
            _projectileManager.Draw(g);  // vẽ projectile trước (dưới nhân vật)
            _player.Draw(g);
        }

        // ═══════════════════════════════════════
        //  HUD INFO
        // ═══════════════════════════════════════
        public int PlayerHP => _player.CurrentHP;
        public int PlayerMaxHP => _player.MaxHP;
        public bool PlayerIsDead => _player.IsDead();
        public string PlayerAnim => _animationManager.CurrentAnimation;
    }
}