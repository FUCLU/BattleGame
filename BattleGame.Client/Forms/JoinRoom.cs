using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class JoinRoom : Form
    {
        public JoinRoom()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        public class Room
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }
            public int CurrentPlayer { get; set; }
        }

        Random rd = new Random();

        string GenerateCode()
        {
            return rd.Next(100000, 999999).ToString();
        }
        List<Room> fakeRooms = new List<Room>();
        Panel CreateRoom(string name, string code, int current, string password)
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
                        lbl.Text = name;

                    else if (ctrl.Name == "lblRoomCode")
                        lbl.Text = "Code: " + code;

                    else if (ctrl.Name == "lblSlot")
                        lbl.Text = $"{current}/2";
                }
                else if (newCtrl is Button btn)
                {
                    if (ctrl.Name == "btnJoin")
                    {
                        btn.Text = "JOIN";
                        btn.Tag = new Tuple<string, string>(code, password);
                        btn.Click += BtnJoin_Click;
                    }
                }
                else if (newCtrl is PictureBox pb)
                {
                    PictureBox oldPb = (PictureBox)ctrl;

                    pb.Image = oldPb.Image;
                    pb.SizeMode = oldPb.SizeMode;

                    if (ctrl.Name == "picLock")
                    {
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
            flowLayoutPanelRooms.Controls.Clear();

            foreach (var room in rooms)
            {
                Panel panel = CreateRoom(
                    room.Name,
                    room.Code,
                    room.CurrentPlayer,
                    room.Password
                );

                flowLayoutPanelRooms.Controls.Add(panel);
            }
        }


        private void JoinRoom_Load(object sender, EventArgs e)
        {
            RenderRooms(fakeRooms);
        }


        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            string roomName = txtRoomName.Text.Trim();
            string password = txtPass.Text.Trim();

            if (string.IsNullOrEmpty(roomName))
            {
                MessageBox.Show("Vui lòng nhập tên phòng!");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                return;
            }

            Room room = new Room
            {
                Name = roomName,
                Code = GenerateCode(),
                Password = password,
                CurrentPlayer = 1
            };

            fakeRooms.Add(room);

            //render list room
            RenderRooms(fakeRooms);

            txtRoomName.Clear();
            txtPass.Clear();
        }


        private void BtnJoin_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tuple<string, string> data)
            {
                string roomCode = data.Item1;
                string password = data.Item2;

                MessageBox.Show($"Join room: {roomCode}\nPassword: {password}");

                //....
                //xử lí JoinRoom(roomCode, password);
                //....
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //server trả về danh sách phòng mới nhất
            RenderRooms(fakeRooms);
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
            RoomForm roomForm = new RoomForm();
            roomForm.Show();
            this.Close();
        }
    }
}
