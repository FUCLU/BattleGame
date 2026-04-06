using BattleGame.Shared.Config;
using System;

namespace BattleGame.Shared.Config
{
    public static class GameConstants
    {
        public const int ServerPort = 9001;
        public const int TickRateMs = 50;
        public const int MaxPlayers = 100;
        public const int OtpExpireSeconds = 300;
        public const int OtpMaxRetry = 3;
        public const int OtpCooldownSeconds = 60;
        public const string ServerHost = "localhost";
    }
}