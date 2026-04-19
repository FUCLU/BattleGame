using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BattleGame.Client.Managers
{
    public static class AssetManager
    {
        private static readonly Dictionary<string, Image> _spriteCache = new Dictionary<string, Image>();

        // Chúng ta dùng tên LoadImage cho đúng với lỗi bạn đang gặp
        public static Image LoadImage(string relativePath)
        {
            if (_spriteCache.TryGetValue(relativePath, out var cachedImage))
            {
                return cachedImage;
            }

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            try
            {
                if (File.Exists(fullPath))
                {
                    Image newImage = Image.FromFile(fullPath);
                    _spriteCache[relativePath] = newImage;
                    return newImage;
                }

                Console.WriteLine($"[Error] Không tìm thấy file: {fullPath}");
                return CreateErrorSprite();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Exception] Lỗi load ảnh: {ex.Message}");
                return CreateErrorSprite();
            }
        }

        // Tạo hàm alias LoadSprite để nếu chỗ khác gọi cũng không bị lỗi
        public static Image LoadSprite(string path) => LoadImage(path);

        private static Image CreateErrorSprite()
        {
            Bitmap bmp = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.Magenta); }
            return bmp;
        }
    }
}