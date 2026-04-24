using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class GameOverForm : Form
    {
        public GameOverForm()
        {
            InitializeComponent();
        }

        private void GameOverForm_Load(object sender, EventArgs e)
        {

        }

        private void btnBackLobby_Click(object sender, EventArgs e)
        {

        }

        private void btnBackLobby_MouseHover(object sender, EventArgs e)
        {
            btnBackLobby.BackColor = ColorTranslator.FromHtml("#2980B9");
        }
    }
}
