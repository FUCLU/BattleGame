using System;
using System.Drawing;
using System.Windows.Forms;
using BattleGame.Client.Game;
using WinTimer = System.Windows.Forms.Timer;

namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private readonly GameEngine _engine;
        private readonly WinTimer _gameTimer; // ✅ dùng alias

        public GameForm()
        {
            InitializeComponent();

            // ===== WINDOW CONFIG =====
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Text = "Battle Game";
            this.ClientSize = new Size(1280, 720);
            this.BackColor = Color.FromArgb(20, 20, 35);

            // ===== ENGINE =====
            _engine = new GameEngine();

            // ===== GAME LOOP (~60 FPS) =====
            _gameTimer = new WinTimer
            {
                Interval = 16
            };

            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // ===== INPUT =====
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            this.Load += (s, e) => this.Focus();
        }

        // ================= GAME LOOP =================
        private void GameLoop(object? sender, EventArgs e)
        {
            _engine.Update();
            this.Invalidate();
        }

        // ================= RENDER =================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _engine.Draw(e.Graphics);
        }

        // ================= INPUT =================
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            _engine.KeyDown(e.KeyCode);

            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            _engine.KeyUp(e.KeyCode);
        }

        // ================= CLEANUP =================
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}