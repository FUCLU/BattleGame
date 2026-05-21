using System;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BattleGame.Client.Config;


namespace BattleGame.Client.Forms
{
    public partial class OfflineMode_CPU : Form
    {
        public OfflineMode_CPU()
        {
            InitializeComponent();
            ApplyOfflineBackground();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private static readonly string AssetsRoot = Path.Combine(
           AppDomain.CurrentDomain.BaseDirectory,
           "..", "..", "..", "Assets");


        private string currentMap = "";
        private string currentMode = "easy";
        private string playerCharacterId = string.Empty;

        private void ApplyOfflineBackground()
        {
            string backgroundPath = Path.Combine(AssetsRoot, "Background", "offlinemode.png");
            if (File.Exists(backgroundPath))
            {
                BackgroundImage = Image.FromFile(backgroundPath);
                BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private string CreateRandomBotId()
        {
            Random rnd = new Random();
            string[] ids = { "lord", "samurai", "kitsune", "wizard", "haladin", "heavycrystal" };
            return ids[rnd.Next(ids.Length)];
        }

        private static string ToDisplayName(string characterId)
        {
            return characterId switch
            {
                "lord" => "Lord",
                "samurai" => "Samurai",
                "kitsune" => "Kitsune",
                "wizard" => "Wizard",
                "haladin" => "Haladin",
                "heavycrystal" => "HeavyCrystal",
                _ => characterId
            };
        }

        private static string? GetMapImageFile(string mapId)
        {
            return mapId switch
            {
                "terrace" => "terrace.png",
                "throneroom" => "throneroom.png",
                "castle" => "castle.png",
                "forest" => "BackgroundForest.png",
                _ => null
            };
        }

        private void SetMap(string mapId)
        {
            currentMap = mapId;
            string? imageFile = GetMapImageFile(mapId);
            if (string.IsNullOrWhiteSpace(imageFile))
                return;

            string imagePath = imageFile.Contains(Path.DirectorySeparatorChar)
                ? Path.Combine(AssetsRoot, imageFile)
                : Path.Combine(AssetsRoot, "Background", imageFile);
            if (File.Exists(imagePath))
                pictureBoxMap.Image = Image.FromFile(imagePath);
        }

        private void comboBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (comboBoxMap.SelectedIndex)
            {
                case 0:
                    SetMap("terrace");
                    break;

                case 1:
                    SetMap("throneroom");
                    break;

                case 2:
                    SetMap("castle");
                    break;

                case 3:
                    SetMap("forest");
                    break;

            }
        }


        private void OfflineModeSelection_Load(object sender, EventArgs e)
        {
            comboBoxMap.SelectedIndex = 0;
            SetMap("terrace");
            if (string.IsNullOrWhiteSpace(playerCharacterId))
            {
                playerCharacterId = "lord";
            }

            lblNameCharPlayer.Text = ToDisplayName(playerCharacterId);
            _ = CreateRandomBotId();
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
                MessageBox.Show("Vui lòng chọn nhân vật trước khi vào game.");
                return;
            }

            if (string.IsNullOrWhiteSpace(currentMap))
            {
                MessageBox.Show("Vui lòng chọn bản đồ trước khi vào game.");
                return;
            }

            GameForm gameForm = new GameForm(playerCharacterId, currentMap, returnFormOnExit: this);
            Hide();
            gameForm.Show();
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
            OfflineMode offlineMode = new OfflineMode();
            offlineMode.Show();
            this.Close();
        }

    }
}
