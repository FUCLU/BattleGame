using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GetRoomResultPacket : Packet
    {
        public List<RoomInfo> Rooms { get; set; } = new();
        public GetRoomResultPacket() : base(PacketType.GetRoomResult) { }
    }
    public class RoomInfo
    {
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public int CurrentPlayers { get; set; }
        public bool HasPassword { get; set; }
        public bool IsOwner { get; set; }
    }
}
