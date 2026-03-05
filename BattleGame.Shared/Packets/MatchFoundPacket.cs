using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class MatchFoundPacket : Packet
    {
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public MatchFoundPacket() : base(PacketTypes.MatchFound)
        {
        }
    }
}
