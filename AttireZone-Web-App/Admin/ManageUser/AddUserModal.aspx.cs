using System;
using System.Data;
using System.Data.SqlClient;
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

            if (!IsPostBack && TryGetEditUserId(out var editUserId))
            {
                LoadUserForEdit(editUserId);
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
            var isEditMode = TryGetEditUserId(out var editUserId);
            var repository = new UserRepository();

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

            if (!isEditMode && (string.IsNullOrWhiteSpace(password) || password.Length < 6))
            {
                ShowError("Password must be at least 6 characters.");
                return;
            }

            if (isEditMode && !string.IsNullOrWhiteSpace(password) && password.Length < 6)
            {
                ShowError("New password must be at least 6 characters.");
                return;
            }

            if (isEditMode && repository.EmailExistsForAnotherUser(editUserId, email))
            {
                ShowError("A user with this email already exists.");
                return;
            }

            if (!isEditMode && UserService.UserExists(email))
            {
                ShowError("A user with this email already exists.");
                return;
            }

            try
            {
                if (isEditMode)
                {
                    var userToUpdate = repository.GetById(editUserId);
                    if (userToUpdate == null)
                    {
                        ShowError("The user you are trying to edit no longer exists.");
                        return;
                    }

                    userToUpdate.FullName = fullName;
                    userToUpdate.Email = email;
                    userToUpdate.Role = role;

                    var updated = repository.UpdateUserByAdmin(
                        userToUpdate,
                        string.IsNullOrWhiteSpace(password) ? null : password);

                    if (!updated)
                    {
                        ShowError("User could not be updated. Please try again.");
                        return;
                    }

                    Response.Redirect("~/Admin/ManageUser/ManageUser.aspx?updated=1", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

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

        private bool TryGetEditUserId(out int userId)
        {
            userId = 0;
            return int.TryParse(Request.QueryString["id"], out userId) && userId > 0;
        }

        private void LoadUserForEdit(int userId)
        {
            User user;
            try
            {
                var repository = new UserRepository();
                user = repository.GetById(userId);
            }
            catch
            {
                ShowError("Unable to load user details for editing.");
                return;
            }

            if (user == null)
            {
                ShowError("User not found for editing.");
                return;
            }

            txtFullName.Value = user.FullName ?? string.Empty;
            txtEmail.Value = user.Email ?? string.Empty;

            var roleItem = ddlRole.Items.FindByValue(NormalizeRole(user.Role));
            if (roleItem != null)
            {
                ddlRole.Value = roleItem.Value;
            }

            litFormHeading.Text = "Edit User";
            btnAddUser.InnerText = "Update User";
        }
    }
}