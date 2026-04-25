using BattleGame.Client.Forms;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class LoginForm : Form
    {
        private bool _isMuted = false;
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            SoundManager.PlayBGM("montagem_hiraki.mp3");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                button1.Enabled = false;

                var result = await NetworkManager.Instance.LoginAsync(
                    new LoginPacket { Username = username, Password = password }
                );

                if (result.Success)
                {
                    new MenuForm().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại: " + result.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.Focus();
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

        //đổi thành btn sign up
        private void button3_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPasswordForm forgotPasswordForm = new ForgotPasswordForm();
            forgotPasswordForm.Show();
            this.Hide();
        }
    }
}
