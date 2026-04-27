using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client
{
    internal static class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();
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

            Application.Run(new LoginForm());

        }
    }
}