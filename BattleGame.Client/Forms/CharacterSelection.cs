using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
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

        private CharacterData kabold;
        private CharacterData girlKnight;
        private CharacterData _selectedChar;
        private CharacterData warrior;
        private CharacterData soldier;

        // Đường dẫn Assets/Characters
        private static readonly string AssetsRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Assets", "Characters");

        public CharacterSelection()
        {
            InitializeComponent();

        }

        public string SelectedCharacterName { get; private set; } //hàm để trả dữ liệu cho form khác

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            kabold = new CharacterData
            {
                Name = "Kabold",
                IdleImage = Path.Combine(AssetsRoot, "Kabold", "KaboldAvt.png"),
                HP = 80,
                ATK = 50,
                DEF = 10,
                SPD = 32,
                Skill = "Claw Strike"
            };

            girlKnight = new CharacterData
            {
                Name = "GirlKnight",
                IdleImage = Path.Combine(AssetsRoot, "GirlKnight", "Idle_KG_1.png"),
                HP = 100,
                ATK = 28,
                DEF = 35,
                SPD = 18,
                Skill = "Shield Bash"
            };
            warrior = new CharacterData
            {
                Name = "Warrior",
                IdleImage = Path.Combine(AssetsRoot, "Warrior", "IDLE.png"),
                HP = 120,
                ATK = 40,
                DEF = 25,
                SPD = 15,
                Skill = "Blade Slash"
            };

            soldier = new CharacterData
            {
                Name = "Soldier",
                IdleImage = Path.Combine(AssetsRoot, "Soldier", "Idle.png"),
                HP = 110,
                ATK = 45,
                DEF = 15,
                SPD = 20,
                Skill = "Shoot"
            };

            // mặc định chọn Kabold
            _selectedChar = kabold;
            UpdateDisplay(_selectedChar);
            HighlightSelected(pnlKabold);

            //trường hợp click vào ảnh haowc label
            void AddClick(Control ctrl, Panel parentPanel, CharacterData data)
            {
                ctrl.Click += (s, e) =>
                {
                    _selectedChar = data;
                    UpdateDisplay(_selectedChar);
                    HighlightSelected(parentPanel);
                };

                foreach (Control child in ctrl.Controls)
                    AddClick(child, parentPanel, data);
            }

            // áp dụng
            AddClick(pnlKabold, pnlKabold, kabold);
            AddClick(pnlGirlKnight, pnlGirlKnight, girlKnight);
            AddClick(pnlWarrior, pnlWarrior, warrior);
            AddClick(pnlSoldier, pnlSoldier, soldier);

            SetHandCursor(pnlKabold);
            SetHandCursor(pnlGirlKnight);
            SetHandCursor(pnlWarrior);
            SetHandCursor(pnlSoldier);
        }


        // Cập nhật UI
        private void UpdateDisplay(CharacterData c)
        {
            if (c == null) return;

            Image img = LoadImage(c.IdleImage);
            pbInfor.Image = img;

            // Hiển thị tên nhân vật đã chọn ở tiêu đề bên phải (label2).
            label2.Text = c.Name;

            // thông số của nhân vật
            lblHP.Text = $"❤ HP     : {c.HP}";
            lblATK.Text = $"⚔ ATK    : {c.ATK}";
            lblDEF.Text = $"🛡 DEF    : {c.DEF}";
            lblSPD.Text = $"💨 SPEED  : {c.SPD}";
            lblSkill.Text = $"✨ SKILL  : {c.Skill}";
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



        //xac nhan chon NV
        private void btnSellect_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Đã chọn: {_selectedChar.Name}");
            if (_selectedChar == null) return;

            SelectedCharacterName = _selectedChar.Name;

            this.DialogResult = DialogResult.OK; // báo form cha biết là đã chọn
            this.Close();
        }


        private void pnlGirlKnight_Click(object sender, EventArgs e)
        {
            _selectedChar = girlKnight;
            UpdateDisplay(_selectedChar);
        }

        private void pnlKabold_Click(object sender, EventArgs e)
        {
            _selectedChar = kabold;
            UpdateDisplay(_selectedChar);
        }

        private void pnlWarrior_Click(object sender, EventArgs e)
        {
            _selectedChar = warrior;
            UpdateDisplay(_selectedChar);
        }

        private void pnlSoldier_Click(object sender, EventArgs e)
        {
            _selectedChar = soldier;
            UpdateDisplay(_selectedChar);
        }

        //hàm highlight khi chón nhân vật
        void HighlightSelected(Panel selected)
        {
            pnlKabold.BackColor = Color.FromArgb(44, 74, 110);
            pnlGirlKnight.BackColor = Color.FromArgb(44, 74, 110);
            pnlWarrior.BackColor = Color.FromArgb(44, 74, 110);
            pnlSoldier.BackColor = Color.FromArgb(44, 74, 110);

            selected.BackColor = Color.FromArgb(63, 110, 165);
        }

        //đổi mouse khi hover vào panel chon nhân vật
        void SetHandCursor(Control ctrl)
        {
            ctrl.Cursor = Cursors.Hand;

            foreach (Control child in ctrl.Controls)
                SetHandCursor(child);
        }
    }
}