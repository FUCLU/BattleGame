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
            string subject = "BattleGame — Mã xác nhận đặt lại mật khẩu";
             


            string body = $@"
<html>
<body style='font-family: Arial, sans-serif; max-width: 480px; margin: auto;'>
  <h2 style='color: #1a1a2e;'>BattleGame</h2>
  <p>Mã xác nhận của bạn là:</p>
  <div style='font-size: 36px; font-weight: bold; letter-spacing: 8px;
              background: #f4f4f4; padding: 16px; text-align: center;
              border-radius: 8px; color: #e94560;'>
    {otpCode}
  </div>
  <p style='color: #666; margin-top: 16px;'>
    Mã có hiệu lực trong <strong>1 phút</strong>.<br/>
    Nếu bạn không yêu cầu điều này, hãy bỏ qua email này.
  </p>
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
                throw;
            }
        }
    }
}