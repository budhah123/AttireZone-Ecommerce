using System;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageCategories
{
    public partial class AddCategoryModal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            if (!IsPostBack && TryGetEditCategoryId(out var categoryId))
            {
                LoadCategoryForEdit(categoryId);
            }

        }

        protected void btnSaveCategory_ServerClick(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            pnlMessage.Visible = false;

            var categoryName = (txtCategoryName.Value ?? string.Empty).Trim();
            var description = (txtCategoryDescription.Value ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ShowError("Category name is required.");
                return;
            }

            if (categoryName.Length > 150)
            {
                ShowError("Category name cannot exceed 150 characters.");
                return;
            }

            if (description.Length > 2000)
            {
                ShowError("Category description cannot exceed 2000 characters.");
                return;
            }

            var isEditMode = TryGetEditCategoryId(out var editCategoryId);

            try
            {
                if (CategoryService.CategoryNameExists(categoryName, isEditMode ? (int?)editCategoryId : null))
                {
                    ShowError("A category with this name already exists.");
                    return;
                }
            }
            catch
            {
                ShowError("Unable to validate category name. Please try again.");
                return;
            }

            try
            {
                if (isEditMode)
                {
                    var existingCategory = CategoryService.GetCategoryById(editCategoryId);
                    if (existingCategory == null)
                    {
                        ShowError("The category you are trying to edit was not found.");
                        return;
                    }

                    existingCategory.Name = categoryName;
                    existingCategory.Description = string.IsNullOrWhiteSpace(description) ? null : description;

                    var updated = CategoryService.UpdateCategory(existingCategory);
                    if (!updated)
                    {
                        ShowError("Category could not be updated. Please try again.");
                        return;
                    }

                    Response.Redirect("~/Admin/ManageCategories/ManageCategory.aspx?updated=1", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                var category = new Category
                {
                    Name = categoryName,
                    Description = string.IsNullOrWhiteSpace(description) ? null : description,
                    CreatedDate = DateTime.Now
                };

                var newCategoryId = CategoryService.CreateCategory(category);
                if (newCategoryId <= 0)
                {
                    ShowError("Category could not be created. Please try again.");
                    return;
                }

                Response.Redirect("~/Admin/ManageCategories/ManageCategory.aspx?created=1", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                ShowError("An unexpected error occurred while saving the category.");
            }
        }

        private bool TryGetEditCategoryId(out int categoryId)
        {
            categoryId = 0;
            return int.TryParse(Request.QueryString["id"], out categoryId) && categoryId > 0;
        }

        private void LoadCategoryForEdit(int categoryId)
        {
            Category category;
            try
            {
                category = CategoryService.GetCategoryById(categoryId);
            }
            catch
            {
                ShowError("Unable to load category details for editing.");
                return;
            }

            if (category == null)
            {
                ShowError("Category not found for editing.");
                return;
            }

            txtCategoryName.Value = category.Name ?? string.Empty;
            txtCategoryDescription.Value = category.Description ?? string.Empty;
            litFormHeading.Text = "Edit Category";
            btnSaveCategory.InnerText = "Update Category";
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageCategories/addcategorymodal.aspx");
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
