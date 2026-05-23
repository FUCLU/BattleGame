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
    public partial class OfflineMode : Form
    {
        public OfflineMode()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnVsBot_Click(object sender, EventArgs e)
        {
            OfflineMode_CPU offlineModeCpu = new OfflineMode_CPU();
            offlineModeCpu.Show();
            this.Close();
        }

        private void btnDungeon_Click(object sender, EventArgs e)
        {
            DungeonMode dungeonMode = new DungeonMode(this);
            Hide();
            dungeonMode.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            ModeForm menuForm = new ModeForm();
            menuForm.Show();
        }
    }
}
