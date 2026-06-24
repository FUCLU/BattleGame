using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Config;
using BattleGame.Client.Managers;
using BattleGame.Client.Game;
using BattleGame.Client.Game.Dungeon;
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Packets;
using BattleGame.Shared.Models;
using BattleGame.Shared.Simulation;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private static readonly Size DefaultGameClientSize = new(1280, 720);
        private const int HudMargin = 31;
        private const int HudPanelWidth = 281;
        private const int BossHudPanelWidth = HudPanelWidth * 2;
        private const int BossHudPanelHeight = 34;
        private const int HudPortraitWidth = 93;
        private const int StatusPanelWidth = 304;

        private GameEngine _engine;
        private readonly bool _isOnline;
        private readonly bool _isDungeonMap;
        private readonly bool _dungeonStatMode;
        private readonly bool _localTwoPlayer;
        private readonly string _playerCharacterId;
        private readonly string _enemyCharacterId;
        private readonly string _mapId;
        private readonly int _localPlayerId;
        private readonly string? _localUsername;
        private readonly string? _enemyUsername;
        private readonly int? _roomId;
        private int _inputSequence;
        private int _clientTick;
        private bool _prevAttack;
        private bool _prevSkill1;
        private bool _prevSkill2;
        private bool _prevDash;
        private BattleInput _latestSampledInput = new();
        private bool _hasSampledInput;
        private bool _pendingAttackPressed;
        private int _pendingSkillSlot;
        private bool _pendingDashPressed;

        private float _roundSecondsRemaining = 180f;
        private int _currentRound = 1;
        private bool _isSuddenDeath;
        private int _player1RoundWins;
        private int _player2RoundWins;
        private bool _offlineRoundResolving;
        private bool _offlineMatchEnded;
        private bool _dungeonRunEnded;
        private float _roundIntroTimer = RoundIntroSeconds;
        private string _roundIntroText = "ROUND 1";
        private const float RoundIntroSeconds = 2.2f;
        private const float OfflineRoundResolveDelaySeconds = 1.2f;
        private const float DungeonResolveDelaySeconds = 1.2f;
        private const float PostDeathRoundDelaySeconds = 0.35f;
        private const float DungeonStatMessageSeconds = 2.2f;
        private readonly Random _dungeonStatRandom = new();
        private int _lastDungeonDefeatedCount;
        private bool _dungeonFailureStatApplied;
        private string _dungeonStatMessage = string.Empty;
        private float _dungeonStatMessageTimer;
        private readonly Dictionary<DungeonStat, int> _dungeonPendingStatDeltas = new();

        private static readonly string ContentRoot = ClientContentRoot.Resolve(AppDomain.CurrentDomain.BaseDirectory);
        private static readonly string AssetsRoot = Path.Combine(ContentRoot, "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");

        private readonly Stopwatch _stopwatch = new();
        private float _frameAccumulator = 0f;
        private const float FixedTimestep = 1f / 60f; // Fixed 60 FPS
        private float _networkSyncAccumulator = 0f;
        private const float NetworkSyncIntervalSeconds = 1f / 60f; // 60 updates/s for reliable skill trigger
        private int _sendingNetworkState;
        private int _sendNetworkAgainRequested;
        private int _inputDebugCounter;
        private bool _isRunning;
        private long _lastTicks;
        private const float MaxFrameDt = 0.05f;
        private float _uiAccumulator = 0f;
        private const float UiUpdateInterval = 0.1f;
        private float _moveSfxTimer;
        private const float MoveSfxIntervalSeconds = 0.28f;
        private CombatantAudioState _playerAudioState = new();
        private CombatantAudioState _enemyAudioState = new();

        private CancellationTokenSource? _networkCts;
        private Task? _networkListenTask;
        private bool _navigatingAway;
        private bool _remoteDisconnected;
        private bool _roomLeaveSent;
        private bool _mirrorView;

        private Bitmap? _backBuffer;
        private Graphics? _backGraphics;
        private readonly Dictionary<string, Image> _imageCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Panel, HudBarState> _hudBars = new();
        private Label? _skill1CooldownLabel;
        private Label? _skill2CooldownLabel;
        private Label? _enemySkill1CooldownLabel;
        private Label? _enemySkill2CooldownLabel;
        private readonly Form? _returnFormOnExit;
        private bool _destinationOpened;
        private string _currentBossHudCharacterId = string.Empty;

        public GameForm(
            string characterId,
            string mapId = "terrace",
            string? enemyCharacterId = null,
            bool isOnline = false,
            int localPlayerId = 0,
            string? localUsername = null,
            string? enemyUsername = null,
            int? roomId = null,
            Form? returnFormOnExit = null,
            bool localTwoPlayer = false,
            bool dungeonStatMode = false)
        {
            try
            {
                InitializeComponent();
                BorderlessFormHelper.Apply(this);

                this.AutoScaleMode = AutoScaleMode.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ClientSize = DefaultGameClientSize;
                this.MinimumSize = SizeFromClientSize(DefaultGameClientSize);
                this.MaximumSize = SizeFromClientSize(DefaultGameClientSize);
                this.DoubleBuffered = true;
                this.KeyPreview = true;
                LayoutHud();
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);
                UpdateStyles();

                InputManager.Clear();
                _isOnline = isOnline;
                _isDungeonMap = IsDungeonMap(mapId);
                _dungeonStatMode = _isDungeonMap && dungeonStatMode;
                _localTwoPlayer = localTwoPlayer;
                _playerCharacterId = characterId;
                _enemyCharacterId = string.IsNullOrWhiteSpace(enemyCharacterId) ? "samurai" : enemyCharacterId.Trim().ToLowerInvariant();
                _mapId = mapId;
                _localPlayerId = localPlayerId;
                _localUsername = string.IsNullOrWhiteSpace(localUsername) ? null : localUsername.Trim();
                _enemyUsername = string.IsNullOrWhiteSpace(enemyUsername) ? null : enemyUsername.Trim();
                _roomId = roomId is > 0 ? roomId : null;
                _returnFormOnExit = returnFormOnExit;
                _mirrorView = false;
                _engine = CreateGameEngine();
                StartRoundIntro();

                CreateBackBuffer();

                Load += GameForm_Load;
                FormClosing += GameForm_FormClosing;

                _stopwatch.Start();
                _lastTicks = _stopwatch.ElapsedTicks;
                _isRunning = true;
                Application.Idle += OnApplicationIdle;

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

        private GameEngine CreateGameEngine()
        {
            return new GameEngine(
                _playerCharacterId,
                _mapId,
                ClientSize.Width,
                ClientSize.Height,
                _enemyCharacterId,
                _localTwoPlayer);
        }

        private void StartRoundIntro()
        {
            _roundIntroText = _isSuddenDeath ? "SUDDEN DEATH" : $"ROUND {_currentRound}";
            _roundIntroTimer = RoundIntroSeconds;
            ResetBattleActionSoundState();
            SoundManager.PlayRoundAnnouncement(_currentRound, _isSuddenDeath);
        }

        private void GameForm_Load(object? sender, EventArgs e)
        {
            SoundManager.PlayBGM(ResolveBattleMusicFileName(), usePreferred: !_isDungeonMap);
            InputManager.Clear();
            LayoutHud();
            panelStatus.BackColor = Color.FromArgb(180, 0, 0, 0);
            label1.ForeColor = Color.WhiteSmoke;
            label2.ForeColor = Color.Gainsboro;
            label1.Text = $"ROUND {_currentRound}";
            label2.Text = FormatTime((int)MathF.Ceiling(_roundSecondsRemaining));

            foreach (Control c in new Control[]
                { panelStatus, panelHPBack, panelManaBack,
                  panel3, panel1, label3, label4, pictureBox1, pictureBox2 })
                c.BringToFront();

            ApplyDungeonHudVisibility();

            // Round/time are rendered directly into backbuffer to avoid WinForms control flicker.
            panelStatus.Visible = false;
            btnExit.Parent = this;
            btnExit.Visible = !_isDungeonMap;
            if (btnExit.Visible)
                btnExit.BringToFront();

            UpdateCharacterHeaders();
            ConfigureHudValueLabels();
            ConfigureSkillCooldownLabels();
            LayoutHud();
            UpdateUIBars();

            if (_isOnline && NetworkManager.Instance.IsConnected)
            {
                _networkCts = new CancellationTokenSource();
                _networkListenTask = ListenForRealtimePacketsAsync(_networkCts.Token);
            }

            Activate();
            Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            TrackKeyDown(e.KeyCode);

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (IsTrackedGameKey(keyCode))
            {
                TrackKeyDown(keyCode);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            return IsTrackedGameKey(keyCode) || base.IsInputKey(keyData);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (IsTrackedGameKey(e.KeyCode))
                e.IsInputKey = true;

            base.OnPreviewKeyDown(e);
        }

        private void TrackKeyDown(Keys keyCode)
        {
            if (!IsTrackedGameKey(keyCode))
                return;

            bool wasDown = InputManager.IsKeyDown(keyCode);
            InputManager.SetKey(keyCode, true);

            if (_isOnline && !wasDown && TryCreateImmediateActionInput(keyCode, out var actionInput))
                LatchSampledInput(actionInput);
        }

        private void UpdateBattleMovementSound(float dt)
        {
            if (_offlineMatchEnded || _dungeonRunEnded || _offlineRoundResolving || _roundIntroTimer > 0f)
                return;

            _moveSfxTimer = Math.Max(0f, _moveSfxTimer - dt);
            if (_moveSfxTimer > 0f || !IsForegroundGameWindow())
                return;

            bool moving = InputManager.IsKeyDown(Keys.A)
                || InputManager.IsKeyDown(Keys.D)
                || InputManager.IsKeyDown(Keys.Left)
                || InputManager.IsKeyDown(Keys.Right);

            if (!moving)
                return;

            SoundManager.PlayBattleMove();
            _moveSfxTimer = MoveSfxIntervalSeconds;
        }

        private void UpdateBattleActionSounds()
        {
            if (_offlineMatchEnded || _dungeonRunEnded || _offlineRoundResolving || _roundIntroTimer > 0f)
            {
                ResetBattleActionSoundState();
                return;
            }

            TrackCombatantSounds(_engine.Player, _playerAudioState);

            if (_engine.Enemy != null)
                TrackCombatantSounds(_engine.Enemy, _enemyAudioState);
            else
                _enemyAudioState.Reset();
        }

        private static void TrackCombatantSounds(BattleGame.Client.Game.Core.Entity entity, CombatantAudioState state)
        {
            var character = entity.Get<CharacterComponent>();

            if (!state.Initialized)
            {
                state.Capture(character);
                return;
            }

            if (character.Hp < state.Hp)
                SoundManager.PlayBattleHit();

            if (character.IsAttacking && !state.IsAttacking)
                SoundManager.PlayBattleAttack();

            if (character.IsUsingSkill && !state.IsUsingSkill)
                SoundManager.PlayBattleSkill();

            if (character.IsDashing && !state.IsDashing)
                SoundManager.PlayBattleDash();

            if (character.IsProtecting && !state.IsProtecting)
                SoundManager.PlayBattleGuard();

            state.Capture(character);
        }

        private void ResetBattleActionSoundState()
        {
            _playerAudioState.Reset();
            _enemyAudioState.Reset();
        }

        private void UpdateCharacterHeaders()
        {
            var selectionItems = CharacterCatalog.LoadSelectionItems(ContentRoot);
            var lookup = selectionItems
                .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            string? playerId = _engine.Player.Get<CharacterComponent>()?.CharacterId;
            SetCharacterHeader(label3, pictureBox1, playerId, lookup);

            if (!_isDungeonMap && _engine.Enemy != null)
            {
                string? enemyId = _engine.Enemy.Get<CharacterComponent>()?.CharacterId;
                SetCharacterHeader(label4, pictureBox2, enemyId, lookup);
            }

            if (!string.IsNullOrWhiteSpace(_localUsername))
                label3.Text = _localUsername;

            if (!string.IsNullOrWhiteSpace(_enemyUsername))
                label4.Text = _enemyUsername;

            LayoutHud();
        }

        private static bool IsDungeonMap(string mapId)
            => DungeonMapRegistry.IsDungeonMap(mapId);

        private string ResolveBattleMusicFileName()
        {
            if (_isDungeonMap && DungeonMapRegistry.TryGet(_mapId, out var dungeonMap))
                return dungeonMap.BattleMusicFileName;

            return "darren_hirst.mp3";
        }

        private void ApplyDungeonHudVisibility()
        {
            if (!_isDungeonMap)
                return;

            bool showBossHud = _engine?.Enemy != null;
            label4.Visible = showBossHud;
            pictureBox2.Visible = false;
            panel3.Visible = showBossHud;
            panel1.Visible = false;
        }

        private void ConfigureHudValueLabels()
        {
            ConfigureHudBar(panelHPBack, panelHPFill, lblHP);
            ConfigureHudBar(panelManaBack, panelManaFill, lblMana);
            ConfigureHudBar(panel3, panel4, label6);
            ConfigureHudBar(panel1, panel2, label5);
        }

        private void ConfigureSkillCooldownLabels()
        {
            _skill1CooldownLabel ??= CreateSkillCooldownLabel();
            _skill2CooldownLabel ??= CreateSkillCooldownLabel();
            _enemySkill1CooldownLabel ??= CreateSkillCooldownLabel();
            _enemySkill2CooldownLabel ??= CreateSkillCooldownLabel();

            AddCooldownLabelToForm(_skill1CooldownLabel);
            AddCooldownLabelToForm(_skill2CooldownLabel);
            AddCooldownLabelToForm(_enemySkill1CooldownLabel);
            AddCooldownLabelToForm(_enemySkill2CooldownLabel);
            LayoutSkillCooldownLabels();
        }

        private static Label CreateSkillCooldownLabel()
        {
            return new Label
            {
                AutoSize = false,
                BackColor = Color.FromArgb(140, 0, 0, 0),
                ForeColor = Color.LightGreen,
                Font = new Font("Book Antiqua", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "READY"
            };
        }

        private void AddCooldownLabelToForm(Label label)
        {
            if (label.Parent != this)
            {
                label.Parent?.Controls.Remove(label);
                Controls.Add(label);
            }

            label.Visible = true;
            label.BringToFront();
        }

        private void ConfigureHudBar(Panel backPanel, Panel fillPanel, Label valueLabel)
        {
            if (!_hudBars.TryGetValue(backPanel, out var state))
            {
                state = new HudBarState(fillPanel.BackColor, backPanel.BackColor, valueLabel.Font);
                _hudBars[backPanel] = state;
                backPanel.Paint += HudBarPanel_Paint;
                backPanel.Resize += HudBarPanel_Resize;
            }

            state.FillColor = fillPanel.BackColor;
            state.BackColor = backPanel.BackColor;
            state.Font = valueLabel.Font;
            state.TextColor = IsManaBar(backPanel) ? Color.WhiteSmoke : Color.Black;

            fillPanel.Visible = false;
            valueLabel.Visible = false;
            backPanel.Invalidate();
        }

        private void HudBarPanel_Resize(object? sender, EventArgs e)
        {
            if (sender is Control control)
                control.Invalidate();
        }

        private void HudBarPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel || !_hudBars.TryGetValue(panel, out var state))
                return;

            Rectangle rect = panel.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            using var backBrush = new SolidBrush(state.BackColor);
            using var fillBrush = new SolidBrush(state.FillColor);
            e.Graphics.FillRectangle(backBrush, rect);

            int fillWidth = (int)MathF.Round(rect.Width * Math.Clamp(state.Ratio, 0f, 1f));
            if (fillWidth > 0)
                e.Graphics.FillRectangle(fillBrush, rect.Left, rect.Top, fillWidth, rect.Height);

            TextRenderer.DrawText(
                e.Graphics,
                state.Text,
                state.Font,
                rect,
                state.TextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }

        private static void CenterHudValueLabel(Panel parentPanel, Label valueLabel)
        {
            int x = Math.Max(0, (parentPanel.ClientSize.Width - valueLabel.Width) / 2);
            int y = Math.Max(-2, (parentPanel.ClientSize.Height - valueLabel.Height) / 2);
            valueLabel.Location = new Point(x, y);
            valueLabel.BringToFront();
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

            Image? portrait = LoadImageCached(GetPortraitPath(characterId));
            if (portrait == null && lookup.TryGetValue(characterId, out var selectionItemForPortrait))
            {
                portrait = LoadImageCached(selectionItemForPortrait.GetPreviewPath(Path.Combine(AssetsRoot, "Characters")));
            }

            portraitBox.Image = portrait;
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

        private Image? LoadImageCached(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    return null;

                if (_imageCache.TryGetValue(path, out var cached))
                    return cached;

                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var temp = Image.FromStream(fs);
                var clone = new Bitmap(temp);
                _imageCache[path] = clone;
                return clone;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadImageCached] {path}: {ex}");
            }

            return null;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (IsTrackedGameKey(e.KeyCode))
                InputManager.SetKey(e.KeyCode, false);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backBuffer == null) return;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint min, uint max, uint remove);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);
        private static bool IsApplicationIdle() => !PeekMessage(out _, IntPtr.Zero, 0, 0, 0);

        private void DrawFrame()
        {
            if (_backGraphics == null) return;
            _backGraphics.Clear(Color.Black);
            _engine.Draw(_backGraphics);
            if (!_isDungeonMap)
                DrawRoundOverlay(_backGraphics);
            DrawDungeonStatMessage(_backGraphics);
            DrawRoundIntroOverlay(_backGraphics);
        }

        private void DrawDungeonStatMessage(Graphics g)
        {
            if (!_isDungeonMap || string.IsNullOrWhiteSpace(_dungeonStatMessage) || _dungeonStatMessageTimer <= 0f)
                return;

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var font = new Font("Book Antiqua", 18f, FontStyle.Bold, GraphicsUnit.Point);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Rectangle rect = new(
                Math.Max(0, (ClientSize.Width - 520) / 2),
                28,
                Math.Min(520, ClientSize.Width),
                52);

            using var bg = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
            using var border = new Pen(Color.FromArgb(220, 255, 230, 120), 2f);
            using var text = new SolidBrush(Color.FromArgb(255, 255, 245, 175));
            g.FillRectangle(bg, rect);
            g.DrawRectangle(border, rect);
            g.DrawString(_dungeonStatMessage, font, text, rect, sf);
        }

        private void DrawRoundOverlay(Graphics g)
        {
            Rectangle rect = panelStatus.Bounds;
            using var bg = new SolidBrush(Color.Black);
            g.FillRectangle(bg, rect);

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var titleFont = new Font("Book Antiqua", 16.2f, FontStyle.Bold, GraphicsUnit.Point);
            using var timeFont = new Font("Book Antiqua", 13.8f, FontStyle.Regular, GraphicsUnit.Point);
            using var textBrush = new SolidBrush(Color.WhiteSmoke);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            string roundText = _isSuddenDeath ? "SUDDEN DEATH" : $"ROUND {_currentRound}";
            string timeText = FormatTime((int)MathF.Ceiling(_roundSecondsRemaining));

            var roundRect = new Rectangle(rect.X, rect.Y + 10, rect.Width, 36);
            var timeRect = new Rectangle(rect.X, rect.Y + 52, rect.Width, 32);
            g.DrawString(roundText, titleFont, textBrush, roundRect, sf);
            g.DrawString(timeText, timeFont, textBrush, timeRect, sf);
        }

        private void DrawRoundIntroOverlay(Graphics g)
        {
            if (_roundIntroTimer <= 0f || _isDungeonMap)
                return;

            var overlayRect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            using var shade = new SolidBrush(Color.FromArgb(145, 0, 0, 0));
            g.FillRectangle(shade, overlayRect);

            var bannerRect = new Rectangle(
                Math.Max(0, (ClientSize.Width - 560) / 2),
                Math.Max(0, (ClientSize.Height - 150) / 2),
                Math.Min(560, ClientSize.Width),
                150);

            using var bannerBrush = new SolidBrush(Color.FromArgb(190, 0, 0, 0));
            using var borderPen = new Pen(Color.FromArgb(210, 140, 0, 0), 4);
            g.FillRectangle(bannerBrush, bannerRect);
            g.DrawRectangle(borderPen, bannerRect);

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var roundFont = new Font("Book Antiqua", 48f, FontStyle.Bold, GraphicsUnit.Point);
            using var textBrush = new SolidBrush(Color.FromArgb(220, 28, 28));
            using var shadowBrush = new SolidBrush(Color.Black);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var shadowRect = bannerRect;
            shadowRect.Offset(4, 4);
            g.DrawString(_roundIntroText, roundFont, shadowBrush, shadowRect, sf);
            g.DrawString(_roundIntroText, roundFont, textBrush, bannerRect, sf);
        }

        private void UpdateUIBarsThrottled(float dt)
        {
            _uiAccumulator += dt;
            if (_uiAccumulator < UiUpdateInterval) return;
            _uiAccumulator = 0f;
            UpdateUIBars();
        }

        private void OnApplicationIdle(object? sender, EventArgs e)
        {
            while (_isRunning && IsApplicationIdle())
            {
                long now = _stopwatch.ElapsedTicks;
                float dt = (now - _lastTicks) / (float)Stopwatch.Frequency;
                _lastTicks = now;
                dt = Math.Min(dt, MaxFrameDt);
                UpdateRoundIntro(dt);
                UpdateBattleMovementSound(dt);

                _frameAccumulator += dt;
                while (_frameAccumulator >= FixedTimestep)
                {
                    bool roundPresentationActive = _roundIntroTimer > 0f || _offlineRoundResolving || _offlineMatchEnded || _dungeonRunEnded;
                    if (!_isOnline && !roundPresentationActive)
                    {
                        _engine.Update(FixedTimestep);
                    }
                    else
                    {
                        if (_isOnline)
                        {
                            LatchSampledInput(SampleBattleInput());
                            _engine.UpdateOnlineVisuals(FixedTimestep);
                        }
                        else if (_offlineRoundResolving || _dungeonRunEnded)
                        {
                            _engine.UpdatePresentation(FixedTimestep);
                        }
                    }

                    TrySendRealtimeState(FixedTimestep);
                    _frameAccumulator -= FixedTimestep;

                    if (_isOnline)
                        _clientTick++;
                }

                UpdateBattleActionSounds();
                UpdateDungeonStatMessage(dt);
                UpdateRoundTimer(dt);
                UpdateOfflineRoundState();
                UpdateDungeonRunState();
                DrawFrame();
                UpdateUIBarsThrottled(dt);
                Invalidate(false);
            }
        }

        private void TrySendRealtimeState(float dt)
        {
            if (!_isOnline)
                return;

            if (!NetworkManager.Instance.IsConnected)
                return;

            _networkSyncAccumulator += dt;
            if (_networkSyncAccumulator < NetworkSyncIntervalSeconds)
                return;

            _networkSyncAccumulator = 0f;
            _ = SendLocalStateAsync();
        }

        private async Task SendLocalStateAsync()
        {
            if (Interlocked.Exchange(ref _sendingNetworkState, 1) == 1)
            {
                Interlocked.Exchange(ref _sendNetworkAgainRequested, 1);
                return;
            }

            try
            {
                if (!NetworkManager.Instance.IsConnected)
                    return;

                if (_isOnline)
                {
                    var input = CreateNetworkInput();
                    var packet = new InputPacket
                    {
                        RoomId = _roomId ?? 0,
                        Input = input
                    };
                    if (Math.Abs(input.MoveX) > 0f || input.AttackPressed || input.SkillSlot > 0 || input.DashPressed)
                    {
                        if ((_inputDebugCounter++ % 15) == 0)
                        {
                            Debug.WriteLine($"[InputClient] room={packet.RoomId} pid={input.PlayerId} seq={input.Sequence} move={input.MoveX} atk={input.AttackPressed} skill={input.SkillSlot} dash={input.DashPressed} block={input.BlockHeld}");
                        }
                    }
                    await NetworkManager.Instance.SendInputAsync(packet);
                }
                else
                    await SendLegacyGameStateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SendLocalStateAsync] {ex}");
            }
            finally
            {
                Interlocked.Exchange(ref _sendingNetworkState, 0);
                if (Interlocked.Exchange(ref _sendNetworkAgainRequested, 0) == 1 && _isOnline && !_navigatingAway && !IsDisposed)
                {
                    _ = SendLocalStateAsync();
                }
            }
        }

        private BattleInput SampleBattleInput()
        {
            bool hasInputFocus = IsForegroundGameWindow();
            if (!hasInputFocus)
                ClearGameInputState();

            bool left = IsGameKeyDown(Keys.A, hasInputFocus);
            bool right = IsGameKeyDown(Keys.D, hasInputFocus);
            bool attack = IsGameKeyDown(Keys.J, hasInputFocus);
            bool skill1 = IsGameKeyDown(Keys.U, hasInputFocus);
            bool skill2 = IsGameKeyDown(Keys.I, hasInputFocus);
            bool dash = IsGameKeyDown(Keys.K, hasInputFocus);

            var mv = _engine.Player.Get<MovementComponent>();
            float screenMoveX = right == left ? 0f : right ? 1f : -1f;
            bool screenFacingRight = screenMoveX > 0f || (screenMoveX == 0f && mv.FacingRight);
            float moveX = _mirrorView ? -screenMoveX : screenMoveX;
            bool facingRight = _mirrorView ? !screenFacingRight : screenFacingRight;
            bool attackPressed = attack && !_prevAttack;
            bool skill1Pressed = skill1 && !_prevSkill1;
            bool skill2Pressed = skill2 && !_prevSkill2;
            bool dashPressed = dash && !_prevDash;
            int skillSlot = skill1Pressed ? 1 : skill2Pressed ? 2 : 0;

            bool blockHeld = hasInputFocus
                && IsGameKeyDown(Keys.S, hasInputFocus)
                && !attackPressed
                && !skill1Pressed
                && !skill2Pressed
                && !dashPressed;
            var input = new BattleInput
            {
                PlayerId = _localPlayerId,
                Sequence = 0,
                ClientTick = _clientTick,
                MoveX = moveX,
                JumpPressed = false,
                BlockHeld = blockHeld,
                AttackPressed = attackPressed,
                SkillSlot = skillSlot,
                DashPressed = dashPressed,
                FacingRight = facingRight
            };

            _prevAttack = attack;
            _prevSkill1 = skill1;
            _prevSkill2 = skill2;
            _prevDash = dash;
            return input;
        }

        private void LatchSampledInput(BattleInput input)
        {
            _latestSampledInput = input;
            _hasSampledInput = true;

            if (input.AttackPressed)
                _pendingAttackPressed = true;

            if (input.SkillSlot > 0 && _pendingSkillSlot == 0)
                _pendingSkillSlot = input.SkillSlot;

            if (input.DashPressed)
                _pendingDashPressed = true;

            if (_isOnline && (input.AttackPressed || input.SkillSlot > 0 || input.DashPressed))
            {
                _engine.TryPredictLocalAction(input);
                _networkSyncAccumulator = 0f;
                _ = SendLocalStateAsync();
            }
        }

        private bool TryCreateImmediateActionInput(Keys key, out BattleInput input)
        {
            input = new BattleInput { PlayerId = _localPlayerId };

            bool attackPressed = key == Keys.J;
            int skillSlot = key == Keys.U ? 1 : key == Keys.I ? 2 : 0;
            bool dashPressed = key == Keys.K;
            if (!attackPressed && skillSlot == 0 && !dashPressed)
                return false;

            var mv = _engine.Player.Get<MovementComponent>();
            bool left = IsGameKeyDown(Keys.A, hasInputFocus: true);
            bool right = IsGameKeyDown(Keys.D, hasInputFocus: true);
            float screenMoveX = right == left ? 0f : right ? 1f : -1f;
            bool screenFacingRight = screenMoveX > 0f || (screenMoveX == 0f && mv.FacingRight);

            input = new BattleInput
            {
                PlayerId = _localPlayerId,
                Sequence = 0,
                ClientTick = _clientTick,
                MoveX = _mirrorView ? -screenMoveX : screenMoveX,
                JumpPressed = false,
                BlockHeld = false,
                AttackPressed = attackPressed,
                SkillSlot = skillSlot,
                DashPressed = dashPressed,
                FacingRight = _mirrorView ? !screenFacingRight : screenFacingRight
            };

            return true;
        }

        private BattleInput CreateNetworkInput()
        {
            if (!_hasSampledInput)
                LatchSampledInput(SampleBattleInput());

            var input = new BattleInput
            {
                PlayerId = _localPlayerId,
                Sequence = ++_inputSequence,
                ClientTick = _clientTick,
                MoveX = _latestSampledInput.MoveX,
                JumpPressed = _latestSampledInput.JumpPressed,
                BlockHeld = _latestSampledInput.BlockHeld,
                AttackPressed = _pendingAttackPressed,
                SkillSlot = _pendingSkillSlot,
                DashPressed = _pendingDashPressed,
                FacingRight = _latestSampledInput.FacingRight
            };

            _pendingAttackPressed = false;
            _pendingSkillSlot = 0;
            _pendingDashPressed = false;

            return input;
        }

        private static bool IsGameKeyDown(Keys key, bool hasInputFocus)
        {
            return hasInputFocus && InputManager.IsKeyDown(key);
        }

        private bool IsForegroundGameWindow()
        {
            if (IsDisposed || !Visible || WindowState == FormWindowState.Minimized)
                return false;

            IntPtr foreground = GetForegroundWindow();
            return foreground == Handle || IsChild(Handle, foreground);
        }

        private static bool IsTrackedGameKey(Keys key)
        {
            return key is Keys.A or Keys.D or Keys.S or Keys.J or Keys.U or Keys.I or Keys.K
                or Keys.Left or Keys.Right or Keys.Down
                or Keys.NumPad1 or Keys.NumPad2 or Keys.NumPad4 or Keys.NumPad5
                or Keys.D1 or Keys.D2 or Keys.D4 or Keys.D5
                or Keys.End or Keys.Clear;
        }

        private async Task SendLegacyGameStateAsync()
        {
            var mv = _engine.Player.Get<MovementComponent>();
            var ch = _engine.Player.Get<CharacterComponent>();
            var sp = _engine.Player.Get<SpriteComponent>();
            var enemyCh = _engine.Enemy?.Get<CharacterComponent>();

            await NetworkManager.Instance.SendAsync(new GameStatePacket
            {
                X = mv.X,
                Y = mv.Y,
                VelocityX = mv.VelocityX,
                VelocityY = mv.VelocityY,
                FacingRight = mv.FacingRight,
                IsGrounded = mv.IsGrounded,
                Hp = ch.Hp,
                Mana = ch.Mana,
                EnemyHp = enemyCh?.Hp ?? 0,
                EnemyMana = enemyCh?.Mana ?? 0,
                IsProtecting = ch.IsProtecting,
                IsAttacking = ch.IsAttacking,
                IsUsingSkill = ch.IsUsingSkill,
                CurrentSkillSlot = ch.CurrentSkillSlot,
                CurrentSkillAnimation = ch.CurrentSkillAnim,
                IsDashing = ch.IsDashing,
                IsHurt = ch.IsHurt,
                IsStunned = ch.IsStunned,
                StunTimer = ch.StunTimer,
                IsDead = ch.IsDead,
                CurrentAnimation = sp.CurrentAnimation,
                CurrentFrame = sp.CurrentFrame
            });
        }

        private async Task ListenForRealtimePacketsAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Packet? packet = await NetworkManager.Instance.TryReceiveAsync(
                        timeoutMs: 250,
                        token: token,
                        acceptPacket: p => p.Type == PacketType.WorldState
                            || p.Type == PacketType.GameOver
                            || p.Type == PacketType.Victory
                            || p.Type == PacketType.Disconnect);

                    if (packet == null)
                    {
                        await Task.Delay(25, token);
                        continue;
                    }

                    if (token.IsCancellationRequested)
                        return;

                    if (IsDisposed || !IsHandleCreated)
                        return;

                    BeginInvoke(new Action(() => HandleRealtimePacket(packet)));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"[ListenForRealtimePacketsAsync/IO] {ex}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ListenForRealtimePacketsAsync] {ex}");
            }
        }

        private void HandleRealtimePacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.GameState:
                    ApplyRemoteState((GameStatePacket)packet);
                    break;
                case PacketType.WorldState:
                    ApplyWorldState((WorldStatePacket)packet);
                    break;
                case PacketType.GameOver:
                    HandleGameOver((GameOverPacket)packet);
                    break;
                case PacketType.Victory:
                    HandleVictory((VictoryPacket)packet);
                    break;
                case PacketType.Disconnect:
                    HandleOpponentDisconnected();
                    break;
            }
        }

        private async void HandleOpponentDisconnected()
        {
            if (_navigatingAway || IsDisposed)
                return;

            _remoteDisconnected = true;
            _navigatingAway = true;

            MessageBox.Show(
                "Đối thủ đã thoát trận. Bạn sẽ quay lại danh sách phòng.",
                "Match Ended",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            await LeaveRoomIfNeededAsync();

            ShowExitDestination();

            Close();
        }

        private void ApplyWorldState(WorldStatePacket packet)
        {
            var state = packet.State;
            int incomingRound = state.RoundNumber <= 0 ? 1 : state.RoundNumber;
            if (incomingRound != _currentRound)
            {
                _currentRound = incomingRound;
                ClearGameInputState();
                _engine.ClearLocalActionPrediction();
                StartRoundIntro();
            }
            _roundSecondsRemaining = Math.Max(0f, state.RoundSecondsRemaining);
            _isSuddenDeath = state.IsSuddenDeath;
            _mirrorView = state.Player1.PlayerId != _localPlayerId;
            _engine.ApplyOnlineWorldState(state, _localPlayerId, _mirrorView);
        }

        private async void HandleGameOver(GameOverPacket packet)
        {
            if (_navigatingAway || IsDisposed)
                return;

            _remoteDisconnected = true;
            _navigatingAway = true;

            await LeaveRoomIfNeededAsync();

            var gameOverForm = new GameOverForm();
            gameOverForm.Show();
            _destinationOpened = true;
            Close();
        }

        private async void HandleVictory(VictoryPacket packet)
        {
            if (_navigatingAway || IsDisposed)
                return;

            _remoteDisconnected = true;
            _navigatingAway = true;

            await LeaveRoomIfNeededAsync();

            var victoryForm = new VictoryForm();
            victoryForm.Show();
            _destinationOpened = true;
            Close();
        }

        private static void ApplySnapshot(BattleGame.Client.Game.Core.Entity entity, PlayerBattleState snapshot)
        {
            var mv = entity.Get<MovementComponent>();
            var ch = entity.Get<CharacterComponent>();
            var sp = entity.Get<SpriteComponent>();

            mv.X = snapshot.X;
            mv.Y = snapshot.Y;
            mv.VelocityX = snapshot.VelocityX;
            mv.VelocityY = snapshot.VelocityY;
            mv.FacingRight = snapshot.FacingRight;
            mv.IsGrounded = snapshot.IsGrounded;

            ch.Hp = Math.Clamp(snapshot.Hp, 0, ch.BaseStats.Hp);
            ch.Mana = Math.Clamp(snapshot.Mana, 0, ch.BaseStats.Mana);
            ch.IsProtecting = snapshot.IsProtecting;
            ch.IsAttacking = snapshot.IsAttacking;
            ch.IsUsingSkill = snapshot.IsUsingSkill;
            ch.IsHurt = snapshot.IsHurt;
            ch.IsStunned = snapshot.IsStunned;
            ch.StunTimer = snapshot.StunTimer;
            ch.HurtTimer = snapshot.HurtTimer;
            ch.IsDead = snapshot.IsDead;

            if (!string.IsNullOrWhiteSpace(snapshot.CurrentAnimation))
                sp.CurrentAnimation = snapshot.CurrentAnimation;

            sp.CurrentFrame = Math.Max(0, snapshot.CurrentFrame);
        }

        private void ApplyRemoteState(GameStatePacket remote)
        {
            if (_engine.Enemy == null)
                return;

            var enemyMv = _engine.Enemy.Get<MovementComponent>();
            var enemyCh = _engine.Enemy.Get<CharacterComponent>();
            var enemySp = _engine.Enemy.Get<SpriteComponent>();
            var localCh = _engine.Player.Get<CharacterComponent>();

            enemyMv.X = remote.X;
            enemyMv.Y = remote.Y;
            enemyMv.VelocityX = remote.VelocityX;
            enemyMv.VelocityY = remote.VelocityY;
            enemyMv.FacingRight = remote.FacingRight;
            enemyMv.IsGrounded = remote.IsGrounded;

            enemyCh.Hp = Math.Clamp(remote.Hp, 0, enemyCh.BaseStats.Hp);
            enemyCh.Mana = Math.Clamp(remote.Mana, 0, enemyCh.BaseStats.Mana);
            enemyCh.IsProtecting = remote.IsProtecting;
            enemyCh.IsAttacking = remote.IsAttacking;
            enemyCh.IsUsingSkill = remote.IsUsingSkill;
            enemyCh.CurrentSkillSlot = remote.CurrentSkillSlot;
            enemyCh.CurrentSkillAnim = remote.CurrentSkillAnimation;
            enemyCh.IsDashing = remote.IsDashing;
            enemyCh.IsHurt = remote.IsHurt;
            enemyCh.IsStunned = remote.IsStunned;
            enemyCh.StunTimer = remote.StunTimer;
            enemyCh.IsDead = remote.IsDead;

            if (!string.IsNullOrWhiteSpace(remote.CurrentAnimation))
                enemySp.CurrentAnimation = remote.CurrentAnimation;

            enemySp.CurrentFrame = Math.Max(0, remote.CurrentFrame);

            localCh.Hp = Math.Clamp(remote.EnemyHp, 0, localCh.BaseStats.Hp);
            localCh.Mana = Math.Clamp(remote.EnemyMana, 0, localCh.BaseStats.Mana);
        }

        private void UpdateRoundTimer(float deltaTime)
        {
            if (!_isOnline && (_offlineRoundResolving || _offlineMatchEnded || _roundIntroTimer > 0f))
                return;

            _roundSecondsRemaining = Math.Max(0f, _roundSecondsRemaining - deltaTime);
        }

        private void UpdateRoundIntro(float deltaTime)
        {
            if (_roundIntroTimer > 0f)
                _roundIntroTimer = Math.Max(0f, _roundIntroTimer - deltaTime);
        }

        private void UpdateOfflineRoundState()
        {
            if (_isOnline || _isDungeonMap || _offlineRoundResolving || _offlineMatchEnded)
                return;

            var player = _engine.Player.Get<CharacterComponent>();
            var enemy = _engine.Enemy?.Get<CharacterComponent>();
            if (enemy == null)
                return;

            bool playerDead = player.IsDead || player.Hp <= 0;
            bool enemyDead = enemy.IsDead || enemy.Hp <= 0;
            if (!playerDead && !enemyDead && _roundSecondsRemaining > 0f)
                return;

            int winner = ResolveOfflineRoundWinner(player, enemy, playerDead, enemyDead);
            if (winner == 1)
                _player1RoundWins++;
            else if (winner == 2)
                _player2RoundWins++;

            _offlineRoundResolving = true;
            _ = ResolveOfflineRoundAsync(winner);
        }

        private void UpdateDungeonRunState()
        {
            if (!_isDungeonMap || _dungeonRunEnded)
                return;

            ApplyPendingDungeonVictoryStats();

            var player = _engine.Player.Get<CharacterComponent>();
            bool playerDead = player.IsDead || player.Hp <= 0;
            if (playerDead)
            {
                _dungeonRunEnded = true;
                ApplyDungeonFailureStatPenalty();
                _ = ResolveDungeonRunAsync(victory: false);
                return;
            }

            if (_engine.IsDungeonCompleted)
            {
                _dungeonRunEnded = true;
                _ = ResolveDungeonRunAsync(victory: true);
            }
        }

        private async Task ResolveDungeonRunAsync(bool victory)
        {
            await Task.Delay(ResolveRoundEndDelay(victory ? _engine.Enemy : _engine.Player));
            if (IsDisposed)
                return;

            ApplyPendingDungeonVictoryStats();
            string savedStatsMessage = PersistDungeonStatChanges();
            string title = victory ? "Dungeon Clear" : "Dungeon Failed";
            string message = victory
                ? "Bạn đã đánh bại toàn bộ boss trong stage này."
                : "Bạn đã bị boss hạ gục. Hãy chọn lại stage hoặc đổi nhân vật để thử lại.";
            MessageBoxIcon icon = victory ? MessageBoxIcon.Information : MessageBoxIcon.Warning;

            if (!string.IsNullOrWhiteSpace(savedStatsMessage))
                message += Environment.NewLine + Environment.NewLine + savedStatsMessage;

            MessageBox.Show(this, message, title, MessageBoxButtons.OK, icon);
            ShowExitDestination();
            Close();
        }

        private void ApplyPendingDungeonVictoryStats()
        {
            if (!_dungeonStatMode)
                return;

            int defeatedCount = _engine.DungeonDefeatedCount;
            while (_lastDungeonDefeatedCount < defeatedCount)
            {
                _lastDungeonDefeatedCount++;
                ApplyRandomDungeonStatDelta(2);
            }
        }

        private void ApplyDungeonFailureStatPenalty()
        {
            if (!_dungeonStatMode || _dungeonFailureStatApplied)
                return;

            _dungeonFailureStatApplied = true;
            ApplyRandomDungeonStatDelta(-1);
        }

        private void ApplyRandomDungeonStatDelta(int amount)
        {
            var stat = (DungeonStat)_dungeonStatRandom.Next(0, 4);
            var player = _engine.Player.Get<CharacterComponent>();
            var movement = _engine.Player.Get<MovementComponent>();
            string sign = amount > 0 ? "+" : string.Empty;
            _dungeonPendingStatDeltas[stat] = _dungeonPendingStatDeltas.GetValueOrDefault(stat) + amount;

            switch (stat)
            {
                case DungeonStat.Hp:
                    player.BaseStats.Hp = Math.Max(1, player.BaseStats.Hp + amount);
                    player.Hp = amount > 0
                        ? Math.Min(player.BaseStats.Hp, player.Hp + amount)
                        : Math.Clamp(player.Hp, 0, player.BaseStats.Hp);
                    _dungeonStatMessage = $"HP {sign}{amount}";
                    break;

                case DungeonStat.Atk:
                    player.BaseStats.Atk = Math.Max(1f, player.BaseStats.Atk + amount);
                    _dungeonStatMessage = $"ATK {sign}{amount}";
                    break;

                case DungeonStat.Def:
                    player.BaseStats.Def = Math.Max(0, player.BaseStats.Def + amount);
                    _dungeonStatMessage = $"DEF {sign}{amount}";
                    break;

                case DungeonStat.Speed:
                    player.BaseStats.Speed = Math.Max(50f, player.BaseStats.Speed + amount);
                    movement.Speed = player.BaseStats.Speed;
                    _dungeonStatMessage = $"SPD {sign}{amount}";
                    break;
            }

            _dungeonStatMessageTimer = DungeonStatMessageSeconds;
            UpdateUIBars();
        }

        private string PersistDungeonStatChanges()
        {
            if (!_dungeonStatMode || _dungeonPendingStatDeltas.Count == 0)
                return string.Empty;

            try
            {
                string contentRoot = ClientContentRoot.Resolve(AppDomain.CurrentDomain.BaseDirectory);
                string configPath = CharacterDefinitionLoader.ResolveConfigPath(contentRoot, _playerCharacterId);
                JsonNode? root = JsonNode.Parse(File.ReadAllText(configPath));
                JsonObject? stats = root?["stats"] as JsonObject;
                if (root == null || stats == null)
                    return "Không thể lưu chỉ số nhân vật.";

                List<string> changes = new();
                foreach (var kv in _dungeonPendingStatDeltas)
                {
                    if (kv.Value == 0)
                        continue;

                    string propertyName = kv.Key switch
                    {
                        DungeonStat.Hp => "hp",
                        DungeonStat.Atk => "atk",
                        DungeonStat.Def => "def",
                        DungeonStat.Speed => "speed",
                        _ => string.Empty
                    };

                    if (string.IsNullOrWhiteSpace(propertyName))
                        continue;

                    double current = stats[propertyName]?.GetValue<double>() ?? 0d;
                    double next = kv.Key switch
                    {
                        DungeonStat.Hp => Math.Max(1d, current + kv.Value),
                        DungeonStat.Atk => Math.Max(1d, current + kv.Value),
                        DungeonStat.Def => Math.Max(0d, current + kv.Value),
                        DungeonStat.Speed => Math.Max(50d, current + kv.Value),
                        _ => current
                    };

                    if (kv.Key is DungeonStat.Hp or DungeonStat.Def)
                        stats[propertyName] = (int)Math.Round(next);
                    else
                        stats[propertyName] = Math.Round(next, 2);

                    string label = kv.Key == DungeonStat.Speed ? "SPD" : kv.Key.ToString().ToUpperInvariant();
                    string sign = kv.Value > 0 ? "+" : string.Empty;
                    changes.Add($"{label} {sign}{kv.Value}");
                }

                File.WriteAllText(configPath, root.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

                _dungeonPendingStatDeltas.Clear();
                return changes.Count == 0
                    ? string.Empty
                    : $"Chỉ số đã được lưu thật: {string.Join(", ", changes)}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DungeonStatPersist] {ex}");
                return "Không thể lưu chỉ số nhân vật.";
            }
        }

        private void UpdateDungeonStatMessage(float dt)
        {
            if (_dungeonStatMessageTimer <= 0f)
                return;

            _dungeonStatMessageTimer = Math.Max(0f, _dungeonStatMessageTimer - dt);
            if (_dungeonStatMessageTimer <= 0f)
                _dungeonStatMessage = string.Empty;
        }

        private TimeSpan ResolveRoundEndDelay(BattleGame.Client.Game.Core.Entity? defeated)
        {
            if (defeated == null)
                return TimeSpan.FromSeconds(PostDeathRoundDelaySeconds);

            var ch = defeated.Get<CharacterComponent>();
            if (!ch.IsDead)
                return TimeSpan.FromSeconds(PostDeathRoundDelaySeconds);

            float deadSeconds = ch.GetAnimationDuration("Dead", DungeonResolveDelaySeconds);
            return TimeSpan.FromSeconds(deadSeconds + PostDeathRoundDelaySeconds);
        }

        private enum DungeonStat
        {
            Hp,
            Atk,
            Def,
            Speed
        }

        private int ResolveOfflineRoundWinner(CharacterComponent player, CharacterComponent enemy, bool playerDead, bool enemyDead)
        {
            if (playerDead && !enemyDead)
                return 2;
            if (enemyDead && !playerDead)
                return 1;
            if (player.Hp > enemy.Hp)
                return 1;
            if (enemy.Hp > player.Hp)
                return 2;
            return 0;
        }

        private async Task ResolveOfflineRoundAsync(int winner)
        {
            await Task.Delay(ResolveOfflineRoundEndDelay(winner));
            if (IsDisposed)
                return;

            if (_player1RoundWins >= 2 || _player2RoundWins >= 2 || _currentRound >= 3)
            {
                ShowOfflineMatchResult();
                return;
            }

            _currentRound++;
            _roundSecondsRemaining = 180f;
            _isSuddenDeath = false;
            _engine = CreateGameEngine();
            ClearGameInputState();
            UpdateCharacterHeaders();
            UpdateUIBars();
            _offlineRoundResolving = false;
            StartRoundIntro();
        }

        private void ShowOfflineMatchResult()
        {
            _offlineMatchEnded = true;
            _offlineRoundResolving = false;

            string result = _player1RoundWins == _player2RoundWins
                ? "DRAW"
                : _player1RoundWins > _player2RoundWins
                    ? "PLAYER 1 WINS"
                    : "PLAYER 2 WINS";

            using (var resultForm = new OfflineMatchResultForm(result, _player1RoundWins, _player2RoundWins))
            {
                resultForm.ShowDialog(this);
            }

            ShowExitDestination();
            Close();
        }

        private TimeSpan ResolveOfflineRoundEndDelay(int winner)
        {
            BattleGame.Client.Game.Core.Entity? defeated = winner switch
            {
                1 => _engine.Enemy,
                2 => _engine.Player,
                _ => null
            };

            if (defeated == null)
                return TimeSpan.FromSeconds(OfflineRoundResolveDelaySeconds);

            return ResolveRoundEndDelay(defeated);
        }

        private static string FormatTime(int totalSeconds)
        {
            int minutes = Math.Clamp(totalSeconds / 60, 0, 99);
            int remainder = Math.Clamp(totalSeconds % 60, 0, 59);
            return $"{minutes:00}:{remainder:00}";
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutHud();
            if (_engine != null)
            {
                _engine.Resize(ClientSize.Width, ClientSize.Height);
            }
            CreateBackBuffer();
        }

        private void LayoutHud()
        {
            if (panelStatus == null)
                return;

            bool localOnLeft = true;
            LayoutHudSide(
                pictureBox1,
                label3,
                panelHPBack,
                panelManaBack,
                localOnLeft);
            LayoutHudSide(
                pictureBox2,
                label4,
                panel3,
                panel1,
                !localOnLeft);
            LayoutDungeonBossHud();
            ApplyDungeonHudVisibility();
            CenterHudValueLabel(panelHPBack, lblHP);
            CenterHudValueLabel(panelManaBack, lblMana);
            CenterHudValueLabel(panel3, label6);
            CenterHudValueLabel(panel1, label5);
            LayoutSkillCooldownLabels();

            panelStatus.Location = new Point(
                Math.Max(HudMargin, (ClientSize.Width - StatusPanelWidth) / 2),
                28);

            // Keep EXIT button inside round frame, below timer text.
            btnExit.Location = new Point(
                panelStatus.Left + (panelStatus.Width - btnExit.Width) / 2,
                panelStatus.Top + 88);
            btnExit.Visible = !_isDungeonMap;
            if (btnExit.Visible)
                btnExit.BringToFront();
        }

        private void LayoutHudSide(PictureBox portrait, Label nameLabel, Panel hpPanel, Panel manaPanel, bool isLeft)
        {
            int portraitX = isLeft
                ? HudMargin
                : ClientSize.Width - HudMargin - HudPortraitWidth;
            int nameY = isLeft ? 38 : 42;
            int hudX = isLeft
                ? HudMargin
                : Math.Max(HudMargin, ClientSize.Width - HudMargin - HudPanelWidth);

            portrait.Location = new Point(portraitX, 12);
            nameLabel.Location = new Point(
                isLeft ? portrait.Right + 6 : Math.Max(HudMargin, portrait.Left - nameLabel.Width - 6),
                nameY);
            hpPanel.Location = new Point(hudX, 95);
            manaPanel.Location = new Point(hudX, 132);
        }

        private void LayoutDungeonBossHud()
        {
            if (!_isDungeonMap)
                return;

            int panelX = Math.Max(HudMargin, ClientSize.Width - HudMargin - BossHudPanelWidth);
            label4.AutoSize = false;
            label4.TextAlign = ContentAlignment.MiddleRight;
            label4.Location = new Point(panelX, 18);
            label4.Size = new Size(BossHudPanelWidth, 32);
            panel3.Location = new Point(panelX, 56);
            panel3.Size = new Size(BossHudPanelWidth, BossHudPanelHeight);
        }

        private void LayoutSkillCooldownLabels()
        {
            if (_skill1CooldownLabel == null || _skill2CooldownLabel == null ||
                _enemySkill1CooldownLabel == null || _enemySkill2CooldownLabel == null)
                return;

            LayoutCooldownPair(_skill1CooldownLabel, _skill2CooldownLabel, panelManaBack, "left");
            LayoutCooldownPair(_enemySkill1CooldownLabel, _enemySkill2CooldownLabel, panel1, "right");
        }

        private void LayoutCooldownPair(Label first, Label second, Panel anchor, string side)
        {
            int gap = 6;
            int top = anchor.Bottom + 4;
            int labelWidth = Math.Max(80, (anchor.Width - gap) / 2);
            int labelHeight = 22;

            first.Location = new Point(anchor.Left, top);
            first.Size = new Size(labelWidth, labelHeight);
            second.Location = new Point(anchor.Left + labelWidth + gap, top);
            second.Size = new Size(labelWidth, labelHeight);
            first.Visible = side == "left" || (!_isDungeonMap && _localTwoPlayer);
            second.Visible = first.Visible;
            first.BringToFront();
            second.BringToFront();
        }

        private void UpdateUIBars()
        {
            try
            {
                var playerChar = _engine.Player.Get<CharacterComponent>();
                if (playerChar != null)
                {
                    SetHudBar(panelHPBack, panelHPFill, lblHP, playerChar.Hp, playerChar.BaseStats.Hp);
                    SetHudBar(panelManaBack, panelManaFill, lblMana, playerChar.Mana, playerChar.BaseStats.Mana);
                    UpdateSkillCooldownLabels(playerChar);
                }

                var enemyChar = _engine.Enemy?.Get<CharacterComponent>();
                if (enemyChar != null && !_isDungeonMap)
                {
                    SetHudBar(panel3, panel4, label6, enemyChar.Hp, enemyChar.BaseStats.Hp);
                    SetHudBar(panel1, panel2, label5, enemyChar.Mana, enemyChar.BaseStats.Mana);
                    UpdateEnemySkillCooldownLabels(enemyChar);
                }
                else if (enemyChar != null && _isDungeonMap)
                {
                    UpdateDungeonBossHud(enemyChar);
                    SetHudBar(panel3, panel4, label6, enemyChar.Hp, enemyChar.BaseStats.Hp);
                }

                ApplyDungeonHudVisibility();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateUIBars] {ex}");
            }
        }

        private void UpdateDungeonBossHud(CharacterComponent boss)
        {
            if (!string.Equals(_currentBossHudCharacterId, boss.CharacterId, StringComparison.OrdinalIgnoreCase))
            {
                _currentBossHudCharacterId = boss.CharacterId;
                label4.Text = ResolveCharacterDisplayName(boss.CharacterId);
            }

            label4.BringToFront();
            panel3.BringToFront();
        }

        private static string ResolveCharacterDisplayName(string characterId)
        {
            try
            {
                string contentRoot = ClientContentRoot.Resolve(AppDomain.CurrentDomain.BaseDirectory);
                string configPath = CharacterDefinitionLoader.ResolveConfigPath(contentRoot, characterId);
                return CharacterDefinitionLoader.Load(configPath).Selection.DisplayName;
            }
            catch
            {
                return CharacterCatalog.ToDisplayName(characterId);
            }
        }

        private void SetHudBar(Panel backPanel, Panel fillPanel, Label valueLabel, int value, int maxValue)
        {
            int clampedMax = Math.Max(0, maxValue);
            int clampedValue = clampedMax > 0 ? Math.Clamp(value, 0, clampedMax) : 0;
            string text = $"{clampedValue}/{clampedMax}";
            if (valueLabel.Text != text)
                valueLabel.Text = text;

            if (!_hudBars.TryGetValue(backPanel, out var state))
            {
                ConfigureHudBar(backPanel, fillPanel, valueLabel);
                state = _hudBars[backPanel];
            }

            state.Text = text;
            state.Ratio = clampedMax > 0 ? clampedValue / (float)clampedMax : 0f;
            state.FillColor = fillPanel.BackColor;
            state.BackColor = backPanel.BackColor;
            state.Font = valueLabel.Font;
            state.TextColor = IsManaBar(backPanel) ? Color.WhiteSmoke : Color.Black;
            backPanel.Invalidate();
        }

        private bool IsManaBar(Panel panel)
            => panel == panelManaBack || panel == panel1;

        private void UpdateSkillCooldownLabels(CharacterComponent character)
        {
            if (_skill1CooldownLabel == null || _skill2CooldownLabel == null)
                ConfigureSkillCooldownLabels();

            if (_skill1CooldownLabel == null || _skill2CooldownLabel == null)
                return;

            SetSkillCooldownLabel(_skill1CooldownLabel, "U", character.Skill1, character.Skill1Cooldown, character.Mana);
            SetSkillCooldownLabel(_skill2CooldownLabel, "I", character.Skill2, character.Skill2Cooldown, character.Mana);
        }

        private void UpdateEnemySkillCooldownLabels(CharacterComponent character)
        {
            if (_enemySkill1CooldownLabel == null || _enemySkill2CooldownLabel == null)
                ConfigureSkillCooldownLabels();

            if (_enemySkill1CooldownLabel == null || _enemySkill2CooldownLabel == null)
                return;

            SetSkillCooldownLabel(_enemySkill1CooldownLabel, _localTwoPlayer ? "4" : "S1", character.Skill1, character.Skill1Cooldown, character.Mana);
            SetSkillCooldownLabel(_enemySkill2CooldownLabel, _localTwoPlayer ? "5" : "S2", character.Skill2, character.Skill2Cooldown, character.Mana);
        }

        private static void SetSkillCooldownLabel(Label label, string key, SkillData? skill, float cooldown, int mana)
        {
            if (skill == null)
            {
                label.Text = $"{key}: --";
                label.ForeColor = Color.Gainsboro;
                return;
            }

            if (cooldown > 0.05f)
            {
                label.Text = $"{key}: {cooldown:0.0}s";
                label.ForeColor = Color.Gold;
                return;
            }

            if (mana < skill.ManaCost)
            {
                label.Text = $"{key}: MANA";
                label.ForeColor = Color.DeepSkyBlue;
                return;
            }

            label.Text = $"{key}: READY";
            label.ForeColor = Color.LightGreen;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _isRunning = false;
            Application.Idle -= OnApplicationIdle;
            _networkCts?.Cancel();
            _networkCts?.Dispose();
            _networkCts = null;

            pictureBox1.Image = null;
            pictureBox2.Image = null;

            foreach (var img in _imageCache.Values) img.Dispose();
            _imageCache.Clear();

            _backGraphics?.Dispose();
            _backBuffer?.Dispose();

            InputManager.Clear();

            if (!_destinationOpened && _returnFormOnExit != null && !_returnFormOnExit.IsDisposed)
            {
                _returnFormOnExit.Show();
                _destinationOpened = true;
            }

            base.OnFormClosed(e);

        }

        private async void GameForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            await SendDisconnectIfNeededAsync();
            await LeaveRoomIfNeededAsync();
        }

        private async Task LeaveRoomIfNeededAsync()
        {
            if (_roomLeaveSent || !_isOnline || _roomId is null)
                return;

            _roomLeaveSent = true;
            JoinRoom.MarkOwnedRoomLeft(_roomId.Value);

            if (!NetworkManager.Instance.IsConnected)
                return;

            try
            {
                await NetworkManager.Instance.LeaveRoomAsync(new LeaveRoomPacket
                {
                    RoomId = _roomId.Value
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LeaveRoomIfNeededAsync] {ex}");
            }
        }

        private async void btnExit_Click(object sender, EventArgs e)
        {
            if (_navigatingAway || IsDisposed)
                return;

            _navigatingAway = true;

            try
            {
                await SendDisconnectIfNeededAsync();
                await LeaveRoomIfNeededAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[btnExit_Click] {ex}");
            }

            ShowExitDestination();
            Close();
        }

        private void ShowExitDestination()
        {
            if (_destinationOpened)
                return;

            if (_returnFormOnExit != null && !_returnFormOnExit.IsDisposed)
                _returnFormOnExit.Show();
            else
            {
                var joinRoom = new JoinRoom();
                joinRoom.Show();
            }

            _destinationOpened = true;
        }

        private async Task SendDisconnectIfNeededAsync()
        {
            if (!_isOnline || _remoteDisconnected || !NetworkManager.Instance.IsConnected)
                return;

            try
            {
                await NetworkManager.Instance.DisconnectAsync(new DisconnectPacket());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SendDisconnectIfNeededAsync] {ex}");
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            ClearGameInputState();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Activate();
            Focus();
        }

        private void ClearGameInputState()
        {
            InputManager.Clear();
            _prevAttack = false;
            _prevSkill1 = false;
            _prevSkill2 = false;
            _prevDash = false;
            _pendingAttackPressed = false;
            _pendingSkillSlot = 0;
            _pendingDashPressed = false;
            _hasSampledInput = false;
            _latestSampledInput = new BattleInput { PlayerId = _localPlayerId };
        }

        private static readonly Keys[] TrackedKeys =
        {
            Keys.A, Keys.D, Keys.S, Keys.J, Keys.U, Keys.I, Keys.K,
            Keys.Left, Keys.Right, Keys.Down,
            Keys.NumPad1, Keys.NumPad2, Keys.NumPad4, Keys.NumPad5,
            Keys.D1, Keys.D2, Keys.D4, Keys.D5,
            Keys.End, Keys.Clear
        };

        private sealed class HudBarState
        {
            public HudBarState(Color fillColor, Color backColor, Font font)
            {
                FillColor = fillColor;
                BackColor = backColor;
                Font = font;
            }

            public string Text { get; set; } = "0/0";
            public float Ratio { get; set; }
            public Color FillColor { get; set; }
            public Color BackColor { get; set; }
            public Font Font { get; set; }
            public Color TextColor { get; set; } = Color.Black;
        }

        private sealed class CombatantAudioState
        {
            public bool Initialized { get; private set; }
            public int Hp { get; private set; }
            public bool IsAttacking { get; private set; }
            public bool IsUsingSkill { get; private set; }
            public bool IsDashing { get; private set; }
            public bool IsProtecting { get; private set; }

            public void Capture(CharacterComponent character)
            {
                Initialized = true;
                Hp = character.Hp;
                IsAttacking = character.IsAttacking;
                IsUsingSkill = character.IsUsingSkill;
                IsDashing = character.IsDashing;
                IsProtecting = character.IsProtecting;
            }

            public void Reset()
            {
                Initialized = false;
                Hp = 0;
                IsAttacking = false;
                IsUsingSkill = false;
                IsDashing = false;
                IsProtecting = false;
            }
        }

    }
}
