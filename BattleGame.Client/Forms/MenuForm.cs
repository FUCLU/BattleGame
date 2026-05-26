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
        public MenuForm()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

     
        private void button3_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
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
            AudioSettingsButton.Attach(this, btnSetting);
        }
    }
}
