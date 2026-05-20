using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class CreateRoomResultPacket : Packet
    {
        public int RoomId { get; set; } //Room code
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int TimeLimitMinutes { get; set; } = 3;
        
        public CreateRoomResultPacket() : base(PacketType.CreateRoomResult) { }
    }
}
