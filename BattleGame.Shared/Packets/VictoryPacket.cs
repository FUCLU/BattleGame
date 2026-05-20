namespace BattleGame.Shared.Packets;

public class VictoryPacket : Packet
{
    public int WinnerPlayerId { get; set; }
    public int Duration { get; set; }
    public int FinalRound { get; set; }
    public int Player1RoundWins { get; set; }
    public int Player2RoundWins { get; set; }

    public VictoryPacket() : base(PacketType.Victory)
    {
    }
}
