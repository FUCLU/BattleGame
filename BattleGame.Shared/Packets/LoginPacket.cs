using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class LoginPacket : Packet
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginPacket() : base(PacketTypes.Login)
        {
        }
    }

}