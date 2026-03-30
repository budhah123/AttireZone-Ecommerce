using AttireZone_Web_App.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AttireZone_Web_App.DataAccess;

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
            string sql = @"SELECT UserId, FullName, Email, Role, Phone, Address, IsActive, CreatedDate, LastModifiedDate
                           FROM Users WHERE Email=@Email AND PasswordHash=@Hash AND IsActive=1";
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
            string sql = @"INSERT INTO Users (FullName, Email, PasswordHash, Phone, Address, Role)
                           VALUES (@Name,@Email,@Hash,@Phone,@Address,'Customer');
                           SELECT SCOPE_IDENTITY();";
            var result = DBHelper.ExecuteScalar(sql, new[] {
                new SqlParameter("@Name",    user.FullName),
                new SqlParameter("@Email",   user.Email),
                new SqlParameter("@Hash",    HashPassword(password)),
                new SqlParameter("@Phone",   (object)user.Phone ?? DBNull.Value),
                new SqlParameter("@Address", (object)user.Address ?? DBNull.Value)
            });
            return Convert.ToInt32(result);
        }

        public User GetById(int userId)
        {
            string sql = "SELECT UserId, FullName, Email, Role, Phone, Address, IsActive, CreatedDate, LastModifiedDate FROM Users WHERE UserId=@Id";
            var dt = DBHelper.ExecuteDataTable(sql, new[] { new SqlParameter("@Id", userId) });
            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public void UpdateProfile(User user)
        {
            string sql = "UPDATE Users SET FullName=@Name, Phone=@Phone, Address=@Address WHERE UserId=@Id";
            DBHelper.ExecuteNonQuery(sql, new[] {
                new SqlParameter("@Name",    user.FullName),
                new SqlParameter("@Phone",   (object)user.Phone ?? DBNull.Value),
                new SqlParameter("@Address", (object)user.Address ?? DBNull.Value),
                new SqlParameter("@Id",      user.UserId)
            });
        }

        public void ChangePassword(int userId, string newPassword)
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE Users SET PasswordHash=@Hash WHERE UserId=@Id",
                new[] {
                    new SqlParameter("@Hash", HashPassword(newPassword)),
                    new SqlParameter("@Id",   userId)
                });
        }

        // Admin: get all users
        public DataTable GetAllUsers()
        {
            return DBHelper.ExecuteDataTable(
                "SELECT UserId, FullName, Email, Role, Phone, IsActive, CreatedDate FROM Users ORDER BY CreatedDate DESC");
        }

        public void SetUserStatus(int userId, bool isActive)
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE Users SET IsActive=@Active WHERE UserId=@Id",
                new[] {
                    new SqlParameter("@Active", isActive),
                    new SqlParameter("@Id",     userId)
                });
        }

        private User MapRow(DataRow row)
        {
            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString(),
                Role = row["Role"].ToString(),
                Phone = row["Phone"] == DBNull.Value ? "" : row["Phone"].ToString(),
                Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                LastModifiedDate = row["LastModifiedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["LastModifiedDate"])
            };
        }
    }
}
   
