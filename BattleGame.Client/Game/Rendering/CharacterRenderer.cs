using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using System.Diagnostics;

namespace BattleGame.Client.Game.Rendering;

public class CharacterRenderer
{
    private readonly Dictionary<string, SpriteAnimation> _animations;
    private const int DrawWidth = 128;
    private const int DrawHeight = 128;

    public CharacterRenderer(Dictionary<string, SpriteAnimation> animations)
        => _animations = animations;

    public void Update(Entity entity, float deltaTime)
    {
        var sp = entity.Get<SpriteComponent>();
        if (!_animations.TryGetValue(sp.CurrentAnimation, out var anim)) return;

        sp.FrameTimer += deltaTime;
        if (sp.FrameTimer >= anim.FrameDuration)
        {
            sp.FrameTimer = 0f;
            if (sp.CurrentFrame < anim.Frames.Length - 1)
            {
                sp.CurrentFrame++;
            }
            else
            {
                sp.AnimationFinished = true;
                if (anim.Loop) sp.CurrentFrame = 0;
            }
        }
    }

    public void Draw(Graphics g, Entity entity)
    {
        var sp = entity.Get<SpriteComponent>();
        var mv = entity.Get<MovementComponent>();

        if (!_animations.TryGetValue(sp.CurrentAnimation, out var anim)) return;
        var frame = anim.Frames[Math.Min(sp.CurrentFrame, anim.Frames.Length - 1)];

        int drawW = frame.Width;
        int drawH = frame.Height;
        int drawX = (int)mv.X - drawW / 2;
        int drawY = (int)mv.Y - drawH + anim.OffsetY;

        var state = g.Save();

        if (mv.FacingRight)
        {
            g.DrawImage(frame, drawX, drawY, DrawWidth, DrawHeight);
        }
        else
        {
            g.TranslateTransform(drawX + DrawWidth / 2f, drawY + DrawHeight / 2f);
            g.ScaleTransform(-1, 1);
            g.DrawImage(frame, -DrawWidth / 2, -DrawHeight / 2, DrawWidth, DrawHeight);
        }

        g.Restore(state);
    }
}
