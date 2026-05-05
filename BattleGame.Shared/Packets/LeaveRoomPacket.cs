namespace BattleGame.Shared.Packets
{
    public class LeaveRoomPacket : Packet
    {
        public int RoomId { get; set; }

        public LeaveRoomPacket() : base(PacketType.LeaveRoom)
        {
        }
    }
}
