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
    private const int HealthBarWidth = 72;
    private const int HealthBarHeight = 8;
    private const int HealthBarOffsetY = 14;

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
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim))
        {
            if (!string.Equals(sp.CurrentAnimation, "Dash", StringComparison.OrdinalIgnoreCase)
                || !animations.TryGetValue("Run", out anim))
                return;
        }
        if (anim.Frames.Length == 0) return;
        sp.CurrentAnimationFrameCount = anim.Frames.Length;

        bool wasFinished = sp.AnimationFinished;
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

        if (!anim.Loop && sp.CurrentFrame == anim.Frames.Length - 1 && wasFinished)
            sp.AnimationFinished = true;
    }

    public void Draw(Graphics g, Entity entity)
    {
        var sp = entity.Get<SpriteComponent>();
        var mv = entity.Get<MovementComponent>();
        var ch = entity.Get<CharacterComponent>();
        bool renderFacingRight = ch.Render.InvertFacing ? !mv.FacingRight : mv.FacingRight;

        var animations = GetAnimationsForEntity(entity);
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim))
        {
            if (!string.Equals(sp.CurrentAnimation, "Dash", StringComparison.OrdinalIgnoreCase)
                || !animations.TryGetValue("Run", out anim))
                return;
        }
        if (anim.Frames.Length == 0) return;

        var frameIndex = Math.Min(sp.CurrentFrame, anim.Frames.Length - 1);
        var frame = anim.Frames[frameIndex];
        var destinationRect = GetDestinationRect(mv, frame, anim, ch.Render.Scale, ch.Render.OffsetY, ch.Render.UseSpriteSize, renderFacingRight);

        // Protection should wrap around the character, so keep Idle as the base layer.
        if (string.Equals(sp.CurrentAnimation, "Protection", StringComparison.OrdinalIgnoreCase)
            && ch.Render.ProtectionUsesIdleBase
            && animations.TryGetValue("Idle", out var idleAnim)
            && idleAnim.Frames.Length > 0)
        {
            int idleFrameIndex = (int)MathF.Floor((float)sp.CurrentFrame / Math.Max(1, anim.Frames.Length) * idleAnim.Frames.Length);
            idleFrameIndex = Math.Clamp(idleFrameIndex, 0, idleAnim.Frames.Length - 1);

            var idleFrame = idleAnim.Frames[idleFrameIndex];
            var idleRect = GetDestinationRect(mv, idleFrame, idleAnim, ch.Render.Scale, ch.Render.OffsetY, ch.Render.UseSpriteSize, renderFacingRight);
            var protectionRect = GetDestinationRect(
                mv,
                frame,
                anim,
                ch.Render.Scale,
                ch.Render.OffsetY + ch.Render.ProtectionOverlayOffsetY,
                ch.Render.UseSpriteSize,
                renderFacingRight);

            DrawFrame(g, idleFrame, idleRect, renderFacingRight);
            DrawFrame(g, frame, protectionRect, renderFacingRight);
            return;
        }

        DrawFrame(g, frame, destinationRect, renderFacingRight);
    }

    public void DrawHealthBar(Graphics g, Entity entity)
    {
        var sp = entity.Get<SpriteComponent>();
        var mv = entity.Get<MovementComponent>();
        var ch = entity.Get<CharacterComponent>();
        bool renderFacingRight = ch.Render.InvertFacing ? !mv.FacingRight : mv.FacingRight;

        var animations = GetAnimationsForEntity(entity);
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim))
        {
            if (!string.Equals(sp.CurrentAnimation, "Dash", StringComparison.OrdinalIgnoreCase)
                || !animations.TryGetValue("Run", out anim))
                return;
        }
        if (anim.Frames.Length == 0 || ch.BaseStats.Hp <= 0) return;

        var frameIndex = Math.Min(sp.CurrentFrame, anim.Frames.Length - 1);
        var frame = anim.Frames[frameIndex];
        var destinationRect = GetDestinationRect(mv, frame, anim, ch.Render.Scale, ch.Render.OffsetY, ch.Render.UseSpriteSize, renderFacingRight);

        float ratio = Math.Clamp(ch.Hp / (float)ch.BaseStats.Hp, 0f, 1f);
        int x = destinationRect.Left + (destinationRect.Width - HealthBarWidth) / 2;
        int y = Math.Max(6, destinationRect.Top - HealthBarOffsetY);
        var outerRect = new Rectangle(x - 1, y - 1, HealthBarWidth + 2, HealthBarHeight + 2);
        var backRect = new Rectangle(x, y, HealthBarWidth, HealthBarHeight);
        var fillRect = new Rectangle(x, y, (int)MathF.Round(HealthBarWidth * ratio), HealthBarHeight);

        using var borderBrush = new SolidBrush(Color.FromArgb(220, 20, 20, 20));
        using var backBrush = new SolidBrush(Color.FromArgb(190, 80, 30, 30));
        using var fillBrush = new SolidBrush(Color.FromArgb(230, 210, 38, 38));

        g.FillRectangle(borderBrush, outerRect);
        g.FillRectangle(backBrush, backRect);
        if (fillRect.Width > 0)
            g.FillRectangle(fillBrush, fillRect);
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

    private static Rectangle GetDestinationRect(
        MovementComponent mv,
        Image frame,
        SpriteAnimation anim,
        float scale,
        float extraOffsetY,
        bool useSpriteSize,
        bool facingRight)
    {
        int baseWidth = useSpriteSize ? frame.Width : DrawWidth;
        int baseHeight = useSpriteSize ? frame.Height : DrawHeight;
        int width = (int)MathF.Round(baseWidth * scale);
        int height = (int)MathF.Round(baseHeight * scale);
        float directionalOffsetX = facingRight ? anim.OffsetX : -anim.OffsetX;
        int x = (int)MathF.Round(mv.X - width / 2f + directionalOffsetX * scale);
        int y = (int)MathF.Round(mv.Y - height + extraOffsetY + anim.OffsetY * scale);
        return new Rectangle(x, y, width, height);
    }
}
