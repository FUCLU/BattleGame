using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleGame.Client.Managers
{
    public class InputManager
    {
        private readonly HashSet<Keys> _keys = new();

        public void SetKeyDown(Keys key)
        {
            _keys.Add(key);
        }

        public void SetKeyUp(Keys key)
        {
            _keys.Remove(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return _keys.Contains(key);
        }

        public void Update()
        {
            // có thể xử lý sau (giữ trống cũng OK)
        }
    }
}