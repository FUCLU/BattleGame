using BattleGame.Shared.Simulation;

namespace BattleGame.Shared.Packets;

public class WorldStatePacket : Packet
{
    public BattleState State { get; set; } = new();

    public WorldStatePacket() : base(PacketType.WorldState)
    {
    }
}
