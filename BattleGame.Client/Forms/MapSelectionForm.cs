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
        private string _selectedMapId = "terrace";

        public string SelectedMapId => _selectedMapId;

        public MapSelectionForm()
        {
            InitializeComponent();
        }

        private static readonly string AssetsRoot = Path.Combine(
           AppDomain.CurrentDomain.BaseDirectory,
           "..", "..", "..", "Assets", "Background");

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

        private void SetMap(string mapId)
        {
            _selectedMapId = mapId;
            string? imageFile = GetMapImageFile(mapId);
            if (string.IsNullOrWhiteSpace(imageFile))
                return;

            string imagePath = Path.Combine(AssetsRoot, imageFile);
            if (File.Exists(imagePath))
                pictureBoxMap.Image = Image.FromFile(imagePath);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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
    }
}
