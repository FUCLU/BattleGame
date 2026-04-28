using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GetRoomPacket : Packet
    {
        public GetRoomPacket() : base(PacketType.GetRoom) { }
    }
}
