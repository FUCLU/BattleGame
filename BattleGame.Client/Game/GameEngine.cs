using BattleGame.Client.Game.Characters;
using BattleGame.Client.Game.Skills;
using BattleGame.Shared.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BattleGame.Client.Game
{
    public class GameEngine
    {
        // Core objects 
        private readonly Character _player; // đổi sang Character
        private readonly PlayerController _controller;
        private readonly AnimationManager _animationManager;
        private readonly ProjectileManager _projectileManager;

        // Input 
        private readonly HashSet<Keys> _keys = new();

        // Constructor mặc định 
        public GameEngine() : this(new Soldier())
        {
        }

        // Constructor mới để test nhân vật
        public GameEngine(Character player)
        {
            _player = player;

            _projectileManager = new ProjectileManager();

            // chỉ tạo controller nếu là Soldier
            if (_player is Soldier soldier)
            {
                _controller = new PlayerController(soldier);
                _animationManager = new AnimationManager(soldier);

                foreach (var skill in soldier.Skills)
                {
                    if (skill is ShootSkill ss) ss.Init(_projectileManager);
                    if (skill is GrenadeSkill gs) gs.Init(_projectileManager);
                }
            }
        }

        //  INPUT
        public void KeyDown(Keys key) => _keys.Add(key);
        public void KeyUp(Keys key) => _keys.Remove(key);

        //  UPDATE
        public void Update()
        {
            if (_player is Soldier soldier)
            {
                _controller.Update(_keys);
                soldier.Update();
                _animationManager.Update();

                foreach (var skill in soldier.Skills)
                    if (skill is GrenadeSkill gs) gs.UpdateDelay();
            }

            _projectileManager.Update();
        }

        //  DRAW
        public void Draw(Graphics g)
        {
            _projectileManager.Draw(g);

            if (_player is Soldier soldier)
                soldier.Draw(g);
        }

        //  HUD INFO
        public int PlayerHP => _player.CurrentHP;
        public int PlayerMaxHP => _player.MaxHP;
        public bool PlayerIsDead => _player.IsDead();

        public string PlayerAnim =>
            _animationManager != null ? _animationManager.CurrentAnimation : "None";
    }
}