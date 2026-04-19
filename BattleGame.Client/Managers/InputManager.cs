using System.Collections.Generic;
using System.Windows.Forms;

namespace BattleGame.Client.Managers
{
    public static class InputManager
    {
        private static readonly HashSet<Keys> _held = new();

        public static void SetKey(Keys key, bool isDown)
        {
            if (isDown) _held.Add(key);
            else _held.Remove(key);
        }

        public static bool IsKeyDown(Keys key) => _held.Contains(key);
    }
}