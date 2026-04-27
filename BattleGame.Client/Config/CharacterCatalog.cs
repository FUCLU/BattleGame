using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BattleGame.Client.Config
{
    public sealed class CharacterSelectionItem
    {
        public CharacterSelectionItem(string id, string displayName, string previewImage, int hp, int atk, int def, int speed, string skillLabel, string? assetFolder = null)
        {
            Id = id;
            DisplayName = displayName;
            PreviewImage = previewImage;
            Hp = hp;
            Atk = atk;
            Def = def;
            Speed = speed;
            SkillLabel = skillLabel;
            AssetFolder = assetFolder;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string PreviewImage { get; }
        public int Hp { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Speed { get; }
        public string SkillLabel { get; }
        public string? AssetFolder { get; }

        public string ResolvedAssetFolder => !string.IsNullOrWhiteSpace(AssetFolder)
            ? AssetFolder
            : string.IsNullOrWhiteSpace(Id)
                ? string.Empty
                : char.ToUpper(Id[0]) + Id[1..];

        public string GetPreviewPath(string assetsRoot)
        {
            return Path.Combine(assetsRoot, ResolvedAssetFolder, PreviewImage);
        }
    }

    public static class CharacterCatalog
    {
        public static List<CharacterSelectionItem> LoadSelectionItems(string configRoot)
        {
            var items = new List<CharacterSelectionItem>();
            string configDir = Path.Combine(configRoot, "Config", "Characters");

            if (Directory.Exists(configDir))
            {
                foreach (string configPath in Directory.EnumerateFiles(configDir, "*.json"))
                {
                    if (TryLoadFromConfig(configPath, out var item))
                        items.Add(item);
                }
            }

            MergeLegacyItems(items);
            items.Sort((left, right) => StringComparer.OrdinalIgnoreCase.Compare(left.DisplayName, right.DisplayName));

            return items;
        }

        private static void MergeLegacyItems(List<CharacterSelectionItem> items)
        {
            // Removed: Legacy items are no longer merged, only JSON characters are used
        }

        private static bool TryLoadFromConfig(string configPath, out CharacterSelectionItem item)
        {
            item = null!;

            try
            {
                var definition = CharacterDefinitionLoader.Load(configPath);
                if (string.IsNullOrWhiteSpace(definition.Id))
                    return false;

                item = new CharacterSelectionItem(
                    definition.Id,
                    definition.Selection.DisplayName,
                    definition.Selection.PreviewImage,
                    definition.Stats.Hp,
                    definition.Stats.Atk,
                    definition.Stats.Def,
                    (int)definition.Stats.Speed,
                    definition.Selection.SkillLabel,
                    definition.Selection.AssetFolder);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static string ToDisplayName(string value)
        {
            string normalized = value.Replace('_', ' ').Trim();
            if (normalized.Length == 0)
                return value;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized.ToLowerInvariant());
        }

        private static IEnumerable<CharacterSelectionItem> GetLegacyItems()
        {
            return new[]
            {
                new CharacterSelectionItem("warrior", "Warrior", "IDLE.png", 120, 40, 25, 15, "Blade Slash", "Warrior"),
                new CharacterSelectionItem("girlnight", "GirlKnight", "Idle_KG_1.png", 100, 28, 35, 18, "Shield Bash", "GirlKnight"),
                new CharacterSelectionItem("kabold", "Kabold", "KaboldAvt.png", 80, 50, 10, 32, "Claw Strike", "Kabold"),
                new CharacterSelectionItem("soldier", "Soldier", "Idle.png", 110, 45, 15, 20, "Shoot", "Soldier"),
            };
        }
    }
}
