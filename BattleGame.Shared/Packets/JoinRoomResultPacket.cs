using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class JoinRoomResultPacket : Packet
    {
        public bool Success { get; set; }
        public int RoomId { get; set; }
        public int MapId { get; set; } = -1;
        public int TimeLimitMinutes { get; set; } = 3;
        public string ServerId { get; set; } = string.Empty;
        public bool RequiresReconnect { get; set; }
        public string? Player1Name { get; set; }
        public string? Player2Name { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSnapshot { get; set; }
        public string? Message { get; set; }
        public JoinRoomResultPacket() : base(PacketType.JoinRoomResult) { }
    }
}
