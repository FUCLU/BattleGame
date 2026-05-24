using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Systems;
using BattleGame.Client.Managers;
using System.Linq;
using System.Windows.Forms;

namespace BattleGame.Client.Game.Input
{
    public class PlayerController
    {
        private readonly Entity _entity;
        private readonly CombatSystem _combat;
        private readonly Entity _target;
        private readonly Keys[] _leftKeys;
        private readonly Keys[] _rightKeys;
        private readonly Keys[] _blockKeys;
        private readonly Keys[] _attackKeys;
        private readonly Keys[] _skill1Keys;
        private readonly Keys[] _skill2Keys;
        private readonly Keys[] _dashKeys;

        private bool _prevAttack;
        private bool _prevSkill1;
        private bool _prevSkill2;
        private bool _prevDash;

        public PlayerController(
            Entity entity,
            Entity target,
            CombatSystem combat,
            Keys leftKey = Keys.A,
            Keys rightKey = Keys.D,
            Keys blockKey = Keys.S,
            Keys attackKey = Keys.J,
            Keys skill1Key = Keys.U,
            Keys skill2Key = Keys.I,
            Keys dashKey = Keys.K)
        {
            _entity = entity;
            _target = target;
            _combat = combat;
            _leftKeys = CreateAliases(leftKey);
            _rightKeys = CreateAliases(rightKey);
            _blockKeys = CreateAliases(blockKey);
            _attackKeys = CreateAliases(attackKey);
            _skill1Keys = CreateAliases(skill1Key);
            _skill2Keys = CreateAliases(skill2Key);
            _dashKeys = CreateAliases(dashKey);
        }

        public void Update()
        {
            var mv = _entity.Get<MovementComponent>();
            var ch = _entity.Get<CharacterComponent>();

            if (ch.IsDead)
                return;

            mv.VelocityX = 0;

            if (!ch.IsBusy && !ch.IsProtecting)
            {
                if (IsAnyKeyDown(_leftKeys))
                {
                    mv.VelocityX = -mv.Speed;
                    mv.FacingRight = false;
                }
                else if (IsAnyKeyDown(_rightKeys))
                {
                    mv.VelocityX = mv.Speed;
                    mv.FacingRight = true;
                }
            }

            ch.IsProtecting = IsAnyKeyDown(_blockKeys) && !ch.IsBusy;

            bool curAttack = IsAnyKeyDown(_attackKeys);
            bool curSkill1 = IsAnyKeyDown(_skill1Keys);
            bool curSkill2 = IsAnyKeyDown(_skill2Keys);
            bool curDash = IsAnyKeyDown(_dashKeys);

            if (curAttack && !_prevAttack && !ch.IsBusy)
                _combat.Attack(_entity);

            if (curSkill1 && !_prevSkill1 && !ch.IsBusy)
                _combat.UseSkill(_entity, 1);

            if (curSkill2 && !_prevSkill2 && !ch.IsBusy)
                _combat.UseSkill(_entity, 2);

            if (curDash && !_prevDash && !ch.IsBusy && !ch.IsProtecting)
            {
                string dashAnimation = ResolveDashAnimation(ch);
                if (!string.IsNullOrWhiteSpace(dashAnimation))
                {
                    float duration = ch.DashDuration > 0f ? ch.DashDuration : 0.22f;
                    ch.IsDashing = true;
                    ch.DashTimer = duration;
                    ch.ActionTimer = duration;
                    ch.ActionDuration = duration;
                    mv.VelocityX = (mv.FacingRight ? 1 : -1) * mv.Speed * ch.DashSpeedMultiplier;

                    var sp = _entity.Get<SpriteComponent>();
                    sp.CurrentAnimation = dashAnimation;
                    sp.CurrentFrame = 0;
                    sp.FrameTimer = 0f;
                    sp.AnimationFinished = false;
                }
            }

            _prevAttack = curAttack;
            _prevSkill1 = curSkill1;
            _prevSkill2 = curSkill2;
            _prevDash = curDash;
        }

        private static string ResolveDashAnimation(CharacterComponent ch)
        {
            if (ch.AvailableAnimations.Contains("Dash"))
                return "Dash";
            if (ch.AvailableAnimations.Contains("Run"))
                return "Run";
            if (ch.AvailableAnimations.Contains("Walk"))
                return "Walk";

            return string.Empty;
        }

        private static bool IsAnyKeyDown(Keys[] keys)
            => keys.Any(InputManager.IsKeyDown);

        private static Keys[] CreateAliases(Keys key)
        {
            return key switch
            {
                Keys.NumPad1 => new[] { Keys.NumPad1, Keys.D1, Keys.End },
                Keys.NumPad2 => new[] { Keys.NumPad2, Keys.D2 },
                Keys.NumPad4 => new[] { Keys.NumPad4, Keys.D4 },
                Keys.NumPad5 => new[] { Keys.NumPad5, Keys.D5, Keys.Clear },
                _ => new[] { key }
            };
        }
    }
}
