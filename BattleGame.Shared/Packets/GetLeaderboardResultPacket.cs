using System;
using System.Collections.Generic;
using BattleGame.Shared.Models;

namespace BattleGame.Shared.Packets
{
    public class GetLeaderboardResultPacket : Packet
    {
        public List<LeaderboardEntry> Entries { get; set; } = new();
        
        public GetLeaderboardResultPacket() : base(PacketType.GetLeaderboardResult) { }
    }
    
    public class LeaderboardEntry
    {
        public int Rank { get; set; }
        public string? Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}