using BattleGame.Shared.Models;
using System.ComponentModel;

namespace BattleGame.Client.Game.Core.Components;

public class CharacterComponent : IComponent
{
    // Chỉ số gốc (load từ config, không đổi)
    public CharacterStats BaseStats { get; set; } = new();

    // Chỉ số hiện tại
    public int Hp { get; set; }
    public int Mana { get; set; }

    // Skills
    public SkillData? Skill1 { get; set; }
    public SkillData? Skill2 { get; set; }
    public float Skill1Cooldown { get; set; }
    public float Skill2Cooldown { get; set; }
    public string CurrentSkillAnim { get; set; } = "Skill1";
    // Trạng thái hành động
    public bool IsProtecting { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsUsingSkill { get; set; }
    public bool IsHurt { get; set; }
    public bool IsStunned { get; set; }
    public bool IsDead { get; set; }

    // Timer hành động (dùng chung cho attack / skill)
    public float ActionTimer { get; set; }
    public float ActionDuration { get; set; }

    public float HurtTimer { get; set; }
    public float HurtDuration { get; set; } = 0.3f;

    public float StunTimer { get; set; }

    // Animation attack hiện tại (random mỗi lần đánh)
    public string CurrentAttackAnim { get; set; } = "Attack_1";
    public int AttackAnimCount { get; set; } = 1;

    // Shortcut
    public bool IsBusy => IsAttacking || IsUsingSkill || IsHurt || IsStunned || IsDead;
}