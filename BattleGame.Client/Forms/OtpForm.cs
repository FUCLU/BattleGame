using BattleGame.Client.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BattleGame.Shared.Packets;

namespace BattleGame.Client.Forms
{
    public partial class OtpForm : Form
    {
        private bool _isMuted = false;

        private readonly string _email;
        private readonly bool _isReset;

        private System.Windows.Forms.Timer countdown;
        private int secondsLeft = 60;

        public OtpForm(string email, bool isReset)
        {
            InitializeComponent();
            _email = email;
            _isReset = isReset;
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void OtpForm_Load(object sender, EventArgs e)
        {
            var textBoxes = new[] { textBox1, textBox2, textBox3,
                            textBox4, textBox5, textBox6 };

            foreach (var tb in textBoxes)
            {
                tb.MaxLength = 1;
                tb.TextAlign = HorizontalAlignment.Center;

                // Tự động nhảy sang ô kế tiếp khi nhập xong
                tb.KeyPress += (s, ev) =>
                {
                    if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    {
                        ev.Handled = true; // Chặn ký tự không phải số
                        return;
                    }
                };

                tb.TextChanged += (s, ev) =>
                {
                    var current = (TextBox)s;
                    int index = Array.IndexOf(textBoxes, current);

                    // Nhập xong → nhảy sang ô tiếp theo
                    if (current.Text.Length == 1 && index < textBoxes.Length - 1)
                    {
                        textBoxes[index + 1].Focus();
                    }
                };

                // Nhấn Backspace → quay lại ô trước
                tb.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Back)
                    {
                        var current = (TextBox)s;
                        int index = Array.IndexOf(textBoxes, current);
                        if (current.Text.Length == 0 && index > 0)
                        {
                            textBoxes[index - 1].Focus();
                            textBoxes[index - 1].Clear();
                        }
                    }
                };
            }

            StartCountdown();
        }

        private void StartCountdown()
        {
            secondsLeft = 60;
            button1.Enabled = true;
            linkLabel1.Enabled = false;
            UpdateCountdownLabel();

            countdown = new System.Windows.Forms.Timer();
            countdown.Interval = 1000; // 1 giây
            countdown.Tick += CountdownTimer_Tick;
            countdown.Start();

        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            secondsLeft--;
            UpdateCountdownLabel();

            if (secondsLeft <= 0)
            {
                countdown.Stop();
                button1.Enabled = false;
                linkLabel1.Enabled = true;
                label4.Text = "Mã OTP đã hết hạn.";
                label4.ForeColor = Color.Red;
            }
        }

        private void UpdateCountdownLabel()
        {
            label4.Text = $"Mã OTP sẽ hết hạn sau: {secondsLeft} giây";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text + textBox2.Text + textBox3.Text + textBox4.Text + textBox5.Text + textBox6.Text;

            if (code.Length != 6)
            {
                MessageBox.Show("Vui lòng nhập đủ 6 chữ số mã OTP!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                button1.Enabled = false;

                var result = await NetworkManager.Instance.VerifyOtpAsync(
                    new OtpVerifyPacket
                    {
                        Email = _email,
                        Code = code,
                        isReset = _isReset,
                    }
                );

                if (result.Status == "success")
                {
                    if (_isReset)
                    {
                        new ResetPasswordForm(_email).Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Xác thực OTP thành công! Bạn có thể đăng nhập ngay bây giờ.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoginForm loginForm = new LoginForm();
                        loginForm.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(result.Message ?? "Mã OTP không đúng!",
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

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                await NetworkManager.Instance.ForgotPasswordAsync(
                    new ForgotPasswordPacket { Email = _email }
                );
                MessageBox.Show("Đã gửi lại mã OTP về email!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                StartCountdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            countdown?.Stop();
            countdown?.Dispose();
            base.OnFormClosed(e);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
