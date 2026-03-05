using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class LoginResultPacket : Packet
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LoginResultPacket() : base(PacketTypes.LoginResult)
        {
        }
    }
}
