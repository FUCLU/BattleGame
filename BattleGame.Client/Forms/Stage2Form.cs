using System;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class Stage2Form : Form
    {
        public Stage2Form()
        {
            InitializeComponent();
            BorderlessFormHelper.Apply(this);
            StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
