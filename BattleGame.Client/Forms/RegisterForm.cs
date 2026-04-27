using BattleGame.Client.Managers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class RegisterForm : Form
    {
        private bool _isMuted = false;

        public RegisterForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string user = textBox2.Text.Trim();
            string password = textBox3.Text;
            string confirm = textBox4.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Clear();
                textBox4.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Clear();
                textBox4.Clear();
                textBox3.Focus();
                return;
            }

            try
            {
                button1.Enabled = false;

                var result = await NetworkManager.Instance.RegisterAsync(
                    new RegisterPacket
                    {
                        Email = email,
                        Username = user,
                        Password = password
                    }
                );

                if (result.Status == "pending")
                {
                    OtpForm otp = new OtpForm(email, isReset: false);
                    otp.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đăng ký thất bại: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
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
    }
}
