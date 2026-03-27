using BattleGame.Shared.Config;

namespace BattleGame.Client.Config
{
    public class ClientConfig
    {
        public string ServerIp { get; private set; } = GameConstants.SERVER_HOST;
        public int ServerPort { get; private set; } = GameConstants.SERVER_PORT;
    }
}