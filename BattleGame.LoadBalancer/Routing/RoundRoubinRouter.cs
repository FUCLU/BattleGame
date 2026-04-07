using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.LoadBalancer.Routing
{
    public class RoundRoubinRouter
    {
        private readonly List<ServerEndpoint> servers = new List<ServerEndpoint>();
        private int index = 0;
        private readonly object lockObj = new object();

        public void Register(ServerEndpoint endpoint)
        {
            lock (lockObj)
            {
                if (!servers.Any(s => s.Host == endpoint.Host && s.Port == endpoint.Port))
                {
                    servers.Add(endpoint);
                }
            }
        }

        public void Remove(ServerEndpoint endpoint)
        {
            lock (lockObj)
            {
                servers.RemoveAll(s => s.Host == endpoint.Host && s.Port == endpoint.Port);
            }
        }

        public ServerEndpoint? GetNext()
        {
            lock (lockObj)
            {
                if (servers.Count == 0) return null;
                index = index % servers.Count;
                return servers[index++];
            }
        }

        public List<ServerEndpoint> GetAll()
        {
            lock (lockObj)
            {
                return new List<ServerEndpoint>(servers);
            }
        }
    }
}
