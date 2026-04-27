using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Client.Config;


namespace BattleGame.Client.Forms
{
    public partial class OfflineModeSelection : Form
    {
        public OfflineModeSelection()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private static readonly string AssetsRoot = Path.Combine(
           AppDomain.CurrentDomain.BaseDirectory,
           "..", "..", "..", "Assets", "Background");


        string currentMap = ""; //biến lưu map đã chọn
        string currentMode = ""; //biến lưu mode đã chọn
        private string playerCharacterId = string.Empty;
        private void comboBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (comboBoxMap.SelectedIndex)
            {
                case 0:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "terrace.png"));
                    currentMap = "terrace";
                    break;

                case 1:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "throneroom.png"));
                    currentMap = "throneroom";
                    break;

                case 2:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "castle.png"));
                    currentMap = "castle";
                    break;
            }
        }


        private void OfflineModeSelection_Load(object sender, EventArgs e)
        {
            comboBoxMap.SelectedIndex = 0;
            pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "terrace.png"));
            currentMap = "terrace";
        }


        void SelectMode(Button selectedBtn)
        {
            Button[] buttons = { btnEasy, btnMedium, btnHard };
            foreach (Button btn in buttons)
            {
                btn.BackColor = Color.FromArgb(42, 93, 143);
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            selectedBtn.BackColor = Color.FromArgb(244, 112, 157);
            selectedBtn.ForeColor = Color.White;
            selectedBtn.FlatAppearance.BorderSize = 2;
            selectedBtn.FlatAppearance.BorderColor = Color.LightBlue;
            currentMode = selectedBtn.Text.ToLower();
        }


        private void btnEasy_Click(object sender, EventArgs e)
        {
            SelectMode(btnEasy);
        }

        private void btnMedium_Click(object sender, EventArgs e)
        {
            SelectMode(btnMedium);
        }

        private void btnHard_Click(object sender, EventArgs e)
        {
            SelectMode(btnHard);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playerCharacterId))
            {
                MessageBox.Show("Vui long chon nhan vat truoc khi vao game.");
                return;
            }

            GameForm gameForm = new GameForm(playerCharacterId);
            gameForm.Show();
            this.Close();

            // gửi lên server
        }

        private void btnSelCharPlayer_Click(object sender, EventArgs e)
        {
            CharacterSelection f = new CharacterSelection();
            if (f.ShowDialog() == DialogResult.OK)
            {
                playerCharacterId = f.SelectedCharacterId;

                string displayName = CharacterCatalog
                    .LoadSelectionItems(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."))
                    .FirstOrDefault(x => x.Id.Equals(playerCharacterId, StringComparison.OrdinalIgnoreCase))
                    ?.DisplayName
                    ?? playerCharacterId;

                lblNameCharPlayer.Text = displayName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();
            this.Close();
        }

    }
}
