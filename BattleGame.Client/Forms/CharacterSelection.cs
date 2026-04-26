using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BattleGame.Client.Config;

namespace BattleGame.Client.Forms
{
    public partial class CharacterSelection : Form
    {
        private List<CharacterSelectionItem> _chars = new();
        private int _idx = 0;

        private static readonly string ConfigRoot = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "Config");

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
            _chars = CharacterCatalog.LoadSelectionItems(ConfigRoot);

            _idx = 0;
            UpdateDisplay();
        }

        // Cập nhật UI
        private void UpdateDisplay()
        {
            var c = _chars[_idx];

            // Load ảnh từ file
            Image img = LoadImage(c.GetPreviewPath(AssetsRoot));
            pictureBox2.Image = img;   // trái
            pictureBox5.Image = img;   // phải

            // Tên
            label1.Text = c.DisplayName;
            label5.Text = c.DisplayName;

            // Thông số
            lblHP.Text = $"❤  HP     :  {c.Hp}";
            lblATK.Text = $"⚔  ATK    :  {c.Atk}";
            lblDEF.Text = $"🛡  DEF    :  {c.Def}";
            lblSPD.Text = $"💨  SPEED  :  {c.Speed}";
            lblSkill.Text = $"✨  SKILL   :  {c.SkillLabel}";
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
            string characterId = _chars[_idx].Id;

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