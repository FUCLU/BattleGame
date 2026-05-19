using BattleGame.Client.Config;
using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BattleGame.Client.Game.Gameplay
{
    public static class CharacterFactory
    {
        private static int _nextEntityId = 0;

        public static Entity Create(string characterId, float startX, float groundY,
                                    Dictionary<string, object> availableAnimations)
        {
            string projectDir = ResolveClientRoot();

            string path = CharacterDefinitionLoader.ResolveConfigPath(projectDir, characterId);
            System.Diagnostics.Debug.WriteLine($"[CharacterFactory] Loading character from: {path}");

            var definition = CharacterDefinitionLoader.Load(path);
            var baseStats = definition.Stats;

            System.Diagnostics.Debug.WriteLine($"[CharacterFactory] Stats parsed: AttackProjectile={baseStats.AttackProjectile}, Speed={baseStats.AttackProjectileSpeed}");

            int attackCount = 0;
            for (int i = 1; i <= 10; i++)
                if (availableAnimations.ContainsKey($"Attack_{i}")) attackCount++;
            attackCount = Math.Max(1, attackCount);

            // Must be unique per entity instance; characterId-based hash collides
            // when both players pick the same character.
            int entityId = System.Threading.Interlocked.Increment(ref _nextEntityId);
            var entity = new Entity(entityId);

            entity.Add(new CharacterComponent
            {
                CharacterId = definition.Id,
                BaseStats = baseStats,
                Render = definition.Render,
                AvailableAnimations = new HashSet<string>(availableAnimations.Keys, StringComparer.OrdinalIgnoreCase),
                AnimationDurations = BuildAnimationDurations(availableAnimations),
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

        private static Dictionary<string, float> BuildAnimationDurations(Dictionary<string, object> animations)
        {
            var durations = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in animations)
            {
                if (kv.Value is not SpriteAnimation animation || animation.Frames.Length == 0)
                    continue;

                durations[kv.Key] = animation.Frames.Length / Math.Max(1f, animation.Fps);
            }

            return durations;
        }

        private static string ResolveClientRoot()
        {
            string startDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? AppDomain.CurrentDomain.BaseDirectory;

            return ClientContentRoot.Resolve(startDirectory);
        }
    }
}
