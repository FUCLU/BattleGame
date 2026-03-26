using BattleGame.Shared.Config;
using System;

namespace BattleGame.Shared.Config
{
    public static class GameConstants
    {
        public const int SERVER_PORT = 9000;
        // Docker Desktop (Windows/Mac): localhost hoặc 127.0.0.1 (container port init map tới host)
        // Linux với Docker (non-desktop): "172.17.0.1" hoặc service name "server"
        // LAN: IP của máy chạy Docker
        public const string SERVER_HOST = "localhost"; //localhost, thay thành v4 chạy lan
        public const int BUFFER_SIZE = 4096;
        public const int TICK_RATE = 50; 
    }
}