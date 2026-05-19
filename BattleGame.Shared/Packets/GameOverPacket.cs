using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GameOverPacket : Packet
    {
        public int WinnerPlayerId { get; set; }
        public int Duration { get; set; }
        public int FinalRound { get; set; }
        public int Player1RoundWins { get; set; }
        public int Player2RoundWins { get; set; }
        public GameOverPacket() : base(PacketType.GameOver)
        {
        }
    }
}
