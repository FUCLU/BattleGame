using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class DisconnectPacket : Packet
    {
        public DisconnectPacket() : base(PacketTypes.Disconnect)
        {
        }
    }
}
