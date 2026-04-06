using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Shared.Packets
{
    public class ResetPasswordPacket : Packet
    {
        public ResetPasswordPacket() : base(PacketType.ResetPassword)
        {

        }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
