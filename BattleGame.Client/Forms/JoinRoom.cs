using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class JoinRoom : Form
    {
        private const int MaxTimeLimitMinutes = 5;
        private const int DefaultTimeLimitMinutes = 3;
        private static readonly string[] MatchmakingMapIds = { "terrace", "throneroom", "castle" };
        private string _selectedMapId = "terrace";
        private static readonly HashSet<int> OwnedRoomIds = new();
        private static readonly HashSet<int> OwnedPublicRoomsAwaitingJoin = new();
        private static readonly Dictionary<int, (string MapId, int TimeLimitMinutes)> OwnedRoomSettings = new();
        private static readonly Dictionary<int, string> OwnedRoomPasswords = new();
        private static readonly Dictionary<int, Room> OwnedRoomCache = new();
        private readonly SemaphoreSlim _roomRequestGate = new(1, 1);
        private readonly Image? _unlockRoomIcon;
        private bool _isRefreshingRooms;
        private string _lastRenderSignature = string.Empty;
        private DateTime _lastRefreshUtc = DateTime.MinValue;
        private static readonly TimeSpan ActivationRefreshCooldown = TimeSpan.FromMilliseconds(900);
        public JoinRoom()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            button2.Click += button2_Click;
            Shown += JoinRoom_Shown;
            Activated += JoinRoom_Activated;
            findroom.Click += FindRoom_Click;
            AcceptButton = btnCreateRoom;
            txtRoomName.KeyDown += CreateRoomInput_KeyDown;
            txtPass.KeyDown += CreateRoomInput_KeyDown;
            textBox1.KeyDown += CreateRoomInput_KeyDown;
            _unlockRoomIcon = LoadOptionalImage("Assets", "Background", "unlock.png");
        }

        private async void JoinRoom_Shown(object? sender, EventArgs e)
        {
            if (!NetworkManager.Instance.IsConnected)
                return;

            try
            {
                await RefreshRoomsWithRetryAsync();
            }
            catch
            {
                RenderRooms(new List<Room>());
            }
        }

        private async void JoinRoom_Activated(object? sender, EventArgs e)
        {
            if (!NetworkManager.Instance.IsConnected)
                return;

            if (DateTime.UtcNow - _lastRefreshUtc < ActivationRefreshCooldown)
                return;

            try
            {
                await RefreshRoomsAsync();
            }
            catch
            {
            }
        }

        public Task RefreshRoomsFromServerAsync()
        {
            if (InvokeRequired)
            {
                var tcs = new TaskCompletionSource<bool>();
                BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        await RefreshRoomsWithRetryAsync();
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }));
                return tcs.Task;
            }

            return RefreshRoomsWithRetryAsync();
        }
        public class Room
        {
            public string Name { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int CurrentPlayer { get; set; }
            public string MapId { get; set; } = "terrace";
            public int MapIdNetwork { get; set; } = -1;
            public int TimeLimitMinutes { get; set; }
            public bool HasPassword { get; set; }
            public bool IsOwner { get; set; }
        }

        public static void MarkOwnedRoomLeft(int roomId)
        {
            if (OwnedRoomIds.Contains(roomId))
            {
                OwnedRoomIds.Remove(roomId);
                OwnedPublicRoomsAwaitingJoin.Remove(roomId);
                OwnedRoomSettings.Remove(roomId);
                OwnedRoomPasswords.Remove(roomId);
                OwnedRoomCache.Remove(roomId);
                return;
            }

            if (OwnedRoomCache.TryGetValue(roomId, out Room? room))
            {
                room.CurrentPlayer = 0;
            }
        }

        public static void ResetOwnedRoomState()
        {
            OwnedRoomIds.Clear();
            OwnedPublicRoomsAwaitingJoin.Clear();
            OwnedRoomSettings.Clear();
            OwnedRoomPasswords.Clear();
            OwnedRoomCache.Clear();
        }

        private static void ForgetOwnedRoomsByName(string roomName)
        {
            foreach (var roomId in OwnedRoomCache.Keys.ToList())
            {
                if (!string.Equals(OwnedRoomCache[roomId].Name, roomName, StringComparison.OrdinalIgnoreCase))
                    continue;

                OwnedRoomIds.Remove(roomId);
                OwnedPublicRoomsAwaitingJoin.Remove(roomId);
                OwnedRoomSettings.Remove(roomId);
                OwnedRoomPasswords.Remove(roomId);
                OwnedRoomCache.Remove(roomId);
            }
        }

        Random rd = new Random();

        string GenerateCode()
        {
            return rd.Next(100000, 999999).ToString();
        }
        private const int MaxPlayers = 2;
        private static readonly List<Room> fakeRooms = new List<Room>();

        Panel CreateRoom(Room room)
        {
            if (panelRoomTemplate == null)
                throw new InvalidOperationException("panelRoomTemplate is null");

            var removeButton = panelRoomTemplate.Controls
                .OfType<Button>()
                .FirstOrDefault(b => b.Name == "button3");
            var joinLocation = removeButton?.Location ?? Point.Empty;
            var joinSize = removeButton?.Size ?? Size.Empty;

            Panel newPanel = new Panel
            {
                Size = panelRoomTemplate.Size,
                BackColor = panelRoomTemplate.BackColor,
                Margin = new Padding(10, 10, 10, 10)
            };

            foreach (Control ctrl in panelRoomTemplate.Controls)
            {
                Control newCtrl = (Control)Activator.CreateInstance(ctrl.GetType());


                newCtrl.Name = ctrl.Name;
                newCtrl.Size = ctrl.Size;
                newCtrl.Location = ctrl.Location;
                newCtrl.Font = ctrl.Font;
                newCtrl.ForeColor = ctrl.ForeColor;
                newCtrl.BackColor = ctrl.BackColor;
                newCtrl.Anchor = ctrl.Anchor;
                newCtrl.Dock = ctrl.Dock;

                // xử lý riêng
                if (newCtrl is Label lbl)
                {
                    lbl.AutoSize = true;
                    if (ctrl.Name == "lblRoomName")
                        lbl.Text = room.Name;

                    else if (ctrl.Name == "lblRoomCode")
                        lbl.Text = $"Code: {room.Code} | {room.TimeLimitMinutes}:00";

                    else if (ctrl.Name == "lblSlot")
                        lbl.Text = $"{room.CurrentPlayer}/{MaxPlayers}";
                }
                else if (newCtrl is Button btn)
                {
                    if (ctrl.Name == "btnJoin")
                    {
                        btn.Text = room.CurrentPlayer >= MaxPlayers ? "FULL" : "JOIN";
                        btn.Tag = room;
                        btn.Click += BtnJoin_Click;
                        btn.Enabled = room.CurrentPlayer < MaxPlayers;
                        if (joinLocation != Point.Empty)
                            btn.Location = joinLocation;
                        if (joinSize != Size.Empty)
                            btn.Size = joinSize;
                    }
                    else if (ctrl.Name == "button3")
                    {
                        continue;
                    }
                }
                else if (newCtrl is PictureBox pb)
                {
                    PictureBox oldPb = (PictureBox)ctrl;

                    pb.Image = oldPb.Image;
                    pb.SizeMode = oldPb.SizeMode;

                    if (ctrl.Name == "picLock")
                    {
                        pb.Image = room.HasPassword
                            ? oldPb.Image
                            : _unlockRoomIcon ?? oldPb.Image;
                        pb.Visible = true;
                        pb.BringToFront();
                    }
                    else
                    {
                        pb.Visible = ctrl.Visible;
                    }
                }

                newPanel.Controls.Add(newCtrl);
            }

            return newPanel;
        }

        void RenderRooms(List<Room> rooms)
        {
            string signature = BuildRenderSignature(rooms);
            if (signature == _lastRenderSignature)
                return;

            _lastRenderSignature = signature;
            flowLayoutPanelRooms.SuspendLayout();
            flowLayoutPanelRooms.Controls.Clear();

            if (rooms.Count == 0)
            {
                flowLayoutPanelRooms.Controls.Add(new Label
                {
                    AutoSize = true,
                    ForeColor = Color.Gold,
                    Font = new Font("Book Antiqua", 13.8F, FontStyle.Bold),
                    Text = "Đang chờ phòng..."
                });
                flowLayoutPanelRooms.ResumeLayout();
                return;
            }

            foreach (var room in rooms.OrderByDescending(r => r.Code))
            {
                Panel panel = CreateRoom(room);

                flowLayoutPanelRooms.Controls.Add(panel);
            }
            flowLayoutPanelRooms.ResumeLayout();
        }


        private async void JoinRoom_Load(object sender, EventArgs e)
        {
            SoundManager.PlayBGM("xtremefreddy.mp3");
            textBox1.Text = DefaultTimeLimitMinutes.ToString();
            UpdateSelectedMapText();
            if (!NetworkManager.Instance.IsConnected)
            {
                RenderRooms(fakeRooms);
                return;
            }
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string roomName = NormalizeRoomName(txtRoomName.Text);
            string password = txtPass.Text.Trim();
            int timeLimitMinutes = ParseTimeLimitMinutes();
            bool hasPassword = !string.IsNullOrWhiteSpace(password);

            if (timeLimitMinutes <= 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(roomName))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!");
                return;
            }

            if (NetworkManager.Instance.IsConnected)
            {
                string? resolvedName = await ResolveDuplicateRoomNameAsync(roomName);
                if (string.IsNullOrWhiteSpace(resolvedName))
                    return;
                roomName = resolvedName;

                ForgetOwnedRoomsByName(roomName);

                CreateRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.CreateRoomAsync(new CreateRoomPacket
                {
                    RoomName = roomName,
                    Password = password,
                    TimeLimitMinutes = timeLimitMinutes,
                    AutoJoin = hasPassword
                }));

                if (result.RoomId <= 0)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "Không thể tạo phòng. Vui lòng thử lại." : result.Message);
                    return;
                }

                OwnedRoomIds.Add(result.RoomId);
                int serverTimeLimitMinutes = result.TimeLimitMinutes > 0 ? result.TimeLimitMinutes : timeLimitMinutes;
                OwnedRoomSettings[result.RoomId] = (_selectedMapId, serverTimeLimitMinutes);
                if (hasPassword)
                {
                    OwnedRoomPasswords[result.RoomId] = password;
                    OwnedPublicRoomsAwaitingJoin.Remove(result.RoomId);
                }
                else
                {
                    OwnedRoomPasswords.Remove(result.RoomId);
                    OwnedPublicRoomsAwaitingJoin.Add(result.RoomId);
                }

                OwnedRoomCache[result.RoomId] = new Room
                {
                    Name = roomName,
                    Code = result.RoomId.ToString(),
                    Password = password,
                    CurrentPlayer = hasPassword ? 1 : 0,
                    MapId = _selectedMapId,
                    TimeLimitMinutes = serverTimeLimitMinutes,
                    HasPassword = hasPassword,
                    IsOwner = true
                };

                txtRoomName.Clear();
                txtPass.Clear();

                if (!hasPassword)
                {
                    await SendRoomMapSelectionAsync(result.RoomId, _selectedMapId);
                    await RefreshRoomsWithRetryAsync();
                    MessageBox.Show(
                        $"Đã tạo phòng mở: {result.RoomId}. Bạn có thể bấm JOIN khi muốn vào phòng.",
                        "Create Room",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                string localUser = string.IsNullOrWhiteSpace(PlayerSession.Username) ? "You" : PlayerSession.Username;
                RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost: true, playerCount: 1, mapId: _selectedMapId, timeLimitMinutes: serverTimeLimitMinutes, player1Name: localUser);
                roomForm.Show();
                Close();
                return;
            }

            string? offlineResolvedName = ResolveDuplicateRoomName(roomName, fakeRooms.Select(r => r.Name));
            if (string.IsNullOrWhiteSpace(offlineResolvedName))
                return;
            roomName = offlineResolvedName;

            Room room = new Room
            {
                Name = roomName,
                Code = GenerateCode(),
                Password = password,
                CurrentPlayer = 0,
                MapId = _selectedMapId,
                TimeLimitMinutes = timeLimitMinutes
            };

            fakeRooms.Add(room);

            RenderRooms(fakeRooms);
            MessageBox.Show($"Mã phòng: {room.Code}", "Tạo phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtRoomName.Clear();
            txtPass.Clear();
        }

        private void CreateRoomInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            btnCreateRoom.PerformClick();
            e.SuppressKeyPress = true;
        }

        private static string NormalizeRoomName(string? value)
        {
            string text = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = string.Join(" ", text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            return text.Length <= 32 ? text : text[..32];
        }

        private async Task<string?> ResolveDuplicateRoomNameAsync(string requestedName)
        {
            try
            {
                GetRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.GetRoomAsync(new GetRoomPacket()));
                return ResolveDuplicateRoomName(requestedName, result.Rooms.Select(r => r.RoomName ?? string.Empty));
            }
            catch
            {
                return requestedName;
            }
        }

        private static string? ResolveDuplicateRoomName(string requestedName, IEnumerable<string> existingNames)
        {
            string normalized = NormalizeRoomName(requestedName);
            if (string.IsNullOrWhiteSpace(normalized))
                return null;

            HashSet<string> names = existingNames
                .Select(NormalizeRoomName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!names.Contains(normalized))
                return normalized;

            string nextName = BuildNextRoomVersionName(normalized, names);
            DialogResult answer = MessageBox.Show(
                $"Đã có phòng tên \"{normalized}\".\nBạn có muốn tạo phòng mới với tên \"{nextName}\" không?",
                "Tên phòng đã tồn tại",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return answer == DialogResult.Yes ? nextName : null;
        }

        private static string BuildNextRoomVersionName(string roomName, HashSet<string> existingNames)
        {
            string baseName = StripRoomVersionSuffix(roomName);
            for (int index = 1; index < 1000; index++)
            {
                string candidate = NormalizeRoomName($"{baseName} ({index})");
                if (!existingNames.Contains(candidate))
                    return candidate;
            }

            return NormalizeRoomName($"{baseName} ({DateTime.Now:HHmmss})");
        }

        private static string StripRoomVersionSuffix(string roomName)
        {
            int closeIndex = roomName.LastIndexOf(')');
            int openIndex = roomName.LastIndexOf(" (", StringComparison.Ordinal);
            if (closeIndex != roomName.Length - 1 || openIndex < 0 || openIndex >= closeIndex)
                return roomName;

            string numberText = roomName[(openIndex + 2)..closeIndex];
            return int.TryParse(numberText, out _)
                ? NormalizeRoomName(roomName[..openIndex])
                : roomName;
        }


        private async void BtnJoin_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Room room)
            {
                if (NetworkManager.Instance.IsConnected)
                {
                    try
                    {
                        if (!int.TryParse(room.Code, out int roomId))
                        {
                            MessageBox.Show("Mã phòng không hợp lệ.");
                            return;
                        }

                        string passwordToSend = string.Empty;
                        if (room.HasPassword)
                        {
                            if (!TryPromptPassword(room.Name, out string? passwordInput))
                                return;
                            passwordToSend = passwordInput ?? string.Empty;
                        }

                        JoinRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.JoinRoomWithServerRedirectAsync(new JoinRoomPacket
                        {
                            RoomId = roomId,
                            Password = passwordToSend
                        }));

                        if (!result.Success)
                        {
                            MessageBox.Show(
                                string.IsNullOrWhiteSpace(result.Message) ? "Không thể vào phòng." : result.Message,
                                "Join Room",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        if (result.IsOwner)
                        {
                            OwnedRoomIds.Add(roomId);
                            OwnedPublicRoomsAwaitingJoin.Remove(roomId);
                            if (OwnedRoomCache.TryGetValue(roomId, out Room? ownedRoom))
                            {
                                ownedRoom.CurrentPlayer = 1;
                            }
                        }
                        bool isHost = result.IsOwner;
                        string roomMapId = result.MapId >= 0 ? MapIdFromNetwork(result.MapId) : room.MapId;
                        int roomTimeLimitMinutes = result.TimeLimitMinutes > 0 ? result.TimeLimitMinutes : room.TimeLimitMinutes;
                        int nextCountOnline = result.IsOwner
                            ? Math.Max(1, room.CurrentPlayer)
                            : Math.Min(MaxPlayers, room.CurrentPlayer + 1);
                        RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost, nextCountOnline, roomMapId, roomTimeLimitMinutes, result.Player1Name, result.Player2Name);
                        roomForm.Show();
                        Close();
                        return;
                    }
                    catch (TimeoutException)
                    {
                        MessageBox.Show(
                            "Server phản hồi chậm khi vào phòng. Vui lòng thử lại.",
                            "Join Room",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Lỗi khi vào phòng: {ex.Message}",
                            "Join Room",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(room.Password))
                {
                    if (!TryPromptPassword(room.Name, out string? passwordInput))
                        return;

                    if (!string.Equals(room.Password, passwordInput, StringComparison.Ordinal))
                    {
                        MessageBox.Show("Mật khẩu không đúng!", "Join Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                int nextCount = Math.Min(MaxPlayers, room.CurrentPlayer + 1);
                OpenRoom(room, isHost: false, nextPlayerCount: nextCount);
            }
        }

        private async void BtnRemove_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Room room)
                return;

            if (!NetworkManager.Instance.IsConnected)
            {
                fakeRooms.Remove(room);
                RenderRooms(fakeRooms);
                return;
            }

            if (!TryGetRoomId(room, out int roomId))
            {
                MessageBox.Show("Mã phòng không hợp lệ.");
                return;
            }

            RemoveRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.RemoveRoomAsync(new RemoveRoomPacket
            {
                RoomId = roomId
            }));

            if (!result.Success)
            {
                MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "Không thể xóa phòng." : result.Message);
                return;
            }

            OwnedRoomIds.Remove(roomId);
            OwnedPublicRoomsAwaitingJoin.Remove(roomId);
            OwnedRoomSettings.Remove(roomId);
            OwnedRoomPasswords.Remove(roomId);
            OwnedRoomCache.Remove(roomId);
            await LoadRoomsAsync();
        }

        private static bool TryPromptPassword(string roomName, out string? password)
        {
            using Form prompt = new Form
            {
                Width = 440,
                Height = 210,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = $"Nhập mật khẩu ({roomName})",
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label textLabel = new Label
            {
                Left = 16,
                Top = 20,
                Width = 320,
                Text = "Mật khẩu phòng:"
            };

            TextBox inputBox = new TextBox
            {
                Left = 16,
                Top = 50,
                Width = 390,
                UseSystemPasswordChar = true
            };

            Button confirmation = new Button
            {
                Text = "OK",
                Top = 90,
                DialogResult = DialogResult.OK
            };

            Button cancel = new Button
            {
                Text = "Cancel",
                Top = 90,
                DialogResult = DialogResult.Cancel
            };

            int buttonPadding = 24;
            int okWidth = TextRenderer.MeasureText(confirmation.Text, confirmation.Font).Width + buttonPadding;
            int cancelWidth = TextRenderer.MeasureText(cancel.Text, cancel.Font).Width + buttonPadding;
            int buttonWidth = Math.Max(okWidth, cancelWidth);
            confirmation.Width = buttonWidth;
            cancel.Width = buttonWidth;
            confirmation.Height = 32;
            cancel.Height = 32;
            cancel.Left = prompt.ClientSize.Width - 16 - cancel.Width;
            confirmation.Left = cancel.Left - 10 - confirmation.Width;

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                password = inputBox.Text.Trim();
                return true;
            }

            password = null;
            return false;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            //server trả về danh sách phòng mới nhất
            if (NetworkManager.Instance.IsConnected)
            {
                await RefreshRoomsWithRetryAsync();
            }
            else
            {
                RenderRooms(fakeRooms);
            }
        }

        private async Task RefreshRoomsAsync()
        {
            if (_isRefreshingRooms)
                return;

            _isRefreshingRooms = true;
            _lastRefreshUtc = DateTime.UtcNow;
            try
            {
                await LoadRoomsAsync();
            }
            catch
            {
                // Online mode should trust authoritative room list from server.
                // Avoid reviving stale locally-cached rooms after owner already left/room closed.
                RenderRooms(new List<Room>());
            }
            finally
            {
                _isRefreshingRooms = false;
            }
        }

        private async Task RefreshRoomsWithRetryAsync()
        {
            const int retryCount = 3;
            const int retryDelayMs = 140;

            for (int i = 0; i < retryCount; i++)
            {
                await RefreshRoomsAsync();
                if (flowLayoutPanelRooms.Controls.Count > 0
                    && flowLayoutPanelRooms.Controls[0] is not Label)
                {
                    return;
                }

                await Task.Delay(retryDelayMs);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using MapSelectionForm mapSelection = new MapSelectionForm();
            if (mapSelection.ShowDialog(this) == DialogResult.OK)
            {
                _selectedMapId = mapSelection.SelectedMapId;
                UpdateSelectedMapText();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ModeForm modeForm = new ModeForm();
            modeForm.Show();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OfflineMode offlineMode = new OfflineMode();
            offlineMode.Show();
            Close();
        }

        private async void FindRoom_Click(object sender, EventArgs e)
        {
            if (!NetworkManager.Instance.IsConnected)
            {
                MessageBox.Show("Chỉ hỗ trợ Find Room khi đang Online.");
                return;
            }

            try
            {
                GetRoomResultPacket rooms = await RunRoomRequestAsync(() => NetworkManager.Instance.GetRoomAsync(new GetRoomPacket()));
                RoomInfo? target = rooms.Rooms
                    .Where(r => !r.HasPassword && !r.IsOwner && r.CurrentPlayers < MaxPlayers)
                    .OrderByDescending(r => r.CurrentPlayers)
                    .ThenBy(r => r.RoomId)
                    .FirstOrDefault();

                if (target == null)
                {
                    await CreateAndOpenRandomPublicRoomAsync();
                    return;
                }

                int roomId = target.RoomId;

                JoinRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.JoinRoomWithServerRedirectAsync(new JoinRoomPacket
                {
                    RoomId = roomId,
                    Password = string.Empty
                }));

                if (!result.Success)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "Không thể vào phòng." : result.Message);
                    return;
                }

                bool hasOwnedSettings = TryGetOwnedSettings(roomId, out var settings);
                string mapId = result.MapId >= 0
                    ? MapIdFromNetwork(result.MapId)
                    : (hasOwnedSettings ? settings.MapId : _selectedMapId);
                int timeLimit = result.TimeLimitMinutes > 0
                    ? result.TimeLimitMinutes
                    : target.TimeLimitMinutes > 0
                        ? target.TimeLimitMinutes
                        : hasOwnedSettings ? settings.TimeLimitMinutes : DefaultTimeLimitMinutes;
                bool isHost = result.IsOwner;
                int playerCount = 2;

                RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost, playerCount, mapId, timeLimit, result.Player1Name, result.Player2Name);
                roomForm.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Find Room thất bại: {ex.Message}");
            }
        }

        private static string? PromptText(string title, string label)
        {
            using Form prompt = new Form
            {
                Width = 440,
                Height = 210,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label textLabel = new Label
            {
                Left = 16,
                Top = 20,
                Width = 320,
                Text = label
            };

            TextBox inputBox = new TextBox
            {
                Left = 16,
                Top = 50,
                Width = 390
            };

            Button confirmation = new Button
            {
                Text = "OK",
                Top = 90,
                DialogResult = DialogResult.OK
            };

            Button cancel = new Button
            {
                Text = "Cancel",
                Top = 90,
                DialogResult = DialogResult.Cancel
            };

            int buttonPadding = 24;
            confirmation.Width = TextRenderer.MeasureText(confirmation.Text, confirmation.Font).Width + buttonPadding;
            cancel.Width = TextRenderer.MeasureText(cancel.Text, cancel.Font).Width + buttonPadding;
            cancel.Left = prompt.ClientSize.Width - 16 - cancel.Width;
            confirmation.Left = cancel.Left - 10 - confirmation.Width;

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text.Trim() : null;
        }

        private void btnJoin_Click_1(object sender, EventArgs e)
        {
            BtnJoin_Click(sender, e);
        }

        private async Task CreateAndOpenRandomPublicRoomAsync()
        {
            string roomName = GenerateRandomRoomName();
            int timeLimitMinutes = ParseTimeLimitMinutes();
            if (timeLimitMinutes <= 0)
                timeLimitMinutes = DefaultTimeLimitMinutes;
            string randomMapId = PickRandomMapId();

            CreateRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.CreateRoomAsync(new CreateRoomPacket
            {
                RoomName = roomName,
                Password = string.Empty,
                TimeLimitMinutes = timeLimitMinutes,
                AutoJoin = true
            }));

            if (result.RoomId <= 0)
            {
                MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "Không thể tạo phòng tự động." : result.Message);
                return;
            }

            OwnedRoomIds.Add(result.RoomId);
            OwnedPublicRoomsAwaitingJoin.Remove(result.RoomId);
            int serverTimeLimitMinutes = result.TimeLimitMinutes > 0 ? result.TimeLimitMinutes : timeLimitMinutes;
            OwnedRoomSettings[result.RoomId] = (randomMapId, serverTimeLimitMinutes);
            OwnedRoomPasswords.Remove(result.RoomId);
            OwnedRoomCache[result.RoomId] = new Room
            {
                Name = roomName,
                Code = result.RoomId.ToString(),
                Password = string.Empty,
                CurrentPlayer = 1,
                MapId = randomMapId,
                TimeLimitMinutes = serverTimeLimitMinutes,
                HasPassword = false,
                IsOwner = true
            };

            await SendRoomMapSelectionAsync(result.RoomId, randomMapId);

            string localUser = string.IsNullOrWhiteSpace(PlayerSession.Username) ? "You" : PlayerSession.Username;
            RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost: true, playerCount: 1, mapId: randomMapId, timeLimitMinutes: serverTimeLimitMinutes, player1Name: localUser);
            roomForm.Show();
            Close();
        }

        private string GenerateRandomRoomName()
        {
            string userPrefix = string.IsNullOrWhiteSpace(PlayerSession.Username)
                ? "Open"
                : NormalizeRoomName(PlayerSession.Username);
            if (string.IsNullOrWhiteSpace(userPrefix))
                userPrefix = "Open";

            return $"{userPrefix} Room {rd.Next(100, 999)}";
        }

        private string PickRandomMapId()
        {
            return MatchmakingMapIds[rd.Next(MatchmakingMapIds.Length)];
        }

        private async Task SendRoomMapSelectionAsync(int roomId, string mapId)
        {
            if (!NetworkManager.Instance.IsConnected || roomId <= 0)
                return;

            await NetworkManager.Instance.SelectMapAsync(new SelectMapPacket
            {
                RoomId = roomId,
                MapId = MapIdToNetwork(mapId)
            });
        }

        private void OpenRoom(Room room, bool isHost, int nextPlayerCount)
        {
            RoomForm roomForm = new RoomForm(room.Code, isHost, nextPlayerCount, room.MapId, room.TimeLimitMinutes);
            roomForm.Show();
            Close();
        }

        private async Task LoadRoomsAsync()
        {
            await RunRoomRequestAsync(async () =>
            {
                GetRoomResultPacket result = await NetworkManager.Instance.GetRoomAsync(new GetRoomPacket());
                SyncOwnedRoomCache(result.Rooms.Select(room => room.RoomId).ToHashSet());

                List<Room> rooms = new();
                foreach (var room in result.Rooms)
                {
                    bool hasOwnedSettings = TryGetOwnedSettings(room.RoomId, out var settings);
                    bool isOwnedLocal = room.IsOwner || OwnedRoomIds.Contains(room.RoomId);
                    if (room.IsOwner)
                    {
                        OwnedRoomIds.Add(room.RoomId);
                    }

                    int currentPlayers = room.CurrentPlayers;

                    string roomCode = room.RoomId.ToString();
                    string roomPassword = OwnedRoomPasswords.TryGetValue(room.RoomId, out string savedPassword) ? savedPassword : string.Empty;
                    bool hasPassword = room.HasPassword || !string.IsNullOrWhiteSpace(roomPassword);
                    if (IsOwnedPublicRoomStillAwaitingJoin(room, hasPassword))
                        currentPlayers = 0;

                    Room mappedRoom = new Room
                    {
                        Name = room.RoomName ?? "Room",
                        Code = roomCode,
                        Password = roomPassword,
                        CurrentPlayer = currentPlayers,
                        MapId = room.MapId >= 0 ? MapIdFromNetwork(room.MapId) : (hasOwnedSettings ? settings.MapId : _selectedMapId),
                        MapIdNetwork = room.MapId,
                        TimeLimitMinutes = room.TimeLimitMinutes > 0
                            ? room.TimeLimitMinutes
                            : hasOwnedSettings ? settings.TimeLimitMinutes : DefaultTimeLimitMinutes,
                        HasPassword = hasPassword,
                        IsOwner = room.IsOwner || isOwnedLocal
                    };

                    if (isOwnedLocal)
                    {
                        OwnedRoomCache[room.RoomId] = new Room
                        {
                            Name = mappedRoom.Name,
                            Code = mappedRoom.Code,
                            Password = mappedRoom.Password,
                            CurrentPlayer = mappedRoom.CurrentPlayer,
                            MapId = mappedRoom.MapId,
                            MapIdNetwork = mappedRoom.MapIdNetwork,
                            TimeLimitMinutes = mappedRoom.TimeLimitMinutes,
                            HasPassword = mappedRoom.HasPassword,
                            IsOwner = true
                        };
                    }

                    rooms.Add(mappedRoom);
                }

                RenderRooms(rooms);
            });
        }

        private static void SyncOwnedRoomCache(HashSet<int> liveRoomIds)
        {
            foreach (int roomId in OwnedRoomCache.Keys.ToList())
            {
                if (liveRoomIds.Contains(roomId))
                    continue;

                OwnedRoomIds.Remove(roomId);
                OwnedPublicRoomsAwaitingJoin.Remove(roomId);
                OwnedRoomSettings.Remove(roomId);
                OwnedRoomPasswords.Remove(roomId);
                OwnedRoomCache.Remove(roomId);
            }
        }

        private static List<Room> BuildOwnedRoomFallbackRooms()
        {
            List<Room> rooms = new();
            foreach (var kv in OwnedRoomCache)
            {
                Room cachedRoom = kv.Value;
                rooms.Add(new Room
                {
                    Name = cachedRoom.Name,
                    Code = cachedRoom.Code,
                    Password = cachedRoom.Password,
                    CurrentPlayer = cachedRoom.CurrentPlayer,
                    MapId = cachedRoom.MapId,
                    MapIdNetwork = cachedRoom.MapIdNetwork,
                    TimeLimitMinutes = cachedRoom.TimeLimitMinutes,
                    HasPassword = cachedRoom.HasPassword,
                    IsOwner = true
                });
            }

            return rooms;
        }

        private async Task RunRoomRequestAsync(Func<Task> action)
        {
            await _roomRequestGate.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                _roomRequestGate.Release();
            }
        }

        private async Task<T> RunRoomRequestAsync<T>(Func<Task<T>> action)
        {
            await _roomRequestGate.WaitAsync();
            try
            {
                return await action();
            }
            finally
            {
                _roomRequestGate.Release();
            }
        }

        private static bool TryGetRoomId(Room room, out int roomId)
        {
            return int.TryParse(room.Code, out roomId);
        }

        private static bool CanRemoveRoom(Room room)
        {
            if (!NetworkManager.Instance.IsConnected)
                return true;

            return room.CurrentPlayer == 0
                && TryGetRoomId(room, out int roomId)
                && room.IsOwner;
        }

        private static bool IsOwnedPublicRoomStillAwaitingJoin(RoomInfo room, bool hasPassword)
        {
            if (hasPassword || !OwnedPublicRoomsAwaitingJoin.Contains(room.RoomId))
                return false;

            int localUserId = NetworkManager.Instance.PreferredUserId;
            return localUserId > 0
                && room.Player1Id == localUserId
                && room.Player2Id == -1;
        }

        private static bool TryGetOwnedSettings(int roomId, out (string MapId, int TimeLimitMinutes) settings)
        {
            return OwnedRoomSettings.TryGetValue(roomId, out settings);
        }

        private int ParseTimeLimitMinutes()
        {
            string input = textBox1.Text.Trim();
            if (!int.TryParse(input, out int minutes))
            {
                MessageBox.Show("Vui lòng nhập thời gian bằng phút.");
                return 0;
            }

            if (minutes <= 0 || minutes > MaxTimeLimitMinutes)
            {
                MessageBox.Show($"Thời gian tối đa là {MaxTimeLimitMinutes} phút.");
                return 0;
            }

            return minutes;
        }

        private void UpdateSelectedMapText()
        {
            button2.Text = GetMapDisplayName(_selectedMapId);
        }

        private static string GetMapDisplayName(string mapId)
        {
            return mapId switch
            {
                "terrace" => "Battle 1",
                "throneroom" => "Battle 2",
                "castle" => "Battle 3",
                _ => mapId
            };
        }

        private static string MapIdFromNetwork(int mapId)
        {
            return mapId switch
            {
                0 => "terrace",
                1 => "throneroom",
                2 => "castle",
                _ => "terrace"
            };
        }

        private static int MapIdToNetwork(string mapId)
        {
            return mapId switch
            {
                "terrace" => 0,
                "throneroom" => 1,
                "castle" => 2,
                _ => 0
            };
        }

        private static string BuildRenderSignature(List<Room> rooms)
        {
            if (rooms.Count == 0)
                return "empty";

            var sb = new StringBuilder(rooms.Count * 32);
            foreach (var room in rooms.OrderBy(r => r.Code))
            {
                sb.Append(room.Code).Append('|')
                    .Append(room.Name).Append('|')
                    .Append(room.CurrentPlayer).Append('|')
                    .Append(room.HasPassword).Append('|')
                    .Append(room.IsOwner).Append('|')
                    .Append(room.TimeLimitMinutes).Append('|')
                    .Append(room.MapId).Append(';');
            }

            return sb.ToString();
        }

        private static Image? LoadOptionalImage(params string[] parts)
        {
            string path = Path.Combine(parts);
            if (!File.Exists(path))
            {
                string basePath = Path.Combine(new[] { AppDomain.CurrentDomain.BaseDirectory }.Concat(parts).ToArray());
                string projectPath = Path.Combine(new[]
                {
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    ".."
                }.Concat(parts).ToArray());

                path = File.Exists(basePath)
                    ? basePath
                    : Path.GetFullPath(projectPath);
            }

            if (!File.Exists(path))
                return null;

            using Image image = Image.FromFile(path);
            return new Bitmap(image);
        }
    }
}
