using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.LoadBalancer
{
    public class ServerEndpoint
    {
        public string Host { get; set; } = string.Empty;      
        public string PublicHost { get; set; } = string.Empty; 
        public int Port { get; set; }
        public int PublicPort { get; set; }

        public string ToPublicString() => $"{PublicHost}:{PublicPort}";
        public override string ToString() => $"{Host}:{Port}";
    }
    public class LoadBalancerConfig
    {
        public int Port { get; set; } = 9000;
        public int HealthCheckIntervalSeconds { get; set; } = 5;
        public List<ServerEndpoint> Servers { get; set; } = new List<ServerEndpoint>(); 

    }
}
