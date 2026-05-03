using System.Text.Json;
using BattleGame.Shared.Packets;

namespace BattleGame.Shared.Network
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
            return packet.Type switch
            {
                PacketType.Login => JsonSerializer.Serialize((LoginPacket)packet, options),
                PacketType.LoginResult => JsonSerializer.Serialize((LoginResultPacket)packet, options),
                PacketType.Register => JsonSerializer.Serialize((RegisterPacket)packet, options),
                PacketType.OtpSent => JsonSerializer.Serialize((OtpPacket)packet, options),
                PacketType.OtpVerify => JsonSerializer.Serialize((OtpVerifyPacket)packet, options),
                PacketType.ForgotPassword => JsonSerializer.Serialize((ForgotPasswordPacket)packet, options),
                PacketType.ResetPassword => JsonSerializer.Serialize((ResetPasswordPacket)packet, options),
                PacketType.MatchRequest => JsonSerializer.Serialize((MatchRequestPacket)packet, options),
                PacketType.MatchFound => JsonSerializer.Serialize((MatchFoundPacket)packet, options),
                PacketType.SelectCharacter => JsonSerializer.Serialize((SelectionCharacterPacket)packet, options),
                PacketType.Move => JsonSerializer.Serialize((MovePacket)packet, options),
                PacketType.Attack => JsonSerializer.Serialize((AttackPacket)packet, options),
                PacketType.GameState => JsonSerializer.Serialize((GameStatePacket)packet, options),
                PacketType.HealthUpdate => JsonSerializer.Serialize((HealthUpdatePacket)packet, options),
                PacketType.GameOver => JsonSerializer.Serialize((GameOverPacket)packet, options),
                PacketType.Disconnect => JsonSerializer.Serialize((DisconnectPacket)packet, options),
                PacketType.CreateRoom => JsonSerializer.Serialize((CreateRoomPacket)packet, options),
                PacketType.CreateRoomResult => JsonSerializer.Serialize((CreateRoomResultPacket)packet, options),
                PacketType.GetRoom => JsonSerializer.Serialize((GetRoomPacket)packet, options),
                PacketType.GetRoomResult => JsonSerializer.Serialize((GetRoomResultPacket)packet, options),
                PacketType.JoinRoom => JsonSerializer.Serialize((JoinRoomPacket)packet, options),
                PacketType.JoinRoomResult => JsonSerializer.Serialize((JoinRoomResultPacket)packet, options),
                PacketType.Ready => JsonSerializer.Serialize((ReadyPacket)packet, options),
                PacketType.SelectMap => JsonSerializer.Serialize((SelectMapPacket)packet, options),
                PacketType.GetLeaderboard => JsonSerializer.Serialize((GetLeaderboardPacket)packet, options),
                PacketType.GetLeaderboardResult => JsonSerializer.Serialize((GetLeaderboardResultPacket)packet, options),
                _ => JsonSerializer.Serialize(packet, options)
            };
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
                PacketType.OtpSent => JsonSerializer.Deserialize<OtpPacket>(json, options)!,
                PacketType.OtpVerify => JsonSerializer.Deserialize<OtpVerifyPacket>(json, options)!,
                PacketType.ForgotPassword => JsonSerializer.Deserialize<ForgotPasswordPacket>(json, options)!,
                PacketType.ResetPassword => JsonSerializer.Deserialize<ResetPasswordPacket>(json, options)!,
                PacketType.MatchRequest => JsonSerializer.Deserialize<MatchRequestPacket>(json, options)!,
                PacketType.MatchFound => JsonSerializer.Deserialize<MatchFoundPacket>(json, options)!,
                PacketType.SelectCharacter => JsonSerializer.Deserialize<SelectionCharacterPacket>(json, options)!,
                PacketType.Move => JsonSerializer.Deserialize<MovePacket>(json, options)!,
                PacketType.Attack => JsonSerializer.Deserialize<AttackPacket>(json, options)!,
                PacketType.GameState => JsonSerializer.Deserialize<GameStatePacket>(json, options)!,
                PacketType.HealthUpdate => JsonSerializer.Deserialize<HealthUpdatePacket>(json, options)!,
                PacketType.GameOver => JsonSerializer.Deserialize<GameOverPacket>(json, options)!,
                PacketType.Disconnect => JsonSerializer.Deserialize<DisconnectPacket>(json, options)!,
                PacketType.CreateRoom => JsonSerializer.Deserialize<CreateRoomPacket>(json, options)!,
                PacketType.CreateRoomResult => JsonSerializer.Deserialize<CreateRoomResultPacket>(json, options)!,
                PacketType.GetRoom => JsonSerializer.Deserialize<GetRoomPacket>(json, options)!,
                PacketType.GetRoomResult => JsonSerializer.Deserialize<GetRoomResultPacket>(json, options)!,
                PacketType.JoinRoom => JsonSerializer.Deserialize<JoinRoomPacket>(json, options)!,
                PacketType.JoinRoomResult => JsonSerializer.Deserialize<JoinRoomResultPacket>(json, options)!,
                PacketType.Ready => JsonSerializer.Deserialize<ReadyPacket>(json, options)!,
                PacketType.SelectMap => JsonSerializer.Deserialize<SelectMapPacket>(json, options)!,
                PacketType.GetLeaderboard => JsonSerializer.Deserialize<GetLeaderboardPacket>(json, options)!,
                PacketType.GetLeaderboardResult => JsonSerializer.Deserialize<GetLeaderboardResultPacket>(json, options)!,
                _ => throw new NotSupportedException($"Chưa hỗ trợ packet type: {type}")
            };
        }
    }
}