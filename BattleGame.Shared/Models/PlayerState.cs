using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Models
{
    public class PlayerState
    {
        public string Username { get; set; }
        public CharacterState Character { get; set; }
    }
}
