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
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void LeaderboardForm_Load(object sender, EventArgs e)
        {

            listView1.View = View.Details;

            listView1.Columns.Add("Name", 200);
            listView1.Columns.Add("Level", 100);
            listView1.Columns.Add("Exp", 100);
            listView1.Items.Add(new ListViewItem(new[] { "Tester", "1", "200exp" }));
            ResizeColumns();
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ResizeColumns()
        {
            int totalWidth = listView1.ClientSize.Width;

            listView1.Columns[0].Width = (int)(totalWidth * 0.2); // LEVEL
            listView1.Columns[1].Width = (int)(totalWidth * 0.5); // NAME
            listView1.Columns[2].Width = (int)(totalWidth * 0.3); // XP
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            ResizeColumns();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
