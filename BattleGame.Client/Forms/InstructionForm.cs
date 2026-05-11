using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class InstructionForm : Form
    {
        public InstructionForm()
        {
            InitializeComponent();
            LoadDefaultTexts();
        }

        private void LoadDefaultTexts()
        {
            // Hiển thị cả 2 tab
            DisplayOfflineControls();
            DisplayOnlineControls();
        }

        // Hàm hỗ trợ lấy ảnh từ thư mục Assets/Background
        private Image GetImage(string fileName)
        {
            // Cấp 1: Tìm trong thư mục thực thi (bin/Debug/Assets/Background)
            string path1 = Path.Combine(Application.StartupPath, "Assets", "Background", fileName);
            if (File.Exists(path1))
                return Image.FromFile(path1);

            // Cấp 2: Lùi lại 2 cấp thư mục để về thư mục gốc Project
            string path2 = Path.Combine(Application.StartupPath, @"..\..\Assets\Background", fileName);
            if (File.Exists(path2))
                return Image.FromFile(path2);

            // Cấp 3: Lùi lại 3 cấp nếu dùng .NET Core / .NET 5+
            string path3 = Path.Combine(Application.StartupPath, @"..\..\..\Assets\Background", fileName);
            if (File.Exists(path3))
                return Image.FromFile(path3);

            return null;
        }

        private void DisplayOfflineControls()
        {
            // Xóa sạch control cũ
            tabOffline.Controls.Clear();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.White // Nền trắng
            };

            int y = 10;

            // Tiêu đề
            var lblTitle = new Label
            {
                Text = "OFFLINE MODE - CONTROLS:",
                Font = new Font("Courier New", 12F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);
            y += 30;

            // Phần Player 1
            var lblP1 = new Label
            {
                Text = "Player 1 Controls:",
                Font = new Font("Courier New", 11F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblP1);
            y += 25;

            var controls1 = new[] {
                ("Move Left:", GetImage("A.png")),
                ("Move Right:", GetImage("D.png")),
                ("Punch:", GetImage("J.png")),
                ("Dash:", GetImage("K.png")),     // Thêm nút Dash là K
                // ("Dash:", GetImage("L.png")),  // Nút L vẫn đang ẩn
                ("Skill 1:", GetImage("U.png")),  // Skill 1
                ("Skill 2:", GetImage("I.png")),  // Skill 2
            };

            foreach (var (name, img) in controls1)
            {
                var row = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    AutoSize = true,
                    Location = new Point(20, y),
                    Height = 60
                };

                var lbl = new Label
                {
                    Text = name,
                    Font = new Font("Courier New", 10F),
                    AutoSize = true,
                    Margin = new Padding(0, 6, 10, 0)
                };
                row.Controls.Add(lbl);

                var pb = new PictureBox
                {
                    Image = img,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(48, 48),
                    Margin = new Padding(0)
                };
                row.Controls.Add(pb);

                panel.Controls.Add(row);
                y += 55;
            }

            y += 10;

            // Tips
            var lblTips = new Label
            {
                Text = "Tips:",
                Font = new Font("Courier New", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblTips);
            y += 25;

            var tips = new[] {
                "- Use parry to negate incoming attacks and gain mana.",
                "- Use dash to quickly close distance.",
                "- Manage stamina and mana for skills."
            };

            foreach (var tip in tips)
            {
                var lblTip = new Label
                {
                    Text = tip,
                    Font = new Font("Courier New", 10F),
                    AutoSize = true,
                    Location = new Point(20, y)
                };
                panel.Controls.Add(lblTip);
                y += 25;
            }

            tabOffline.Controls.Add(panel);
        }

        private void DisplayOnlineControls()
        {
            // Xóa sạch control cũ
            tabOnline.Controls.Clear();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.White // Nền trắng
            };

            int y = 10;

            // Tiêu đề
            var lblTitle = new Label
            {
                Text = "ONLINE MODE - CONTROLS & NOTES:",
                Font = new Font("Courier New", 12F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblTitle);
            y += 30;

            // Player 1 (Host)
            var lblP1 = new Label
            {
                Text = "Player Controls:",
                Font = new Font("Courier New", 11F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblP1);
            y += 25;

            var hostControls = new[] {
                ("Movement:", new[] { GetImage("A.png"), GetImage("D.png") }),
                ("Punch:", new[] { GetImage("J.png") }),
                ("Dash:", new[] { GetImage("K.png") }),     // Thêm nút Dash là K
                // ("Dash:", new[] { GetImage("L.png") }),  // Nút L vẫn đang ẩn
                ("Skill 1:", new[] { GetImage("U.png") }),  // Skill 1
                ("Skill 2:", new[] { GetImage("I.png") }),  // Skill 2
            };

            foreach (var (name, imgs) in hostControls)
            {
                var row = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    AutoSize = true,
                    Location = new Point(20, y),
                    Height = 60
                };

                var lbl = new Label
                {
                    Text = name,
                    Font = new Font("Courier New", 10F),
                    AutoSize = true,
                    Margin = new Padding(0, 6, 10, 0)
                };
                row.Controls.Add(lbl);

                foreach (var img in imgs)
                {
                    var pb = new PictureBox
                    {
                        Image = img,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(45, 45),
                        Margin = new Padding(4, 0, 4, 0)
                    };
                    row.Controls.Add(pb);
                }

                panel.Controls.Add(row);
                y += 55;
            }

            y += 10;

            // Network Notes
            var lblNotes = new Label
            {
                Text = "Network Notes:",
                Font = new Font("Courier New", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, y),
                AutoSize = true
            };
            panel.Controls.Add(lblNotes);
            y += 25;

            var notes = new[] {
                "- Both players connect to the same game room.",
                "- Controls respond with minimal latency.",
                "- Game syncs every 50ms."
            };

            foreach (var note in notes)
            {
                var lblNote = new Label
                {
                    Text = note,
                    Font = new Font("Courier New", 10F),
                    AutoSize = true,
                    Location = new Point(20, y)
                };
                panel.Controls.Add(lblNote);
                y += 25;
            }

            tabOnline.Controls.Add(panel);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}