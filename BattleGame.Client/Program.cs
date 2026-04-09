using BattleGame.Client.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Tạm thời mở GameForm để test
                Application.Run(new Forms.GameForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chạy GameForm:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                                "Lỗi khởi động",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }

}