using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using BattleGame.Client.Config;

namespace BattleGame.Client.Game.Rendering
{
    public class AnimationLoader
    {
        private readonly string _assetRoot;
        private readonly string _configRoot;

        public AnimationLoader(string assetFolder)
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            string projectDir = ResolveClientRoot(assemblyDir);
            _assetRoot = Path.Combine(projectDir, assetFolder);
            _configRoot = Path.Combine(projectDir, "Config");
        }

        private static string ResolveClientRoot(string startDirectory)
        {
            return ClientContentRoot.Resolve(startDirectory);
        }

        public Dictionary<string, SpriteAnimation> Load(string characterId)
        {
            var configPath = CharacterDefinitionLoader.ResolveConfigPath(
                Directory.GetParent(_configRoot)?.FullName ?? _configRoot,
                characterId);

            using var doc = JsonDocument.Parse(File.ReadAllText(configPath));
            var root = doc.RootElement;
            string? assetFolderHint = null;
            if (root.TryGetProperty("selection", out var selection) &&
                selection.TryGetProperty("assetFolder", out var assetFolderProp))
            {
                assetFolderHint = assetFolderProp.GetString();
            }

            var animations = root.GetProperty("animations");
            var result = new Dictionary<string, SpriteAnimation>();

            foreach (var anim in animations.EnumerateObject())
            {
                var name = anim.Name;
                var frameCount = anim.Value.GetProperty("frameCount").GetInt32();
                var fps = anim.Value.GetProperty("fps").GetSingle();
                var loop = anim.Value.GetProperty("loop").GetBoolean();
                var fileName = anim.Value.TryGetProperty("file", out var file)
                    ? file.GetString()
                    : null;
                var layout = anim.Value.TryGetProperty("layout", out var layoutProp)
                    ? (layoutProp.GetString() ?? "horizontal")
                    : "horizontal";
                var offsetX = anim.Value.TryGetProperty("offsetX", out var ox) ? ox.GetSingle() : 0f;
                var offsetY = anim.Value.TryGetProperty("offsetY", out var oy) ? oy.GetSingle() : 0f;

                var sheet = LoadSheet(characterId, name, fileName, assetFolderHint);
                if (sheet == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[AnimationLoader] Animation {characterId}/{name} NOT found (file missing)");
                    continue;
                }

                var frames = SliceFrames(sheet, frameCount, layout);

                result[name] = new SpriteAnimation
                {
                    Name = name,
                    Frames = frames,
                    Fps = fps,
                    Loop = loop,
                    OffsetX = offsetX,
                    OffsetY = offsetY
                };

                System.Diagnostics.Debug.WriteLine($"[AnimationLoader] Animation {characterId}/{name} loaded successfully");

                sheet.Dispose();
            }

            System.Diagnostics.Debug.WriteLine($"[AnimationLoader] Total animations loaded for {characterId}: {result.Count}");
            return result;
        }

        private Bitmap? LoadSheet(string characterId, string animName, string? fileName = null, string? assetFolderHint = null)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return null;

            string folder = char.ToUpper(characterId[0]) + characterId[1..];
            string spriteFileName = string.IsNullOrWhiteSpace(fileName) ? $"{animName}.png" : fileName;
            string path = Path.Combine(_assetRoot, "Characters", folder, spriteFileName);

            if (!string.IsNullOrWhiteSpace(assetFolderHint))
            {
                string hintedPath = Path.Combine(_assetRoot, assetFolderHint, spriteFileName);
                if (File.Exists(hintedPath))
                    path = hintedPath;
            }
            if (!File.Exists(path) && animName.StartsWith("Attack_", StringComparison.OrdinalIgnoreCase))
            {
                string legacyAttackName = "Attack" + animName[7..];
                string legacyPath = Path.Combine(_assetRoot, "Characters", folder, $"{legacyAttackName}.png");
                if (File.Exists(legacyPath))
                    path = legacyPath;
            }

            if (!File.Exists(path))
            {
                string dungeonBossPath = Path.Combine(_assetRoot, "dungeon", "boss", characterId.ToLowerInvariant(), spriteFileName);
                if (File.Exists(dungeonBossPath))
                    path = dungeonBossPath;
            }

            if (!File.Exists(path)) return null;

            using var raw = new Bitmap(path);
            var converted = new Bitmap(raw.Width, raw.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(converted);
            g.Clear(Color.Transparent);
            g.DrawImage(raw, 0, 0);
            return converted;
        }

        private static Bitmap[] SliceFrames(Bitmap sheet, int frameCount, string layout)
        {
            bool vertical = string.Equals(layout, "vertical", StringComparison.OrdinalIgnoreCase);
            int fw = vertical ? sheet.Width : Math.Max(1, sheet.Width / Math.Max(1, frameCount));
            int fh = vertical ? Math.Max(1, sheet.Height / Math.Max(1, frameCount)) : sheet.Height;
            var frames = new Bitmap[frameCount];

            var rect = new Rectangle(0, 0, sheet.Width, sheet.Height);
            var sheetData = sheet.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = sheetData.Stride;
            byte[] sheetBytes = new byte[stride * sheet.Height];
            Marshal.Copy(sheetData.Scan0, sheetBytes, 0, sheetBytes.Length);

            sheet.UnlockBits(sheetData);

            for (int i = 0; i < frameCount; i++)
            {
                var frame = new Bitmap(fw, fh, PixelFormat.Format32bppArgb);
                var frameData = frame.LockBits(
                    new Rectangle(0, 0, fw, fh),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                byte[] frameBytes = new byte[frameData.Stride * fh];

                for (int y = 0; y < fh; y++)
                {
                    int srcY = vertical ? (i * fh + y) : y;
                    int srcX = vertical ? 0 : i * fw;
                    int srcRow = srcY * stride + srcX * 4;
                    int dstRow = y * frameData.Stride;
                    Buffer.BlockCopy(sheetBytes, srcRow, frameBytes, dstRow, fw * 4);
                }

                Marshal.Copy(frameBytes, 0, frameData.Scan0, frameBytes.Length);
                frame.UnlockBits(frameData);

                frames[i] = frame;
            }

            return frames;
        }

    }
}
