using System;
using System.Drawing;
using System.Windows.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client.Forms
{
    internal static class AudioSettingsButton
    {
        public static Button Attach(Form form, Button? existingButton = null)
        {
            bool useDesignerStyle = existingButton != null;
            Button button = existingButton ?? new Button();
            button.Tag = "AudioSettings";
            button.Cursor = Cursors.Hand;

            if (!useDesignerStyle)
            {
                button.Name = "btnAudioSettings";
                button.Text = string.Empty;
                button.Size = new Size(48, 48);
                button.Location = new Point(form.ClientSize.Width - button.Width - 24, form.ClientSize.Height - button.Height - 22);
                button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = Color.FromArgb(44, 74, 110);
                button.ForeColor = Color.FromArgb(255, 235, 156);
                button.Font = new Font("Courier New", 12F, FontStyle.Bold);
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.FlatAppearance.BorderSize = 2;
                button.FlatAppearance.BorderColor = Color.PaleTurquoise;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(57, 100, 150);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(31, 47, 86);
            }

            button.Click -= OpenAudioSettings;
            button.Click += OpenAudioSettings;

            SoundManager.SettingsChanged -= SettingsChanged;
            SoundManager.SettingsChanged += SettingsChanged;
            button.Invalidate();

            if (existingButton == null && !form.Controls.Contains(button))
                form.Controls.Add(button);

            button.BringToFront();
            return button;
        }

        private static void OpenAudioSettings(object? sender, EventArgs e)
        {
            if (sender is not Control control)
                return;

            Form? owner = control.FindForm();
            using var settings = new AudioSettingsForm();
            if (owner != null && !owner.IsDisposed)
                settings.ShowDialog(owner);
            else
                settings.ShowDialog();
        }

        private static void SettingsChanged(object? sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                foreach (Control control in form.Controls)
                {
                    if (control is Button button && string.Equals(button.Tag as string, "AudioSettings", StringComparison.Ordinal))
                        button.Invalidate();
                }
            }
        }
    }
}
