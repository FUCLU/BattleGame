using System.Net;
using System.Net.Mail;
using BattleGame.Server.Config;
using BattleGame.Server.Logging;

namespace BattleGame.Server.Services
{
    public class EmailService
    {
        private readonly SmtpConfig _config;

        public EmailService(SmtpConfig config)
        {
            _config = config;
        }
        public void SendOtpEmail(string toEmail, string otpCode)
        {
            string subject = "BattleGame — Mã xác nhận OTP";

            string body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0; padding:0; background-color:#f0f2f5; font-family: Arial, sans-serif;'>

  <!-- Wrapper -->
  <table width='100%' cellpadding='0' cellspacing='0' style='background-color:#f0f2f5; padding: 40px 0;'>
    <tr>
      <td align='center'>

        <!-- Card -->
        <table width='520' cellpadding='0' cellspacing='0' style='background:#ffffff; border-radius:12px; overflow:hidden; box-shadow: 0 4px 24px rgba(0,0,0,0.08);'>

          <!-- Header -->
          <tr>
            <td style='background: linear-gradient(135deg, #1a1a2e 0%, #16213e 60%, #0f3460 100%); padding: 36px 40px; text-align:center;'>
              <h1 style='margin:0; color:#e94560; font-size:28px; letter-spacing:3px; text-transform:uppercase;'>⚔ BattleGame</h1>
              <p style='margin:8px 0 0; color:#a0a8b8; font-size:13px; letter-spacing:1px;'>BATTLE ARENA ONLINE</p>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style='padding: 40px 40px 32px;'>
              <p style='margin:0 0 8px; color:#1a1a2e; font-size:22px; font-weight:bold;'>Xác nhận tài khoản</p>
              <p style='margin:0 0 24px; color:#666; font-size:14px; line-height:1.6;'>
                Chúng tôi nhận được yêu cầu từ tài khoản của bạn.<br/>
                Vui lòng sử dụng mã OTP bên dưới để tiếp tục.
              </p>

              <!-- OTP Box -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td align='center' style='padding: 8px 0 32px;'>
                    <div style='display:inline-block; background:#f8f9ff; border:2px dashed #e94560; border-radius:12px; padding: 20px 48px;'>
                      <p style='margin:0 0 4px; color:#999; font-size:11px; letter-spacing:2px; text-transform:uppercase;'>MÃ XÁC NHẬN</p>
                      <p style='margin:0; color:#e94560; font-size:42px; font-weight:900; letter-spacing:12px;'>{otpCode}</p>
                    </div>
                  </td>
                </tr>
              </table>

              <!-- Timer warning -->
              <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>
                  <td style='background:#fff8f0; border-left:4px solid #f59e0b; border-radius:0 8px 8px 0; padding:14px 16px;'>
                    <p style='margin:0; color:#92400e; font-size:13px;'>
                      ⏱ Mã có hiệu lực trong <strong>1 phút</strong> kể từ khi nhận email này.
                    </p>
                  </td>
                </tr>
              </table>

              <p style='margin:24px 0 0; color:#888; font-size:13px; line-height:1.7;'>
                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email và 
                <strong>không chia sẻ mã OTP</strong> với bất kỳ ai.<br/>
                Đội ngũ BattleGame sẽ không bao giờ yêu cầu mã OTP của bạn.
              </p>
            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style='padding: 0 40px;'>
              <hr style='border:none; border-top:1px solid #eee; margin:0;'/>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='padding: 24px 40px; text-align:center;'>
              <p style='margin:0 0 4px; color:#aaa; font-size:12px;'>© 2026 BattleGame. All rights reserved.</p>
              <p style='margin:0; color:#ccc; font-size:11px;'>
                Email này được gửi tự động, vui lòng không trả lời.
              </p>
            </td>
          </tr>

        </table>
        <!-- End Card -->

      </td>
    </tr>
  </table>

</body>
</html>";

            try
            {
                using var client = new SmtpClient(_config.Host, _config.Port)
                {
                    EnableSsl = _config.EnableSsl,
                    Credentials = string.IsNullOrEmpty(_config.Password)
                        ? null
                        : new NetworkCredential(_config.Username, _config.Password)
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_config.Username, _config.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                client.Send(mail);
            }
            catch (Exception ex)
            {
                ServerLogger.Error($"SendOtpEmail failed: {ex.Message}");
                throw;
            }
        }
    }
}