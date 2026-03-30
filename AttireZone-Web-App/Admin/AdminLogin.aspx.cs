using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin
{
    public partial class AdminLogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack && IsAdminAlreadyAuthenticated())
            {
                RedirectToAdminArea();
            }
        }

        protected void btnAdminLogin_Click(object sender, EventArgs e)
        {
            lblAdminLoginMessage.Visible = false;
            Page.Validate("AdminLoginForm");

            if (!Page.IsValid)
            {
                ShowError("Admin email/username and secret key are required.");
                return;
            }

            string identifier = (txtAdminIdentifier.Text ?? string.Empty).Trim();
            string password = txtAdminSecretKey.Text ?? string.Empty;

            if (!IsValidIdentifier(identifier))
            {
                ShowError("Enter a valid admin email or username.");
                return;
            }

            try
            {
                User adminUser = AuthenticateAdmin(identifier, password);
                if (adminUser == null)
                {
                    ShowError("Access denied. Invalid credentials or insufficient privileges.");
                    return;
                }

                Session["CurrentUser"] = adminUser;
                Session["UserId"] = adminUser.UserId;
                Session["UserName"] = adminUser.FullName;
                Session["UserEmail"] = adminUser.Email;
                Session["AdminUserId"] = adminUser.UserId;
                Session["AdminRole"] = "Admin";

                RedirectToAdminArea();
            }
            catch
            {
                ShowError("Unable to sign in right now. Please try again.");
            }
        }

        // Admin authentication uses the standardized Users schema.
        private User AuthenticateAdmin(string identifier, string password)
        {
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            if (!ColumnExists("Password") || !ColumnExists("Role"))
            {
                return null;
            }

            const string sql = @"
SELECT TOP 1
    UserId,
    FullName,
    Email,
    [Password] AS PasswordValue,
    Role,
    CreatedDate,
    LastModifiedDate
FROM dbo.Users
WHERE (Email = @Identifier
       OR FullName = @Identifier
       OR LEFT(Email, CHARINDEX('@', Email + '@') - 1) = @Identifier);";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Identifier", SqlDbType.NVarChar, 150) { Value = identifier }
            };

            DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = dt.Rows[0];
            string storedPasswordValue = Convert.ToString(row["PasswordValue"]);
            if (!VerifyPassword(password, storedPasswordValue))
            {
                return null;
            }

            string role = Convert.ToString(row["Role"]);
            if (!"Admin".Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FullName = Convert.ToString(row["FullName"]),
                Email = Convert.ToString(row["Email"]),
                Role = role,
                CreatedDate = row["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["CreatedDate"]),
                LastModifiedDate = row["LastModifiedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["LastModifiedDate"])
            };
        }

        private static bool ColumnExists(string columnName)
        {
            const string sql = @"
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'Users'
  AND COLUMN_NAME = @ColumnName;";

            object result = DBHelper.ExecuteScalar(sql, new[]
            {
                new SqlParameter("@ColumnName", SqlDbType.NVarChar, 128) { Value = columnName }
            });

            return Convert.ToInt32(result ?? 0) > 0;
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder(hashBytes.Length * 2);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static bool VerifyPassword(string password, string storedPasswordValue)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedPasswordValue))
            {
                return false;
            }

            // Primary path: SHA-256 hash comparison (same style as Login.aspx.cs flow).
            string hashOfInput = HashPassword(password);
            if (hashOfInput.Equals(storedPasswordValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Compatibility path for databases that still store plain password strings.
            return password.Equals(storedPasswordValue, StringComparison.Ordinal);
        }

        private static bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return false;
            }

            if (identifier.Contains("@"))
            {
                return Regex.IsMatch(identifier, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            }

            return identifier.Length >= 3;
        }

        private bool IsAdminAlreadyAuthenticated()
        {
            string adminRole = Convert.ToString(Session["AdminRole"]);
            if (adminRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            User currentUser = Session["CurrentUser"] as User;
            return currentUser != null &&
                   currentUser.Role != null &&
                   currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToAdminArea()
        {
            string returnUrl = Request.QueryString["returnUrl"];
            string target = "~/Admin/Dashboard.aspx";

            if (IsSafeAdminReturnUrl(returnUrl))
            {
                target = returnUrl.StartsWith("~/", StringComparison.Ordinal)
                    ? returnUrl
                    : "~" + returnUrl;
            }

            Response.Redirect(target, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private static bool IsSafeAdminReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return false;
            }

            if (returnUrl.Contains("://") || returnUrl.StartsWith("//", StringComparison.Ordinal))
            {
                return false;
            }

            return returnUrl.StartsWith("/Admin/", StringComparison.OrdinalIgnoreCase) ||
                   returnUrl.StartsWith("~/Admin/", StringComparison.OrdinalIgnoreCase);
        }

        private void ShowError(string message)
        {
            lblAdminLoginMessage.Text = HttpUtility.HtmlEncode(message ?? "Access denied.");
            lblAdminLoginMessage.Visible = true;
        }

    }
}