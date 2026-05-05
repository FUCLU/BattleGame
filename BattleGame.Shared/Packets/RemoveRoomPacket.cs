namespace BattleGame.Shared.Packets
{
    public class RemoveRoomPacket : Packet
    {
        public int RoomId { get; set; }

        public RemoveRoomPacket() : base(PacketType.RemoveRoom)
        {
        }
    }
}
