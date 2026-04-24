using BattleGame.Client.Forms;
using BattleGame.Client.Game;
using BattleGame.Client.Game.Characters;
using BattleGame.Client.Managers;

namespace BattleGame.Client
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();

            // 🔥 SWITCH MODE 
            bool isTestMode = false; // 👉 bật/tắt ở đây

            if (isTestMode)
            {
                // TEST MODE (OFFLINE) 
                Application.Run(new GameForm(new Soldier()));
                return;
            }

            // ONLINE MODE 
            try
            {
                await NetworkManager.Instance.ConnectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể kết nối Server!\n{ex.Message}",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MatchHistoryForm());
        }
    }
}