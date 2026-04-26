using BattleGame.Client.Config;
using BattleGame.Shared.Models;
using System.Collections.Generic;

namespace BattleGame.Client.Game.Core.Components
{
    public class CharacterComponent : IComponent
    {
        public string CharacterId { get; set; } = "";
        public CharacterRenderConfig Render { get; set; } = new();

        // ===== BASE STATS =====
        public CharacterStats BaseStats { get; set; } = new();

        // ===== CURRENT STATS =====
        public int Hp { get; set; }
        public int Mana { get; set; }

        // ===== SKILLS =====
        public SkillData? Skill1 { get; set; }
        public SkillData? Skill2 { get; set; }

        public float Skill1Cooldown { get; set; }
        public float Skill2Cooldown { get; set; }

        public string CurrentSkillAnim { get; set; } = "Skill1";

        // ===== STATES =====
        public bool IsProtecting { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsUsingSkill { get; set; }
        public bool IsHurt { get; set; }
        public bool IsStunned { get; set; }
        public bool IsDead { get; set; }

        // ===== TIMERS =====
        public float ActionTimer { get; set; }
        public float ActionDuration { get; set; }

        public float HurtTimer { get; set; }
        public float HurtDuration { get; set; } = 0.3f;

        public float StunTimer { get; set; }

        // ===== ATTACK =====
        public int AttackHitFrame { get; set; } = 2;
        public bool AttackHitDone { get; set; } = false;
        public string CurrentAttackAnim { get; set; } = "Attack_1";
        public int AttackAnimCount { get; set; } = 1;
        public List<EffectData> AttackEffects { get; set; } = new();

        // ===== STATE LOCK =====
        public bool IsBusy => IsAttacking || IsUsingSkill || IsStunned || IsDead;

        // ===== SKILL SYSTEM =====
        public int CurrentSkillSlot { get; set; }
        public HashSet<int> TriggeredEffects { get; set; } = new();
        public HashSet<(int, int)> TriggeredFrames { get; set; } = new(); // (effectIdx, frameIdx)
        public HashSet<int> TriggeredAttackEffects { get; set; } = new();
        public HashSet<(int, int)> TriggeredAttackFrames { get; set; } = new(); // (effectIdx, frameIdx)
    }
}
