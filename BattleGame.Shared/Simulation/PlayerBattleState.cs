namespace BattleGame.Shared.Simulation;

public class PlayerBattleState
{
    public int PlayerId { get; set; }
    public string CharacterId { get; set; } = string.Empty;
    public BattleCharacterStats Stats { get; set; } = new();

    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public bool FacingRight { get; set; } = true;
    public bool IsGrounded { get; set; } = true;

    public int Hp { get; set; }
    public int Mana { get; set; }

    public bool IsProtecting { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsUsingSkill { get; set; }
    public bool IsDashing { get; set; }
    public bool IsHurt { get; set; }
    public bool IsStunned { get; set; }
    public bool IsDead { get; set; }

    public float ActionTimer { get; set; }
    public float ActionDuration { get; set; }
    public bool ActionHitDone { get; set; }
    public int CurrentSkillSlot { get; set; }
    public string CurrentSkillAnimation { get; set; } = "";
    public HashSet<int> TriggeredEffects { get; set; } = new();
    public HashSet<int> TriggeredAttackEffects { get; set; } = new();
    public HashSet<string> TriggeredEffectFrames { get; set; } = new();
    public HashSet<string> TriggeredAttackEffectFrames { get; set; } = new();

    public float HurtTimer { get; set; }
    public float StunTimer { get; set; }
    public float DashTimer { get; set; }

    public float Skill1Cooldown { get; set; }
    public float Skill2Cooldown { get; set; }

    public string CurrentAnimation { get; set; } = "Idle";
    public int CurrentFrame { get; set; }
    public int CurrentActionId { get; set; }
    public int CurrentActionTick { get; set; }

    public bool IsBusy => IsAttacking || IsUsingSkill || IsDashing || IsStunned || IsDead;
    public bool IsInvulnerable => IsUsingSkill
        && ((CurrentSkillSlot == 1 && Stats.Skill1?.InvulnerableWhileCasting == true)
            || (CurrentSkillSlot == 2 && Stats.Skill2?.InvulnerableWhileCasting == true));
}
