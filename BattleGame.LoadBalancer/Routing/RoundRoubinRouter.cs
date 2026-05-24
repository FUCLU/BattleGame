using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BattleGame.LoadBalancer.Logging;
using BattleGame.LoadBalancer.Registry;

namespace BattleGame.LoadBalancer.Routing
{
    public class RoundRoubinRouter
    {
        private readonly RedisBackendRegistry _registry;
        private int index = 0;
        private readonly object lockObj = new object();

        public RoundRoubinRouter(RedisBackendRegistry registry)
        {
            _registry = registry;
        }

        public Task RegisterAsync(ServerEndpoint endpoint)
        {
            if (!endpoint.IsValid())
                return Task.CompletedTask;

            return _registry.UpsertAsync(endpoint, true);
        }

        public async Task SetHealthAsync(ServerEndpoint endpoint, bool isHealthy)
        {
            await _registry.SetHealthAsync(endpoint, isHealthy);
            LbLogger.Event("health", isHealthy ? "up" : "down",
                ("serverId", endpoint.ServerId),
                ("endpoint", endpoint.ToString()));
        }

        public async Task<ServerEndpoint?> GetNextAsync(int? userId = null, int? roomId = null)
        {
            var active = await _registry.GetHealthyAsync();
            string? preferredServerId = await _registry.ResolvePreferredServerIdAsync(userId, roomId);
            lock (lockObj)
            {
                if (active.Count == 0)
                    return null;
                if (!string.IsNullOrWhiteSpace(preferredServerId))
                {
                    var preferred = active.FirstOrDefault(s => s.ServerId == preferredServerId);
                    if (preferred != null)
                        return preferred;

                    if (roomId.HasValue && roomId.Value > 0)
                    {
                        LbLogger.Warn(
                            $"room {roomId.Value} belongs to unavailable server {preferredServerId}; refusing fallback route",
                            "routing");
                        return null;
                    }
                }
                index = index % active.Count;
                return active[index++];
            }
        }

        public Task<List<ServerEndpoint>> GetAllAsync()
        {
            return _registry.GetAllAsync();
        }

        public async Task<int> ActiveCountAsync()
        {
            var active = await _registry.GetHealthyAsync();
            return active.Count;
        }
    }
}
