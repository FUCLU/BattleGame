using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Models
{
    public class GameState
    {
        public PlayerState Player1 { get; set; }
        public PlayerState Player2 { get; set; }
        public int Round { get; set; }
    }
}
