namespace BattleGame.Shared.Packets;

public class HitEventPacket : Packet
{
    public int AttackerId { get; set; }
    public int TargetId { get; set; }
    public int Damage { get; set; }
    public bool IsBlocked { get; set; }
    public float HitX { get; set; }
    public float HitY { get; set; }

    public HitEventPacket() : base(PacketType.HitEvent)
    {
    }
}
