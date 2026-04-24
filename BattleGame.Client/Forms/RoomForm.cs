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
    public partial class RoomForm : Form
    {
        public RoomForm()
        {
            InitializeComponent();
        }

        private void RoomForm_Load(object sender, EventArgs e)
        {
            AddMessage("", "Connecting to room...");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapSelectionForm mapSelectionForm = new MapSelectionForm();
            mapSelectionForm.Show();
        }

  
        void AddMessage(string sender, string message)
        {
            string time = DateTime.Now.ToString("HH:mm");

            string formatted;

            formatted = $"[{time}] {sender}: {message}\n";
            richtxtBoxMessage.AppendText(formatted);
            richtxtBoxMessage.ScrollToCaret();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtBoxInp.Text.Trim();

            if (string.IsNullOrEmpty(msg)) return;

            AddMessage("tester", msg); // TODO: thay bằng username thật

            txtBoxInp.Clear();
        }

        private void txtBoxInp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

    }
}
