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
    public partial class DungeonMode : Form
    {
        public DungeonMode()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnStage1_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm("haladin", "cave");
            gameForm.Show();
            this.Close();
        }

        private void btnStage2_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm("lord", "stage2");
            gameForm.Show();
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            OfflineMode offlineMode = new OfflineMode();
            offlineMode.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
