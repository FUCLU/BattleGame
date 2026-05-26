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
    public partial class VictoryForm : Form
    {
        public VictoryForm()
        {
            InitializeComponent();
            NormalizeImageButton(button1);
            NormalizeImageButton(button2);
            BorderlessFormHelper.Apply(this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private static void NormalizeImageButton(Button button)
        {
            button.UseVisualStyleBackColor = false;
            button.BackColor = Color.FromArgb(30, 88, 160);
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(38, 108, 190);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 64, 126);
        }

        private void VictoryForm_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            LeaderboardForm leaderboardForm = new LeaderboardForm();
            leaderboardForm.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm(); 
            menuForm.ShowDialog();
            this.Close();
        }
    }
}
