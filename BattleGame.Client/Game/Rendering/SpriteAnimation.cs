namespace BattleGame.Client.Game.Rendering;

public class SpriteAnimation
{
    public string Name { get; init; } = "";
    public Bitmap[] Frames { get; init; } = [];
    public float Fps { get; init; } = 10f;
    public bool Loop { get; init; } = true;
    public float OffsetX { get; init; } = 0f;
    public float OffsetY { get; init; } = 0f;

    public float FrameDuration => 1f / Fps;
}
