using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Client.Game.Core.Components
{
    public class CharacterStats
    {
        public int Hp { get; set; }
        public int Def { get; set; }
        public int Mana { get; set; }
        public int Atk { get; set; }
        public float Speed { get; set; }
        public float AtkSpeed { get; set; } = 1.0f;
        public float StunDuration { get; set; } = 0f;
    }
}
