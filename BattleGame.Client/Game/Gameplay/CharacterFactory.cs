using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BattleGame.Client.Game.Gameplay
{
    public static class CharacterFactory
    {
        public static Entity Create(string characterId, float startX, float groundY,
                                    Dictionary<string, object> availableAnimations)
        {
            string projectDir = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));

            string path = Path.Combine(projectDir, "Config", "Characters", $"{characterId}.json");
            var doc = JsonDocument.Parse(File.ReadAllText(path)).RootElement;
            var stats = doc.GetProperty("stats");

            var baseStats = new CharacterStats
            {
                Hp = stats.GetProperty("hp").GetInt32(),
                Def = stats.GetProperty("def").GetInt32(),
                Mana = stats.GetProperty("mana").GetInt32(),
                Atk = stats.GetProperty("atk").GetInt32(),
                Speed = stats.GetProperty("speed").GetSingle(),
                AtkSpeed = stats.GetProperty("atkSpeed").GetSingle(),
                StunDuration = stats.GetProperty("stunDuration").GetSingle(),
                AttackRange = stats.TryGetProperty("attackRange", out var ar) ? ar.GetSingle() : 150f
            };

            SkillData? skill1 = null;
            SkillData? skill2 = null;
            if (doc.TryGetProperty("skills", out var skills))
            {
                if (skills.TryGetProperty("skill1", out var s1)) skill1 = ParseSkill(s1);
                if (skills.TryGetProperty("skill2", out var s2)) skill2 = ParseSkill(s2);
            }

            int attackCount = 0;
            for (int i = 1; i <= 10; i++)
                if (availableAnimations.ContainsKey($"Attack_{i}")) attackCount++;
            attackCount = Math.Max(1, attackCount);

            // Tạo ID số từ characterId
            int entityId = characterId.GetHashCode();
            var entity = new Entity(entityId);

            entity.Add(new CharacterComponent
            {
                BaseStats = baseStats,
                Hp = baseStats.Hp,
                Mana = baseStats.Mana,
                Skill1 = skill1,
                Skill2 = skill2,
                ActionDuration = 1f / baseStats.AtkSpeed,
                AttackAnimCount = attackCount
            });

            entity.Add(new MovementComponent
            {
                X = startX,
                Y = groundY,
                GroundY = groundY,
                Speed = baseStats.Speed,
                VelocityX = 0f,
                VelocityY = 0f
            });

            entity.Add(new SpriteComponent());

            return entity;
        }

        private static SkillData ParseSkill(JsonElement el)
        {
            var skill = new SkillData
            {
                Id = el.GetProperty("id").GetString() ?? "",
                ManaCost = el.GetProperty("manaCost").GetInt32(),
                Cooldown = el.GetProperty("cooldown").GetSingle(),
                Animation = el.TryGetProperty("animation", out var anim) ? anim.GetString() ?? "" : ""
            };

            if (el.TryGetProperty("effects", out var effects))
            {
                foreach (var e in effects.EnumerateArray())
                {
                    var effect = new EffectData
                    {
                        Type = e.GetProperty("type").GetString() ?? "",
                        Trigger = e.GetProperty("trigger").GetString() ?? "",
                        Damage = e.TryGetProperty("damage", out var d) ? d.GetInt32() : 0,
                        Stun = e.TryGetProperty("stun", out var s) ? s.GetSingle() : 0,
                        Speed = e.TryGetProperty("speed", out var sp) ? sp.GetSingle() : 0,
                        ProjectileAnim = e.TryGetProperty("projectileAnim", out var pa) ? pa.GetString() ?? "" : "",
                        Range = e.TryGetProperty("range", out var r) ? r.GetSingle() : 50f
                    };

                    if (e.TryGetProperty("frames", out var frames))
                    {
                        effect.Frames = new List<int>();
                        foreach (var f in frames.EnumerateArray())
                            effect.Frames.Add(f.GetInt32());
                    }

                    skill.Effects.Add(effect);
                }
            }

            return skill;
        }
    }
}