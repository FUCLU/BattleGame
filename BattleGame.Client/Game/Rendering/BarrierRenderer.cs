using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Models;
using System;
using System.Collections.Generic;

namespace BattleGame.Client.Game.Rendering;

public class BarrierRenderer
{
    private readonly Dictionary<string, SpriteAnimation> _animations;
    private const int DefaultDrawWidth = 80;
    private const int DefaultDrawHeight = 80;

    public BarrierRenderer(Dictionary<string, SpriteAnimation> animations)
    {
        _animations = animations;
    }

    public void Update(Entity barrier, float deltaTime)
    {
        var bc = barrier.Get<BarrierComponent>();
        var sp = barrier.Get<SpriteComponent>();

        if (!_animations.TryGetValue(bc.AnimationKey, out var anim)) 
            return;

        if (anim.Frames.Length == 0) 
            return;

        sp.FrameTimer += deltaTime;
        while (sp.FrameTimer >= anim.FrameDuration)
        {
            sp.FrameTimer -= anim.FrameDuration;

            if (sp.CurrentFrame < anim.Frames.Length - 1)
            {
                sp.CurrentFrame++;
            }
            else
            {
                if (anim.Loop) 
                    sp.CurrentFrame = 0;
            }
        }
    }

    public void Draw(Graphics g, Entity barrier)
    {
        var bc = barrier.Get<BarrierComponent>();
        var sp = barrier.Get<SpriteComponent>();

        if (!_animations.TryGetValue(bc.AnimationKey, out var anim)) 
            return;

        if (anim.Frames.Length == 0) 
            return;

        var frameIndex = System.Math.Min(sp.CurrentFrame, anim.Frames.Length - 1);
        var frame = anim.Frames[frameIndex];

        int baseWidth = bc.Render.UseSpriteSize ? frame.Width : DefaultDrawWidth;
        int baseHeight = bc.Render.UseSpriteSize ? frame.Height : DefaultDrawHeight;
        int drawWidth = (int)MathF.Round(baseWidth * bc.Render.Scale);
        int drawHeight = (int)MathF.Round(baseHeight * bc.Render.Scale);

        int x = (int)MathF.Round(bc.X + bc.Render.OffsetX - drawWidth / 2f);
        int y = ResolveDrawY(bc.Y, drawHeight, bc.Render);
        var state = g.Save();

        if (!bc.FacingRight)
        {
            g.DrawImage(frame, x, y, drawWidth, drawHeight);
        }
        else
        {
            g.TranslateTransform(x + drawWidth / 2f, y + drawHeight / 2f);
            g.ScaleTransform(-1, 1);
            g.DrawImage(frame, -drawWidth / 2, -drawHeight / 2, drawWidth, drawHeight);
        }

        g.Restore(state);
    }

    private static int ResolveDrawY(float y, int drawHeight, EffectRenderData render)
    {
        float finalY = (render.AlignY ?? "center").Trim().ToLowerInvariant() switch
        {
            "bottom" => y + render.OffsetY - drawHeight,
            "top" => y + render.OffsetY,
            _ => y + render.OffsetY - drawHeight / 2f
        };

        return (int)MathF.Round(finalY);
    }
}
