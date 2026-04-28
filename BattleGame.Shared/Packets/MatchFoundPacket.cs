using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class MatchFoundPacket : Packet
    {
        public int RoomId { get; set; }
        public int MapId { get; set; }
        public int Player1Id { get; set; }
        public string? Player1Name { get; set; }
        public int Player1CharacterId { get; set; }
        public int Player2Id { get; set; }
        public string? Player2Name { get; set; }
        public int Player2CharacterId { get; set; }
        public MatchFoundPacket() : base(PacketType.MatchFound)
        {
        }
    }
}
