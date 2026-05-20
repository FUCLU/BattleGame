using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class SelectionCharacterPacket : Packet
    {
        public int RoomId { get; set; }
        public int CharacterId { get; set; }
        
        public SelectionCharacterPacket() : base(PacketType.SelectCharacter)
        {
        }
    }
}
