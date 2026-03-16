using BattleGame.Shared.Models;

namespace BattleGame.Server.Game;

/// <summary>
/// Xử lý đúng 1 lượt đánh: attacker → defender
/// </summary>
public class BattleEngine
{
    private static readonly Random _rng = new();

    public TurnResult ProcessTurn(Character attacker, Character defender)
    {
        // 1. Chọn skill tốt nhất đang sẵn sàng
        Skill? chosenSkill = attacker.Skills
            .Where(s => s.IsReady)
            .OrderByDescending(s => s.GetDamagePreview(defender))
            .FirstOrDefault();

        int rawDamage;
        string actionName;

        if (chosenSkill != null)
        {
            rawDamage = attacker.ATK + chosenSkill.CalculateDamage(defender);
            actionName = chosenSkill.Name;
            chosenSkill.Use();
        }
        else
        {
            rawDamage = attacker.ATK;
            actionName = "Normal Attack";
        }

        // 2. Critical hit: 20% chance × 1.5
        bool isCrit = _rng.Next(100) < 20;
        if (isCrit) rawDamage = (int)(rawDamage * 1.5);

        // 3. Áp sát thương
        int actualDamage = defender.TakeDamage(rawDamage);

        // 4. Tick cooldown sau lượt
        attacker.TickCooldowns();

        return new TurnResult
        {
            Attacker = attacker.Name,
            Defender = defender.Name,
            ActionName = actionName,
            Damage = actualDamage,
            IsCritical = isCrit,
            DefenderHP = defender.CurrentHP,
            DefenderMaxHP = defender.MaxHP,
            IsDefenderDead = !defender.IsAlive
        };
    }
}

public class TurnResult
{
    public string Attacker { get; set; } = string.Empty;
    public string Defender { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public int Damage { get; set; }
    public bool IsCritical { get; set; }
    public int DefenderHP { get; set; }
    public int DefenderMaxHP { get; set; }
    public bool IsDefenderDead { get; set; }

    public override string ToString()
    {
        string crit = IsCritical ? " ✦CRIT" : "";
        string dead = IsDefenderDead ? " → DEFEATED!" : "";
        return $"{Attacker,-12} [{ActionName}]{crit,-8} → {Defender,-12} " +
               $"-{Damage} HP ({DefenderHP}/{DefenderMaxHP}){dead}";
    }
}