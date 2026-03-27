using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Shared.Packets
{
    public class RegisterPacket : Packet
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public RegisterPacket() : base(PacketType.Register)
        {
        }
    }
}
