using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GameStatePacket : Packet
    {
        public int Player1HP { get; set; }
        public int Player2HP { get; set; }
        public GameStatePacket() : base(PacketTypes.GameState)
        {
        }
    }
}
