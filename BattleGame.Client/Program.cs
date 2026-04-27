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
                    $"Khong the ket noi Server!\n{ex.Message}",
                    "Loi ket noi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
        }
    }
}