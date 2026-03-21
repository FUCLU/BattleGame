using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Server.Config
{
    public class ServerConfig
    {
        public int Port { get; set; } = 9000;
    }

    public class SmtpConfig
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 1025;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromName { get; set; } = "BattleGame";
        public bool EnableSsl { get; set; } = false;
    }
}