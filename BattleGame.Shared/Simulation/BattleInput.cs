namespace BattleGame.Shared.Simulation;

public class BattleInput
{
    public int PlayerId { get; set; }
    public int Sequence { get; set; }
    public int ClientTick { get; set; }

    public float MoveX { get; set; }
    public bool JumpPressed { get; set; }
    public bool BlockHeld { get; set; }

    public bool AttackPressed { get; set; }
    public int SkillSlot { get; set; }
    public bool DashPressed { get; set; }

    public bool FacingRight { get; set; }
}
