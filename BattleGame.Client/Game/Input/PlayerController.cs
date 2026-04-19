using BattleGame.Client.Game.Core;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Client.Game.Systems;
using BattleGame.Client.Managers;
using System.Windows.Forms;

namespace BattleGame.Client.Game.Input
{
    public class PlayerController
    {
        private readonly Entity _entity;
        private readonly CombatSystem _combat;
        private readonly Entity _target;

        // Theo dõi phím trigger một lần (attack, skill)
        private bool _prevJ, _prevU, _prevI;

        public PlayerController(Entity entity, Entity target, CombatSystem combat)
        {
            _entity = entity;
            _target = target;
            _combat = combat;
        }

        public void Update()
        {
            var mv = _entity.Get<MovementComponent>();
            var ch = _entity.Get<CharacterComponent>();

            if (ch.IsDead) return;

            // ===== MOVEMENT =====
            mv.VelocityX = 0;

            if (!ch.IsBusy && !ch.IsProtecting)
            {
                if (InputManager.IsKeyDown(Keys.A))
                {
                    mv.VelocityX = -mv.Speed;
                    mv.FacingRight = false;
                }
                else if (InputManager.IsKeyDown(Keys.D))
                {
                    mv.VelocityX = mv.Speed;
                    mv.FacingRight = true;
                }
            }

            // ===== BLOCK =====
            ch.IsProtecting = InputManager.IsKeyDown(Keys.S) && !ch.IsBusy;

            // ===== INPUT TRIGGER =====
            bool curJ = InputManager.IsKeyDown(Keys.J);
            bool curU = InputManager.IsKeyDown(Keys.U);
            bool curI = InputManager.IsKeyDown(Keys.I);

            // ===== ATTACK =====
            if (curJ && !_prevJ && !ch.IsBusy)
            {
                _combat.Attack(_entity);
            }

            // ===== SKILL 1 (DAMAGE) =====
            if (curU && !_prevU && !ch.IsBusy)
            {
                _combat.UseSkill(_entity, 1);
            }

            // ===== SKILL 2 (STUN) =====
            if (curI && !_prevI && !ch.IsBusy)
            {
                _combat.UseSkill(_entity, 2);
            }

            _prevJ = curJ;
            _prevU = curU;
            _prevI = curI;
        }
    }
}