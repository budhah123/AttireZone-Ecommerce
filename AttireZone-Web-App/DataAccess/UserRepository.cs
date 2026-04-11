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

        public bool EmailExistsForAnotherUser(int userId, string email)
        {
            var result = DBHelper.ExecuteScalar(
                "SELECT COUNT(1) FROM Users WHERE Email=@Email AND UserId<>@UserId",
                new[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@UserId", userId)
                });

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
            const string sql = "UPDATE Users SET FullName=@Name, Email=@Email, LastModifiedDate=GETDATE() WHERE UserId=@Id";
            DBHelper.ExecuteNonQuery(sql, new[] {
                new SqlParameter("@Name",    user.FullName),
                new SqlParameter("@Email",   user.Email),
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

        public DataTable SearchUsers(string searchTerm, string roleFilter, string statusFilter)
        {
            var normalizedSearch = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
            var normalizedRole = string.IsNullOrWhiteSpace(roleFilter) ? null : roleFilter.Trim();
            var normalizedStatus = string.IsNullOrWhiteSpace(statusFilter) ? null : statusFilter.Trim();

            const string sql = @"
SELECT
    UserId,
    FullName,
    Email,
    Role,
    CreatedDate,
    LastModifiedDate
FROM Users
WHERE
    (@SearchTerm IS NULL
        OR FullName LIKE '%' + @SearchTerm + '%'
        OR Email LIKE '%' + @SearchTerm + '%'
        OR Role LIKE '%' + @SearchTerm + '%'
        OR CAST(UserId AS NVARCHAR(20)) = @SearchTerm)
    AND (@RoleFilter IS NULL OR Role = @RoleFilter)
    AND (
        @StatusFilter IS NULL
        OR (@StatusFilter = 'Active' AND LastModifiedDate >= DATEADD(HOUR, -24, GETDATE()))
        OR (@StatusFilter = 'Inactive' AND (LastModifiedDate < DATEADD(HOUR, -24, GETDATE()) OR LastModifiedDate IS NULL))
    )
ORDER BY CreatedDate DESC;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 150)
                {
                    Value = (object)normalizedSearch ?? DBNull.Value
                },
                new SqlParameter("@RoleFilter", SqlDbType.NVarChar, 20)
                {
                    Value = (object)normalizedRole ?? DBNull.Value
                },
                new SqlParameter("@StatusFilter", SqlDbType.NVarChar, 20)
                {
                    Value = (object)normalizedStatus ?? DBNull.Value
                }
            };

            return DBHelper.ExecuteDataTable(sql, parameters);
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

        public bool UpdateUserByAdmin(User user, string newPassword = null)
        {
            if (user == null || user.UserId <= 0)
            {
                return false;
            }

            var shouldUpdatePassword = !string.IsNullOrWhiteSpace(newPassword);
            if (shouldUpdatePassword)
            {
                const string sqlWithPassword = @"
UPDATE Users
SET
    FullName = @FullName,
    Email = @Email,
    Role = @Role,
    Password = @Password,
    LastModifiedDate = GETDATE()
WHERE UserId = @UserId;";

                var parametersWithPassword = new[]
                {
                    new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = user.FullName ?? string.Empty },
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = user.Email ?? string.Empty },
                    new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role ?? "Customer" },
                    new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = HashPassword(newPassword) },
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId }
                };

                return DBHelper.ExecuteNonQuery(sqlWithPassword, parametersWithPassword) > 0;
            }

            const string sqlWithoutPassword = @"
UPDATE Users
SET
    FullName = @FullName,
    Email = @Email,
    Role = @Role,
    LastModifiedDate = GETDATE()
WHERE UserId = @UserId;";

            var parameters = new[]
            {
                new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = user.FullName ?? string.Empty },
                new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = user.Email ?? string.Empty },
                new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = user.Role ?? "Customer" },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId }
            };

            return DBHelper.ExecuteNonQuery(sqlWithoutPassword, parameters) > 0;
        }

        public bool DeleteUser(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            using (var connection = DBHelper.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        DeleteRowsByUserId(connection, transaction, "CustomerWishlists", userId);
                        DeleteRowsByUserId(connection, transaction, "Feedback", userId);
                        DeleteRowsByUserId(connection, transaction, "Cart", userId);

                        DeleteOrderRelatedRows(connection, transaction, userId);

                        var deletedUsers = ExecuteDeleteByUserId(
                            connection,
                            transaction,
                            "DELETE FROM [dbo].[Users] WHERE [UserId] = @UserId;",
                            userId);

                        transaction.Commit();
                        return deletedUsers > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void DeleteRowsByUserId(SqlConnection connection, SqlTransaction transaction, string tableName, int userId)
        {
            if (!TableExists(connection, transaction, tableName) || !ColumnExists(connection, transaction, tableName, "UserId"))
            {
                return;
            }

            var sql = string.Format("DELETE FROM [dbo].[{0}] WHERE [UserId] = @UserId;", tableName);
            ExecuteDeleteByUserId(connection, transaction, sql, userId);
        }

        private static void DeleteOrderRelatedRows(SqlConnection connection, SqlTransaction transaction, int userId)
        {
            if (!TableExists(connection, transaction, "Orders") || !ColumnExists(connection, transaction, "Orders", "UserId"))
            {
                return;
            }

            var orderPrimaryKeyColumn = GetOrderPrimaryKeyColumn(connection, transaction);
            if (string.IsNullOrWhiteSpace(orderPrimaryKeyColumn))
            {
                return;
            }

            if (TableExists(connection, transaction, "Payments") && ColumnExists(connection, transaction, "Payments", "OrderId"))
            {
                var paymentDeleteSql = @"
DELETE [p]
FROM [dbo].[Payments] [p]
INNER JOIN [dbo].[Orders] [o] ON [p].[OrderId] = [o]." + "[" + orderPrimaryKeyColumn + @"]
WHERE [o].[UserId] = @UserId;";

                ExecuteDeleteByUserId(connection, transaction, paymentDeleteSql, userId);
            }

            if (TableExists(connection, transaction, "OrderItems") && ColumnExists(connection, transaction, "OrderItems", "OrderId"))
            {
                var orderItemDeleteSql = @"
DELETE [oi]
FROM [dbo].[OrderItems] [oi]
INNER JOIN [dbo].[Orders] [o] ON [oi].[OrderId] = [o]." + "[" + orderPrimaryKeyColumn + @"]
WHERE [o].[UserId] = @UserId;";

                ExecuteDeleteByUserId(connection, transaction, orderItemDeleteSql, userId);
            }

            ExecuteDeleteByUserId(connection, transaction, "DELETE FROM [dbo].[Orders] WHERE [UserId] = @UserId;", userId);
        }

        private static string GetOrderPrimaryKeyColumn(SqlConnection connection, SqlTransaction transaction)
        {
            if (ColumnExists(connection, transaction, "Orders", "Id"))
            {
                return "Id";
            }

            if (ColumnExists(connection, transaction, "Orders", "OrderId"))
            {
                return "OrderId";
            }

            return string.Empty;
        }

        private static int ExecuteDeleteByUserId(SqlConnection connection, SqlTransaction transaction, string sql, int userId)
        {
            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                return command.ExecuteNonQuery();
            }
        }

        private static bool TableExists(SqlConnection connection, SqlTransaction transaction, string tableName)
        {
            const string sql = @"
SELECT COUNT(1)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = @TableName;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = tableName;
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result ?? 0) > 0;
            }
        }

        private static bool ColumnExists(SqlConnection connection, SqlTransaction transaction, string tableName, string columnName)
        {
            const string sql = @"
SELECT COUNT(1)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = @TableName
  AND COLUMN_NAME = @ColumnName;";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@TableName", SqlDbType.NVarChar, 128).Value = tableName;
                command.Parameters.Add("@ColumnName", SqlDbType.NVarChar, 128).Value = columnName;
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result ?? 0) > 0;
            }
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

