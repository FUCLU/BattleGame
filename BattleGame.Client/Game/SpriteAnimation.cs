using System;
using System.IO;
using System.Drawing;
using BattleGame.Client.Managers;

namespace BattleGame.Client.Game
{
    public class SpriteAnimation
    {
        private readonly Bitmap spriteSheet;
        private readonly int frameWidth;
        private readonly int frameHeight;
        private readonly int totalFrames;

        public int CurrentFrame { get; private set; } = 0;
        public int FrameDelayMs { get; set; }
        public bool Loop { get; set; } = true;
        public bool IsFinished { get; private set; } = false;

        private DateTime lastFrameTime = DateTime.Now;

        public SpriteAnimation(string relativePath, int frameWidth, int frameHeight, int frameDelayMs = 80)
        {
            spriteSheet = AssetManager.LoadSprite(relativePath)
                ?? throw new FileNotFoundException($"Sprite sheet not found: {relativePath}");

            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.FrameDelayMs = frameDelayMs;
            this.totalFrames = spriteSheet.Width / frameWidth;
        }

        public void Update()
        {
            if ((DateTime.Now - lastFrameTime).TotalMilliseconds < FrameDelayMs)
                return;

            CurrentFrame++;

            if (CurrentFrame >= totalFrames)
            {
                if (Loop)
                    CurrentFrame = 0;
                else
                {
                    CurrentFrame = totalFrames - 1;
                    IsFinished = true;
                }
            }

            lastFrameTime = DateTime.Now;
        }

        public void Draw(Graphics g, float x, float y, bool flipHorizontal = false)
        {
            Rectangle sourceRect = new Rectangle(CurrentFrame * frameWidth, 0, frameWidth, frameHeight);

            if (flipHorizontal)
            {
                var state = g.Save();
                g.TranslateTransform(x + frameWidth, y);
                g.ScaleTransform(-1f, 1f);
                g.DrawImage(spriteSheet, new RectangleF(0, 0, frameWidth, frameHeight), sourceRect, GraphicsUnit.Pixel);
                g.Restore(state);
            }
            else
            {
                g.DrawImage(spriteSheet,
                    new Rectangle((int)x, (int)y, frameWidth, frameHeight),
                    sourceRect,
                    GraphicsUnit.Pixel);
            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            IsFinished = false;
            lastFrameTime = DateTime.Now;
        }
    }
}