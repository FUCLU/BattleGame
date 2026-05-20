using BattleGame.Client.Managers;
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
    public partial class MenuForm : Form
    {
        private bool _isMuted = false;
        public MenuForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

     
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ModeForm modeForm = new ModeForm();
            modeForm.Show();
            this.Close();
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            StyleMusicButton();
            UpdateMusicButtonText();
        }

        private void btnMusic_Click(object sender, EventArgs e)
        {
            if (_isMuted)
            {
                SoundManager.SetVolume(1.0f);
                _isMuted = false;
            }
            else
            {
                SoundManager.SetVolume(0.0f);
                _isMuted = true;
            }

            UpdateMusicButtonText();
        }

        private void UpdateMusicButtonText()
        {
            if (_isMuted)
            {
                btnMusic.Text = "♪ Music: Off";
                btnMusic.BackColor = Color.FromArgb(15, 23, 42);
                btnMusic.ForeColor = Color.FromArgb(147, 197, 253);
            }
            else
            {
                btnMusic.Text = "♫ Music: On";
                btnMusic.BackColor = Color.FromArgb(37, 99, 235);
                btnMusic.ForeColor = Color.White;
            }
        }
        private void StyleMusicButton()
        {
            {
                btnMusic.FlatStyle = FlatStyle.Flat;
                btnMusic.FlatAppearance.BorderSize = 2;
                btnMusic.FlatAppearance.BorderColor = Color.FromArgb(96, 165, 250);

                btnMusic.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnMusic.Cursor = Cursors.Hand;

                btnMusic.Width = 135;
                btnMusic.Height = 40;

                btnMusic.TextAlign = ContentAlignment.MiddleCenter;

                btnMusic.FlatAppearance.MouseOverBackColor = Color.FromArgb(59, 130, 246);
                btnMusic.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
            }

        }
    }
}
