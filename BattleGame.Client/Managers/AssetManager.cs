using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace BattleGame.Client.Managers
{
    public static class AssetManager
    {
        private static readonly Dictionary<string, Bitmap> _sprites = new();

        public static Bitmap LoadSprite(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null!;

            string key = relativePath.ToLowerInvariant();

            if (_sprites.TryGetValue(key, out Bitmap? bmp) && bmp != null)
                return bmp;

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (File.Exists(fullPath))
            {
                bmp = new Bitmap(fullPath);
                _sprites[key] = bmp;
                return bmp;
            }

            Console.WriteLine($"[AssetManager] Không tìm thấy: {relativePath}");
            return null!;
        }

        public static void ClearCache()
        {
            foreach (var bmp in _sprites.Values)
                bmp?.Dispose();
            _sprites.Clear();
        }
    }
}