using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class ReadyPacket : Packet
    {
        public bool Player1Ready { get; set; }
        public bool Player2Ready { get; set; }

        public ReadyPacket() : base(PacketType.Ready) { }
    }
}
