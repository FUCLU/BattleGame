using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public abstract class Packet    
    {
        public PacketType Type { get; set; }
        protected Packet(PacketType type)
        {
            Type = type;
        }
    }
}

