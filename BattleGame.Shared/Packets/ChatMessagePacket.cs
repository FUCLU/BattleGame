namespace BattleGame.Shared.Packets
{
    public class ChatMessagePacket : Packet
    {
        public int RoomId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public ChatMessagePacket() : base(PacketType.ChatMessage)
        {
        }
    }
}
