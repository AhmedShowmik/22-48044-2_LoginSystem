using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace _22_48044_2_LoginSystem
{
    // BONUS TASK: all database code lives here. Forms never touch
    // SqlConnection / SqlCommand directly - they just call these methods.
    public static class DatabaseHelper
    {
        // Reads the connection string from App.config - Task 2 requirement.
        private static string ConnStr =>
            ConfigurationManager.ConnectionStrings["LoginDB"].ConnectionString;

        // ---------- Task 2: connection test ----------
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using SqlConnection conn = new SqlConnection(ConnStr);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ---------- Password hashing (Task 3: SHA-256 minimum) ----------
        public static string HashPassword(string plainPassword)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        // ---------- Task 3: duplicate username check ----------
        public static bool UsernameExists(string username)
        {
            using SqlConnection conn = new SqlConnection(ConnStr);
            using SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Users WHERE Username = @username", conn);
            cmd.Parameters.AddWithValue("@username", username);

            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        // ---------- Task 3: registration insert (parameterized) ----------
        public static void RegisterUser(string username, string passwordHash, string email, string fullName)
        {
            using SqlConnection conn = new SqlConnection(ConnStr);
            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO dbo.Users (Username, PasswordHash, Email, FullName, CreatedAt)
                  VALUES (@username, @passwordHash, @email, @fullName, GETDATE())", conn);

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
            cmd.Parameters.AddWithValue("@email", (object)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fullName", (object)fullName ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------- Task 4: login lookup (parameterized, hash comparison) ----------
        // Returns the FullName on success, or null if the username/password is wrong.
        public static string ValidateLogin(string username, string passwordHash)
        {
            using SqlConnection conn = new SqlConnection(ConnStr);
            using SqlCommand cmd = new SqlCommand(
                @"SELECT FullName FROM dbo.Users
                  WHERE Username = @username AND PasswordHash = @passwordHash", conn);

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // DBNull check - a NULL FullName should not crash .ToString()
                return reader["FullName"] == DBNull.Value ? username : reader["FullName"].ToString();
            }
            return null;
        }

        // ---------- Task 7: DataGridView listing (SqlDataAdapter + DataTable) ----------
        // Never selects PasswordHash - the grid must not show it.
        public static DataTable GetAllUsers()
        {
            using SqlConnection conn = new SqlConnection(ConnStr);
            using SqlDataAdapter adapter = new SqlDataAdapter(
                "SELECT UserID, Username, Email, CreatedAt FROM dbo.Users ORDER BY UserID", conn);

            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        // ---------- Bonus: search/filter by username (parameterized LIKE) ----------
        public static DataTable SearchUsers(string term)
        {
            using SqlConnection conn = new SqlConnection(ConnStr);
            using SqlDataAdapter adapter = new SqlDataAdapter(
                @"SELECT UserID, Username, Email, CreatedAt FROM dbo.Users
                  WHERE Username LIKE @term ORDER BY UserID", conn);
            adapter.SelectCommand.Parameters.AddWithValue("@term", "%" + term + "%");

            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
    }
}
