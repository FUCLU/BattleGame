using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class JoinRoom : Form
    {
        private const int MaxTimeLimitMinutes = 5;
        private const int DefaultTimeLimitMinutes = 3;
        private string _selectedMapId = "terrace";

        public JoinRoom()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            button2.Click += button2_Click;
        }
        public class Room
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
            public int CurrentPlayer { get; set; }
            public string MapId { get; set; }
            public int TimeLimitMinutes { get; set; }
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
                        lbl.Text = "Code: " + room.Code;

                    else if (ctrl.Name == "lblSlot")
                        lbl.Text = $"{room.CurrentPlayer}/{MaxPlayers}";
                }
                else if (newCtrl is Button btn)
                {
                    if (ctrl.Name == "btnJoin")
                    {
                        btn.Text = "JOIN";
                        btn.Tag = room;
                        btn.Click += BtnJoin_Click;
                        btn.Enabled = room.CurrentPlayer < MaxPlayers;
                    }
                }
                else if (newCtrl is PictureBox pb)
                {
                    PictureBox oldPb = (PictureBox)ctrl;

                    pb.Image = oldPb.Image;
                    pb.SizeMode = oldPb.SizeMode;

                    if (ctrl.Name == "picLock")
                    {
                        pb.Visible = !string.IsNullOrWhiteSpace(room.Password);
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
                return;
            }

            foreach (var room in rooms)
            {
                Panel panel = CreateRoom(room);

                flowLayoutPanelRooms.Controls.Add(panel);
            }
        }


        private async void JoinRoom_Load(object sender, EventArgs e)
        {
            textBox1.Text = DefaultTimeLimitMinutes.ToString();
            UpdateSelectedMapText();
            if (NetworkManager.Instance.IsConnected)
            {
                await LoadRoomsAsync();
            }
            else
            {
                RenderRooms(fakeRooms);
            }
        }

        private async void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string roomName = txtRoomName.Text.Trim();
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
                CreateRoomResultPacket result = await NetworkManager.Instance.CreateRoomAsync(new CreateRoomPacket
                {
                    RoomName = roomName,
                    Password = password
                });

                string roomCode = result.RoomId.ToString();
                RoomForm roomForm = new RoomForm(roomCode, isHost: true, playerCount: 1, _selectedMapId, timeLimitMinutes);
                roomForm.Show();
                Close();
                return;
            }

            Room room = new Room
            {
                Name = roomName,
                Code = GenerateCode(),
                Password = password,
                CurrentPlayer = 1,
                MapId = _selectedMapId,
                TimeLimitMinutes = timeLimitMinutes
            };

            fakeRooms.Add(room);

            RenderRooms(fakeRooms);
            MessageBox.Show($"Mã phòng: {room.Code}", "Tạo phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtRoomName.Clear();
            txtPass.Clear();
        }


        private async void BtnJoin_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Room room)
            {
                if (NetworkManager.Instance.IsConnected)
                {
                    if (!int.TryParse(room.Code, out int roomId))
                    {
                        MessageBox.Show("Mã phòng không hợp lệ.");
                        return;
                    }

                    JoinRoomResultPacket result = await NetworkManager.Instance.JoinRoomAsync(new JoinRoomPacket
                    {
                        RoomId = roomId,
                        Password = room.Password
                    });

                    if (!result.Success)
                    {
                        MessageBox.Show("Không thể vào phòng.");
                        return;
                    }

                    RoomForm roomForm = new RoomForm(result.RoomId.ToString(), isHost: false, playerCount: 2, _selectedMapId, DefaultTimeLimitMinutes);
                    roomForm.Show();
                    Close();
                    return;
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

        private static bool TryPromptPassword(string roomName, out string? password)
        {
            using Form prompt = new Form
            {
                Width = 360,
                Height = 180,
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
                Width = 320,
                UseSystemPasswordChar = true
            };

            Button confirmation = new Button
            {
                Text = "OK",
                Left = 176,
                Width = 75,
                Top = 90,
                DialogResult = DialogResult.OK
            };

            Button cancel = new Button
            {
                Text = "Cancel",
                Left = 261,
                Width = 75,
                Top = 90,
                DialogResult = DialogResult.Cancel
            };

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //server trả về danh sách phòng mới nhất
            if (NetworkManager.Instance.IsConnected)
            {
                _ = LoadRoomsAsync();
            }
            else
            {
                RenderRooms(fakeRooms);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using MapSelectionForm mapSelection = new MapSelectionForm();
            if (mapSelection.ShowDialog() == DialogResult.OK)
            {
                _selectedMapId = mapSelection.SelectedMapId;
                UpdateSelectedMapText();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ModeForm modeForm = new ModeForm();
            modeForm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OfflineModeSelection offlineModeSelection = new OfflineModeSelection();
            offlineModeSelection.Show();
            this.Close();
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
            GetRoomResultPacket result = await NetworkManager.Instance.GetRoomAsync(new GetRoomPacket());
            List<Room> rooms = result.Rooms.Select(room => new Room
            {
                Name = room.RoomName ?? "Room",
                Code = room.RoomId.ToString(),
                Password = string.Empty,
                CurrentPlayer = room.CurrentPlayers,
                MapId = _selectedMapId,
                TimeLimitMinutes = DefaultTimeLimitMinutes
            }).ToList();

            RenderRooms(rooms);
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
    }
}
