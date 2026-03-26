using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class AttackPacket : Packet
    {
        public int PlayerId { get; set; }
        public AttackPacket() : base(PacketType.Attack)
        {
        }
    }
}
