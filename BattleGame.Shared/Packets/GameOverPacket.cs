using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GameOverPacket : Packet
    {
        public int WinnerPlayerId { get; set; }
        public int Duration { get; set; }
        public GameOverPacket() : base(PacketType.GameOver)
        {
        }
    }
}
