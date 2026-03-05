using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public abstract class Packet    
    {
        public PacketTypes Type { get; set; }
        protected Packet(PacketTypes type)
        {
            Type = type;
        }
    }
}

