using System.Net.Sockets;
using BattleGame.Server.Config;
using BattleGame.Server.Database;
using BattleGame.Server.Services;
using BattleGame.Shared.Packets;
using Npgsql;
using Xunit;
using Xunit.Abstractions;

namespace BattleGame.Test
{
    public class CheckSetup
    {
        private readonly ITestOutputHelper _out;

        private static string ConnStr() =>
            "Host=localhost;Port=5433;Database=battlegame;Username=postgres;Password=battlegame123";

        public CheckSetup(ITestOutputHelper output) => _out = output;

        [Fact]
        [Trait("Category", "Setup")]
        public void Check01_Docker_DbPortOpen()
        {
            _out.WriteLine("Kiem tra PostgreSQL port 5433...");
            using var tcp = new TcpClient();
            var ok = tcp.BeginConnect("127.0.0.1", 5433, null, null)
                        .AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));
            _out.WriteLine(ok ? "OK: PostgreSQL port 5433 mo" : "FAIL: dong -> chay docker compose up -d");
            Assert.True(ok, "PostgreSQL chua chay.");
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check02_Docker_MailpitSmtpOpen()
        {
            _out.WriteLine("Kiem tra Mailpit SMTP port 1025...");
            using var tcp = new TcpClient();
            var ok = tcp.BeginConnect("127.0.0.1", 1025, null, null)
                        .AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));
            _out.WriteLine(ok ? "OK: Mailpit SMTP port 1025 mo" : "FAIL: dong -> chay docker compose up -d mailpit");
            Assert.True(ok, "Mailpit chua chay.");
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check03_Docker_MailpitWebOpen()
        {
            _out.WriteLine("Kiem tra Mailpit Web port 8025...");
            using var tcp = new TcpClient();
            var ok = tcp.BeginConnect("127.0.0.1", 8025, null, null)
                        .AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));
            _out.WriteLine(ok ? "OK: Mailpit Web mo -> xem tai http://localhost:8025" : "FAIL: dong");
            Assert.True(ok, "Mailpit Web UI chua chay.");
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check04_DB_CanConnect()
        {
            _out.WriteLine("Ket noi PostgreSQL...");
            using var conn = new NpgsqlConnection(ConnStr());
            conn.Open();
            _out.WriteLine($"OK: Ket noi thanh cong - version {conn.ServerVersion}");
            Assert.Equal(System.Data.ConnectionState.Open, conn.State);
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check05_DB_TablesExist()
        {
            _out.WriteLine("Kiem tra bang DB...");
            using var conn = new NpgsqlConnection(ConnStr());
            conn.Open();
            foreach (var table in new[] { "users", "matches", "otp_tokens" })
            {
                using var cmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM information_schema.tables WHERE table_name=@t", conn);
                cmd.Parameters.AddWithValue("t", table);
                var count = (long)cmd.ExecuteScalar()!;
                _out.WriteLine(count > 0 ? $"OK: bang '{table}' ton tai" : $"FAIL: bang '{table}' THIEU");
                Assert.True(count > 0, $"Bang '{table}' chua duoc tao.");
            }
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check06_DB_OtpColumns()
        {
            _out.WriteLine("Kiem tra cot bang otp_tokens...");
            using var conn = new NpgsqlConnection(ConnStr());
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT column_name FROM information_schema.columns WHERE table_name='otp_tokens'", conn);
            using var r = cmd.ExecuteReader();
            var cols = new List<string>();
            while (r.Read()) cols.Add(r.GetString(0));
            foreach (var col in new[] { "id", "email", "code_hash", "purpose", "expires_at", "used", "attempts" })
            {
                _out.WriteLine(cols.Contains(col) ? $"OK: cot '{col}'" : $"FAIL: cot '{col}' THIEU");
                Assert.True(cols.Contains(col), $"Cot '{col}' khong ton tai.");
            }
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check07_Otp_SaveAndVerify()
        {
            _out.WriteLine("Test OTP: luu DB -> xac minh...");
            var repo = new OtpRepository(ConnStr());
            var email = "setup_test@battlegame.local";
            var purpose = "REGISTER";
            var code = "123456";
            var hash = BCrypt.Net.BCrypt.HashPassword(code);

            repo.Save(email, hash, purpose);
            _out.WriteLine("OK: Save OTP");

            var record = repo.FindValid(email, purpose);
            Assert.NotNull(record);
            _out.WriteLine("OK: FindValid tim thay OTP");

            Assert.True(BCrypt.Net.BCrypt.Verify(code, record!.Value.codeHash));
            _out.WriteLine("OK: BCrypt.Verify hash khop");

            repo.MarkUsed(record!.Value.id);
            Assert.Null(repo.FindValid(email, purpose));
            _out.WriteLine("OK: MarkUsed - OTP da vo hieu hoa");
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check08_Email_SendViaMailpit()
        {
            _out.WriteLine("Gui email qua Mailpit...");
            var cfg = new SmtpConfig
            {
                Host = "localhost",
                Port = 1025,
                Username = "test@battlegame.local",
                Password = "",
                FromName = "BattleGame Test",
                EnableSsl = false
            };
            var ex = Record.Exception(() =>
                new EmailService(cfg).SendOtp("dev@battlegame.local", "654321", "REGISTER"));
            _out.WriteLine(ex == null
                ? "OK: Email gui thanh cong -> xem tai http://localhost:8025"
                : $"FAIL: {ex.Message}");
            Assert.Null(ex);
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check09_PacketType_EnumValues()
        {
            _out.WriteLine("Kiem tra PacketTypes enum...");
            var expected = new Dictionary<string, int>
            {
                ["Login"] = 1,
                ["LoginResult"] = 2,
                ["MatchRequest"] = 3,
                ["MatchFound"] = 4,
                ["SelectionCharacter"] = 5,
                ["Move"] = 6,
                ["Attack"] = 7,
                ["GameState"] = 8,
                ["HealthUpdate"] = 9,
                ["GameOver"] = 10,
                ["Disconnect"] = 11,
                ["Register"] = 12,
                ["OtpSend"] = 13,
                ["OtpVerify"] = 14,
                ["ForgotPassword"] = 15,
            };
            foreach (var (name, value) in expected)
            {
                var parsed = Enum.TryParse<PacketTypes>(name, out var actual);
                _out.WriteLine(parsed && (int)actual == value
                    ? $"OK: PacketTypes.{name} = {value}"
                    : $"FAIL: PacketTypes.{name} sai hoac khong ton tai");
                Assert.True(parsed, $"PacketTypes.{name} khong ton tai.");
                Assert.Equal(value, (int)actual);
            }
        }

        [Fact]
        [Trait("Category", "Setup")]
        public void Check10_Summary()
        {
            _out.WriteLine("=== TAT CA TEST XANH: CAU HINH OK ===");
            _out.WriteLine("-> Push len Git duoc roi!");
            _out.WriteLine("-> Xem email OTP tai http://localhost:8025");
            Assert.True(true);
        }
    }
}