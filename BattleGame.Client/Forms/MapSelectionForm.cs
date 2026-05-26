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
    public partial class MapSelectionForm : Form
    {
        private static readonly Color BorderColor = Color.FromArgb(185, 220, 245);
        private static readonly Color AccentColor = Color.PaleTurquoise;
        private static readonly Color NormalButtonColor = Color.FromArgb(44, 74, 110);
        private static readonly Color HoverButtonColor = Color.FromArgb(63, 110, 165);
        private static readonly Color GoldColor = Color.FromArgb(255, 235, 156);
        private static readonly Color PanelColor = Color.FromArgb(24, 36, 68);
        private static readonly string[] MapIds = { "terrace", "throneroom", "castle" };
        private readonly Random _random = new();
        private string _selectedMapId = "terrace";

        public string SelectedMapId => _selectedMapId;

        public MapSelectionForm()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            StartPosition = FormStartPosition.CenterParent;
            DoubleBuffered = true;
            ApplyUnifiedStyle();
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
            _selectedMapId = mapId;
            string? imageFile = GetMapImageFile(mapId);
            if (string.IsNullOrWhiteSpace(imageFile))
                return;

            string imagePath = ResolveMapImagePath(imageFile);
            if (!File.Exists(imagePath))
            {
                pictureBoxMap.Image = null;
                return;
            }

            using Image image = Image.FromFile(imagePath);
            pictureBoxMap.Image?.Dispose();
            pictureBoxMap.Image = new Bitmap(image);
        }

        private void ApplyUnifiedStyle()
        {
            BackColor = Color.FromArgb(36, 58, 94);

            label1.Font = new Font("Courier New", 26F, FontStyle.Bold);
            label1.ForeColor = AccentColor;
            label1.BackColor = Color.Transparent;
            label1.Text = "CHOOSE ARENA";
            label1.TextAlign = ContentAlignment.MiddleCenter;

            comboBoxMap.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMap.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            comboBoxMap.BackColor = Color.WhiteSmoke;
            comboBoxMap.ForeColor = Color.Black;

            pictureBoxMap.BackColor = PanelColor;
            pictureBoxMap.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxMap.SizeMode = PictureBoxSizeMode.StretchImage;

            StyleActionButton(buttonSelect, "SELECT");
            StyleActionButton(button1, "RANDOM");
            StyleActionButton(buttonCancel, "CANCEL");
        }

        private static void StyleActionButton(Button button, string text)
        {
            button.Text = text;
            button.Font = new Font("Courier New", 14F, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = AccentColor;
            button.FlatAppearance.MouseOverBackColor = HoverButtonColor;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(31, 47, 86);
            button.BackColor = NormalButtonColor;
            button.ForeColor = GoldColor;
            button.UseVisualStyleBackColor = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var outerPen = new Pen(Color.FromArgb(10, 18, 36), 5);
            using var innerPen = new Pen(BorderColor, 2);
            e.Graphics.DrawRectangle(outerPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            e.Graphics.DrawRectangle(innerPen, 7, 7, ClientSize.Width - 15, ClientSize.Height - 15);
            e.Graphics.DrawRectangle(innerPen, pictureBoxMap.Left - 6, pictureBoxMap.Top - 6, pictureBoxMap.Width + 11, pictureBoxMap.Height + 11);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMap.SelectedIndex < 0 || comboBoxMap.SelectedIndex >= MapIds.Length)
                return;

            SetMap(MapIds[comboBoxMap.SelectedIndex]);
        }

        private void MapSelectionForm_Load(object sender, EventArgs e)
        {
            comboBoxMap.SelectedIndex = 0;
            SetMap("terrace");
        }

        private void pictureBoxMap_Click(object sender, EventArgs e)
        {

        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonRandom_Click(object sender, EventArgs e)
        {
            if (comboBoxMap.Items.Count == 0)
                return;

            int nextIndex = _random.Next(comboBoxMap.Items.Count);
            if (comboBoxMap.Items.Count > 1 && nextIndex == comboBoxMap.SelectedIndex)
            {
                nextIndex = (nextIndex + _random.Next(1, comboBoxMap.Items.Count)) % comboBoxMap.Items.Count;
            }

            comboBoxMap.SelectedIndex = nextIndex;
        }
    }
}
