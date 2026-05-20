using System;

namespace BattleGame.Shared.Packets
{
    public class RoomClosedPacket : Packet
    {
        public int RoomId { get; set; }
        public string Message { get; set; } = string.Empty;

        public RoomClosedPacket() : base(PacketType.RoomClosed)
        {
        }
    }
}
