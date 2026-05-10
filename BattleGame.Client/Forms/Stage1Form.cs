using System;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class Stage1Form : Form
    {
        public Stage1Form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void Stage1Form_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm("lord", "cave");
            gameForm.Show();
            this.Close();
        }
    }
}
