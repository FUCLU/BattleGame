using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    internal static class MessageBox
    {
        public static DialogResult Show(string text)
        {
            return Show(null, text, "Battle Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(null, text, caption, buttons, MessageBoxIcon.Information);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return Show(null, text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using var dialog = new GameMessageDialog(text, caption, buttons, icon);
            Form? ownerForm = owner as Form ?? Form.ActiveForm;
            return ownerForm != null && !ownerForm.IsDisposed
                ? dialog.ShowDialog(ownerForm)
                : dialog.ShowDialog();
        }
    }

    internal sealed class GameMessageDialog : Form
    {
        private readonly MessageBoxButtons _buttons;
        private readonly MessageBoxIcon _icon;
        private readonly string _caption;

        public GameMessageDialog(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            _buttons = buttons;
            _icon = icon;
            _caption = string.IsNullOrWhiteSpace(caption) ? "Battle Game" : caption.Trim();
            InitializeDialog(message ?? string.Empty);
        }

        private void InitializeDialog(string message)
        {
            AutoScaleMode = AutoScaleMode.None;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            BackColor = Color.FromArgb(16, 24, 48);
            DoubleBuffered = true;
            Padding = new Padding(18);
            KeyPreview = true;

            int textHeight = Math.Max(76, TextRenderer.MeasureText(
                message,
                new Font("Segoe UI", 11F, FontStyle.Bold),
                new Size(430, 0),
                TextFormatFlags.WordBreak).Height + 18);
            ClientSize = new Size(520, Math.Min(360, 170 + textHeight));

            var title = new Label
            {
                Text = _caption.ToUpperInvariant(),
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.PaleTurquoise,
                Font = new Font("Courier New", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(24, 18),
                Size = new Size(ClientSize.Width - 48, 36)
            };

            const int contentTop = 82;
            var badge = new Label
            {
                Text = BadgeText(),
                AutoSize = false,
                BackColor = BadgeBackColor(),
                ForeColor = BadgeForeColor(),
                Font = new Font("Courier New", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(34, contentTop),
                Size = new Size(86, 34)
            };
            badge.Paint += PaintBadge;

            var body = new Label
            {
                Text = message,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft,
                Location = new Point(140, contentTop),
                Size = new Size(ClientSize.Width - 178, textHeight),
                UseCompatibleTextRendering = false
            };

            var buttonPanel = new FlowLayoutPanel
            {
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Location = new Point(24, ClientSize.Height - 70),
                Size = new Size(ClientSize.Width - 48, 48)
            };

            foreach (Button button in CreateButtons())
                buttonPanel.Controls.Add(button);

            Controls.Add(title);
            Controls.Add(badge);
            Controls.Add(body);
            Controls.Add(buttonPanel);
        }

        private Button[] CreateButtons()
        {
            if (_buttons == MessageBoxButtons.YesNo)
            {
                var no = CreateButton("NO", DialogResult.No, Color.FromArgb(52, 66, 96));
                var yes = CreateButton("YES", DialogResult.Yes, Color.FromArgb(44, 100, 150));
                AcceptButton = yes;
                CancelButton = no;
                return new[] { no, yes };
            }

            var ok = CreateButton("OK", DialogResult.OK, Color.FromArgb(44, 100, 150));
            AcceptButton = ok;
            CancelButton = ok;
            return new[] { ok };
        }

        private static Button CreateButton(string text, DialogResult result, Color backColor)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                BackColor = backColor,
                ForeColor = Color.FromArgb(255, 235, 156),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Courier New", 13F, FontStyle.Bold),
                Size = new Size(118, 42),
                Margin = new Padding(8, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderColor = Color.PaleTurquoise;
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 118, 168);
            return button;
        }

        private string BadgeText()
        {
            return _icon switch
            {
                MessageBoxIcon.Error => "ERROR",
                MessageBoxIcon.Warning => "WARN",
                MessageBoxIcon.Question => "ASK",
                _ => "INFO"
            };
        }

        private Color BadgeBackColor()
        {
            return _icon switch
            {
                MessageBoxIcon.Error => Color.FromArgb(126, 30, 38),
                MessageBoxIcon.Warning => Color.FromArgb(140, 94, 26),
                MessageBoxIcon.Question => Color.FromArgb(48, 86, 142),
                _ => Color.FromArgb(34, 92, 132)
            };
        }

        private Color BadgeForeColor()
        {
            return _icon == MessageBoxIcon.Warning
                ? Color.FromArgb(255, 235, 156)
                : Color.WhiteSmoke;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.None;

            using var shadow = new SolidBrush(Color.FromArgb(90, 0, 0, 0));
            e.Graphics.FillRectangle(shadow, 8, 8, ClientSize.Width - 8, ClientSize.Height - 8);

            using var outer = new Pen(Color.FromArgb(6, 12, 26), 6);
            using var inner = new Pen(Color.PaleTurquoise, 2);
            using var accent = new Pen(Color.FromArgb(255, 235, 156), 1);

            var bounds = new Rectangle(3, 3, ClientSize.Width - 7, ClientSize.Height - 7);
            e.Graphics.DrawRectangle(outer, bounds);
            bounds.Inflate(-7, -7);
            e.Graphics.DrawRectangle(inner, bounds);
            bounds.Inflate(-5, -5);
            e.Graphics.DrawRectangle(accent, bounds);
        }

        private static void PaintBadge(object? sender, PaintEventArgs e)
        {
            if (sender is not Control control)
                return;

            using var border = new Pen(Color.FromArgb(255, 235, 156), 2);
            e.Graphics.DrawRectangle(border, 0, 0, control.Width - 1, control.Height - 1);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult = _buttons == MessageBoxButtons.YesNo ? DialogResult.No : DialogResult.OK;
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
