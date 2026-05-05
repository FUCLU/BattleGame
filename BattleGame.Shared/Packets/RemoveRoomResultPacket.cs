namespace BattleGame.Shared.Packets
{
    public class RemoveRoomResultPacket : Packet
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public RemoveRoomResultPacket() : base(PacketType.RemoveRoomResult)
        {
        }
    }
}
