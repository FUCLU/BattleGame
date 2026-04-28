using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class ReadyPacket : Packet
    {
        
        public ReadyPacket() : base(PacketType.Ready) { }
    }
}