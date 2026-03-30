using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class RegistrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// UserService handles all business logic related to user registration and authentication
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="fullName">The full name of the user</param>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The password (will be hashed)</param>
        /// <returns>RegistrationResult with outcome and message</returns>
        public static RegistrationResult RegisterUser(string fullName, string email, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return new RegistrationResult
                    {
                        IsSuccess = false,
                        Message = "Please provide all required fields."
                    };
                }

                if (UserExists(email))
                {
                    return new RegistrationResult
                    {
                        IsSuccess = false,
                        Message = "An account with this email already exists."
                    };
                }

                string hashedPassword = HashPassword(password);

                const string sql = @"
            INSERT INTO dbo.Users (FullName, Email, Password, CreatedDate, LastModifiedDate, Role)
            VALUES (@FullName, @Email, @Password, @CreatedDate, @LastModifiedDate, @Role)
        ";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName },
            new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email },
            new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = hashedPassword },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
            new SqlParameter("@LastModifiedDate", SqlDbType.DateTime) { Value = DateTime.Now },
            new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = "Customer" }
                };

                int result = DBHelper.ExecuteNonQuery(sql, parameters);
                return new RegistrationResult
                {
                    IsSuccess = result > 0,
                    Message = result > 0 ? "Registration successful. Please sign in." : "Unable to create account. Please try again."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during registration: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Provide more specific error feedback
                if (ex is SqlException sqlEx)
                {
                    System.Diagnostics.Debug.WriteLine($"SQL Error Number: {sqlEx.Number}");
                    foreach (SqlError error in sqlEx.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"SQL Error: {error.Message}");
                    }
                }

                return new RegistrationResult
                {
                    IsSuccess = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Authenticates a user by email and password
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The password to verify</param>
        /// <returns>User object if authentication is successful; null otherwise</returns>
        public static User AuthenticateUser(string email, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return null;
                }

                // Get user from database
                const string sql = "SELECT UserId, FullName, Email, Password AS PasswordValue, Role, CreatedDate, LastModifiedDate FROM dbo.Users WHERE Email = @Email";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email }
                };

                DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);

                if (dt.Rows.Count == 0)
                {
                    return null; // User not found
                }

                DataRow row = dt.Rows[0];
                string storedHash = row["PasswordValue"].ToString();

                // Verify password
                if (!VerifyPassword(password, storedHash))
                {
                    return null; // Password incorrect
                }

                // Return user object
                return new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = row["Role"] == DBNull.Value ? "Customer" : row["Role"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastModifiedDate = Convert.ToDateTime(row["LastModifiedDate"])
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error during authentication: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Checks if a user with the given email already exists
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <returns>True if user exists; False otherwise</returns>
        public static bool UserExists(string email)
        {
            try
            {
                const string sql = "SELECT COUNT(*) FROM dbo.Users WHERE Email = @Email";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email }
                };

                object result = DBHelper.ExecuteScalar(sql, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error checking if user exists: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Hashes a password using SHA-256.
        /// Stored as a hex string to match the DB seed script.
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>Hashed password as a hex string</returns>
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder(hashedBytes.Length * 2);
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    sb.Append(hashedBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Verifies if a password matches its hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="hash">The stored hash</param>
        /// <returns>True if password matches hash; False otherwise</returns>
        private static bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Diagnostic method to test database connectivity and Users table setup
        /// </summary>
        public static RegistrationResult TestDatabaseConnection()
        {
            try
            {
                // Test if Users table exists
                const string tableCheckSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users' AND TABLE_SCHEMA = 'dbo'";
                object tableResult = DBHelper.ExecuteScalar(tableCheckSql);
                
                int tableExists = Convert.ToInt32(tableResult ?? 0);
                if (tableExists == 0)
                {
                    return new RegistrationResult
                    {
                        IsSuccess = false,
                        Message = "ERROR: Users table does not exist in database."
                    };
                }

                // Verify all required columns exist
                const string columnCheckSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND TABLE_SCHEMA = 'dbo' AND COLUMN_NAME IN ('UserId', 'FullName', 'Email', 'Password', 'CreatedDate', 'LastModifiedDate', 'Role')";
                object columnResult = DBHelper.ExecuteScalar(columnCheckSql);
                int columnCount = Convert.ToInt32(columnResult ?? 0);
                
                if (columnCount < 7)
                {
                    return new RegistrationResult
                    {
                        IsSuccess = false,
                        Message = $"ERROR: Users table missing required columns (found {columnCount}/7)."
                    };
                }

                return new RegistrationResult
                {
                    IsSuccess = true,
                    Message = "Database verified: Users table exists with all required columns."
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database diagnostic error: {ex.Message}");
                return new RegistrationResult
                {
                    IsSuccess = false,
                    Message = $"DATABASE CONNECTION ERROR: {ex.Message}"
                };
            }
        }
    }
}