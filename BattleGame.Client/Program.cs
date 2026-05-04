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
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                NetworkManager.Instance.ConnectAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Cho phép offline mode nếu không thể kết nối server
                Console.WriteLine($"[Program] Server connection failed: {ex.Message}. Allowing offline mode.");
                // MessageBox.Show(
                //     $"Không thể kết nối Server!\n{ex.Message}",
                //     "Lỗi kết nối",
                //     MessageBoxButtons.OK,
                //     MessageBoxIcon.Error);
                // return;
            }

            Application.Run(new LoginForm());
        }
    }
}