using Npgsql;

namespace BattleGame.Server.Database
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public (string Username, string PasswordHash, string Email)? FindByUsername(string username)
        {
            const string sql = "SELECT username, password_hash, email FROM users WHERE username = @username LIMIT 1";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", username);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return (
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2)
            );
        }

        public bool ExistsByUsername(string username)
        {
            const string sql = "SELECT 1 FROM users WHERE username = @username LIMIT 1";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", username);
            return cmd.ExecuteScalar() != null;
        }

        public bool ExistsByEmail(string email)
        {
            const string sql = "SELECT 1 FROM users WHERE email = @email LIMIT 1";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("email", email);
            return cmd.ExecuteScalar() != null;
        }

        public void Save(string username, string passwordHash, string email)
        {
            const string sql = @"
                INSERT INTO users (username, password_hash, email, created_at)
                VALUES (@username, @password_hash, @email, NOW())";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password_hash", passwordHash);
            cmd.Parameters.AddWithValue("email", email);
            cmd.ExecuteNonQuery();
        }

        public void UpdatePassword(string email, string newPasswordHash)
        {
            const string sql = "UPDATE users SET password_hash = @hash WHERE email = @email";
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("hash", newPasswordHash);
            cmd.Parameters.AddWithValue("email", email);
            cmd.ExecuteNonQuery();
        }
    }
}