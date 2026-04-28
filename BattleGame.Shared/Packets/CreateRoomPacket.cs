using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class CreateRoomPacket : Packet
    {
        public string? RoomName { get; set; }
        public string? Password { get; set; }
        
        public CreateRoomPacket() : base(PacketType.CreateRoom) { }
    }
}