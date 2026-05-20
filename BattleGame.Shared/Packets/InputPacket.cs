using BattleGame.Shared.Simulation;

namespace BattleGame.Shared.Packets;

public class InputPacket : Packet
{
    public int RoomId { get; set; }
    public BattleInput Input { get; set; } = new();

    public InputPacket() : base(PacketType.Input)
    {
    }
}
