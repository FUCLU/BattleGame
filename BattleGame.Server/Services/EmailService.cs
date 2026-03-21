using System.Net;
using System.Net.Mail;
using BattleGame.Server.Config;

namespace BattleGame.Server.Services
{
    public class EmailService
    {
        private readonly SmtpConfig _cfg;

        public EmailService(SmtpConfig cfg) => _cfg = cfg;

        public virtual void SendOtp(string toEmail, string code, string purpose)
        {
            var subject = purpose == "REGISTER"
                ? "[BattleGame] Mã xác thực đăng ký tài khoản"
                : "[BattleGame] Mã đặt lại mật khẩu";

            var body = $@"Xin chào,

                        Mã OTP của bạn là: {code}

                        Mã có hiệu lực trong 5 phút.
                        Không chia sẻ mã này cho bất kỳ ai.

                        — BattleGame Team";

            using var client = new SmtpClient(_cfg.Host, _cfg.Port)
            {
                Credentials = new NetworkCredential(_cfg.Username, _cfg.Password),
                EnableSsl = _cfg.EnableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_cfg.Username, _cfg.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);
            client.Send(mail);
        }
    }
}