using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class LoginForm : Form
    {
        public string email = "";
        public int countdown = 60;

        public LoginForm()
        {
            InitializeComponent();
            label2.Hide();
            label1.Hide();
            label3.Hide();
            panel1.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            panel2.Hide();
            label9.Hide();
            label10.Hide();
            label14.Hide();
            label13.Hide();
            panel3.Hide();
            label16.Hide();
            label17.Hide();
        }

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
            bool check1 = true;
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                label2.Show();
                check1 = false;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                label1.Show();
                check1 = false;
            }
            return check1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            label2.Hide();
            label1.Hide();
            if (isvalid())
            {
                try
                {
                    var result = NetworkManager.Instance.Login(new LoginPacket
                    {
                        Username = txtUsername.Text,
                        Password = txtPassword.Text
                    });
                    if (result.Success) {
                        MenuForm menu = new MenuForm();
                        menu.FormClosed += (s, args) => Application.Exit();
                        menu.Show();
                        this.Hide();
                    }
                    else
                    {
                        label1.Hide();
                        label2.Hide();
                        label3.Show();
                    }
                }
                catch (IOException ioEx)
                {
                    
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            pnlLogin.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            panel1.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBox5.Clear();
            textBox6.Clear();
            label9.Hide();
            label10.Hide();
            panel2.Show();
            pnlLogin.Hide();
            email = "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public Boolean isvalid2()
        {
            bool check1 = true;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label5.Show();
                check1 = false;
            }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                label6.Show();
                check1 = false;
            }
            if (textBox3.Text != textBox2.Text)
            {
                label7.Show();
                check1 = false;
            }
            if (string.IsNullOrEmpty(textBox4.Text) || !Regex.IsMatch(textBox4.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                label8.Show();
                check1 = false;
            }
            return check1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label16.Hide(); 
            label17.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            if (isvalid2())
            {
                var result = NetworkManager.Instance.Register(new RegisterPacket
                {
                    Username = textBox1.Text,
                    Password = textBox2.Text,
                    Email = textBox4.Text
                });
                if (result.Success)
                {
                    panel1.Hide();
                    label1.Hide();
                    label2.Hide();
                    label3.Hide();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    pnlLogin.Show();
                }
                else if (result.Message == "Tên đăng nhập đã tồn tại!")
                {
                    label16.Show();
                }
                else if (result.Message == "Email đã được sử dụng!")
                {
                    label17.Show();
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            label9.Hide();
            label10.Hide();
            if (string.IsNullOrEmpty(textBox5.Text) || !Regex.IsMatch(textBox5.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                label9.Show();
            }
            else
            {
                email = textBox5.Text;
                NetworkManager.Instance.Otpsend(new OtpPacket
                {
                    Email = email
                });
                button2.Enabled = false;
                countdown = 60;
                button2.Text = $"({countdown})";
                timer1.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            txtUsername.Clear();
            txtPassword.Clear();
            pnlLogin.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label9.Hide();
            label10.Hide();
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                label10.Show();
            }
            else
            {
                var result = NetworkManager.Instance.VerifyOtp(new OtpVerifyPacket
                {
                    Email = email,
                    OtpCode = textBox6.Text
                });
                if (!result.Success)
                {
                    label10.Show();
                    return;
                }
                label13.Hide();
                label14.Hide();
                textBox7.Clear();
                textBox8.Clear();
                panel3.Show();
                panel2.Hide();
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel3.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            txtUsername.Clear();
            txtPassword.Clear();
            pnlLogin.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label14.Hide();
            label13.Hide();
            if (string.IsNullOrEmpty(textBox8.Text))
            {
                label14.Show();
            }
            else if (textBox7.Text != textBox8.Text)
            {
                label13.Show();
            }
            else
            {
                var result = NetworkManager.Instance.Forgotpass(new ForgotPasswordPacket
                {
                    Email = email,
                    Password = textBox8.Text
                });
                if (!result.Success)
                {
                    return;
                }
                panel3.Hide();
                label1.Hide();
                label2.Hide();
                label3.Hide();
                txtUsername.Clear();
                txtPassword.Clear();
                pnlLogin.Show();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            countdown--;
            if (countdown > 0)
            {
                button2.Text = $"({countdown})";
            }
            else
            {
                timer1.Stop();
                button2.Enabled = true;
                button2.Text = "Gửi";
            }
        }
    }
}
