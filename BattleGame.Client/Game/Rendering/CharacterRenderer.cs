using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using System;

namespace BattleGame.Client.Game.Rendering;

public class CharacterRenderer
{
    private readonly Dictionary<string, SpriteAnimation> _playerAnimations;
    private readonly Dictionary<string, SpriteAnimation> _enemyAnimations;
    private readonly int _playerId;
    private const int DrawWidth = 128;
    private const int DrawHeight = 128;

    public CharacterRenderer(int playerId, Dictionary<string, SpriteAnimation> playerAnimations, Dictionary<string, SpriteAnimation> enemyAnimations)
    {
        _playerId = playerId;
        _playerAnimations = playerAnimations;
        _enemyAnimations = enemyAnimations;
    }

    private Dictionary<string, SpriteAnimation> GetAnimationsForEntity(Entity entity)
        => entity.Id == _playerId ? _playerAnimations : _enemyAnimations;

    public void Update(Entity entity, float deltaTime)
    {
        var sp = entity.Get<SpriteComponent>();
        var animations = GetAnimationsForEntity(entity);
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim)) return;
        if (anim.Frames.Length == 0) return;
        sp.CurrentAnimationFrameCount = anim.Frames.Length;

        // Reset AnimationFinished mỗi frame
        sp.AnimationFinished = false;

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
                sp.AnimationFinished = true;
                if (anim.Loop) sp.CurrentFrame = 0;
            }
        }
    }

    public void Draw(Graphics g, Entity entity)
    {
        var sp = entity.Get<SpriteComponent>();
        var mv = entity.Get<MovementComponent>();
        var ch = entity.Get<CharacterComponent>();

        var animations = GetAnimationsForEntity(entity);
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim)) return;
        if (anim.Frames.Length == 0) return;

        var frameIndex = Math.Min(sp.CurrentFrame, anim.Frames.Length - 1);
        var frame = anim.Frames[frameIndex];
        var destinationRect = GetDestinationRect(mv, anim, ch.Render.Scale, ch.Render.OffsetY);

        // Protection should wrap around the character, so keep Idle as the base layer.
        if (string.Equals(sp.CurrentAnimation, "Protection", StringComparison.OrdinalIgnoreCase)
            && ch.Render.ProtectionUsesIdleBase
            && animations.TryGetValue("Idle", out var idleAnim)
            && idleAnim.Frames.Length > 0)
        {
            int idleFrameIndex = (int)MathF.Floor((float)sp.CurrentFrame / Math.Max(1, anim.Frames.Length) * idleAnim.Frames.Length);
            idleFrameIndex = Math.Clamp(idleFrameIndex, 0, idleAnim.Frames.Length - 1);

            var idleFrame = idleAnim.Frames[idleFrameIndex];
            var idleRect = GetDestinationRect(mv, idleAnim, ch.Render.Scale, ch.Render.OffsetY);
            var protectionRect = GetDestinationRect(
                mv,
                anim,
                ch.Render.Scale,
                ch.Render.OffsetY + ch.Render.ProtectionOverlayOffsetY);

            DrawFrame(g, idleFrame, idleRect, mv.FacingRight);
            DrawFrame(g, frame, protectionRect, mv.FacingRight);
            return;
        }

        DrawFrame(g, frame, destinationRect, mv.FacingRight);
    }

    private static void DrawFrame(Graphics g, Image frame, Rectangle destinationRect, bool facingRight)
    {
        var state = g.Save();

        if (facingRight)
        {
            g.DrawImage(frame, destinationRect);
        }
        else
        {
            g.TranslateTransform(destinationRect.X + destinationRect.Width / 2f, destinationRect.Y + destinationRect.Height / 2f);
            g.ScaleTransform(-1, 1);
            g.DrawImage(frame, -destinationRect.Width / 2, -destinationRect.Height / 2, destinationRect.Width, destinationRect.Height);
        }

        g.Restore(state);
    }

    private static Rectangle GetDestinationRect(MovementComponent mv, SpriteAnimation anim, float scale, float extraOffsetY)
    {
        int width = (int)MathF.Round(DrawWidth * scale);
        int height = (int)MathF.Round(DrawHeight * scale);
        int x = (int)MathF.Round(mv.X - width / 2f);
        int y = (int)MathF.Round(mv.Y - height + anim.OffsetY + extraOffsetY);
        return new Rectangle(x, y, width, height);
    }
}
