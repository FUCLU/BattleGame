using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BattleGame.Client.Config;
using BattleGame.Client.Managers;
using BattleGame.Client.Game;
using BattleGame.Client.Game.Core.Components;

namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private readonly GameEngine _engine;
        private System.Windows.Forms.Timer gameTimer;

        private float _roundSecondsRemaining = 180f;
        private int _currentRound = 1;

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");

        private readonly Stopwatch _stopwatch = new();
        private float _frameAccumulator = 0f;
        private const float FixedTimestep = 1f / 60f; // Fixed 60 FPS

        private Bitmap? _backBuffer;
        private Graphics? _backGraphics;

        public GameForm(string characterId, string mapId = "terrace")
        {
            try
            {
                InitializeComponent();

                this.AutoScaleMode = AutoScaleMode.None;
                this.DoubleBuffered = true;
                this.KeyPreview = true;

                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);
                UpdateStyles();

                InputManager.Clear();
                _engine = new GameEngine(characterId, mapId, this.ClientSize.Width, this.ClientSize.Height);

                CreateBackBuffer();

                Load += GameForm_Load;

                gameTimer = new System.Windows.Forms.Timer();
                gameTimer.Interval = 16; // ~60fps
                gameTimer.Tick += gameTimer_Tick;
                gameTimer.Start();

                _stopwatch.Start();

                this.Visible = true;
                this.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khởi tạo GameForm:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void CreateBackBuffer()
        {
            _backBuffer?.Dispose();
            _backGraphics?.Dispose();

            int w = Math.Max(1, this.ClientSize.Width);
            int h = Math.Max(1, this.ClientSize.Height);

            _backBuffer = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            _backGraphics = Graphics.FromImage(_backBuffer);

            _backGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _backGraphics.SmoothingMode = SmoothingMode.None;
            _backGraphics.PixelOffsetMode = PixelOffsetMode.None;
            _backGraphics.CompositingQuality = CompositingQuality.HighSpeed;
        }

        private void GameForm_Load(object? sender, EventArgs e)
        {
            panelStatus.BackColor = Color.FromArgb(180, 0, 0, 0);
            label1.ForeColor = Color.WhiteSmoke;
            label2.ForeColor = Color.Gainsboro;
            label1.Text = $"ROUND {_currentRound}";
            label2.Text = FormatTime(_roundSecondsRemaining);

            foreach (Control c in new Control[]
                { panelStatus, panelHPBack, panelManaBack,
                  panel3, panel1, label3, label4, pictureBox1, pictureBox2 })
                c.BringToFront();

            UpdateUIBars();
            UpdateCharacterHeaders();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            InputManager.SetKey(e.KeyCode, true);
            e.Handled = true;
        }

        private void UpdateCharacterHeaders()
        {
            var selectionItems = CharacterCatalog.LoadSelectionItems(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            var lookup = selectionItems
                .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            string? playerId = _engine.Player.Get<CharacterComponent>()?.CharacterId;
            SetCharacterHeader(label3, pictureBox1, playerId, lookup);

            string? enemyId = _engine.Enemy.Get<CharacterComponent>()?.CharacterId;
            SetCharacterHeader(label4, pictureBox2, enemyId, lookup);
        }

        private void SetCharacterHeader(Label nameLabel, PictureBox portraitBox, string? characterId,
            Dictionary<string, CharacterSelectionItem> lookup)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return;

            if (lookup.TryGetValue(characterId, out var selectionItem))
            {
                nameLabel.Text = selectionItem.DisplayName;
            }
            else
            {
                nameLabel.Text = characterId;
            }

            portraitBox.Image = LoadImage(GetPortraitPath(characterId));
        }

        private static string GetPortraitPath(string characterId)
        {
            string portraitFileName = characterId.ToLowerInvariant() switch
            {
                "wizard" => "wizard.png",
                "samurai" => "samurai.png",
                "kitsune" => "kitsune.png",
                "lord" => "lord.png",
                _ => $"{characterId.ToLowerInvariant()}.png"
            };

            return Path.Combine(PortraitRoot, portraitFileName);
        }

        private static Image? LoadImage(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return Image.FromFile(path);
                }
            }
            catch
            {
            }

            return null;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            InputManager.SetKey(e.KeyCode, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backBuffer == null) return;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        private void gameTimer_Tick(object? sender, EventArgs e)
        {
            // Tính delta time từ Stopwatch
            float dt = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            
            // Giới hạn frame time để tránh simulation lag
            dt = Math.Min(dt, FixedTimestep * 2);

            // Accumulate delta time
            _frameAccumulator += dt;

            // Update với fixed timestep
            while (_frameAccumulator >= FixedTimestep)
            {
                _engine.Update(FixedTimestep);
                _frameAccumulator -= FixedTimestep;
            }

            // Vẽ frame
            if (_backGraphics != null)
            {
                _backGraphics.Clear(Color.Black);
                _engine.Draw(_backGraphics);
            }

            UpdateRoundTimer(dt);
            UpdateUIBars();

            // Vẽ lên màn hình
            this.Invalidate(false);
        }

        private void UpdateRoundTimer(float deltaTime)
        {
            if (_roundSecondsRemaining <= 0f)
            {
                if (label2.Text != "00:00")
                    label2.Text = "00:00";
                return;
            }

            _roundSecondsRemaining = Math.Max(0f, _roundSecondsRemaining - deltaTime);
            string timeText = FormatTime(_roundSecondsRemaining);
            if (label2.Text != timeText)
                label2.Text = timeText;
        }

        private static string FormatTime(float totalSeconds)
        {
            int seconds = (int)MathF.Ceiling(totalSeconds);
            int minutes = Math.Clamp(seconds / 60, 0, 99);
            int remainder = Math.Clamp(seconds % 60, 0, 59);
            return $"{minutes:00}:{remainder:00}";
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CreateBackBuffer();
        }

        private void UpdateUIBars()
        {
            try
            {
                var playerChar = _engine.Player.Get<CharacterComponent>();
                if (playerChar != null)
                {
                    string hpText = $"{playerChar.Hp}/{playerChar.BaseStats.Hp}";
                    if (lblHP.Text != hpText) lblHP.Text = hpText;

                    string manaText = $"{playerChar.Mana}/{playerChar.BaseStats.Mana}";
                    if (lblMana.Text != manaText) lblMana.Text = manaText;

                    int maxW = 301;
                    int hpW = playerChar.BaseStats.Hp > 0
                        ? (int)(maxW * playerChar.Hp / (float)playerChar.BaseStats.Hp) : 0;
                    int manaW = playerChar.BaseStats.Mana > 0
                        ? (int)(maxW * playerChar.Mana / (float)playerChar.BaseStats.Mana) : 0;

                    if (panelHPFill.Width != hpW) panelHPFill.Width = Math.Max(0, hpW);
                    if (panelManaFill.Width != manaW) panelManaFill.Width = Math.Max(0, manaW);
                }

                var enemyChar = _engine.Enemy.Get<CharacterComponent>();
                if (enemyChar != null)
                {
                    string hpText = $"{enemyChar.Hp}/{enemyChar.BaseStats.Hp}";
                    if (label6.Text != hpText) label6.Text = hpText;

                    string manaText = $"{enemyChar.Mana}/{enemyChar.BaseStats.Mana}";
                    if (label5.Text != manaText) label5.Text = manaText;

                    int maxW = 301;
                    int hpW = enemyChar.BaseStats.Hp > 0
                        ? (int)(maxW * enemyChar.Hp / (float)enemyChar.BaseStats.Hp) : 0;
                    int manaW = enemyChar.BaseStats.Mana > 0
                        ? (int)(maxW * enemyChar.Mana / (float)enemyChar.BaseStats.Mana) : 0;

                    if (panel4.Width != hpW) panel4.Width = Math.Max(0, hpW);
                    if (panel2.Width != manaW) panel2.Width = Math.Max(0, manaW);
                }
            }
            catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            gameTimer.Stop();
            gameTimer.Dispose();
            _backGraphics?.Dispose();
            _backBuffer?.Dispose();
            InputManager.Clear();
            base.OnFormClosed(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            InputManager.Clear();
        }
    }
}