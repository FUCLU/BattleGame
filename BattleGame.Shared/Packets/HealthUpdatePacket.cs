using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    internal class HealthUpdatePacket : Packet
    {
        public HealthUpdatePacket() : base(PacketType.HealthUpdate)
        {
        }
        public int PlayerId { get; set; }
        public int NewHealth { get; set; }
    }
}
