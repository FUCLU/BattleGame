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
            BorderlessFormHelper.Apply(this);
            this.StartPosition = FormStartPosition.CenterScreen;
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
                MessageBox.Show("Vui lòng nh?p d?y d? thông tin!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("M?t kh?u ph?i có ít nh?t 6 ký t?!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirm)
            {
                MessageBox.Show("M?t kh?u xác nh?n không kh?p!",
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
                    MessageBox.Show("Ð?t l?i m?t kh?u thành công! Vui lòng dang nh?p l?i.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    new LoginForm().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(result.Message ?? "Ð?t l?i m?t kh?u th?t b?i!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ðã có l?i x?y ra: " + ex.Message,
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
