namespace BattleGame.Shared.Simulation;

public static class BattleHitbox
{
    public const float CharacterWidth = 100f;
    public const float CharacterHeight = 100f;

    private const float HalfCharacterWidth = CharacterWidth / 2f;
    private const float HalfCharacterHeight = CharacterHeight / 2f;

    public static float GetHorizontalGap(float firstX, float secondX)
    {
        float centerDistance = Math.Abs(firstX - secondX);
        return Math.Max(0f, centerDistance - CharacterWidth);
    }

    public static bool ContainsPoint(float characterX, float characterY, float pointX, float pointY)
    {
        return pointX >= characterX - HalfCharacterWidth
            && pointX <= characterX + HalfCharacterWidth
            && pointY >= characterY - HalfCharacterHeight
            && pointY <= characterY + HalfCharacterHeight;
    }

    public static bool IntersectsRectangle(
        float characterX,
        float characterY,
        float rectangleCenterX,
        float rectangleCenterY,
        float rectangleWidth,
        float rectangleHeight)
    {
        return Math.Abs(characterX - rectangleCenterX) <= HalfCharacterWidth + rectangleWidth / 2f
            && Math.Abs(characterY - rectangleCenterY) <= HalfCharacterHeight + rectangleHeight / 2f;
    }
}
