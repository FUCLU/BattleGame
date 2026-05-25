using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BattleGame.Client.Config
{
    public sealed class CharacterRenderConfig
    {
        public float Scale { get; init; } = 1f;
        public float OffsetY { get; init; } = 0f;
        public bool UseSpriteSize { get; init; } = false;
        public float ProtectionOverlayOffsetY { get; init; } = 0f;
        public bool ProtectionUsesIdleBase { get; init; } = true;
        public bool ProtectionBlocksAllDirections { get; init; } = false;
        public bool InvertFacing { get; init; } = false;
    }

    public sealed class CharacterSelectionConfig
    {
        public string DisplayName { get; init; } = "";
        public string PreviewImage { get; init; } = "Idle.png";
        public string SkillLabel { get; init; } = "Skill 1";
        public string? AssetFolder { get; init; }
    }

    public sealed class CharacterDefinition
    {
        public string Id { get; init; } = "";
        public CharacterStats Stats { get; init; } = new();
        public SkillData? Skill1 { get; init; }
        public SkillData? Skill2 { get; init; }
        public List<EffectData> AttackEffects { get; init; } = new();
        public CharacterRenderConfig Render { get; init; } = new();
        public CharacterSelectionConfig Selection { get; init; } = new();
    }

    public static class CharacterDefinitionLoader
    {
        public static string ResolveConfigPath(string configRoot, string characterId)
        {
            string characterPath = Path.Combine(configRoot, "Config", "Characters", $"{characterId}.json");
            if (File.Exists(characterPath))
                return characterPath;

            string bossPath = Path.Combine(configRoot, "Config", "Bosses", $"{characterId}.json");
            if (File.Exists(bossPath))
                return bossPath;

            throw new FileNotFoundException(
                $"Config not found for '{characterId}' in Config/Characters or Config/Bosses.",
                characterPath);
        }

        public static CharacterDefinition Load(string configPath)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(configPath));
            var root = doc.RootElement;
            var stats = root.GetProperty("stats");

            string id = ReadString(root, "id") ?? Path.GetFileNameWithoutExtension(configPath);
            var skill1 = TryParseSkill(root, "skill1");
            var skill2 = TryParseSkill(root, "skill2");

            return new CharacterDefinition
            {
                Id = id,
                Stats = ParseStats(stats),
                Skill1 = skill1,
                Skill2 = skill2,
                AttackEffects = ParseEffects(root, "attackEffects"),
                Render = ParseRender(root),
                Selection = ParseSelection(root, id, skill1)
            };
        }

        private static CharacterStats ParseStats(JsonElement stats)
        {
            var attackProj = stats.TryGetProperty("attackProjectile", out var ap) ? ap.GetString() : null;
            var attackProjSpeed = stats.TryGetProperty("attackProjectileSpeed", out var aps) ? aps.GetSingle() : 0f;
            var attackProjectileSpawnOffsetX = stats.TryGetProperty("attackProjectileSpawnOffsetX", out var apox) ? apox.GetSingle() : 30f;
            var attackProjectileSpawnOffsetY = stats.TryGetProperty("attackProjectileSpawnOffsetY", out var apoy) ? apoy.GetSingle() : -50f;
            var attackProjectileScale = stats.TryGetProperty("attackProjectileScale", out var apsc) ? apsc.GetSingle() : 1f;

            return new CharacterStats
            {
                Hp = stats.GetProperty("hp").GetInt32(),
                Def = stats.GetProperty("def").GetInt32(),
                ArmorPen = stats.TryGetProperty("armorPen", out var armorPen) ? armorPen.GetInt32() : 0,
                Mana = stats.GetProperty("mana").GetInt32(),
                ManaRegen = stats.TryGetProperty("manaRegen", out var manaRegen) ? manaRegen.GetSingle() : 8f,
                Atk = stats.GetProperty("atk").GetSingle(),
                Speed = stats.GetProperty("speed").GetSingle(),
                AtkSpeed = stats.GetProperty("atkSpeed").GetSingle(),
                StunDuration = stats.TryGetProperty("stunDuration", out var stunDuration) ? stunDuration.GetSingle() : 0f,
                AttackRange = stats.TryGetProperty("attackRange", out var ar) ? ar.GetSingle() : 150f,
                AttackProjectile = attackProj,
                AttackProjectileSpeed = attackProjSpeed,
                AttackProjectileSpawnOffsetX = attackProjectileSpawnOffsetX,
                AttackProjectileSpawnOffsetY = attackProjectileSpawnOffsetY,
                AttackProjectileScale = attackProjectileScale
            };
        }

        private static CharacterRenderConfig ParseRender(JsonElement root)
        {
            if (!root.TryGetProperty("render", out var render))
                return new CharacterRenderConfig();

            return new CharacterRenderConfig
            {
                Scale = render.TryGetProperty("scale", out var scale) ? scale.GetSingle() : 1f,
                OffsetY = render.TryGetProperty("offsetY", out var offsetY) ? offsetY.GetSingle() : 0f,
                UseSpriteSize = render.TryGetProperty("useSpriteSize", out var useSpriteSize)
                    ? useSpriteSize.GetBoolean()
                    : false,
                ProtectionOverlayOffsetY = render.TryGetProperty("protectionOverlayOffsetY", out var protectionOffsetY)
                    ? protectionOffsetY.GetSingle()
                    : 0f,
                ProtectionUsesIdleBase = render.TryGetProperty("protectionUsesIdleBase", out var protectionUsesIdleBase)
                    ? protectionUsesIdleBase.GetBoolean()
                    : true,
                ProtectionBlocksAllDirections = render.TryGetProperty("protectionBlocksAllDirections", out var protectionBlocksAllDirections)
                    ? protectionBlocksAllDirections.GetBoolean()
                    : false,
                InvertFacing = render.TryGetProperty("invertFacing", out var invertFacing)
                    && invertFacing.GetBoolean()
            };
        }

        private static CharacterSelectionConfig ParseSelection(JsonElement root, string id, SkillData? skill1)
        {
            var selection = root.TryGetProperty("selection", out var selectionElement)
                ? selectionElement
                : default;

            return new CharacterSelectionConfig
            {
                DisplayName = ReadString(selection, "displayName") ?? CharacterCatalog.ToDisplayName(id),
                PreviewImage = ReadString(selection, "previewImage") ?? "Idle.png",
                SkillLabel = ReadString(selection, "skillLabel") ?? GetDefaultSkillLabel(skill1),
                AssetFolder = ReadString(selection, "assetFolder")
            };
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
                ArmorPen = el.TryGetProperty("armorPen", out var armorPen) ? armorPen.GetInt32() : null,
                InvulnerableWhileCasting = el.TryGetProperty("invulnerableWhileCasting", out var invulnerableWhileCasting)
                    && invulnerableWhileCasting.GetBoolean()
            };

            if (el.TryGetProperty("effects", out var effects))
            {
                skill.Effects = ParseEffects(effects);
                foreach (var effect in skill.Effects)
                    effect.ArmorPen ??= skill.ArmorPen;
            }

            return skill;
        }

        private static List<EffectData> ParseEffects(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var effects) || effects.ValueKind != JsonValueKind.Array)
                return new List<EffectData>();

            return ParseEffects(effects);
        }

        private static List<EffectData> ParseEffects(JsonElement effects)
        {
            var parsed = new List<EffectData>();

            foreach (var e in effects.EnumerateArray())
            {
                var effect = new EffectData
                {
                    Type = e.GetProperty("type").GetString() ?? "",
                    Animation = e.TryGetProperty("animation", out var anim) ? anim.GetString() ?? "" : "",
                    Trigger = e.GetProperty("trigger").GetString() ?? "",
                    Damage = e.TryGetProperty("damage", out var d) ? d.GetSingle() : 0,
                    ArmorPen = e.TryGetProperty("armorPen", out var armorPen) ? armorPen.GetInt32() : null,
                    Stun = e.TryGetProperty("stun", out var s) ? s.GetSingle() : 0,
                    Speed = e.TryGetProperty("speed", out var sp) ? sp.GetSingle() : 0,
                    ProjectileAnim = e.TryGetProperty("projectileAnim", out var pa) ? pa.GetString() ?? "" : "",
                    ObjectAnim = e.TryGetProperty("objectAnim", out var oa) ? oa.GetString() ?? "" : "",
                    SpawnMode = e.TryGetProperty("spawnMode", out var sm) ? sm.GetString() ?? "between" : "between",
                    SpawnOffsetX = e.TryGetProperty("spawnOffsetX", out var sox) ? sox.GetSingle() : 10f,
                    SpawnOffsetY = e.TryGetProperty("spawnOffsetY", out var soy) ? soy.GetSingle() : -30f,
                    CollisionWidth = e.TryGetProperty("collisionWidth", out var cw) ? cw.GetInt32() : 80,
                    CollisionHeight = e.TryGetProperty("collisionHeight", out var ch) ? ch.GetInt32() : 80,
                    BlockEnemyAttack = e.TryGetProperty("blockEnemyAttack", out var bea) ? bea.GetBoolean() : true,
                    BlockEnemyProjectile = e.TryGetProperty("blockEnemyProjectile", out var bep) ? bep.GetBoolean() : true,
                    BlockEnemySkill = e.TryGetProperty("blockEnemySkill", out var bes) ? bes.GetBoolean() : true,
                    Range = e.TryGetProperty("range", out var r) ? r.GetSingle() : 50f,
                    Duration = e.TryGetProperty("duration", out var dur) ? dur.GetSingle() : 3.0f,
                    Render = ParseEffectRender(e)
                };

                if (e.TryGetProperty("triggerFrames", out var triggerFrames))
                {
                    effect.TriggerFrames = new List<int>();
                    foreach (var f in triggerFrames.EnumerateArray())
                        effect.TriggerFrames.Add(f.GetInt32());
                }

                if (e.TryGetProperty("frames", out var frames))
                {
                    effect.Frames = new List<int>();
                    foreach (var f in frames.EnumerateArray())
                        effect.Frames.Add(f.GetInt32());

                    effect.TriggerFrames ??= new List<int>(effect.Frames);
                }

                if (e.TryGetProperty("hitFrames", out var hitFrames))
                {
                    effect.HitFrames = new List<int>();
                    foreach (var f in hitFrames.EnumerateArray())
                        effect.HitFrames.Add(f.GetInt32());
                }

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
                AlignY = render.TryGetProperty("alignY", out var alignY)
                    ? alignY.GetString() ?? "center"
                    : "center",
                FacingSource = render.TryGetProperty("facingSource", out var facingSource)
                    ? facingSource.GetString() ?? "owner"
                    : "owner"
            };
        }

        private static string GetDefaultSkillLabel(SkillData? skill1)
        {
            if (skill1 is null)
                return "Skill 1";

            if (!string.IsNullOrWhiteSpace(skill1.Id))
                return CharacterCatalog.ToDisplayName(skill1.Id);

            if (!string.IsNullOrWhiteSpace(skill1.Animation))
                return CharacterCatalog.ToDisplayName(skill1.Animation);

            return "Skill 1";
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
}
