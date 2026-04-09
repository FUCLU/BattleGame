using BattleGame.Client.Game.Characters;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleGame.Client.Game
{
    public class PlayerController
    {
        private readonly Soldier _player;
        private readonly HashSet<Keys> _prevKeys = new();

        public PlayerController(Soldier player)
        {
            _player = player;
        }

        public void Update(HashSet<Keys> keys)
        {
            // ── Di chuyển ngang ──────────────────
            float dx = 0;
            if (keys.Contains(Keys.A)) dx -= 1f;
            if (keys.Contains(Keys.D)) dx += 1f;

            _player.Move(dx); 

            // ── Skills (chỉ trigger khi vừa nhấn) ─
            TryUseSkill(keys, Keys.J, 0); // J → BasicAttack
            TryUseSkill(keys, Keys.U, 1); // U → Shoot
            TryUseSkill(keys, Keys.I, 2); // I → Grenade

            _prevKeys.Clear();
            foreach (var k in keys) _prevKeys.Add(k);
        }

        private void TryUseSkill(HashSet<Keys> keys, Keys key, int index)
        {
            if (keys.Contains(key) && !_prevKeys.Contains(key))
                _player.UseSkill(index);
        }
    }
}