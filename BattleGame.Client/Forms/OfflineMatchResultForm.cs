using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public sealed class OfflineMatchResultForm : Form
    {
        private readonly string _resultText;
        private readonly int _player1Rounds;
        private readonly int _player2Rounds;

        public OfflineMatchResultForm(string resultText, int player1Rounds, int player2Rounds)
        {
            _resultText = resultText;
            _player1Rounds = player1Rounds;
            _player2Rounds = player2Rounds;

            InitializeResultDialog();
        }

        private void InitializeResultDialog()
        {
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(500, 310);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(18, 24, 42);
            DoubleBuffered = true;
            Padding = new Padding(22);

            var title = new Label
            {
                Text = "MATCH RESULT",
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.PaleTurquoise,
                Font = new Font("Courier New", 20F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(22, 26),
                Size = new Size(456, 42)
            };

            var result = new Label
            {
                Text = _resultText,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(230, 38, 38),
                Font = new Font("Book Antiqua", 28F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(22, 82),
                Size = new Size(456, 62)
            };

            var scorePanel = new Panel
            {
                BackColor = Color.FromArgb(32, 45, 78),
                Location = new Point(70, 158),
                Size = new Size(360, 74)
            };
            scorePanel.Paint += ScorePanel_Paint;

            var player1 = CreateScoreLabel($"PLAYER 1     {_player1Rounds}", new Point(38, 10));
            var player2 = CreateScoreLabel($"PLAYER 2     {_player2Rounds}", new Point(38, 40));
            scorePanel.Controls.Add(player1);
            scorePanel.Controls.Add(player2);

            var okButton = new Button
            {
                Text = "OK",
                BackColor = Color.FromArgb(44, 74, 110),
                ForeColor = Color.FromArgb(255, 235, 156),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Courier New", 14F, FontStyle.Bold),
                Location = new Point(180, 250),
                Size = new Size(140, 44),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            okButton.FlatAppearance.BorderColor = Color.PaleTurquoise;
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(57, 100, 150);

            Controls.Add(title);
            Controls.Add(result);
            Controls.Add(scorePanel);
            Controls.Add(okButton);

            AcceptButton = okButton;
        }

        private static Label CreateScoreLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                BackColor = Color.Transparent,
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Consolas", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = location,
                Size = new Size(284, 24)
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.None;

            using var outer = new Pen(Color.FromArgb(8, 12, 24), 6);
            using var inner = new Pen(Color.FromArgb(230, 38, 38), 3);
            using var glow = new Pen(Color.FromArgb(255, 235, 156), 1);

            var bounds = new Rectangle(3, 3, ClientSize.Width - 7, ClientSize.Height - 7);
            e.Graphics.DrawRectangle(outer, bounds);
            bounds.Inflate(-6, -6);
            e.Graphics.DrawRectangle(inner, bounds);
            bounds.Inflate(-5, -5);
            e.Graphics.DrawRectangle(glow, bounds);
        }

        private static void ScorePanel_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Control control)
                return;

            using var border = new Pen(Color.PaleTurquoise, 2);
            e.Graphics.DrawRectangle(border, 0, 0, control.Width - 1, control.Height - 1);
        }
    }
}
