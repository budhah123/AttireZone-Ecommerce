using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageUser
{
    public partial class ManageUser : System.Web.UI.Page
    {
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        private sealed class UserRowVm
        {
            public int UserId { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public string Role { get; set; }

            public DateTime LastModifiedDate { get; set; }

            public string CreatedDateDisplay { get; set; }

            public string LastModifiedDateDisplay { get; set; }

            public string RoleBadgeCssClass { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            if (!IsPostBack)
            {
                ApplyFiltersFromQueryString();
                HandleActionMessage();
                LoadUsers();
            }
        }

        protected void txtUserSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected void ddlRoleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageUser/ManageUser.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadUsers()
        {
            var users = new List<UserRowVm>();
            var searchTerm = NormalizeSearch(txtUserSearch == null ? null : txtUserSearch.Text);
            var roleFilter = NormalizeFilter(ddlRoleFilter == null ? null : ddlRoleFilter.SelectedValue);
            var statusFilter = NormalizeFilter(ddlStatusFilter == null ? null : ddlStatusFilter.SelectedValue);

            try
            {
                var repository = new UserRepository();
                var table = repository.SearchUsers(searchTerm, roleFilter, statusFilter) ?? new DataTable();

                foreach (DataRow row in table.Rows)
                {
                    users.Add(MapRow(row));
                }
            }
            catch
            {
                ShowActionMessage("Unable to load users. Please refresh and try again.", true);
            }

            rptUsers.DataSource = users;
            rptUsers.DataBind();

            pnlEmptyState.Visible = users.Count == 0;

            var totalUsers = users.Count;
            var activeNow = users.Count(item => item.LastModifiedDate >= DateTime.Now.AddHours(-24));
            var administrators = users.Count(item => item.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            litTotalUsers.Text = FormatNumber(totalUsers);
            litActiveNow.Text = FormatNumber(activeNow);
            litAdmins.Text = FormatNumber(administrators);
            litPendingInvitations.Text = "0";

            litShown.Text = FormatNumber(totalUsers);
            litTotal.Text = FormatNumber(totalUsers);
        }

        private void ApplyFiltersFromQueryString()
        {
            if (txtUserSearch != null)
            {
                txtUserSearch.Text = NormalizeSearch(Request.QueryString["q"]) ?? string.Empty;
            }

            SetSelectedValue(ddlRoleFilter, NormalizeFilter(Request.QueryString["role"]));
            SetSelectedValue(ddlStatusFilter, NormalizeFilter(Request.QueryString["status"]));
        }

        private static void SetSelectedValue(DropDownList dropDown, string value)
        {
            if (dropDown == null)
            {
                return;
            }

            var normalizedValue = value ?? string.Empty;
            var item = dropDown.Items.FindByValue(normalizedValue);
            if (item == null)
            {
                return;
            }

            dropDown.ClearSelection();
            item.Selected = true;
        }

        private static string NormalizeSearch(string rawSearch)
        {
            if (string.IsNullOrWhiteSpace(rawSearch))
            {
                return null;
            }

            return rawSearch.Trim();
        }

        private static string NormalizeFilter(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return null;
            }

            return rawValue.Trim();
        }

        private static UserRowVm MapRow(DataRow row)
        {
            var userId = row != null && row.Table.Columns.Contains("UserId") && row["UserId"] != DBNull.Value
                ? Convert.ToInt32(row["UserId"], InvariantCulture)
                : 0;

            var fullName = row != null && row.Table.Columns.Contains("FullName") && row["FullName"] != DBNull.Value
                ? row["FullName"].ToString()
                : string.Empty;

            var email = row != null && row.Table.Columns.Contains("Email") && row["Email"] != DBNull.Value
                ? row["Email"].ToString()
                : string.Empty;

            var role = row != null && row.Table.Columns.Contains("Role") && row["Role"] != DBNull.Value
                ? row["Role"].ToString()
                : "Customer";

            var createdDate = row != null && row.Table.Columns.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value
                ? Convert.ToDateTime(row["CreatedDate"], InvariantCulture)
                : DateTime.Now;

            var lastModifiedDate = row != null && row.Table.Columns.Contains("LastModifiedDate") && row["LastModifiedDate"] != DBNull.Value
                ? Convert.ToDateTime(row["LastModifiedDate"], InvariantCulture)
                : createdDate;

            var normalizedRole = string.IsNullOrWhiteSpace(role) ? "Customer" : role.Trim();

            return new UserRowVm
            {
                UserId = userId,
                FullName = string.IsNullOrWhiteSpace(fullName) ? "Unknown User" : fullName.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? "-" : email.Trim(),
                Role = normalizedRole,
                LastModifiedDate = lastModifiedDate,
                CreatedDateDisplay = createdDate.ToString("dd MMM yyyy, hh:mm tt", InvariantCulture),
                LastModifiedDateDisplay = lastModifiedDate.ToString("dd MMM yyyy, hh:mm tt", InvariantCulture),
                RoleBadgeCssClass = GetRoleBadgeCssClass(normalizedRole)
            };
        }

        private static string GetRoleBadgeCssClass(string role)
        {
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "text-[10px] px-2.5 py-1 bg-primary-container text-on-primary-container font-bold uppercase tracking-widest";
            }

            return "text-[10px] px-2.5 py-1 bg-surface-container-highest text-on-surface-variant font-bold uppercase tracking-widest";
        }

        private void HandleActionMessage()
        {
            var created = Request.QueryString["created"];
            if (string.Equals(created, "1", StringComparison.Ordinal))
            {
                ShowActionMessage("User invited successfully.", false);
                return;
            }

            if (string.Equals(created, "0", StringComparison.Ordinal))
            {
                ShowActionMessage("User could not be invited.", true);
            }
        }

        private void ShowActionMessage(string message, bool isError)
        {
            pnlActionMessage.Visible = true;
            pnlActionMessage.CssClass = isError
                ? "mb-8 border border-error/40 bg-error-container/30 px-4 py-3 text-xs uppercase tracking-widest text-error"
                : "mb-8 border border-secondary/30 bg-secondary/10 px-4 py-3 text-xs uppercase tracking-widest text-secondary";
            litActionMessage.Text = HttpUtility.HtmlEncode(message ?? string.Empty);
        }

        private static string FormatNumber(int value)
        {
            return value.ToString("N0", InvariantCulture);
        }
    }
}