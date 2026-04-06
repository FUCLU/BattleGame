using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Shared.Packets
{
    public class ForgotPasswordPacket : Packet
    {
        public string Email { get; set; }
        public ForgotPasswordPacket() : base(PacketType.ForgotPassword)
        {
        }
    }
}
