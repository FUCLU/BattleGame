using BattleGame.Server.Database;
using BattleGame.Server.Logging;

namespace BattleGame.Server.Services
{
    public class OtpService
    {
        private const int OTP_EXPIRE_MINUTES = 1;
        private const int MAX_ATTEMPTS = 3;
        private readonly OtpRepository _otpRepo;
        private readonly EmailService _emailService;

        public OtpService(OtpRepository otpRepo, EmailService emailService)
        {
            _otpRepo = otpRepo;
            _emailService = emailService;
        }

        public bool SendOtp(string email)
        {
            try
            {
                string otp = Random.Shared.Next(100000, 999999).ToString();
                string otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
                _otpRepo.Save(email, otpHash);
                _emailService.SendOtpEmail(email, otp);
                return true;
            }
            catch (Exception ex)
            {
                ServerLogger.Error($"SendOtp failed: {ex.Message}");
                return false;
            }
        }

        public enum VerifyResult
        {
            Success,          // Đúng mã
            InvalidCode,      // Sai mã, còn lần thử
            MaxAttemptsReached, // Đã sai đủ 3 lần — OTP bị block
            NotFound,         // Không có OTP active (hết hạn hoặc chưa gửi)
        }

        public VerifyResult VerifyOtp(string email, string otpCode)
        {
            var record = _otpRepo.FindValid(email);
            if (record == null)
            {
                return VerifyResult.NotFound;
            }
            var (id, otpHash, attempts) = record.Value;
            if (attempts >= MAX_ATTEMPTS)
            {
                return VerifyResult.MaxAttemptsReached;
            }
            bool isCorrect = BCrypt.Net.BCrypt.Verify(otpCode, otpHash);
            if (!isCorrect)
            {
                _otpRepo.IncrementAttempts(id);
                if (attempts + 1 >= MAX_ATTEMPTS)
                    return VerifyResult.MaxAttemptsReached;
                return VerifyResult.InvalidCode;
            }
            _otpRepo.MarkUsed(id);
            return VerifyResult.Success;
        }
    }
}