namespace BattleGame.Client.Game.Core.Components;

public class SpriteComponent : IComponent
{
    public string CurrentAnimation { get; set; } = "Idle";
    public int CurrentFrame { get; set; } = 0;
    public int CurrentAnimationFrameCount { get; set; } = 0;
    public float FrameTimer { get; set; } = 0f;
    public bool LoopAnimation { get; set; } = true;
    public bool AnimationFinished { get; set; } = false;
}