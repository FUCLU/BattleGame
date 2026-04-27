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
<<<<<<< HEAD
                    $"Khong the ket noi Server!\n{ex.Message}",
                    "Loi ket noi",
=======
                    $"Không thể kết nối Server!\n{ex.Message}",
                    "Lỗi kết nối",
>>>>>>> d463fd899256006a69dba5bd7180966dd0a7c34d
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
<<<<<<< HEAD
=======

>>>>>>> d463fd899256006a69dba5bd7180966dd0a7c34d
        }
    }
}