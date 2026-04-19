using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;

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

        var animations = GetAnimationsForEntity(entity);
        if (!animations.TryGetValue(sp.CurrentAnimation, out var anim)) return;
        if (anim.Frames.Length == 0) return;

        var frameIndex = Math.Min(sp.CurrentFrame, anim.Frames.Length - 1);
        var frame = anim.Frames[frameIndex];
        var destinationRect = GetDestinationRect(mv, anim);

        var state = g.Save();

        if (mv.FacingRight)
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

    private static Rectangle GetDestinationRect(MovementComponent mv, SpriteAnimation anim)
    {
        int x = (int)MathF.Round(mv.X - DrawWidth / 2f);
        int y = (int)MathF.Round(mv.Y - DrawHeight + anim.OffsetY);
        return new Rectangle(x, y, DrawWidth, DrawHeight);
    }
}
