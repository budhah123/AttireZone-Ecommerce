using AttireZone_Web_App.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AttireZone_Web_App.DataAccess
{
    public class UserRepository
    {
        // Hash password using SHA256
        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public User GetByEmailAndPassword(string email, string password)
        {
            string hash = HashPassword(password);
            const string sql = @"SELECT UserId, FullName, Email, Password, Role, CreatedDate, LastModifiedDate
                           FROM Users WHERE Email=@Email AND Password=@Hash";
            var dt = DBHelper.ExecuteDataTable(sql, new[] {
                new SqlParameter("@Email", email),
                new SqlParameter("@Hash", hash)
            });
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public bool EmailExists(string email)
        {
            var result = DBHelper.ExecuteScalar(
                "SELECT COUNT(1) FROM Users WHERE Email=@Email",
                new[] { new SqlParameter("@Email", email) });
            return Convert.ToInt32(result) > 0;
        }

        public int Register(User user, string password)
        {
            const string sql = @"INSERT INTO Users (FullName, Email, Password, Role, CreatedDate, LastModifiedDate)
                           VALUES (@Name,@Email,@Hash,'Customer',GETDATE(),GETDATE());
                           SELECT SCOPE_IDENTITY();";
            var result = DBHelper.ExecuteScalar(sql, new[] {
                new SqlParameter("@Name",    user.FullName),
                new SqlParameter("@Email",   user.Email),
                new SqlParameter("@Hash",    HashPassword(password))
            });
            return Convert.ToInt32(result);
        }

        public User GetById(int userId)
        {
            const string sql = "SELECT UserId, FullName, Email, Password, Role, CreatedDate, LastModifiedDate FROM Users WHERE UserId=@Id";
            var dt = DBHelper.ExecuteDataTable(sql, new[] { new SqlParameter("@Id", userId) });
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public void UpdateProfile(User user)
        {
            const string sql = "UPDATE Users SET FullName=@Name, LastModifiedDate=GETDATE() WHERE UserId=@Id";
            DBHelper.ExecuteNonQuery(sql, new[] {
                new SqlParameter("@Name",    user.FullName),
                new SqlParameter("@Id",      user.UserId)
            });
        }

        public void ChangePassword(int userId, string newPassword)
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE Users SET Password=@Hash, LastModifiedDate=GETDATE() WHERE UserId=@Id",
                new[] {
                    new SqlParameter("@Hash", HashPassword(newPassword)),
                    new SqlParameter("@Id",   userId)
                });
        }

        // Admin: get all users
        public DataTable GetAllUsers()
        {
            return DBHelper.ExecuteDataTable(
                "SELECT UserId, FullName, Email, Role, CreatedDate, LastModifiedDate FROM Users ORDER BY CreatedDate DESC");
        }

        public void SetUserStatus(int userId, bool isActive)
        {
            _ = isActive;
            DBHelper.ExecuteNonQuery(
                "UPDATE Users SET LastModifiedDate=GETDATE() WHERE UserId=@Id",
                new[] {
                    new SqlParameter("@Id",     userId)
                });
        }

        private User MapRow(DataRow row)
        {
            var role = row.Table.Columns.Contains("Role") && row["Role"] != DBNull.Value
                ? row["Role"].ToString()
                : "Customer";

            var password = row.Table.Columns.Contains("Password") && row["Password"] != DBNull.Value
                ? row["Password"].ToString()
                : string.Empty;

            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString(),
                Password = password,
                Role = role,
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                LastModifiedDate = row["LastModifiedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["LastModifiedDate"])
            };
        }
    }
}

