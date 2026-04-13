using System;
using System.Collections.Generic;
using BattleGame.Shared.Models;

using BattleGame.Shared.Models;

namespace BattleGame.Server.Game;

/// <summary>
/// Quản lý 1 trận đấu hoàn chỉnh giữa 2 nhân vật
/// </summary>
public class GameRoom
{
<<<<<<< HEAD
    private readonly BattleEngine _engine = new();

    public Character Player1 { get; }
    public Character Player2 { get; }
    public int RoundCount { get; private set; } = 0;
    public List<TurnResult> BattleLog { get; } = new();

    public GameRoom(Character p1, Character p2)
    {
        Player1 = p1;
        Player2 = p2;
    }

    /// <summary>
    /// Chạy 1 round: cả 2 bên đánh nhau theo thứ tự Speed
    /// </summary>
    public List<TurnResult> RunRound()
    {
        RoundCount++;
        var results = new List<TurnResult>();

        // Ai Speed cao hơn đánh trước
        var (first, second) = Player1.Speed >= Player2.Speed
            ? (Player1, Player2)
            : (Player2, Player1);

        var t1 = _engine.ProcessTurn(first, second);
        results.Add(t1);
        BattleLog.Add(t1);

        // Chỉ đánh lại nếu còn sống
        if (second.IsAlive)
        {
            var t2 = _engine.ProcessTurn(second, first);
            results.Add(t2);
            BattleLog.Add(t2);
        }

        return results;
    }

    public bool IsBattleOver() => !Player1.IsAlive || !Player2.IsAlive;

    public Character? GetWinner() =>
        Player1.IsAlive ? Player1 :
        Player2.IsAlive ? Player2 : null;
=======
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
>>>>>>> 09d9af6a210382cf282f9c367fb18f97f0c14841
}