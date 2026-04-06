using BattleGame.Shared.Config;

namespace BattleGame.Client.Config
{
    public class ClientConfig
    {
        public string ServerIP { get; private set; } = GameConstants.ServerHost;
        public int ServerPort { get; private set; } = GameConstants.ServerPort;
    }
}