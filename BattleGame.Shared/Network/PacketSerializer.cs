using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using BattleGame.Shared.Packets;

namespace BattleGame.Shared.Network
{
    public static class PacketSerializer
    {
        public static string Serialize(Packet packet)
        {
            return JsonSerializer.Serialize(packet);
        }

        public static T Deserialize<T>(string json) where T : Packet
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
