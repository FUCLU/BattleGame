using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class MatchRequestPacket : Packet
    {
        public MatchRequestPacket() : base(PacketTypes.MatchRequest) { }
    }
}
