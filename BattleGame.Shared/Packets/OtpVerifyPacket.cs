using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Shared.Packets
{
    public class OtpVerifyPacket : Packet
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public OtpVerifyPacket() : base(PacketType.OtpVerify)
        {
        }
    }
}
