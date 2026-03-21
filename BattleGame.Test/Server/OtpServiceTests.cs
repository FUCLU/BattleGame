using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Server.Config;
using Xunit;

namespace BattleGame.Test.Server
{
    public class OtpServiceTests
    {
        // ── Fake repository — không cần DB thật ──
        private class FakeOtpRepository : OtpRepository
        {
            private readonly Dictionary<string, (string hash, int attempts, DateTime expires, bool used)> _store = new();

            public FakeOtpRepository() : base("fake") { }

            public override void Save(string email, string codeHash, string purpose)
            {
                var key = $"{email}:{purpose}";
                _store[key] = (codeHash, 0, DateTime.UtcNow.AddMinutes(5), false);
            }

            public override (int id, string codeHash, int attempts)? FindValid(string email, string purpose)
            {
                var key = $"{email}:{purpose}";
                if (!_store.TryGetValue(key, out var v)) return null;
                if (v.used || v.expires < DateTime.UtcNow) return null;
                return (1, v.hash, v.attempts);
            }

            public override void IncrementAttempts(int id)
            {
                foreach (var k in _store.Keys.ToList())
                {
                    var v = _store[k];
                    _store[k] = (v.hash, v.attempts + 1, v.expires, v.used);
                }
            }

            public override void MarkUsed(int id)
            {
                foreach (var k in _store.Keys.ToList())
                {
                    var v = _store[k];
                    _store[k] = (v.hash, v.attempts, v.expires, true);
                }
            }

            public void ExpireAll()
            {
                foreach (var k in _store.Keys.ToList())
                {
                    var v = _store[k];
                    _store[k] = (v.hash, v.attempts, DateTime.UtcNow.AddMinutes(-1), v.used);
                }
            }
        }

        // ── Fake EmailService — không gửi email thật ──
        private class FakeEmailService : EmailService
        {
            public List<(string email, string code, string purpose)> Sent = new();
            public FakeEmailService() : base(new SmtpConfig()) { }
            public override void SendOtp(string toEmail, string code, string purpose)
                => Sent.Add((toEmail, code, purpose));
        }

        private (OtpService svc, FakeOtpRepository repo, FakeEmailService email) Build()
        {
            var repo = new FakeOtpRepository();
            var email = new FakeEmailService();
            var svc = new OtpService(repo, email);
            return (svc, repo, email);
        }

        // ── TEST 1: Gửi OTP → email nhận được mã ──
        [Fact]
        [Trait("Category", "Otp")]
        public void SendOtp_ShouldSendEmail()
        {
            var (svc, _, email) = Build();

            svc.SendOtp("test@example.com", "REGISTER");

            Assert.Single(email.Sent);
            Assert.Equal("test@example.com", email.Sent[0].email);
            Assert.Equal("REGISTER", email.Sent[0].purpose);
            Assert.Equal(6, email.Sent[0].code.Length);
        }

        // ── TEST 2: Mã đúng → verify thành công ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_CorrectCode_ShouldReturnOk()
        {
            var (svc, _, email) = Build();
            svc.SendOtp("test@example.com", "REGISTER");
            var code = email.Sent[0].code;

            var (ok, err) = svc.Verify("test@example.com", "REGISTER", code);

            Assert.True(ok);
            Assert.Empty(err);
        }

        // ── TEST 3: Mã sai → verify thất bại ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_WrongCode_ShouldReturnError()
        {
            var (svc, _, _) = Build();
            svc.SendOtp("test@example.com", "REGISTER");

            var (ok, err) = svc.Verify("test@example.com", "REGISTER", "000000");

            Assert.False(ok);
            Assert.Contains("lần", err);
        }

        // ── TEST 4: Sai 3 lần → bị block ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_ThreeWrongAttempts_ShouldBlock()
        {
            var (svc, _, _) = Build();
            svc.SendOtp("test@example.com", "REGISTER");

            svc.Verify("test@example.com", "REGISTER", "000000");
            svc.Verify("test@example.com", "REGISTER", "000000");
            svc.Verify("test@example.com", "REGISTER", "000000");
            var (ok, err) = svc.Verify("test@example.com", "REGISTER", "000000");

            Assert.False(ok);
            Assert.Contains("3 lần", err);
        }

        // ── TEST 5: OTP hết hạn → fail ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_ExpiredCode_ShouldFail()
        {
            var (svc, repo, _) = Build();
            svc.SendOtp("test@example.com", "REGISTER");
            repo.ExpireAll();

            var (ok, err) = svc.Verify("test@example.com", "REGISTER", "123456");

            Assert.False(ok);
            Assert.Contains("hết hạn", err);
        }

        // ── TEST 6: Mã luôn đúng 6 số ──
        [Fact]
        [Trait("Category", "Otp")]
        public void SendOtp_CodeShouldBeSixDigits()
        {
            var (svc, _, email) = Build();

            for (int i = 0; i < 20; i++)
            {
                email.Sent.Clear();
                svc.SendOtp($"user{i}@example.com", "REGISTER");
                var code = email.Sent[0].code;
                Assert.Equal(6, code.Length);
                Assert.True(int.TryParse(code, out var n));
                Assert.InRange(n, 100_000, 999_999);
            }
        }

        // ── TEST 7: Sai purpose → không verify được ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_DifferentPurpose_ShouldNotMatch()
        {
            var (svc, _, email) = Build();
            svc.SendOtp("test@example.com", "REGISTER");
            var code = email.Sent[0].code;

            var (ok, _) = svc.Verify("test@example.com", "RESET_PASSWORD", code);

            Assert.False(ok);
        }

        // ── TEST 8: Verify xong không dùng lại được ──
        [Fact]
        [Trait("Category", "Otp")]
        public void Verify_AfterSuccess_ShouldNotVerifyAgain()
        {
            var (svc, _, email) = Build();
            svc.SendOtp("test@example.com", "REGISTER");
            var code = email.Sent[0].code;

            svc.Verify("test@example.com", "REGISTER", code);
            var (ok, _) = svc.Verify("test@example.com", "REGISTER", code);

            Assert.False(ok);
        }
    }
}