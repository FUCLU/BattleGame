using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class JoinRoomPacket : Packet
    {
        public int RoomId { get; set; }
        public string? Password { get; set; }
        
        public JoinRoomPacket() : base(PacketType.JoinRoom) { }
    }
}