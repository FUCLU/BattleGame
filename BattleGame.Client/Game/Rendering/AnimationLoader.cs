using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace BattleGame.Client.Game.Rendering
{
    public class AnimationLoader
    {
        private readonly string _assetRoot;
        private readonly string _configRoot;

        public AnimationLoader(string assetFolder)
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            string projectDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", ".."));
            _assetRoot = Path.Combine(projectDir, assetFolder);
            _configRoot = Path.Combine(projectDir, "Config");
        }

        public Dictionary<string, SpriteAnimation> Load(string characterId)
        {
            var configPath = Path.Combine(_configRoot, "Characters", $"{characterId}.json");
            if (!File.Exists(configPath))
                throw new FileNotFoundException($"Config not found: {configPath}");

            using var doc = JsonDocument.Parse(File.ReadAllText(configPath));
            var root = doc.RootElement;

            var animations = root.GetProperty("animations");
            var result = new Dictionary<string, SpriteAnimation>();

            foreach (var anim in animations.EnumerateObject())
            {
                var name = anim.Name;
                var frameCount = anim.Value.GetProperty("frameCount").GetInt32();
                var fps = anim.Value.GetProperty("fps").GetSingle();
                var loop = anim.Value.GetProperty("loop").GetBoolean();

                var sheet = LoadSheet(characterId, name);
                if (sheet == null) continue;

                var frames = SliceFrames(sheet, frameCount);

                result[name] = new SpriteAnimation
                {
                    Name = name,
                    Frames = frames,
                    Fps = fps,
                    Loop = loop
                };

                sheet.Dispose();
            }

            return result;
        }

        private Bitmap? LoadSheet(string characterId, string animName)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return null;

            string folder = char.ToUpper(characterId[0]) + characterId[1..];
            string path = Path.Combine(_assetRoot, "Characters", folder, $"{animName}.png");
            if (!File.Exists(path)) return null;

            using var raw = new Bitmap(path);
            var converted = new Bitmap(raw.Width, raw.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(converted);
            g.Clear(Color.Transparent);
            g.DrawImage(raw, 0, 0);
            return converted;
        }

        private static Bitmap[] SliceFrames(Bitmap sheet, int frameCount)
        {
            int fw = sheet.Width / frameCount;
            int fh = sheet.Height;
            var frames = new Bitmap[frameCount];

            var rect = new Rectangle(0, 0, sheet.Width, fh);
            var sheetData = sheet.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = sheetData.Stride;
            byte[] sheetBytes = new byte[stride * fh];
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
                    int srcRow = y * stride + i * fw * 4;
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