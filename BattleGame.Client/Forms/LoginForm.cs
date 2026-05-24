using BattleGame.Client.Forms;
using BattleGame.Client.Managers;
using BattleGame.Shared.Packets;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AcceptButton = button1;
            this.ActiveControl = textBox1;
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
            AudioSettingsButton.Attach(this, btnSetting);
            SoundManager.PlayBGM("xtremefreddy.mp3");
            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    textBox1.Focus();
                    textBox1.SelectionStart = textBox1.TextLength;
                }
            }));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (ActiveControl == null || ActiveControl == this)
            {
                textBox1.Focus();
                textBox1.SelectionStart = textBox1.TextLength;
            }
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

                bool connected = await NetworkManager.Instance.EnsureConnectedAsync();
                if (!connected)
                {
                    MessageBox.Show(
                        "Không thể kết nối server.\nHãy chạy server/load balancer trước (ví dụ: docker compose up --build).",
                        "Lỗi kết nối",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var result = await NetworkManager.Instance.LoginAsync(
                    new LoginPacket { Username = username, Password = password }
                );

                if (result.Success)
                {
                    PlayerSession.Username = username;
                    NetworkManager.Instance.RememberLogin(username, password, result.UserId);
                    JoinRoom.ResetOwnedRoomState();
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
                MessageBox.Show("Đăng nhập lỗi: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
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

        private void btnSetting_Click(object sender, EventArgs e)
        {

        }
    }
}
