using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class SelectMapPacket : Packet
    {
        public int RoomId { get; set; }
        public int MapId { get; set; }
        
        public SelectMapPacket() : base(PacketType.SelectMap) { }
    }
}