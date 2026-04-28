using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class CreateRoomResultPacket : Packet
    {
        public int RoomId { get; set; } //Room code
        
        public CreateRoomResultPacket() : base(PacketType.CreateRoomResult) { }
    }
}