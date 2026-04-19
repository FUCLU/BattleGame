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

            // Sửa logic để không bị báo Unreachable code
            bool isTestMode = true;

            try
            {
                if (isTestMode)
                {
                    Application.Run(new GameForm("samurai"));
                }
                else
                {
                    // ONLINE MODE 
                    try
                    {
                        await NetworkManager.Instance.ConnectAsync();
                        Application.Run(new LoginForm());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Không thể kết nối Server!\n{ex.Message}",
                            "Lỗi kết nối",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khởi động ứng dụng:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}