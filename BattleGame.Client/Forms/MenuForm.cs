using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Net.NetworkInformation;

namespace BattleGame.Client.Forms
{
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
        }

       


        private void MenuForm_Load(object sender, EventArgs e)
        {

        }
        bool isHover = false;
        private void btnPlay_MouseEnter(object sender, EventArgs e)
        {
            btnPlay.Image = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            btnPlay.Image = Properties.Resources.fr;
        }

        private void btnLogout_MouseEnter(object sender, EventArgs e)
        {
            btnLogout.Image = null;
        }

        private void btnLogout_MouseLeave(object sender, EventArgs e)
        {
            btnLogout.Image = Properties.Resources.fr;
        }

        private void btnExit_MouseEnter(object sender, EventArgs e)
        {
            btnExit.Image = null;
        }

        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.Image = Properties.Resources.fr;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            this.Hide();
            CharacterSelection characterSelection = new CharacterSelection();
            characterSelection.Show();

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                LoginForm login = new LoginForm();

                // đóng LoginForm thì ứng dụng cũng thoát 
                login.FormClosed += (s, args) => this.Close();

                login.Show();
            }
        }
    }
}
