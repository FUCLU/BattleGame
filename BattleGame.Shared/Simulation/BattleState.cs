namespace BattleGame.Shared.Simulation;

public class BattleState
{
    public int ServerTick { get; set; }
    public int RoundNumber { get; set; } = 1;
    public int Player1RoundWins { get; set; }
    public int Player2RoundWins { get; set; }
    public float RoundSecondsRemaining { get; set; } = 180f;
    public float RoundDurationSeconds { get; set; } = 180f;
    public bool IsSuddenDeath { get; set; }
    public PlayerBattleState Player1 { get; set; } = new();
    public PlayerBattleState Player2 { get; set; } = new();
    public List<ProjectileState> Projectiles { get; set; } = new();
    public List<EffectState> Effects { get; set; } = new();
    public bool IsGameOver { get; set; }
    public int WinnerPlayerId { get; set; }
}
