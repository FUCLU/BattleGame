using System;
using System.Drawing;
using System.Windows.Forms;
using BattleGame.Client.Managers;

namespace BattleGame.Client.Forms
{
    internal static class AudioSettingsButton
    {
        public static void AttachExistingButtons(Form form)
        {
            foreach (Button button in EnumerateAudioButtons(form.Controls))
                Attach(form, button);
        }

        public static Button Attach(Form form, Button? existingButton = null)
        {
            Button button = existingButton ?? new Button();
            if (existingButton == null)
            {
                ApplyAppearance(button);
                ApplyDefaultLayout(form, button);
            }
            else
            {
                button.Tag = "AudioSettings";
                button.Cursor = Cursors.Hand;
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

        private static IEnumerable<Button> EnumerateAudioButtons(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button button && IsAudioButton(button))
                    yield return button;

                foreach (Button childButton in EnumerateAudioButtons(control.Controls))
                    yield return childButton;
            }
        }

        private static bool IsAudioButton(Button button)
        {
            if (button is GearButton)
                return true;

            if (string.Equals(button.Tag as string, "AudioSettings", StringComparison.Ordinal))
                return true;

            return button.Name.Contains("Setting", StringComparison.OrdinalIgnoreCase)
                || button.Name.Contains("Audio", StringComparison.OrdinalIgnoreCase);
        }

        private static void ApplyAppearance(Button button)
        {
            button.Tag = "AudioSettings";
            button.Cursor = Cursors.Hand;
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

        private static void ApplyDefaultLayout(Form form, Button button)
        {
            button.Name = "btnAudioSettings";
            button.Text = string.Empty;
            button.Size = new Size(48, 48);
            button.Location = new Point(form.ClientSize.Width - button.Width - 24, form.ClientSize.Height - button.Height - 22);
            button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
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
