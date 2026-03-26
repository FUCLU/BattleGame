using System.Net.Sockets;
using System.Text.Json;
using BattleGame.Shared.Packets;

namespace BattleGame.Shared.Packets
{
    public static class PacketSerializer
    {
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(Packet packet)
        {
            string json = packet.Type switch
            {
                PacketType.Login => JsonSerializer.Serialize((LoginPacket)packet, options),
                PacketType.LoginResult => JsonSerializer.Serialize((LoginResultPacket)packet, options),
                PacketType.Register => JsonSerializer.Serialize((RegisterPacket)packet, options),
                PacketType.OtpSend => JsonSerializer.Serialize((OtpPacket)packet, options),
                PacketType.OtpVerify => JsonSerializer.Serialize((OtpVerifyPacket)packet, options),
                PacketType.ForgotPassword => JsonSerializer.Serialize((ForgotPasswordPacket)packet, options),
                _ => JsonSerializer.Serialize(packet, options)
            };
            return json;
        }

        public static Packet Deserialize(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var typeValue = doc.RootElement.GetProperty("type").GetInt32();
            var type = (PacketType)typeValue;
            return type switch
            {
                PacketType.Login => JsonSerializer.Deserialize<LoginPacket>(json, options)!,
                PacketType.LoginResult => JsonSerializer.Deserialize<LoginResultPacket>(json, options)!,
                PacketType.Register => JsonSerializer.Deserialize<RegisterPacket>(json, options)!,
                PacketType.OtpSend => JsonSerializer.Deserialize<OtpPacket>(json, options)!,
                PacketType.OtpVerify => JsonSerializer.Deserialize<OtpVerifyPacket>(json, options)!,
                PacketType.ForgotPassword => JsonSerializer.Deserialize<ForgotPasswordPacket>(json, options)!,
                _ => throw new NotSupportedException($"Chua ho tro")
            };
        }
    }
}