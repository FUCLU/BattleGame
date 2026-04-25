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

        private void button2_Click(object sender, EventArgs e)
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

        }
    }
}
