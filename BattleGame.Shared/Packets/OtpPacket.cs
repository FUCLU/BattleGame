using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Shared.Packets
{
    public class OtpPacket : Packet
    {
        public string Email { get; set; }
        public OtpPacket() : base(PacketType.OtpSend)
        {
        }
    }
}
