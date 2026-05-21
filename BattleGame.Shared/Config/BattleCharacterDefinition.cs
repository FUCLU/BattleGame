using System;
using System.Text.Json;
using BattleGame.Shared.Models;
using BattleGame.Shared.Simulation;

namespace BattleGame.Shared.Config;

public sealed class BattleCharacterDefinition
{
    public string Id { get; init; } = "";
    public BattleCharacterStats Stats { get; init; } = new();
    public SkillData? Skill1 { get; init; }
    public SkillData? Skill2 { get; init; }
    public List<EffectData> AttackEffects { get; init; } = new();
}

public static class BattleCharacterDefinitionLoader
{
    public static BattleCharacterDefinition Load(string configPath)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(configPath));
        var root = doc.RootElement;
        var stats = root.GetProperty("stats");

        var skill1 = TryParseSkill(root, "skill1");
        var skill2 = TryParseSkill(root, "skill2");
        string id = ReadString(root, "id") ?? Path.GetFileNameWithoutExtension(configPath);

        return new BattleCharacterDefinition
        {
            Id = id,
            Stats = ParseStats(stats, root),
            Skill1 = skill1,
            Skill2 = skill2,
            AttackEffects = ParseEffects(root, "attackEffects")
        };
    }

    public static BattleCharacterDefinition LoadById(string configRoot, string characterId)
    {
        string normalizedId = (characterId ?? string.Empty).Trim().ToLowerInvariant();
        string[] candidates =
        {
            Path.Combine(configRoot, "Config", "Characters", $"{normalizedId}.json"),
            Path.Combine(configRoot, "Config", "Bosses", $"{normalizedId}.json")
        };

        foreach (string path in candidates)
        {
            if (File.Exists(path))
                return Load(path);
        }

        throw new FileNotFoundException(
            $"Cannot find battle character config for '{characterId}' in Config/Characters or Config/Bosses under '{configRoot}'.",
            candidates[0]);
    }

    private static BattleCharacterStats ParseStats(JsonElement stats, JsonElement root)
    {
        bool protectionBlocksAllDirections = false;
        if (root.TryGetProperty("render", out var render) &&
            render.TryGetProperty("protectionBlocksAllDirections", out var protectionBlocks))
        {
            protectionBlocksAllDirections = protectionBlocks.GetBoolean();
        }

        float atkSpeed = stats.GetProperty("atkSpeed").GetSingle();
        var parsed = new BattleCharacterStats
        {
            Hp = stats.GetProperty("hp").GetInt32(),
            Def = stats.GetProperty("def").GetInt32(),
            Mana = stats.GetProperty("mana").GetInt32(),
            ManaRegen = stats.TryGetProperty("manaRegen", out var manaRegen) ? manaRegen.GetSingle() : 8f,
            Atk = stats.GetProperty("atk").GetInt32(),
            Speed = stats.GetProperty("speed").GetSingle(),
            AtkSpeed = atkSpeed,
            StunDuration = stats.GetProperty("stunDuration").GetSingle(),
            AttackRange = stats.TryGetProperty("attackRange", out var ar) ? ar.GetSingle() : 150f,
            AttackDuration = atkSpeed > 0f ? 1f / atkSpeed : 1f,
            AttackProjectile = stats.TryGetProperty("attackProjectile", out var ap) ? ap.GetString() : null,
            AttackProjectileSpeed = stats.TryGetProperty("attackProjectileSpeed", out var aps) ? aps.GetSingle() : 0f,
            AttackProjectileSpawnOffsetX = stats.TryGetProperty("attackProjectileSpawnOffsetX", out var apox) ? apox.GetSingle() : 30f,
            AttackProjectileSpawnOffsetY = stats.TryGetProperty("attackProjectileSpawnOffsetY", out var apoy) ? apoy.GetSingle() : -50f,
            AttackProjectileScale = stats.TryGetProperty("attackProjectileScale", out var apsc) ? apsc.GetSingle() : 1f,
            ProtectionBlocksAllDirections = protectionBlocksAllDirections
        };

        if (root.TryGetProperty("animations", out var animations) && animations.ValueKind == JsonValueKind.Object)
        {
            int attackAnimCount = 0;
            foreach (var animation in animations.EnumerateObject())
            {
                if (!animation.Value.TryGetProperty("frameCount", out var frameCountElement))
                    continue;

                int frameCount = Math.Max(1, frameCountElement.GetInt32());
                float fps = animation.Value.TryGetProperty("fps", out var fpsElement)
                    ? Math.Max(1f, fpsElement.GetSingle())
                    : 10f;

                parsed.Animations[animation.Name] = new BattleCharacterStats.AnimationMeta
                {
                    FrameCount = frameCount,
                    Fps = fps
                };

                if (animation.Name.StartsWith("Attack_", StringComparison.OrdinalIgnoreCase))
                    attackAnimCount++;
            }

            parsed.AttackAnimCount = Math.Max(1, attackAnimCount);
        }

        return parsed;
    }

    private static SkillData? TryParseSkill(JsonElement root, string skillName)
    {
        if (!root.TryGetProperty("skills", out var skills) || !skills.TryGetProperty(skillName, out var skill))
            return null;

        return ParseSkill(skill);
    }

    private static SkillData ParseSkill(JsonElement el)
    {
        var skill = new SkillData
        {
            Id = el.GetProperty("id").GetString() ?? "",
            ManaCost = el.GetProperty("manaCost").GetInt32(),
            Cooldown = el.GetProperty("cooldown").GetSingle(),
            Animation = el.TryGetProperty("animation", out var anim) ? anim.GetString() ?? "" : "",
            InvulnerableWhileCasting = el.TryGetProperty("invulnerableWhileCasting", out var invulnerable)
                && invulnerable.GetBoolean()
        };

        if (el.TryGetProperty("effects", out var effects))
            skill.Effects = ParseEffects(effects);

        return skill;
    }

    private static List<EffectData> ParseEffects(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var effects) && effects.ValueKind == JsonValueKind.Array
            ? ParseEffects(effects)
            : new List<EffectData>();
    }

    private static List<EffectData> ParseEffects(JsonElement effects)
    {
        var parsed = new List<EffectData>();

        foreach (var e in effects.EnumerateArray())
        {
            string type = ReadString(e, "type") ?? "";
            bool isMelee = string.Equals(type, "melee", StringComparison.OrdinalIgnoreCase);
            float range = e.TryGetProperty("range", out var r) ? r.GetSingle() : 50f;
            int collisionWidth = e.TryGetProperty("collisionWidth", out var cw)
                ? cw.GetInt32()
                : isMelee ? Math.Max(1, (int)MathF.Round(range)) : 80;
            int collisionHeight = e.TryGetProperty("collisionHeight", out var ch)
                ? ch.GetInt32()
                : isMelee ? (int)BattleHitbox.CharacterHeight : 80;

            var effect = new EffectData
            {
                Type = type,
                Animation = ReadString(e, "animation") ?? "",
                Trigger = ReadString(e, "trigger") ?? "",
                Damage = e.TryGetProperty("damage", out var d) ? d.GetInt32() : 0,
                Stun = e.TryGetProperty("stun", out var s) ? s.GetSingle() : 0f,
                Speed = e.TryGetProperty("speed", out var sp) ? sp.GetSingle() : 0f,
                ProjectileAnim = ReadString(e, "projectileAnim") ?? "",
                ObjectAnim = ReadString(e, "objectAnim") ?? "",
                SpawnMode = ReadString(e, "spawnMode") ?? (isMelee ? "casterFront" : "between"),
                SpawnOffsetX = e.TryGetProperty("spawnOffsetX", out var sox)
                    ? sox.GetSingle()
                    : isMelee ? BattleHitbox.CharacterWidth / 2f + collisionWidth / 2f : 10f,
                SpawnOffsetY = e.TryGetProperty("spawnOffsetY", out var soy) ? soy.GetSingle() : isMelee ? 0f : -30f,
                CollisionWidth = collisionWidth,
                CollisionHeight = collisionHeight,
                BlockEnemyAttack = e.TryGetProperty("blockEnemyAttack", out var bea) ? bea.GetBoolean() : true,
                BlockEnemyProjectile = e.TryGetProperty("blockEnemyProjectile", out var bep) ? bep.GetBoolean() : true,
                BlockEnemySkill = e.TryGetProperty("blockEnemySkill", out var bes) ? bes.GetBoolean() : true,
                Range = range,
                Duration = e.TryGetProperty("duration", out var dur) ? dur.GetSingle() : 3f,
                Render = ParseEffectRender(e)
            };

            if (e.TryGetProperty("frames", out var frames) && frames.ValueKind == JsonValueKind.Array)
                effect.Frames = frames.EnumerateArray().Select(frame => frame.GetInt32()).ToList();

            if (e.TryGetProperty("hitFrames", out var hitFrames) && hitFrames.ValueKind == JsonValueKind.Array)
                effect.HitFrames = hitFrames.EnumerateArray().Select(frame => frame.GetInt32()).ToList();

            parsed.Add(effect);
        }

        return parsed;
    }

    private static EffectRenderData ParseEffectRender(JsonElement effect)
    {
        if (!effect.TryGetProperty("render", out var render))
            return new EffectRenderData();

        return new EffectRenderData
        {
            Scale = render.TryGetProperty("scale", out var scale) ? scale.GetSingle() : 1f,
            OffsetX = render.TryGetProperty("offsetX", out var offsetX) ? offsetX.GetSingle() : 0f,
            OffsetY = render.TryGetProperty("offsetY", out var offsetY) ? offsetY.GetSingle() : 0f,
            UseSpriteSize = render.TryGetProperty("useSpriteSize", out var useSpriteSize)
                ? useSpriteSize.GetBoolean()
                : true,
            AlignY = ReadString(render, "alignY") ?? "center",
            FacingSource = ReadString(render, "facingSource") ?? "owner"
        };
    }

    private static string? ReadString(JsonElement element, string propertyName)
    {
        return element.ValueKind == JsonValueKind.Undefined
            ? null
            : element.TryGetProperty(propertyName, out var property)
                ? property.GetString()
                : null;
    }
}
