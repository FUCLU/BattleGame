using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    internal sealed class GearButton : Button
    {
        private const string GearAssetName = "setting_gear.png";
        private const string GearEmbeddedResourceName = "BattleGame.Client.Assets.UI.setting_gear.png";
        private static Image? _gearImage;

        public GearButton()
        {
            Text = string.Empty;
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.FromArgb(44, 74, 110);
            ForeColor = Color.FromArgb(255, 235, 156);
            Cursor = Cursors.Hand;
            Size = new Size(48, 48);
            FlatAppearance.BorderSize = 2;
            FlatAppearance.BorderColor = Color.PaleTurquoise;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(57, 100, 150);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(31, 47, 86);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Image? gearImage = GetGearImage();
            if (gearImage == null)
                return;

            Rectangle target = GetCenteredImageRect(gearImage);
            pevent.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            pevent.Graphics.DrawImage(gearImage, target);
        }

        private Rectangle GetCenteredImageRect(Image image)
        {
            int padding = Math.Max(5, Math.Min(ClientSize.Width, ClientSize.Height) / 8);
            int maxWidth = Math.Max(1, ClientSize.Width - padding * 2);
            int maxHeight = Math.Max(1, ClientSize.Height - padding * 2);
            float scale = Math.Min(maxWidth / (float)image.Width, maxHeight / (float)image.Height);
            int width = Math.Max(1, (int)(image.Width * scale));
            int height = Math.Max(1, (int)(image.Height * scale));
            return new Rectangle((ClientSize.Width - width) / 2, (ClientSize.Height - height) / 2, width, height);
        }

        private static Image? GetGearImage()
        {
            if (_gearImage != null)
                return _gearImage;

            using Stream? resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GearEmbeddedResourceName);
            if (resourceStream != null)
            {
                using var resourceImage = Image.FromStream(resourceStream);
                _gearImage = new Bitmap(resourceImage);
                return _gearImage;
            }

            string? path = FindGearAsset();
            if (path == null)
                return null;

            using var stream = File.OpenRead(path);
            using var image = Image.FromStream(stream);
            _gearImage = new Bitmap(image);
            return _gearImage;
        }

        private static string? FindGearAsset()
        {
            string[] roots =
            {
                AppContext.BaseDirectory,
                Environment.CurrentDirectory
            };

            foreach (string root in roots)
            {
                string? found = FindGearAssetFromRoot(root);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static string? FindGearAssetFromRoot(string root)
        {
            DirectoryInfo? directory = new DirectoryInfo(root);
            while (directory != null)
            {
                string direct = Path.Combine(directory.FullName, "Assets", "UI", GearAssetName);
                if (File.Exists(direct))
                    return direct;

                string projectRelative = Path.Combine(directory.FullName, "BattleGame.Client", "Assets", "UI", GearAssetName);
                if (File.Exists(projectRelative))
                    return projectRelative;

                directory = directory.Parent;
            }

            return null;
        }
    }
}
