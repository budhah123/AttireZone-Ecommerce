using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageCategories
{
    public partial class ManageCategory : System.Web.UI.Page
    {
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
                LoadCategories();
            }

        }

        protected void txtCategorySearch_TextChanged(object sender, EventArgs e)
        {
            LoadCategories();
        }

        protected void btnDeleteCategoryConfirmed_Click(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            if (!int.TryParse(hfDeleteCategoryId.Value, out var categoryId) || categoryId <= 0)
            {
                ShowActionMessage("Invalid category selection.", true);
                LoadCategories();
                return;
            }

            int linkedProducts;
            try
            {
                linkedProducts = CategoryService.GetProductCountByCategoryId(categoryId);
            }
            catch
            {
                linkedProducts = -1;
            }

            if (linkedProducts > 0)
            {
                Response.Redirect("~/Admin/ManageCategories/ManageCategory.aspx?inuse=1", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool deleted;
            try
            {
                deleted = CategoryService.DeleteCategory(categoryId);
            }
            catch
            {
                deleted = false;
            }

            Response.Redirect(deleted
                ? "~/Admin/ManageCategories/ManageCategory.aspx?deleted=1"
                : "~/Admin/ManageCategories/ManageCategory.aspx?deleted=0", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            var commandName = e.CommandName ?? string.Empty;
            if (!commandName.Equals("EditCategory", StringComparison.OrdinalIgnoreCase) &&
                !commandName.Equals("DeleteCategory", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument), out var categoryId) || categoryId <= 0)
            {
                ShowActionMessage("Invalid category selection.", true);
                return;
            }

            if (commandName.Equals("EditCategory", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Admin/ManageCategories/addcategorymodal.aspx?id=" + categoryId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int linkedProducts;
            try
            {
                linkedProducts = CategoryService.GetProductCountByCategoryId(categoryId);
            }
            catch
            {
                linkedProducts = -1;
            }

            if (linkedProducts > 0)
            {
                Response.Redirect("~/Admin/ManageCategories/ManageCategory.aspx?inuse=1", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool deleted;
            try
            {
                deleted = CategoryService.DeleteCategory(categoryId);
            }
            catch
            {
                deleted = false;
            }

            Response.Redirect(deleted
                ? "~/Admin/ManageCategories/ManageCategory.aspx?deleted=1"
                : "~/Admin/ManageCategories/ManageCategory.aspx?deleted=0", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private sealed class CategoryRowVm
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string DescriptionPreview { get; set; }

            public string CreatedDateDisplay { get; set; }

            public int ProductCount { get; set; }

            public string ProductCountBadgeCssClass { get; set; }
        }

        private void LoadCategories()
        {
            var searchTerm = NormalizeSearch(txtCategorySearch == null ? null : txtCategorySearch.Text);
            var allCategories = new List<Category>();
            var rows = new List<CategoryRowVm>();

            try
            {
                var categories = CategoryService.GetAllCategories() ?? new List<Category>();
                allCategories = categories
                    .Where(item => item != null && item.Id > 0 && !string.IsNullOrWhiteSpace(item.Name))
                    .ToList();

                var productCounts = CategoryService.GetProductCountsByCategoryIds(allCategories.Select(item => item.Id));

                var filteredCategories = allCategories;
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    filteredCategories = filteredCategories
                        .Where(item =>
                            item.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (item.Description ?? string.Empty).IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                rows = filteredCategories
                    .Select(item => MapCategory(item, productCounts))
                    .ToList();

                var totalCategories = allCategories.Count;
                var categoriesInUse = allCategories.Count(item => ReadProductCount(productCounts, item.Id) > 0);
                var createdThisMonth = allCategories.Count(item => item.CreatedDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));

                litTotalCategories.Text = FormatNumber(totalCategories);
                litCategoriesInUse.Text = FormatNumber(categoriesInUse);
                litUnusedCategories.Text = FormatNumber(Math.Max(0, totalCategories - categoriesInUse));
                litCreatedThisMonth.Text = FormatNumber(createdThisMonth);
            }
            catch
            {
                ShowActionMessage("Unable to load categories. Please refresh and try again.", true);
                litTotalCategories.Text = "0";
                litCategoriesInUse.Text = "0";
                litUnusedCategories.Text = "0";
                litCreatedThisMonth.Text = "0";
            }

            rptCategories.DataSource = rows;
            rptCategories.DataBind();

            pnlEmptyState.Visible = rows.Count == 0;

            litShownFrom.Text = rows.Count > 0 ? "1" : "0";
            litShownTo.Text = FormatNumber(rows.Count);
            litShownTotal.Text = FormatNumber(rows.Count);
        }

        private static CategoryRowVm MapCategory(Category category, IDictionary<int, int> productCounts)
        {
            var productCount = ReadProductCount(productCounts, category.Id);
            var normalizedDescription = (category.Description ?? string.Empty).Trim();

            if (normalizedDescription.Length > 120)
            {
                normalizedDescription = normalizedDescription.Substring(0, 117) + "...";
            }

            return new CategoryRowVm
            {
                Id = category.Id,
                Name = category.Name.Trim(),
                DescriptionPreview = string.IsNullOrWhiteSpace(normalizedDescription)
                    ? "No description provided."
                    : normalizedDescription,
                CreatedDateDisplay = category.CreatedDate == default(DateTime)
                    ? "-"
                    : category.CreatedDate.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                ProductCount = productCount,
                ProductCountBadgeCssClass = productCount > 0
                    ? "inline-block px-3 py-1 bg-secondary/20 text-secondary text-[10px] font-bold uppercase tracking-widest border border-secondary/40"
                    : "inline-block px-3 py-1 bg-surface-container-highest text-on-surface-variant text-[10px] font-bold uppercase tracking-widest"
            };
        }

        private static int ReadProductCount(IDictionary<int, int> productCounts, int categoryId)
        {
            if (productCounts == null || categoryId <= 0)
            {
                return 0;
            }

            return productCounts.TryGetValue(categoryId, out var count)
                ? Math.Max(0, count)
                : 0;
        }

        private static string FormatNumber(int value)
        {
            return value.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string NormalizeSearch(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return null;
            }

            return rawValue.Trim();
        }

        private void ApplyFiltersFromQueryString()
        {
            if (txtCategorySearch != null)
            {
                txtCategorySearch.Text = NormalizeSearch(Request.QueryString["q"]) ?? string.Empty;
            }
        }

        private void HandleActionMessage()
        {
            if (string.Equals(Request.QueryString["updated"], "1", StringComparison.Ordinal))
            {
                ShowSnackbar("Category edited successfully.");
                return;
            }

            if (string.Equals(Request.QueryString["deleted"], "1", StringComparison.Ordinal))
            {
                ShowSnackbar("Category deleted successfully.");
                return;
            }

            if (string.Equals(Request.QueryString["created"], "1", StringComparison.Ordinal))
            {
                ShowSnackbar("Category added successfully.");
                return;
            }

            if (string.Equals(Request.QueryString["inuse"], "1", StringComparison.Ordinal))
            {
                ShowActionMessage("Category cannot be deleted while products are assigned to it.", true);
                return;
            }

            if (string.Equals(Request.QueryString["deleted"], "0", StringComparison.Ordinal))
            {
                ShowActionMessage("Category could not be deleted.", true);
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

        private void ShowSnackbar(string message)
        {
            pnlSnackbar.Visible = true;
            litSnackbarMessage.Text = HttpUtility.HtmlEncode(message ?? string.Empty);
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageCategories/ManageCategory.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();

        }
    }
}
