using BattleGame.Client.Game.Core.Components;
using System;
using System.Drawing;

namespace BattleGame.Client.Game.Gameplay;

public static class CharacterHitbox
{
    public const float Width = 100f;
    public const float Height = 100f;

    private const float HalfWidth = Width / 2f;
    private const float HalfHeight = Height / 2f;

    public static RectangleF GetBounds(MovementComponent movement)
    {
        return new RectangleF(
            movement.X - HalfWidth,
            movement.Y - HalfHeight,
            Width,
            Height);
    }

    public static float GetHorizontalGap(MovementComponent a, MovementComponent b)
    {
        float centerDistance = Math.Abs(a.X - b.X);
        return Math.Max(0f, centerDistance - Width);
    }

    public static bool IntersectsRectangle(MovementComponent movement, float centerX, float centerY, float width, float height)
    {
        float halfW = width / 2f;
        float halfH = height / 2f;

        return Math.Abs(movement.X - centerX) <= HalfWidth + halfW
            && Math.Abs(movement.Y - centerY) <= HalfHeight + halfH;
    }

    public static bool ContainsPoint(MovementComponent movement, float x, float y)
    {
        return x >= movement.X - HalfWidth
            && x <= movement.X + HalfWidth
            && y >= movement.Y - HalfHeight
            && y <= movement.Y + HalfHeight;
    }
}
