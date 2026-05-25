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
        private static readonly Color PageColor = Color.FromArgb(36, 58, 94);
        private static readonly Color PanelColor = Color.FromArgb(24, 36, 68);
        private static readonly Color PanelBorderColor = Color.FromArgb(185, 220, 245);
        private static readonly Color AccentColor = Color.PaleTurquoise;
        private static readonly Color TextColor = Color.FromArgb(220, 235, 255);
        private static readonly Color NormalButtonColor = Color.FromArgb(44, 74, 110);
        private static readonly Color HoverButtonColor = Color.FromArgb(57, 100, 150);
        private static readonly Color GoldColor = Color.FromArgb(255, 235, 156);

        private static readonly Font TitleFont = new("Courier New", 26F, FontStyle.Bold);
        private static readonly Font PanelTitleFont = new("Book Antiqua", 20F, FontStyle.Bold);
        private static readonly Font BodyFont = new("Segoe UI", 12F, FontStyle.Bold);
        private static readonly Font ButtonFont = new("Courier New", 13F, FontStyle.Bold);
        private static readonly Font MapFont = new("Segoe UI", 12F, FontStyle.Bold);

        private string _player1CharacterId = "lord";
        private string _player2CharacterId = "samurai";
        private string _currentMap = "terrace";

        public OfflineMode_CPU()
        {
            InitializeComponent();
            ApplyUnifiedStyle();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void ApplyUnifiedStyle()
        {
            BackgroundImage = null;
            BackColor = PageColor;
            AutoScaleMode = AutoScaleMode.None;
            Text = "Offline Local Battle";

            label1.Text = "OFFLINE MODE";
            label1.Font = TitleFont;
            label1.ForeColor = AccentColor;
            label1.BackColor = Color.Transparent;
            label1.TextAlign = ContentAlignment.MiddleCenter;

            StylePanel(panel2);
            StylePanel(panel1);
            StylePanel(panelMap);

            StyleHeader(lblYouTitle, "PLAYER 1");
            StyleHeader(lblBotTitle, "PLAYER 2");

            StyleCharacterArea(lblCharacterCaption, lblNameCharPlayer, btnSelCharPlayer, _player1CharacterId);
            StyleCharacterArea(lblPlayer2CharacterCaption, lblNameCharPlayer2, btnSelCharPlayer2, _player2CharacterId);
            StyleMapSelector();

            StyleActionButton(button2, "BACK");
            StyleActionButton(btnPlay, "PLAY");
        }

        private void StylePanel(Panel panel)
        {
            panel.BackgroundImage = null;
            panel.BackColor = PanelColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Paint -= PanelFrame_Paint;
            panel.Paint += PanelFrame_Paint;
        }

        private void StyleHeader(Label label, string text)
        {
            label.Text = text;
            label.Font = PanelTitleFont;
            label.ForeColor = GoldColor;
            label.BackColor = Color.Transparent;
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void StyleCharacterArea(Label caption, Label nameLabel, Button selectButton, string characterId)
        {
            caption.Text = "Character:";
            caption.Font = BodyFont;
            caption.ForeColor = TextColor;
            caption.BackColor = Color.Transparent;
            caption.TextAlign = ContentAlignment.MiddleLeft;

            nameLabel.Text = ToDisplayName(characterId);
            nameLabel.Font = BodyFont;
            nameLabel.ForeColor = GoldColor;
            nameLabel.BackColor = Color.Transparent;
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;

            selectButton.BackgroundImage = null;
            selectButton.FlatStyle = FlatStyle.Flat;
            selectButton.FlatAppearance.BorderSize = 2;
            selectButton.FlatAppearance.BorderColor = AccentColor;
            selectButton.FlatAppearance.MouseOverBackColor = HoverButtonColor;
            selectButton.BackColor = NormalButtonColor;
            selectButton.ForeColor = GoldColor;
            selectButton.Font = ButtonFont;
            selectButton.Text = "SELECT CHARACTER";
            selectButton.UseVisualStyleBackColor = false;
        }

        private void StyleMapSelector()
        {
            lblMapTitle.Text = "MAP SELECT";
            lblMapTitle.Font = new Font("Book Antiqua", 18F, FontStyle.Bold);
            lblMapTitle.ForeColor = GoldColor;
            lblMapTitle.BackColor = Color.Transparent;
            lblMapTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblMapCaption.Text = "Arena:";
            lblMapCaption.Font = MapFont;
            lblMapCaption.ForeColor = TextColor;
            lblMapCaption.BackColor = Color.Transparent;
            lblMapCaption.TextAlign = ContentAlignment.MiddleLeft;

            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            comboBoxMap.BackColor = Color.WhiteSmoke;
            comboBoxMap.ForeColor = Color.Black;
            if (comboBoxMap.Items.Count == 0)
                comboBoxMap.Items.AddRange(new object[] { "Map 1", "Map 2", "Map 3" });
            if (comboBoxMap.SelectedIndex < 0)
                comboBoxMap.SelectedIndex = 0;

            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMap.BorderStyle = BorderStyle.FixedSingle;
        }

        private void PanelFrame_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
                return;

            using var outerPen = new Pen(Color.FromArgb(10, 18, 36), 4);
            using var innerPen = new Pen(PanelBorderColor, 2);
            e.Graphics.DrawRectangle(outerPen, 0, 0, panel.Width - 1, panel.Height - 1);
            e.Graphics.DrawRectangle(innerPen, 5, 5, panel.Width - 11, panel.Height - 11);
        }

        private void StyleActionButton(Button button, string text)
        {
            button.BackgroundImage = null;
            button.Text = text;
            button.Font = new Font("Courier New", 15F, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = AccentColor;
            button.FlatAppearance.MouseOverBackColor = HoverButtonColor;
            button.ForeColor = GoldColor;
            button.BackColor = NormalButtonColor;
            button.UseVisualStyleBackColor = false;
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
                "stonegolem" => "Golem",
                _ => characterId
            };
        }

        private static string ResolveDisplayName(string characterId)
        {
            return CharacterCatalog
                .LoadSelectionItems(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."))
                .FirstOrDefault(x => x.Id.Equals(characterId, StringComparison.OrdinalIgnoreCase))
                ?.DisplayName
                ?? ToDisplayName(characterId);
        }

        private static string? GetMapImageFile(string mapId)
        {
            return mapId switch
            {
                "terrace" => "terrace.png",
                "throneroom" => "throneroom.png",
                "castle" => "castle.png",
                _ => null
            };
        }

        private static string ResolveMapImagePath(string imageFile)
        {
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Background", imageFile);
            if (File.Exists(outputPath))
                return outputPath;

            return Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "Assets", "Background", imageFile));
        }

        private void SetMap(string mapId)
        {
            _currentMap = mapId;
            string? imageFile = GetMapImageFile(mapId);
            if (string.IsNullOrWhiteSpace(imageFile))
                return;

            string imagePath = ResolveMapImagePath(imageFile);
            if (File.Exists(imagePath))
                pictureBoxMap.Image = Image.FromFile(imagePath);
        }

        private void OfflineModeSelection_Load(object sender, EventArgs e)
        {
            lblNameCharPlayer.Text = ResolveDisplayName(_player1CharacterId);
            lblNameCharPlayer2.Text = ResolveDisplayName(_player2CharacterId);
            if (comboBoxMap.SelectedIndex < 0)
                comboBoxMap.SelectedIndex = 0;
            SetMap(_currentMap);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_player1CharacterId) || string.IsNullOrWhiteSpace(_player2CharacterId))
            {
                MessageBox.Show("Please select characters for both players.");
                return;
            }

            GameForm gameForm = new(
                _player1CharacterId,
                _currentMap,
                _player2CharacterId,
                localUsername: "Player 1",
                enemyUsername: "Player 2",
                returnFormOnExit: this,
                localTwoPlayer: true);
            Hide();
            gameForm.Show();
        }

        private void btnSelCharPlayer_Click(object sender, EventArgs e)
        {
            using CharacterSelection selection = new();
            if (selection.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(selection.SelectedCharacterId))
            {
                _player1CharacterId = selection.SelectedCharacterId;
                lblNameCharPlayer.Text = ResolveDisplayName(_player1CharacterId);
            }
        }

        private void btnSelCharPlayer2_Click(object sender, EventArgs e)
        {
            using CharacterSelection selection = new();
            if (selection.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(selection.SelectedCharacterId))
            {
                _player2CharacterId = selection.SelectedCharacterId;
                lblNameCharPlayer2.Text = ResolveDisplayName(_player2CharacterId);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OfflineMode offlineMode = new();
            offlineMode.Show();
            Close();
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
            }
        }

        private void pictureBoxMap_Click(object sender, EventArgs e)
        {

        }
    }
}
