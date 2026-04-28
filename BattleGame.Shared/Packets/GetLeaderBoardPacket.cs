using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GetLeaderboardPacket : Packet
    {
        public GetLeaderboardPacket() : base(PacketType.GetLeaderboard) { }
    }
}