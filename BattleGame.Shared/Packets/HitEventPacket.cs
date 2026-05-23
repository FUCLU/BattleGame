using System.Collections.Generic;

namespace BattleGame.Shared.Packets;

public class HitEventPacket : Packet
{
    public int AttackerId { get; set; }
    public int TargetId { get; set; }
    public int Damage { get; set; }
    public float Stun { get; set; }
    public bool IsBlocked { get; set; }
    public float HitX { get; set; }
    public float HitY { get; set; }
    public int EffectId { get; set; }
    public string EffectType { get; set; } = string.Empty;
    public string AnimationKey { get; set; } = string.Empty;
    public int EffectFrame { get; set; }
    public List<int> HitFrames { get; set; } = new();

    public HitEventPacket() : base(PacketType.HitEvent)
    {
    }
}
