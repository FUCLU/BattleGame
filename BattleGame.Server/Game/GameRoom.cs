using System;
using System.Collections.Generic;
using BattleGame.Shared.Models;

namespace BattleGame.Server.Game
{
    public class GameRoom
    {
        private readonly BattleEngine _engine = new();

        public Character Player1 { get; }
        public Character Player2 { get; }
        public int RoundCount { get; private set; } = 0;
        public List<TurnResult> BattleLog { get; } = new();

        public GameRoom(Character p1, Character p2)
        {
            Player1 = p1 ?? throw new ArgumentNullException(nameof(p1));
            Player2 = p2 ?? throw new ArgumentNullException(nameof(p2));
        }

        public List<TurnResult> RunRound()
        {
            if (IsBattleOver())
                return new List<TurnResult>();

            RoundCount++;
            var results = new List<TurnResult>();

            // ⚠️ HIỆN TẠI: chưa có Speed → Player1 đánh trước
            var first = Player1;
            var second = Player2;

            // ===== TURN 1 =====
            var t1 = _engine.ProcessTurn(first, second);
            results.Add(t1);
            BattleLog.Add(t1);

            // ===== TURN 2 =====
            if (!second.IsDead())
            {
                var t2 = _engine.ProcessTurn(second, first);
                results.Add(t2);
                BattleLog.Add(t2);
            }

            return results;
        }

        public bool IsBattleOver()
        {
            return Player1.IsDead() || Player2.IsDead();
        }

        public Character? GetWinner()
        {
            if (Player1.IsDead() && Player2.IsDead()) return null;
            if (!Player1.IsDead()) return Player1;
            if (!Player2.IsDead()) return Player2;
            return null;
        }
    }
}