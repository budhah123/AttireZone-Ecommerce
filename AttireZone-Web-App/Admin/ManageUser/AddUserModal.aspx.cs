using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageUser
{
    public partial class AddUserModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }
        }

        protected void btnAddUser_ServerClick(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            pnlMessage.Visible = false;

            var fullName = (txtFullName.Value ?? string.Empty).Trim();
            var email = (txtEmail.Value ?? string.Empty).Trim();
            var password = txtPassword.Value ?? string.Empty;
            var role = NormalizeRole(ddlRole.Value);

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowError("Full name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                ShowError("Please provide a valid email address.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ShowError("Temporary password must be at least 6 characters.");
                return;
            }

            if (UserService.UserExists(email))
            {
                ShowError("A user with this email already exists.");
                return;
            }

            try
            {
                const string sql = @"
INSERT INTO dbo.Users (FullName, Email, Password, CreatedDate, LastModifiedDate, Role)
VALUES (@FullName, @Email, @Password, @CreatedDate, @LastModifiedDate, @Role)";

                var parameters = new[]
                {
                    new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName },
                    new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = email },
                    new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = UserRepository.HashPassword(password) },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@LastModifiedDate", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@Role", SqlDbType.NVarChar, 20) { Value = role }
                };

                var rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);
                if (rowsAffected <= 0)
                {
                    ShowError("User could not be created. Please try again.");
                    return;
                }
            }
            catch
            {
                ShowError("User could not be created. Please try again.");
                return;
            }

            Response.Redirect("~/Admin/ManageUser/ManageUser.aspx?created=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return string.Equals(mailAddress.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeRole(string role)
        {
            return string.Equals((role ?? string.Empty).Trim(), "Admin", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "Customer";
        }

        private bool HasAdminAccess()
        {
            var adminRole = Convert.ToString(Session["AdminRole"]);
            if (!string.IsNullOrWhiteSpace(adminRole) &&
                adminRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var currentUser = Session["CurrentUser"] as User;
            return currentUser != null &&
                   !string.IsNullOrWhiteSpace(currentUser.Role) &&
                   currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToAdminLogin()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageUser/AddUserModal.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "mb-6 border border-error/40 bg-error-container/30 px-4 py-3 text-xs uppercase tracking-widest text-error";
            litMessage.Text = HttpUtility.HtmlEncode(message ?? string.Empty);

        }
    }
}