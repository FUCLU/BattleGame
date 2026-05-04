using Npgsql;
using System;
using System.Collections.Generic;
using BattleGame.Server.Game;

namespace BattleGame.Server.Database
{
    public class MatchRepository
    {
        private readonly string _connectionString;

        public MatchRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Save(Match match)
        {
            const string sql = @"
                INSERT INTO matches (winner_name, loser_name, duration, played_at)
                VALUES (@winner, @loser, @duration, @played_at)
            ";
            
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("winner", match.WinnerName ?? "");
            cmd.Parameters.AddWithValue("loser", match.LoserName ?? "");
            cmd.Parameters.AddWithValue("duration", match.Duration);
            cmd.Parameters.AddWithValue("played_at", match.PlayedAt);
            cmd.ExecuteNonQuery();
        }

        public List<(string Username, int Wins, int Losses)> GetLeaderboard(int limit = 100)
        {
            const string sql = @"
                WITH all_players AS (
                    SELECT DISTINCT winner_name as username FROM matches
                    UNION
                    SELECT DISTINCT loser_name FROM matches
                )
                SELECT 
                    ap.username,
                    COUNT(CASE WHEN m.winner_name = ap.username THEN 1 END) as wins,
                    COUNT(CASE WHEN m.loser_name = ap.username THEN 1 END) as losses
                FROM all_players ap
                LEFT JOIN matches m ON (m.winner_name = ap.username OR m.loser_name = ap.username)
                GROUP BY ap.username
                ORDER BY wins DESC
                LIMIT @limit";
            
            var entries = new List<(string, int, int)>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("limit", limit);
            using var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                entries.Add((reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2)));
            }
            
            return entries;
        }
    }
}
