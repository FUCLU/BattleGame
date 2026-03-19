using BattleGame.Shared.Models;
using BattleGame.Server.Game;

// ─── Setup nhân vật ───────────────────────────────────
var warrior = new Character("Warrior", hp: 200, atk: 30, def: 10, speed: 8);
warrior.Skills.Add(Skill.CreateFixed("Slash", damage: 40, cooldown: 2));
warrior.Skills.Add(Skill.CreateFixed("Shield Bash", damage: 20, cooldown: 1));

var boss = new Character("Dark Boss", hp: 300, atk: 40, def: 5, speed: 6);
boss.Skills.Add(Skill.CreateHPScale("Death Mark", ratio: 0.25f, cooldown: 3));
boss.Skills.Add(Skill.CreateFixed("Dark Slash", damage: 55, cooldown: 2));

// ─── Bắt đầu trận ─────────────────────────────────────
var room = new GameRoom(warrior, boss);

Console.WriteLine("╔══════════════════════════════╗");
Console.WriteLine("║        BATTLE START          ║");
Console.WriteLine("╚══════════════════════════════╝");
Console.WriteLine($"  {warrior}");
Console.WriteLine($"  {boss}");
Console.WriteLine();

while (!room.IsBattleOver())
{
    Console.WriteLine($"┌─── Round {room.RoundCount + 1} ───────────────────┐");

    var results = room.RunRound();
    foreach (var r in results)
        Console.WriteLine("  " + r);

    Console.WriteLine($"  >> {warrior.Name}: {HPBar(warrior)}");
    Console.WriteLine($"  >> {boss.Name}: {HPBar(boss)}");
    Console.WriteLine();

    Thread.Sleep(300);
}

Console.WriteLine("╔══════════════════════════════╗");
Console.WriteLine("║         BATTLE OVER          ║");
Console.WriteLine("╚══════════════════════════════╝");

var winner = room.GetWinner();
Console.WriteLine(winner != null
    ? $"  🏆 Winner: {winner.Name}!"
    : "  Draw!");
Console.WriteLine($"  Total rounds : {room.RoundCount}");
Console.WriteLine($"  Total turns  : {room.BattleLog.Count}");

static string HPBar(Character c)
{
    int filled = (int)((float)c.CurrentHP / c.MaxHP * 20);
    string bar = new string('█', filled) + new string('░', 20 - filled);
    return $"[{bar}] {c.CurrentHP}/{c.MaxHP}";
}