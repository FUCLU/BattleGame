using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Models
{
    public class CharacterState
    {
        public string CharacterId { get; set; }
        public int HP { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
