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

        // Theo dõi phím trigger một lần (attack, skill)
        private bool _prevJ, _prevU, _prevI;

        public PlayerController(Entity entity, CombatSystem combat)
        {
            _entity = entity;
            _combat = combat;
        }

        public void Update()
        {
            var mv = _entity.Get<MovementComponent>();
            var ch = _entity.Get<CharacterComponent>();

            if (ch.IsDead) return;

            // --- Di chuyển (held) ---
            mv.VelocityX = 0;
            if (!ch.IsHurt && !ch.IsStunned && !ch.IsDead && !ch.IsBusy && !ch.IsProtecting)
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

            // --- Đỡ đòn (held) ---
            ch.IsProtecting = InputManager.IsKeyDown(Keys.S) && !ch.IsBusy;

            // --- Attack / Skill (trigger một lần khi nhấn xuống) ---
            bool curJ = InputManager.IsKeyDown(Keys.J);
            bool curU = InputManager.IsKeyDown(Keys.U);
            bool curI = InputManager.IsKeyDown(Keys.I);

            if (curJ && !_prevJ) _combat.Attack(_entity);
            if (curU && !_prevU) _combat.UseSkill(_entity, 1);
            if (curI && !_prevI) _combat.UseSkill(_entity, 2);

            _prevJ = curJ;
            _prevU = curU;
            _prevI = curI;
        }
    }
}