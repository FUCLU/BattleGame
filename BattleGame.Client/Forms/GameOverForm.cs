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
            NormalizeImageButton(btnBackLobby);
            NormalizeImageButton(BtnLeaderBoard);
            BorderlessFormHelper.Apply(this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private static void NormalizeImageButton(Button button)
        {
            button.UseVisualStyleBackColor = false;
            button.BackColor = Color.FromArgb(52, 152, 219);
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(31, 97, 141);
        }

        private void GameOverForm_Load(object sender, EventArgs e)
        {

        }

        private void btnBackLobby_Click(object sender, EventArgs e)
        {
            this.Close();
            JoinRoom joinRoom = new JoinRoom();
            joinRoom.Show();
        }

        private void btnBackLobby_MouseHover(object sender, EventArgs e)
        {
            btnBackLobby.BackColor = ColorTranslator.FromHtml("#2980B9");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BtnLeaderBoard_Click(object sender, EventArgs e)
        {
            this.Close();
            LeaderboardForm leaderboardForm = new LeaderboardForm();
            leaderboardForm.Show();
        }
    }
}
