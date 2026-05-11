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
        private readonly int _localPlayerId;
        private int _inputSequence;
        private int _clientTick;
        private bool _prevAttack;
        private bool _prevSkill1;
        private bool _prevSkill2;
        private bool _prevDash;

        private float _roundSecondsRemaining = 180f;
        private int _currentRound = 1;

        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets");

        private static readonly string PortraitRoot = Path.Combine(AssetsRoot, "PotraitPic");

        private readonly Stopwatch _stopwatch = new();
        private float _frameAccumulator = 0f;
        private const float FixedTimestep = 1f / 60f; // Fixed 60 FPS
        private float _networkSyncAccumulator = 0f;
        private const float NetworkSyncIntervalSeconds = 1f / 15f; // 15 updates/s
        private int _sendingNetworkState;
        private bool _isRunning;
        private long _lastTicks;
        private const float MaxFrameDt = 0.05f;
        private float _uiAccumulator = 0f;
        private const float UiUpdateInterval = 0.1f;

        private CancellationTokenSource? _networkCts;
        private Task? _networkListenTask;
        private bool _navigatingAway;
        private bool _remoteDisconnected;

        private Bitmap? _backBuffer;
        private Graphics? _backGraphics;
        private readonly Dictionary<string, Image> _imageCache = new(StringComparer.OrdinalIgnoreCase);

        public GameForm(
            string characterId,
            string mapId = "terrace",
            string? enemyCharacterId = null,
            bool isOnline = false,
            int localPlayerId = 0)
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
                _localPlayerId = localPlayerId;
                _engine = new GameEngine(characterId, mapId, this.ClientSize.Width, this.ClientSize.Height, enemyCharacterId);

                CreateBackBuffer();

                Load += GameForm_Load;

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

            // Round/time are rendered directly into backbuffer to avoid WinForms control flicker.
            panelStatus.Visible = false;
            btnExit.Parent = this;
            btnExit.Visible = true;
            btnExit.BringToFront();

            UpdateUIBars();
            UpdateCharacterHeaders();

            if (_isOnline && NetworkManager.Instance.IsConnected)
            {
                _networkCts = new CancellationTokenSource();
                _networkListenTask = ListenForRealtimePacketsAsync(_networkCts.Token);
            }
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
            InputManager.SetKey(e.KeyCode, false);
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

            string roundText = $"ROUND {_currentRound}";
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
                        _engine.UpdateOnlineVisuals(FixedTimestep);

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
                return;

            try
            {
                if (!NetworkManager.Instance.IsConnected)
                    return;

                if (_isOnline)
                    await NetworkManager.Instance.SendInputAsync(new InputPacket { Input = BuildBattleInput() });
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
            }
        }

        private BattleInput BuildBattleInput()
        {
            bool left = InputManager.IsKeyDown(Keys.A);
            bool right = InputManager.IsKeyDown(Keys.D);
            bool attack = InputManager.IsKeyDown(Keys.J);
            bool skill1 = InputManager.IsKeyDown(Keys.U);
            bool skill2 = InputManager.IsKeyDown(Keys.I);
            bool dash = InputManager.IsKeyDown(Keys.K);

            var mv = _engine.Player.Get<MovementComponent>();
            float moveX = right == left ? 0f : right ? 1f : -1f;
            bool facingRight = moveX > 0f || (moveX == 0f && mv.FacingRight);

            var input = new BattleInput
            {
                PlayerId = _localPlayerId,
                Sequence = ++_inputSequence,
                ClientTick = _clientTick,
                MoveX = moveX,
                JumpPressed = false,
                BlockHeld = InputManager.IsKeyDown(Keys.S),
                AttackPressed = attack && !_prevAttack,
                SkillSlot = skill1 && !_prevSkill1 ? 1 : skill2 && !_prevSkill2 ? 2 : 0,
                DashPressed = dash && !_prevDash,
                FacingRight = facingRight
            };

            _prevAttack = attack;
            _prevSkill1 = skill1;
            _prevSkill2 = skill2;
            _prevDash = dash;

            return input;
        }

        private async Task SendLegacyGameStateAsync()
        {
            var mv = _engine.Player.Get<MovementComponent>();
            var ch = _engine.Player.Get<CharacterComponent>();
            var sp = _engine.Player.Get<SpriteComponent>();
            var enemyCh = _engine.Enemy.Get<CharacterComponent>();

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
                EnemyHp = enemyCh.Hp,
                EnemyMana = enemyCh.Mana,
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
                    break;
                case PacketType.Disconnect:
                    HandleOpponentDisconnected();
                    break;
            }
        }

        private void HandleOpponentDisconnected()
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

            var joinRoom = new JoinRoom();
            joinRoom.Show();
            Close();
        }

        private void ApplyWorldState(WorldStatePacket packet)
        {
            var state = packet.State;
            PlayerBattleState local = state.Player1.PlayerId == _localPlayerId ? state.Player1 : state.Player2;
            PlayerBattleState remote = state.Player1.PlayerId == _localPlayerId ? state.Player2 : state.Player1;

            _engine.ApplyOnlineWorldState(state, _localPlayerId);
            ApplySnapshot(_engine.Player, local);
            ApplySnapshot(_engine.Enemy, remote);
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

            int rightHudX = Math.Max(HudMargin, ClientSize.Width - HudMargin - HudPanelWidth);

            pictureBox1.Location = new Point(HudMargin, 12);
            label3.Location = new Point(pictureBox1.Right + 6, 38);
            panelHPBack.Location = new Point(HudMargin, 95);
            panelManaBack.Location = new Point(HudMargin, 132);

            pictureBox2.Location = new Point(ClientSize.Width - HudMargin - HudPortraitWidth, 10);
            label4.Location = new Point(Math.Max(HudMargin, pictureBox2.Left - label4.Width - 6), 42);
            panel3.Location = new Point(rightHudX, 94);
            panel1.Location = new Point(rightHudX, 132);

            panelStatus.Location = new Point(
                Math.Max(HudMargin, (ClientSize.Width - StatusPanelWidth) / 2),
                28);

            // Keep EXIT button inside round frame, below timer text.
            btnExit.Location = new Point(
                panelStatus.Left + (panelStatus.Width - btnExit.Width) / 2,
                panelStatus.Top + 88);
            btnExit.BringToFront();
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateUIBars] {ex}");
            }
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

        private async void btnExit_Click(object sender, EventArgs e)
        {
            if (_navigatingAway || IsDisposed)
                return;

            _navigatingAway = true;

            try
            {
                if (NetworkManager.Instance.IsConnected && !_remoteDisconnected)
                {
                    await NetworkManager.Instance.DisconnectAsync(new DisconnectPacket());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[btnExit_Click] {ex}");
            }

            var modeForm = new ModeForm();
            modeForm.Show();
            Close();
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            InputManager.Clear();
        }
    }
}
