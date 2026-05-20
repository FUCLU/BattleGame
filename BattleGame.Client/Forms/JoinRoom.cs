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
        private string _selectedMapId = "terrace";
        private static readonly HashSet<int> OwnedRoomIds = new();
        private static readonly Dictionary<int, (string MapId, int TimeLimitMinutes)> OwnedRoomSettings = new();
        private static readonly Dictionary<int, string> OwnedRoomPasswords = new();
        private static readonly Dictionary<int, Room> OwnedRoomCache = new();
        private readonly SemaphoreSlim _roomRequestGate = new(1, 1);
        private bool _isRefreshingRooms;
        private string _lastRenderSignature = string.Empty;
        private DateTime _lastRefreshUtc = DateTime.MinValue;
        private static readonly TimeSpan ActivationRefreshCooldown = TimeSpan.FromMilliseconds(900);

        private bool _isMuted = false;
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
                        pb.Visible = room.HasPassword;
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
            SoundManager.SetVolume(1.0f);
            UpdateMusicButtonText();
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
                ForgetOwnedRoomsByName(roomName);

                CreateRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.CreateRoomAsync(new CreateRoomPacket
                {
                    RoomName = roomName,
                    Password = password,
                    TimeLimitMinutes = timeLimitMinutes
                }));

                if (result.RoomId <= 0)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "Không thể tạo phòng. Vui lòng thử lại." : result.Message);
                    return;
                }

                OwnedRoomIds.Add(result.RoomId);
                int serverTimeLimitMinutes = result.TimeLimitMinutes > 0 ? result.TimeLimitMinutes : timeLimitMinutes;
                OwnedRoomSettings[result.RoomId] = (_selectedMapId, serverTimeLimitMinutes);
                OwnedRoomPasswords[result.RoomId] = password;
                OwnedRoomCache[result.RoomId] = new Room
                {
                    Name = roomName,
                    Code = result.RoomId.ToString(),
                    Password = password,
                    CurrentPlayer = 1,
                    MapId = _selectedMapId,
                    TimeLimitMinutes = serverTimeLimitMinutes,
                    HasPassword = !string.IsNullOrWhiteSpace(password),
                    IsOwner = true
                };
                string localUser = string.IsNullOrWhiteSpace(PlayerSession.Username) ? "You" : PlayerSession.Username;
                RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost: true, playerCount: 1, mapId: _selectedMapId, timeLimitMinutes: serverTimeLimitMinutes, player1Name: localUser);
                roomForm.Show();
                Close();
                return;
            }

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

                        int nextCountOnline = Math.Min(MaxPlayers, room.CurrentPlayer + 1);
                        if (result.IsOwner)
                        {
                            OwnedRoomIds.Add(roomId);
                            if (OwnedRoomCache.TryGetValue(roomId, out Room? ownedRoom))
                            {
                                ownedRoom.CurrentPlayer = 1;
                            }
                        }
                        bool isHost = result.IsOwner;
                        string roomMapId = result.MapId >= 0 ? MapIdFromNetwork(result.MapId) : room.MapId;
                        int roomTimeLimitMinutes = result.TimeLimitMinutes > 0 ? result.TimeLimitMinutes : room.TimeLimitMinutes;
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
                    .Where(r => r.CurrentPlayers < MaxPlayers)
                    .OrderBy(r => r.HasPassword)
                    .FirstOrDefault();

                if (target == null)
                {
                    MessageBox.Show("Không có phòng trống.");
                    return;
                }

                int roomId = target.RoomId;
                string password = string.Empty;
                if (target.HasPassword)
                {
                    if (!TryPromptPassword(target.RoomName ?? $"Room {roomId}", out string? passwordInput))
                        return;
                    password = passwordInput ?? string.Empty;
                }

                JoinRoomResultPacket result = await RunRoomRequestAsync(() => NetworkManager.Instance.JoinRoomWithServerRedirectAsync(new JoinRoomPacket
                {
                    RoomId = roomId,
                    Password = password
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
                "forest" => "Forest",
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
                3 => "forest",
                _ => "terrace"
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

        private void btnMusic_Click(object sender, EventArgs e)
        {
            _isMuted = !_isMuted;

            SoundManager.SetVolume(_isMuted ? 0.0f : 1.0f);

            UpdateMusicButtonText();
        }

        private void UpdateMusicButtonText()
        {
            if (_isMuted)
            {
                btnMusic.Text = "♪ Music: Off";
                btnMusic.BackColor = Color.FromArgb(15, 23, 42);
                btnMusic.ForeColor = Color.FromArgb(147, 197, 253);
            }
            else
            {
                btnMusic.Text = "♫ Music: On";
                btnMusic.BackColor = Color.FromArgb(37, 99, 235);
                btnMusic.ForeColor = Color.White;
            }
        }
    }
}
