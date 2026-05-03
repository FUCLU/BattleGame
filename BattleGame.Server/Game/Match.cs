using System;

namespace BattleGame.Server.Game
{
    public class Match
    {
        public string? WinnerName { get; set; }
        public string? LoserName { get; set; }
        public int Duration { get; set; } 
        public DateTime PlayedAt { get; set; }
    }
}
