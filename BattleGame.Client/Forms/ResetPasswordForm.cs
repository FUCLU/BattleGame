using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace BattleGame.Client.Forms
{
    public partial class ResetPasswordForm : Form
    {
        private readonly string _email;
        private bool _isMuted = false;
        public ResetPasswordForm(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private void ResetPasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private async void button1_Click(object sender, EventArgs e)
        {
            string newPassword = textBox1.Text;
            string confirm = textBox2.Text;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirm))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Clear();
                textBox2.Focus();
                return;
            }

            try
            {
                button1.Enabled = false;

                var result = await NetworkManager.Instance.ResetPasswordAsync(
                    new ResetPasswordPacket
                    {
                        Email = _email,
                        NewPassword = newPassword
                    }
                );

                if (result.Status == "success")
                {
                    MessageBox.Show("Đặt lại mật khẩu thành công! Vui lòng đăng nhập lại.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new LoginForm().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message ?? "Đặt lại mật khẩu thất bại!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi xảy ra: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
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
