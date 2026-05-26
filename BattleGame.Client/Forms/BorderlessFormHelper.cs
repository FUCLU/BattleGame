using System.Runtime.InteropServices;

namespace BattleGame.Client.Forms
{
    internal static class BorderlessFormHelper
    {
        private const int WmNclButtonDown = 0xA1;
        private const int HtCaption = 0x2;

        public static void Apply(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            AudioSettingsButton.AttachExistingButtons(form);
            form.MouseDown -= DragForm;
            form.MouseDown += DragForm;
        }

        private static void DragForm(object? sender, MouseEventArgs e)
        {
            if (sender is not Form form || e.Button != MouseButtons.Left)
                return;

            ReleaseCapture();
            SendMessage(form.Handle, WmNclButtonDown, HtCaption, 0);
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
    }
}
