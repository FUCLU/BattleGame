using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    internal class GameOverPacket : Packet
    {
        public GameOverPacket() : base(PacketType.GameOver)
        {
        }
        public int WinnerPlayerId { get; set; }
    }
}
