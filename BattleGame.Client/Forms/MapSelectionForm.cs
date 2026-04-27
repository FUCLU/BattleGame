using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class MapSelectionForm : Form
    {
        public MapSelectionForm()
        {
            InitializeComponent();
        }

        private static readonly string AssetsRoot = Path.Combine(
           AppDomain.CurrentDomain.BaseDirectory,
           "..", "..", "..", "Assets", "Background");

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (comboBoxMap.SelectedIndex)
            {
                case 0:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "terrace.png"));
                    break;

                case 1:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "throneroom.png"));
                    break;

                case 2:
                    pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "castle.png"));
                    break;
            }
        }

        private void MapSelectionForm_Load(object sender, EventArgs e)
        {
            comboBoxMap.SelectedIndex = 0;
            pictureBoxMap.Image = Image.FromFile(Path.Combine(AssetsRoot, "terrace.png"));
        }

        private void pictureBoxMap_Click(object sender, EventArgs e)
        {

        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
