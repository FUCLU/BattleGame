using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Shared.Models;


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

        private Character playerChar; //biến lưu nhân vật người chơi đã chọn
        private Character botChar;

        private Character CreateRandomBot()
        {
            Random rnd = new Random();
            int r = rnd.Next(2);

            if (r == 0)
            {
                return CreateCharacter("Knight");
            }
            else
            {
                return CreateCharacter("Mage");
            }
        }

        private Character CreateCharacter(string name)
        {
            return name switch
            {
                "Knight" => new Character("Knight", 150, 20, 10, 5),
                "Mage" => new Character("Mage", 100, 30, 3, 10),
                _ => throw new Exception("Character không hợp lệ")
            };
        }
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
            string map = currentMap;
            string mode = currentMode;

            GameForm gameForm = new GameForm(playerChar, botChar, map, mode);
            gameForm.Show();
            this.Close();

            // gửi lên server
        }

        private void btnSelCharPlayer_Click(object sender, EventArgs e)
        {
            CharacterSelection f = new CharacterSelection();
            if (f.ShowDialog() == DialogResult.OK)
            {
                playerChar = CreateCharacter(f.SelectedCharacterName);
                lblNameCharPlayer.Text = playerChar.Name;
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
