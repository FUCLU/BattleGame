namespace BattleGame.Shared.Models
{
    public class SkillData
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public int ManaCost { get; set; }
        public float Cooldown { get; set; }
        public int Damage { get; set; }
        public float StunDuration { get; set; }
        public float AnimDuration { get; set; } = 1f; // thời gian animation skill
    }
}