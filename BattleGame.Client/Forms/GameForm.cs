using System;
using System.Drawing;
using System.Windows.Forms;
using BattleGame.Client.Game;
using BattleGame.Shared.Models;
using WinTimer = System.Windows.Forms.Timer;

namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private readonly GameEngine _engine;
        private readonly WinTimer _gameTimer;
        private Character _bot;
        private string _map;
        private string _mode;

        private Character _player; // nhận từ ngoài

        public GameForm(Character character)
        {
            InitializeComponent();

            _player = character;

            // WINDOW CONFIG 
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Text = $"Battle Game - TEST ({_player.Name})";
            this.ClientSize = new Size(1280, 720);
            this.BackColor = Color.FromArgb(20, 20, 35);

            // ENGINE 
            _engine = new GameEngine(_player);

            // GAME LOOP (~60 FPS) 
            _gameTimer = new WinTimer
            {
                Interval = 16
            };

            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            // INPUT
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            this.Load += (s, e) => this.Focus();
        }

        public GameForm(Character player, Character bot, string map, string mode)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            _player = player;
            _bot = bot;
            _map = map;
            _mode = mode;

            // WINDOW CONFIG
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Text = $"Battle Game - {_mode} - {_map}";
            this.ClientSize = new Size(1280, 720);
            this.BackColor = Color.FromArgb(20, 20, 35);

            // ENGINE (truyền cả bot nếu cần)
            _engine = new GameEngine(_player /*, _bot nếu có */);

            _gameTimer = new WinTimer
            {
                Interval = 16
            };

            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            this.Load += (s, e) => this.Focus();
        }

        //  GAME LOOP 
        private void GameLoop(object? sender, EventArgs e)
        {
            _engine.Update();

            // update UI theo player thật
            UpdateHPBar();

            this.Invalidate();
        }

        // RENDER 
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            _engine.Draw(e.Graphics);
        }

        // INPUT
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

        // CLEANUP 
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _gameTimer?.Stop();
            base.OnFormClosing(e);
        }

        // UI 
        private void GameForm_Load(object sender, EventArgs e)
        {
            panelStatus.BackColor = Color.FromArgb(180, 0, 0, 0);

            lblHP.Parent = panelHPBack;
            lblHP.BackColor = Color.Transparent;
            lblHP.BringToFront();

            UpdateHPBar();
        }

        private void UpdateHPBar()
        {
            float percent = (float)_player.CurrentHP / _player.MaxHP;

            panelHPFill.Width = (int)(panelHPBack.Width * percent);

            lblHP.Text = $"{_player.CurrentHP}/{_player.MaxHP}";
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}