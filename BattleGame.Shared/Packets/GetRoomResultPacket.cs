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
        public string ServerId { get; set; } = string.Empty;
        public string? RoomName { get; set; }
        public int MapId { get; set; } = -1;
        public int TimeLimitMinutes { get; set; } = 3;
        public int CurrentPlayers { get; set; }
        public bool HasPassword { get; set; }
        public bool IsOwner { get; set; }
        public int Player1Id { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public bool Player1Ready { get; set; }
        public int Player2Id { get; set; } = -1;
        public string Player2Name { get; set; } = string.Empty;
        public bool Player2Ready { get; set; }
    }
}
