using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BattleGame.Client.Controls
{
    public class ToggleSwitch : CheckBox
    {
        [Category("Appearance")]
        public Color OnBackColor { get; set; } = Color.FromArgb(220, 34, 84, 132);

        [Category("Appearance")]
        public Color OffBackColor { get; set; } = Color.FromArgb(205, 18, 22, 44);

        [Category("Appearance")]
        public Color ThumbColor { get; set; } = Color.FromArgb(255, 218, 95);

        [Category("Appearance")]
        public Color BorderColor { get; set; } = Color.FromArgb(230, 244, 194, 91);

        [Category("Appearance")]
        public Color OnTextColor { get; set; } = Color.White;

        [Category("Appearance")]
        public Color OffTextColor { get; set; } = Color.LightSteelBlue;

        public ToggleSwitch()
        {
            SetStyle(ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            MinimumSize = new Size(90, 28);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            PaintParentBackground(e.Graphics);

            Rectangle track = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = Math.Max(1, Height / 2);
            using GraphicsPath trackPath = CreateRoundRect(track, radius);
            using SolidBrush backBrush = new SolidBrush(Checked ? OnBackColor : OffBackColor);
            e.Graphics.FillPath(backBrush, trackPath);

            using Pen borderPen = new Pen(BorderColor, 1.6f);
            e.Graphics.DrawPath(borderPen, trackPath);

            int padding = 4;
            int thumbSize = Math.Max(1, Height - padding * 2);
            int thumbX = Checked ? Width - thumbSize - padding : padding;
            Rectangle thumb = new Rectangle(thumbX, padding, thumbSize, thumbSize);
            using GraphicsPath thumbPath = CreateRoundRect(thumb, thumbSize / 2);
            using SolidBrush thumbBrush = new SolidBrush(ThumbColor);
            e.Graphics.FillPath(thumbBrush, thumbPath);

            string stateText = string.IsNullOrWhiteSpace(Text)
                ? (Checked ? "ON" : "OFF")
                : Text;

            Rectangle textRect = Checked
                ? new Rectangle(8, 0, Math.Max(1, Width - thumbSize - padding * 3), Height)
                : new Rectangle(thumbSize + padding * 2, 0, Math.Max(1, Width - thumbSize - padding * 3), Height);

            TextRenderer.DrawText(
                e.Graphics,
                stateText,
                Font,
                textRect,
                Checked ? OnTextColor : OffTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Invalidate();
        }

        private void PaintParentBackground(Graphics graphics)
        {
            if (Parent == null)
            {
                graphics.Clear(Color.Transparent);
                return;
            }

            using SolidBrush backBrush = new SolidBrush(Parent.BackColor);
            graphics.FillRectangle(backBrush, ClientRectangle);

            Image? background = Parent.BackgroundImage;
            if (background == null)
                return;

            Rectangle parentClient = Parent.ClientRectangle;
            Rectangle imageBounds = ResolveBackgroundBounds(parentClient, background, Parent.BackgroundImageLayout);
            Rectangle shiftedBounds = new Rectangle(
                imageBounds.X - Left,
                imageBounds.Y - Top,
                imageBounds.Width,
                imageBounds.Height);

            if (Parent.BackgroundImageLayout == ImageLayout.Tile)
            {
                using TextureBrush brush = new TextureBrush(background, WrapMode.Tile);
                brush.TranslateTransform(-Left, -Top);
                graphics.FillRectangle(brush, ClientRectangle);
                return;
            }

            graphics.DrawImage(background, shiftedBounds);
        }

        private static Rectangle ResolveBackgroundBounds(Rectangle parentClient, Image image, ImageLayout layout)
        {
            return layout switch
            {
                ImageLayout.Center => new Rectangle(
                    parentClient.X + (parentClient.Width - image.Width) / 2,
                    parentClient.Y + (parentClient.Height - image.Height) / 2,
                    image.Width,
                    image.Height),
                ImageLayout.Zoom => ResolveZoomBounds(parentClient, image),
                ImageLayout.None => new Rectangle(parentClient.Location, image.Size),
                _ => parentClient
            };
        }

        private static Rectangle ResolveZoomBounds(Rectangle parentClient, Image image)
        {
            float imageRatio = image.Width / (float)Math.Max(1, image.Height);
            float parentRatio = parentClient.Width / (float)Math.Max(1, parentClient.Height);

            int width;
            int height;
            if (imageRatio > parentRatio)
            {
                width = parentClient.Width;
                height = (int)Math.Round(width / imageRatio);
            }
            else
            {
                height = parentClient.Height;
                width = (int)Math.Round(height * imageRatio);
            }

            return new Rectangle(
                parentClient.X + (parentClient.Width - width) / 2,
                parentClient.Y + (parentClient.Height - height) / 2,
                width,
                height);
        }

        private static GraphicsPath CreateRoundRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
