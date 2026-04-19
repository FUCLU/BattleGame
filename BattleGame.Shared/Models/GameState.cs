using System.Collections.Generic;

namespace BattleGame.Shared.Models
{
    public class GameState
    {
        public PlayerState Player1 { get; set; } = null!;
        public PlayerState Player2 { get; set; } = null!;
        public int Round { get; set; }
    }
}