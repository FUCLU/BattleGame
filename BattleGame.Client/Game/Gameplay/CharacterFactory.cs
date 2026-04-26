using BattleGame.Client.Config;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using System;
using System.Collections.Generic;
using System.IO;

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
            System.Diagnostics.Debug.WriteLine($"[CharacterFactory] Loading character from: {path}");

            var definition = CharacterDefinitionLoader.Load(path);
            var baseStats = definition.Stats;

            System.Diagnostics.Debug.WriteLine($"[CharacterFactory] Stats parsed: AttackProjectile={baseStats.AttackProjectile}, Speed={baseStats.AttackProjectileSpeed}");

            int attackCount = 0;
            for (int i = 1; i <= 10; i++)
                if (availableAnimations.ContainsKey($"Attack_{i}")) attackCount++;
            attackCount = Math.Max(1, attackCount);

            int entityId = characterId.GetHashCode();
            var entity = new Entity(entityId);

            entity.Add(new CharacterComponent
            {
                CharacterId = definition.Id,
                BaseStats = baseStats,
                Render = definition.Render,
                Hp = baseStats.Hp,
                Mana = baseStats.Mana,
                Skill1 = definition.Skill1,
                Skill2 = definition.Skill2,
                AttackEffects = definition.AttackEffects,
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
    }
}
