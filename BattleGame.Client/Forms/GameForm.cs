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
using BattleGame.Client.Game.Core.Components;
using BattleGame.Shared.Packets;
using BattleGame.Shared.Models;
using BattleGame.Shared.Simulation;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing.Text;


namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private static readonly Size DefaultGameClientSize = new(1280, 720);
        private const int HudMargin = 31;
        private const int HudPanelWidth = 281;
        private const int HudPortraitWidth = 93;
        private const int StatusPanelWidth = 304;

        private readonly GameEngine _engine;
        private readonly bool _isOnline;
        private readonly bool _isDungeonMap;
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

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

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

        public GameForm(
            string characterId,
            string mapId = "terrace",
            string? enemyCharacterId = null,
            bool isOnline = false,
            int localPlayerId = 0,
            string? localUsername = null,
            string? enemyUsername = null,
            int? roomId = null)
        {
            try
            {
                InitializeComponent();

                this.AutoScaleMode = AutoScaleMode.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.ClientSize = DefaultGameClientSize;
                this.MinimumSize = SizeFromClientSize(DefaultGameClientSize);
                this.MaximumSize = SizeFromClientSize(DefaultGameClientSize);
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
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
                _localPlayerId = localPlayerId;
                _localUsername = string.IsNullOrWhiteSpace(localUsername) ? null : localUsername.Trim();
                _enemyUsername = string.IsNullOrWhiteSpace(enemyUsername) ? null : enemyUsername.Trim();
                _roomId = roomId is > 0 ? roomId : null;
                _mirrorView = false;
                _engine = new GameEngine(characterId, mapId, this.ClientSize.Width, this.ClientSize.Height, enemyCharacterId);

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

        private void GameForm_Load(object? sender, EventArgs e)
        {
            SoundManager.PlayBGM("darren_hirst.mp3");
            SoundManager.SetVolume(0.1f);
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
            btnExit.Visible = true;
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
            if (IsTrackedGameKey(e.KeyCode))
            {
                bool wasDown = InputManager.IsKeyDown(e.KeyCode);
                InputManager.SetKey(e.KeyCode, true);

                if (_isOnline && !wasDown && TryCreateImmediateActionInput(e.KeyCode, out var actionInput))
                    LatchSampledInput(actionInput);
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
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
            => string.Equals(mapId, "cave", StringComparison.OrdinalIgnoreCase)
               || string.Equals(mapId, "stage2", StringComparison.OrdinalIgnoreCase);

        private void ApplyDungeonHudVisibility()
        {
            if (!_isDungeonMap)
                return;

            label4.Visible = false;
            pictureBox2.Visible = false;
            panel3.Visible = false;
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

            AddCooldownLabelToForm(_skill1CooldownLabel);
            AddCooldownLabelToForm(_skill2CooldownLabel);
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
            DrawRoundOverlay(_backGraphics);
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

                _frameAccumulator += dt;
                while (_frameAccumulator >= FixedTimestep)
                {
                    if (!_isOnline)
                        _engine.Update(FixedTimestep);
                    else
                    {
                        LatchSampledInput(SampleBattleInput());
                        _engine.UpdateOnlineVisuals(FixedTimestep);
                    }

                    TrySendRealtimeState(FixedTimestep);
                    _frameAccumulator -= FixedTimestep;

                    if (_isOnline)
                        _clientTick++;
                }

                UpdateRoundTimer(dt);
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
            return key is Keys.A or Keys.D or Keys.S or Keys.J or Keys.U or Keys.I or Keys.K;
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
                IsHurt = ch.IsHurt,
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

            var joinRoom = new JoinRoom();
            joinRoom.Show();
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
            enemyCh.IsHurt = remote.IsHurt;
            enemyCh.IsDead = remote.IsDead;

            if (!string.IsNullOrWhiteSpace(remote.CurrentAnimation))
                enemySp.CurrentAnimation = remote.CurrentAnimation;

            enemySp.CurrentFrame = Math.Max(0, remote.CurrentFrame);

            localCh.Hp = Math.Clamp(remote.EnemyHp, 0, localCh.BaseStats.Hp);
            localCh.Mana = Math.Clamp(remote.EnemyMana, 0, localCh.BaseStats.Mana);
        }

        private void UpdateRoundTimer(float deltaTime)
        {
            _roundSecondsRemaining = Math.Max(0f, _roundSecondsRemaining - deltaTime);
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

        private void LayoutSkillCooldownLabels()
        {
            if (_skill1CooldownLabel == null || _skill2CooldownLabel == null)
                return;

            int gap = 6;
            int top = panelManaBack.Bottom + 4;
            int labelWidth = Math.Max(80, (panelManaBack.Width - gap) / 2);
            int labelHeight = 22;

            _skill1CooldownLabel.Location = new Point(panelManaBack.Left, top);
            _skill1CooldownLabel.Size = new Size(labelWidth, labelHeight);
            _skill2CooldownLabel.Location = new Point(panelManaBack.Left + labelWidth + gap, top);
            _skill2CooldownLabel.Size = new Size(labelWidth, labelHeight);
            _skill1CooldownLabel.BringToFront();
            _skill2CooldownLabel.BringToFront();
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateUIBars] {ex}");
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

            var joinRoom = new JoinRoom();
            joinRoom.Show();
            Close();
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
            Keys.A, Keys.D, Keys.S, Keys.J, Keys.U, Keys.I, Keys.K
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

    }
}
