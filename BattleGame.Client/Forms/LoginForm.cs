using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class LoginForm : Form
    {

        public LoginForm()
        {
            InitializeComponent();
        }
        private string userTest = "test", pwTest = "test", u = "a", p = "a";
        private void SetRoundedPanel(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            panel.Region = new Region(path);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            SetRoundedPanel(pnlLogin, 45);
        }

        private GraphicsPath GetRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }



        private void pnlLogin_Paint_1(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var rect = new Rectangle(0, 0, pnlLogin.Width - 1, pnlLogin.Height - 1);
            using (var backgroundBrush = new LinearGradientBrush(
                pnlLogin.ClientRectangle,
                Color.FromArgb(45, 45, 55),
                Color.FromArgb(20, 20, 35),
                90f))
            {

                e.Graphics.FillRectangle(backgroundBrush, pnlLogin.ClientRectangle);
            }
            int glowWidth = 8;
            Color glowColor = Color.Cyan;

            for (int i = glowWidth; i >= 1; i--)
            {
                int alpha = (int)(180 * (i / (float)glowWidth));
                using (Pen glowPen = new Pen(Color.FromArgb(alpha, glowColor), i * 2f))
                {
                    glowPen.StartCap = LineCap.Round;
                    glowPen.EndCap = LineCap.Round;
                    e.Graphics.DrawPath(glowPen, GetRoundPath(rect, 20));
                }
            }
            using (Pen borderPen = new Pen(Color.FromArgb(220, 0, 255, 255), 3f))
            {
                borderPen.StartCap = LineCap.Round;
                borderPen.EndCap = LineCap.Round;
                e.Graphics.DrawPath(borderPen, GetRoundPath(rect, 20));
            }
            using (var fillBrush = new LinearGradientBrush(
                pnlLogin.ClientRectangle,
                Color.FromArgb(80, 35, 45, 55),
                Color.FromArgb(140, 15, 25, 40),
                90f))
            {
                e.Graphics.FillPath(fillBrush, GetRoundPath(rect, 20));
            }

            using (var innerGlow = new LinearGradientBrush(
                pnlLogin.ClientRectangle,
                Color.FromArgb(60, 0, 180, 255),
                Color.FromArgb(0, 0, 80, 120),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillPath(innerGlow, GetRoundPath(rect, 20));
            }
        }

        private void checkShowpw_CheckedChanged(object sender, EventArgs e)
        {
            if (checkShowpassword.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }
        public Boolean isvalid()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {


            if (isvalid())
            {
                if (txtUsername.Text == userTest && txtPassword.Text == pwTest || txtUsername.Text == u && txtPassword.Text == p)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MenuForm menu = new MenuForm();

                    // Khi đóng MenuForm thì tắt hẳn chương trình 
                    menu.FormClosed += (s, args) => Application.Exit();

                    menu.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu hoặc username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {


            btnLogin.BackColor = Color.DeepPink;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.Gray;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
