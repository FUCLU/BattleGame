using BattleGame.Client.Game.Characters;
using System.Collections.Generic;

namespace BattleGame.Client.Game
{
    public class AnimationManager
    {
        private readonly Soldier _soldier;
        private string _lastAnimationName = "";

        public AnimationManager(Soldier soldier)
        {
            _soldier = soldier;
        }

        public void Update()
        {
            string current = _soldier.CurrentAnimationName; 

            if (current != _lastAnimationName)
            {
                OnAnimationChanged(_lastAnimationName, current);
                _lastAnimationName = current;
            }
        }

        private void OnAnimationChanged(string from, string to)
        {
            // Gắn SoundManager hoặc VFX ở đây nếu cần
            // if (to == "Attack")  SoundManager.Play("swing");
            // if (to == "Dead")    SoundManager.Play("death");
        }

        public string CurrentAnimation => _soldier.CurrentAnimationName; 
    }
}