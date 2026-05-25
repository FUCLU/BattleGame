using System;
using System.IO;
using System.Text.Json;

namespace BattleGame.Client.Game.AI;

public sealed class BossAiProfile
{
    public string Id { get; set; } = string.Empty;
    public string CharacterId { get; set; } = string.Empty;
    public float ChaseSpeedMultiplier { get; set; } = 0.65f;
    public float StopChaseRange { get; set; } = 0f;
    public float ChaseAttackRange { get; set; } = 0f;
    public float BasicAttackRange { get; set; } = 0f;
    public float EngageRangeBonus { get; set; } = 10f;
    public float Skill1Range { get; set; } = 210f;
    public float Skill2Range { get; set; } = 0f;
    public float BasicAttackCooldown { get; set; } = 0.85f;
    public float PostSkillActionCooldown { get; set; } = 0.45f;
    public float DashMinRange { get; set; } = 0f;
    public float DashMaxRange { get; set; } = 0f;
    public float DashStopRange { get; set; } = 0f;
    public float DashCooldown { get; set; } = 0f;
    public float DashDuration { get; set; } = 0.22f;
    public float DashSpeedMultiplier { get; set; } = 3.0f;
    public bool DashComboOnDash { get; set; } = false;
    public int DashComboFirstSkill { get; set; } = 2;
    public int DashComboSecondSkill { get; set; } = 1;

    public bool CanDash => DashMinRange > 0f && DashCooldown > 0f;
}

public static class BossAiProfileLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static BossAiProfile Load(string clientRoot, string characterId)
    {
        string path = Path.Combine(clientRoot, "Config", "BossAiProfiles", $"{characterId}.json");
        if (!File.Exists(path))
            return CreateDefault(characterId);

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<BossAiProfile>(json, JsonOptions)
                   ?? CreateDefault(characterId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BossAI] Failed to load profile {path}: {ex.Message}");
            return CreateDefault(characterId);
        }
    }

    private static BossAiProfile CreateDefault(string characterId)
        => new()
        {
            Id = $"{characterId}_default",
            CharacterId = characterId
        };
}
