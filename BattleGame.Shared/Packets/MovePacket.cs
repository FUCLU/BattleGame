using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class MovePacket : Packet
    {
        public int PlayerId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public MovePacket() : base(PacketTypes.Move)
        {
        }
    }
}
