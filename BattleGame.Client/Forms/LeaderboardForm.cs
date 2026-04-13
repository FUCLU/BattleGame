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
    public partial class LeaderboardForm : Form
    {
        public LeaderboardForm()
        {
            InitializeComponent();
        }
        private void LeaderboardForm_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;

            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("Level", 100);
            listView1.Columns.Add("Exp", 100);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
