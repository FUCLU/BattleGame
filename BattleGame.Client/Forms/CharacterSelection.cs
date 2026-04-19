using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class CharacterSelection : Form
    {
        // Model nhân vật 
        private class CharacterData
        {
            public string Name { get; set; }
            public string IdleImage { get; set; }  // đường dẫn ảnh IDLE
            public int HP { get; set; }
            public int ATK { get; set; }
            public int DEF { get; set; }
            public int SPD { get; set; }
            public string Skill { get; set; }
        }

        private List<CharacterData> _chars;
        private int _idx = 0;

        // Đường dẫn Assets/Characters
        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets", "Characters");

        public CharacterSelection()
        {
            InitializeComponent();

            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;
        }

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            _chars = new List<CharacterData>
            {
                new CharacterData {
                    Name      = "Warrior",
                    IdleImage = Path.Combine(AssetsRoot, "Warrior", "IDLE.png"),
                    HP = 120, ATK = 40, DEF = 25, SPD = 15,
                    Skill = "Blade Slash"
                },
                new CharacterData {
                    Name      = "GirlKnight",
                    IdleImage = Path.Combine(AssetsRoot, "GirlKnight", "Idle_KG_1.png"),
                    HP = 100, ATK = 28, DEF = 35, SPD = 18,
                    Skill = "Shield Bash"
                },
                new CharacterData {
                    Name      = "Kabold",
                    IdleImage = Path.Combine(AssetsRoot, "Kabold", "KaboldAvt.png"),
                    HP = 80, ATK = 50, DEF = 10, SPD = 32,
                    Skill = "Claw Strike"
                },
                new CharacterData {
                    Name      = "Soldier",
                    IdleImage = Path.Combine(AssetsRoot, "Soldier", "Idle.png"),
                    HP = 110, ATK = 45, DEF = 15, SPD = 20,
                    Skill = "Shoot"
                },
                new CharacterData {
                    Name      = "Wizard",
                    IdleImage = Path.Combine(AssetsRoot, "Wizard", "Idle.png"),
                    HP = 100, ATK = 20, DEF = 10, SPD = 20,
                    Skill = "Light Ball"
                },
                new CharacterData {
                    Name      = "Samurai",
                    IdleImage = Path.Combine(AssetsRoot, "Samurai", "Idle.png"),
                    HP = 100, ATK = 20, DEF = 10, SPD = 20,
                    Skill = "Blade Wave"
                },
            };

            _idx = 0;
            UpdateDisplay();
        }

        // Cập nhật UI
        private void UpdateDisplay()
        {
            var c = _chars[_idx];

            // Load ảnh từ file
            Image img = LoadImage(c.IdleImage);
            pictureBox2.Image = img;   // trái
            pictureBox5.Image = img;   // phải

            // Tên
            label1.Text = c.Name;
            label5.Text = c.Name;

            // Thông số
            lblHP.Text = $"❤  HP     :  {c.HP}";
            lblATK.Text = $"⚔  ATK    :  {c.ATK}";
            lblDEF.Text = $"🛡  DEF    :  {c.DEF}";
            lblSPD.Text = $"💨  SPEED  :  {c.SPD}";
            lblSkill.Text = $"✨  SKILL   :  {c.Skill}";
        }

        private Image LoadImage(string path)
        {
            try
            {
                if (File.Exists(path))
                    return Image.FromFile(path);
            }
            catch { }
            return null;
        }

        // Mũi tên carouse
        private void btnPrev_Click(object sender, EventArgs e)
        {
            _idx = (_idx - 1 + _chars.Count) % _chars.Count;
            UpdateDisplay();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _idx = (_idx + 1) % _chars.Count;
            UpdateDisplay();
        }

        // SELECT 
        private void button1_Click(object sender, EventArgs e)
        {
            string characterName = _chars[_idx].Name;
            // Convert tên nhân vật thành characterId (lowercase)
            string characterId = characterName.ToLower();

            // Chuyển sang GameForm
            GameForm gameForm = new GameForm(characterId);
            gameForm.Show();
            this.Hide();
        }

        // Giữ nguyên handler cũ để tránh lỗi build
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click_1(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

    }
}