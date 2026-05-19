using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Simulation;
using System;
using System.Drawing;

namespace BattleGame.Client.Game.Gameplay;

public static class CharacterHitbox
{
    public const float Width = BattleHitbox.CharacterWidth;
    public const float Height = BattleHitbox.CharacterHeight;

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
        return BattleHitbox.GetHorizontalGap(a.X, b.X);
    }

    public static bool IntersectsRectangle(MovementComponent movement, float centerX, float centerY, float width, float height)
    {
        return BattleHitbox.IntersectsRectangle(movement.X, movement.Y, centerX, centerY, width, height);
    }

    public static bool ContainsPoint(MovementComponent movement, float x, float y)
    {
        return BattleHitbox.ContainsPoint(movement.X, movement.Y, x, y);
    }
}
