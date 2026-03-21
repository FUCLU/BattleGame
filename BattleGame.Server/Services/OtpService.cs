using BattleGame.Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleGame.Server.Services
{
    public class OtpService
    {
        private readonly OtpRepository _repo;
        private readonly EmailService _email;
        private const int MAX_ATTEMPTS = 3;

        public OtpService(OtpRepository repo, EmailService email)
        {
            _repo = repo;
            _email = email;
        }

        // Sinh mã, lưu DB, gửi email — gọi khi đăng ký hoặc quên mật khẩu
        public void SendOtp(string email, string purpose)
        {
            var code = GenerateCode();
            var codeHash = BCrypt.Net.BCrypt.HashPassword(code);
            _repo.Save(email, codeHash, purpose);
            _email.SendOtp(email, code, purpose);
        }

        // Xác minh mã — trả về true/false kèm lý do
        public (bool ok, string error) Verify(string email, string purpose, string inputCode)
        {
            var record = _repo.FindValid(email, purpose);
            if (record == null)
                return (false, "OTP không tồn tại hoặc đã hết hạn");

            var (id, codeHash, attempts) = record.Value;

            if (attempts >= MAX_ATTEMPTS)
                return (false, "Nhập sai quá 3 lần, vui lòng yêu cầu gửi lại mã");

            if (!BCrypt.Net.BCrypt.Verify(inputCode, codeHash))
            {
                _repo.IncrementAttempts(id);
                var left = MAX_ATTEMPTS - attempts - 1;
                return (false, $"Mã không đúng, còn {left} lần thử");
            }

            _repo.MarkUsed(id);
            return (true, "");
        }

        private static string GenerateCode() =>
            Random.Shared.Next(100_000, 999_999).ToString();
    }
}
