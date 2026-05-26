using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class InstructionForm : Form
    {
        private static readonly Color BgColor = Color.FromArgb(10, 15, 40);
        private static readonly Color PanelColor = Color.FromArgb(15, 22, 55);
        private static readonly Color BorderColor = Color.FromArgb(0, 120, 220);
        private static readonly Color AccentColor = Color.FromArgb(0, 200, 255);
        private static readonly Color TitleColor = Color.FromArgb(0, 200, 255);
        private static readonly Color HeaderColor = Color.FromArgb(80, 160, 255);
        private static readonly Color TextColor = Color.FromArgb(180, 215, 255);
        private static readonly Color TipColor = Color.FromArgb(255, 210, 60);
        private static readonly Color KeyBgColor = Color.FromArgb(8, 35, 90);
        private static readonly Color KeyTextColor = Color.FromArgb(210, 235, 255);

        private static readonly Font FontTitle = new Font("Courier New", 16F, FontStyle.Bold);
        private static readonly Font FontSection = new Font("Courier New", 13F, FontStyle.Bold);
        private static readonly Font FontBody = new Font("Courier New", 11.5F, FontStyle.Bold);
        private static readonly Font FontBtn = new Font("Courier New", 12F, FontStyle.Bold);
        private static readonly Font FontTab = new Font("Courier New", 12F, FontStyle.Bold);

        public InstructionForm()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            StyleForm();
            LoadDefaultTexts();
        }

        private void StyleForm()
        {
            this.BackColor = BgColor;
            this.ForeColor = TextColor;
            this.Font = FontBody;
            this.Text = "[ BATTLE GAME - INSTRUCTIONS ]";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(900, 600);

            pnlMain.BackColor = BgColor;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Size = ClientSize;

            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += TabControl_DrawItem;
            tabControl.BackColor = BgColor;
            tabControl.Font = FontTab;
            tabControl.ItemSize = new Size(180, 42);
            tabControl.Location = new Point(24, 18);
            tabControl.Size = new Size(852, 500);
            tabControl.SelectedIndexChanged += (s, e) => tabControl.Invalidate();

            tabOffline.BackColor = PanelColor;
            tabOnline.BackColor = PanelColor;

            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderColor = BorderColor;
            btnBack.FlatAppearance.BorderSize = 2;
            btnBack.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 60, 160);
            btnBack.BackColor = KeyBgColor;
            btnBack.ForeColor = AccentColor;
            btnBack.Font = FontBtn;
            btnBack.Text = "[ BACK ]";
            btnBack.Cursor = Cursors.Hand;
            btnBack.BackgroundImage = null;
            btnBack.Size = new Size(160, 52);
            btnBack.Location = new Point((ClientSize.Width - btnBack.Width) / 2, 532);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = (e.Index == tabControl.SelectedIndex);
            var rect = e.Bounds;

            Color tabBg = selected ? Color.FromArgb(0, 80, 180) : Color.FromArgb(10, 25, 70);
            e.Graphics.FillRectangle(new SolidBrush(tabBg), rect);
            e.Graphics.DrawRectangle(new Pen(selected ? AccentColor : BorderColor, 1),
                rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

            if (selected)
                e.Graphics.FillRectangle(new SolidBrush(AccentColor), rect.Left, rect.Bottom - 2, rect.Width, 2);

            string text = (selected ? "? " : "  ") + tabControl.TabPages[e.Index].Text;
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                e.Graphics.DrawString(text, FontTab, new SolidBrush(selected ? AccentColor : HeaderColor), rect, sf);
        }

        private void LoadDefaultTexts()
        {
            DisplayOfflineControls();
            DisplayOnlineControls();
        }

        private Image GetImage(string fileName)
        {
            string path1 = Path.Combine(Application.StartupPath, "Assets", "Background", fileName);
            if (File.Exists(path1)) return Image.FromFile(path1);

            string path2 = Path.Combine(Application.StartupPath, @"..\..\Assets\Background", fileName);
            if (File.Exists(path2)) return Image.FromFile(path2);

            string path3 = Path.Combine(Application.StartupPath, @"..\..\..\Assets\Background", fileName);
            if (File.Exists(path3)) return Image.FromFile(path3);

            return null;
        }

        private Label MakeSectionLabel(string text, Color color, int x, int y)
        {
            return new Label
            {
                Text = "? " + text,
                Font = FontSection,
                ForeColor = color,
                BackColor = Color.FromArgb(25, color.R, color.G, color.B),
                AutoSize = false,
                Width = 790,
                Height = 34,
                Location = new Point(x, y),
                Padding = new Padding(8, 5, 0, 0)
            };
        }

        // M?i hàng: tên action + ?nh phím (không có ô ch? key n?a)
        private FlowLayoutPanel MakeControlRow(string actionName, string imageFile)
        {
            var row = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Height = 46,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            row.Controls.Add(new Label
            {
                Text = actionName,
                Font = FontBody,
                ForeColor = TextColor,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = 150,
                Height = 42,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 2, 14, 0)
            });

            var img = GetImage(imageFile);
            if (img != null)
                row.Controls.Add(new PictureBox
                {
                    Image = img,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(40, 40),
                    Margin = new Padding(2, 2, 0, 0)
                });

            return row;
        }

        private FlowLayoutPanel MakeTextControlRow(string actionName, string keyText)
        {
            var row = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Height = 42,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            row.Controls.Add(new Label
            {
                Text = actionName,
                Font = FontBody,
                ForeColor = TextColor,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = 150,
                Height = 38,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 2, 14, 0)
            });

            row.Controls.Add(new Label
            {
                Text = keyText,
                Font = FontBody,
                ForeColor = KeyTextColor,
                BackColor = KeyBgColor,
                AutoSize = false,
                Width = 160,
                Height = 34,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(2, 4, 0, 0)
            });

            return row;
        }

        private void DisplayOfflineControls()
        {
            tabOffline.Controls.Clear();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = PanelColor,
                Padding = new Padding(18, 14, 18, 14)
            };

            int y = 14;

            panel.Controls.Add(new Label
            {
                Text = "=[ OFFLINE MODE - CONTROLS ]=",
                Font = FontTitle,
                ForeColor = TitleColor,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, y)
            });
            y += 44;

            // Player 1
            panel.Controls.Add(MakeSectionLabel("PLAYER 1", AccentColor, 10, y));
            y += 42;

            var p1 = new (string action, string img)[]
            {
                ("Move Left",  "A.png"),
                ("Move Right", "D.png"),
                ("Punch",      "J.png"),
                ("Dash",       "K.png"),
                ("Skill 1",    "U.png"),
                ("Skill 2",    "I.png"),
            };

            foreach (var (action, img) in p1)
            {
                var row = MakeControlRow(action, img);
                row.Location = new Point(20, y);
                panel.Controls.Add(row);
                y += 48;
            }

            y += 12;

            panel.Controls.Add(MakeSectionLabel("PLAYER 2 - SAME KEYBOARD", AccentColor, 10, y));
            y += 42;

            var p2 = new (string action, string key)[] 
            {
                ("Move Left",  "Left Arrow"),
                ("Move Right", "Right Arrow"),
                ("Guard",      "Down Arrow"),
                ("Punch",      "Num 1"),
                ("Dash",       "Num 2"),
                ("Skill 1",    "Num 4"),
                ("Skill 2",    "Num 5"),
            };

            foreach (var (action, key) in p2)
            {
                var row = MakeTextControlRow(action, key);
                row.Location = new Point(20, y);
                panel.Controls.Add(row);
                y += 44;
            }

            y += 12;

            // Tips
            panel.Controls.Add(MakeSectionLabel("TIPS", TipColor, 10, y));
            y += 42;

            foreach (var tip in new[]
            {
                "- Use parry to negate incoming attacks and gain mana.",
                "- Dash to quickly close distance or escape.",
                "- Manage stamina and mana for skills.",
            })
            {
                panel.Controls.Add(new Label
                {
                    Text = tip,
                    Font = FontBody,
                    ForeColor = TipColor,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(780, 30),
                    Location = new Point(24, y),
                    TextAlign = ContentAlignment.MiddleLeft
                });
                y += 32;
            }

            tabOffline.Controls.Add(panel);
        }

        private void DisplayOnlineControls()
        {
            tabOnline.Controls.Clear();

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = PanelColor,
                Padding = new Padding(18, 14, 18, 14)
            };

            int y = 14;

            panel.Controls.Add(new Label
            {
                Text = "=[ ONLINE MODE - CONTROLS & NOTES ]=",
                Font = FontTitle,
                ForeColor = TitleColor,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, y)
            });
            y += 44;

            panel.Controls.Add(MakeSectionLabel("PLAYER CONTROLS", AccentColor, 10, y));
            y += 42;

            var controls = new (string action, string img)[]
            {
                ("Move Left",  "A.png"),
                ("Move Right", "D.png"),
                ("Punch",      "J.png"),
                ("Dash",       "K.png"),
                ("Skill 1",    "U.png"),
                ("Skill 2",    "I.png"),
            };

            foreach (var (action, img) in controls)
            {
                var row = MakeControlRow(action, img);
                row.Location = new Point(20, y);
                panel.Controls.Add(row);
                y += 48;
            }

            y += 10;

            panel.Controls.Add(MakeSectionLabel("NETWORK NOTES", Color.FromArgb(120, 190, 255), 10, y));
            y += 42;

            foreach (var note in new[]
            {
                "- Both players connect to the same game room.",
                "- Controls respond with minimal latency.",
                "- Game syncs every 50ms.",
            })
            {
                panel.Controls.Add(new Label
                {
                    Text = note,
                    Font = FontBody,
                    ForeColor = Color.FromArgb(160, 210, 255),
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(780, 30),
                    Location = new Point(24, y),
                    TextAlign = ContentAlignment.MiddleLeft
                });
                y += 32;
            }

            tabOnline.Controls.Add(panel);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
