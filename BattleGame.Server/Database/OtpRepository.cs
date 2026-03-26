using Npgsql;

namespace BattleGame.Server.Database
{
    public class OtpRepository
    {
        private readonly string _conn;

        public OtpRepository(string connectionString) => _conn = connectionString;

        public virtual void Save(string email, string codeHash)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();
            var sql = @"
                DELETE FROM otp_tokens WHERE email=@e;
                INSERT INTO otp_tokens (email, code_hash, expires_at)
                VALUES (@e, @h, @exp);";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("e", email);
            cmd.Parameters.AddWithValue("h", codeHash);
            cmd.Parameters.AddWithValue("exp", DateTime.UtcNow.AddMinutes(1));
            cmd.ExecuteNonQuery();
        }

        public virtual (int id, string codeHash, int attempts)?
            FindValid(string email)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();
            var sql = @"
                SELECT id, code_hash, attempts
                FROM otp_tokens
                WHERE email=@e
                  AND used=false AND expires_at > NOW()
                ORDER BY created_at DESC LIMIT 1";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("e", email);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return (r.GetInt32(0), r.GetString(1), r.GetInt32(2));
        }

        public virtual void IncrementAttempts(int id)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE otp_tokens SET attempts=attempts+1 WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        public virtual void MarkUsed(int id)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE otp_tokens SET used=true WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }
    }
}