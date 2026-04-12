using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        int maxHP = 100;
        int currentHP = 100;
        //tao them bien mana cho player
        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            panelStatus.BackColor = Color.FromArgb(180, 0, 0, 0);

            lblHP.Parent = panelHPBack;
            lblHP.BackColor = Color.Transparent;
            lblHP.BringToFront();
            UpdateHPBar();
        }

        private void UpdateHPBar()
        {
            float percent = (float)currentHP / maxHP;

            panelHPFill.Width = (int)(panelHPBack.Width * percent);

            lblHP.Text = $"{currentHP}/{maxHP}";
        }
    }
}
