using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageProduct
{
    public partial class AddProductModal : System.Web.UI.Page
    {
        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const int MaxUploadBytes = 10 * 1024 * 1024;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            if (!IsPostBack)
            {
                BindCategories();

                if (TryGetEditProductId(out var editProductId))
                {
                    LoadProductForEdit(editProductId);
                }
            }
        }

        protected void btnAddProduct_ServerClick(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            pnlMessage.Visible = false;

            var productName = (txtProductName.Value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(productName))
            {
                ShowError("Product name is required.");
                return;
            }

            if (!decimal.TryParse(
                    txtPrice.Value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var price) || price <= 0)
            {
                ShowError("Retail price must be greater than zero.");
                return;
            }

            if (!int.TryParse(ddlCategory.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var categoryId) || categoryId <= 0)
            {
                ShowError("Please select a valid category.");
                return;
            }

            var isEditMode = TryGetEditProductId(out var editingProductId);
            Product existingProduct = null;
            if (isEditMode)
            {
                try
                {
                    existingProduct = ProductService.GetProductById(editingProductId);
                }
                catch
                {
                    ShowError("Unable to load product details for update.");
                    return;
                }

                if (existingProduct == null)
                {
                    ShowError("The product you are trying to edit no longer exists.");
                    return;
                }
            }

            Category selectedCategory;
            try
            {
                selectedCategory = CategoryService.GetCategoryById(categoryId);
            }
            catch
            {
                ShowError("Category validation failed. Please try again.");
                return;
            }

            if (selectedCategory == null)
            {
                ShowError("Selected category does not exist.");
                return;
            }

            var hasNewImageUpload = fuImage.PostedFile != null && fuImage.PostedFile.ContentLength > 0;
            var imagePath = existingProduct != null ? (existingProduct.ImagePath ?? string.Empty) : string.Empty;

            if (hasNewImageUpload)
            {
                if (!TrySaveUploadedImage(out imagePath))
                {
                    return;
                }
            }
            else if (!isEditMode)
            {
                ShowError("Please upload a product image.");
                return;
            }

            var product = new Product
            {
                Id = isEditMode ? editingProductId : 0,
                ProductName = productName,
                Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero),
                Edition = NormalizeOrDefault(ddlEdition.Value, "Standard"),
                CategoryId = categoryId,
                SelectedSize = NormalizeOrDefault(ddlSize.Value, "Medium"),
                Description = (txtDescription.Value ?? string.Empty).Trim(),
                StockQuantity = existingProduct != null ? existingProduct.StockQuantity : 0,
                IsPopular = chkIsPopular.Checked,
                Status = NormalizeOrDefault(ddlStatus.Value, "In Stock"),
                ImagePath = imagePath
            };

            try
            {
                if (isEditMode)
                {
                    var updated = ProductService.UpdateProduct(product);
                    if (!updated)
                    {
                        ShowError("Product could not be updated. Please try again.");
                        return;
                    }
                }
                else
                {
                    var createdId = ProductService.CreateProduct(product);
                    if (createdId <= 0)
                    {
                        ShowError("Product could not be created. Please try again.");
                        return;
                    }
                }
            }
            catch
            {
                ShowError("An unexpected error occurred while saving the product.");
                return;
            }

            Response.Redirect(isEditMode
                ? "~/Admin/ManageProduct/ManageProducts.aspx?updated=1"
                : "~/Admin/ManageProduct/ManageProducts.aspx?created=1", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private bool TryGetEditProductId(out int productId)
        {
            productId = 0;
            return int.TryParse(
                Request.QueryString["id"],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out productId) && productId > 0;
        }

        private void LoadProductForEdit(int productId)
        {
            Product product;
            try
            {
                product = ProductService.GetProductById(productId);
            }
            catch
            {
                ShowError("Unable to load product details for editing.");
                return;
            }

            if (product == null)
            {
                ShowError("Product not found for editing.");
                return;
            }

            txtProductName.Value = product.ProductName ?? string.Empty;
            txtPrice.Value = product.Price > 0m
                ? product.Price.ToString("0.##", CultureInfo.InvariantCulture)
                : string.Empty;
            txtDescription.Value = product.Description ?? string.Empty;

            SetSelectedValueIfExists(ddlEdition, NormalizeOrDefault(product.Edition, "Standard"));
            SetSelectedValueIfExists(ddlSize, NormalizeOrDefault(product.SelectedSize, "Medium"));
            SetSelectedValueIfExists(ddlStatus, NormalizeStatusForSelection(product.Status));
            chkIsPopular.Checked = product.IsPopular;

            if (product.CategoryId.HasValue)
            {
                SetSelectedValueIfExists(ddlCategory, product.CategoryId.Value.ToString(CultureInfo.InvariantCulture));
            }

            litFormHeading.Text = "Edit Product";
            btnAddProduct.InnerText = "Update Product";
        }

        private static string NormalizeStatusForSelection(string status)
        {
            var normalized = (status ?? string.Empty).Trim();
            if (normalized.Equals("InStock", StringComparison.OrdinalIgnoreCase)) return "In Stock";
            if (normalized.Equals("LowStock", StringComparison.OrdinalIgnoreCase)) return "Low Stock";
            if (normalized.Equals("OutOfStock", StringComparison.OrdinalIgnoreCase)) return "Out Of Stock";

            return string.IsNullOrWhiteSpace(normalized) ? "In Stock" : normalized;
        }

        private static void SetSelectedValueIfExists(HtmlSelect select, string value)
        {
            if (select == null || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var item = select.Items.FindByValue(value);
            if (item != null)
            {
                select.Value = value;
            }
        }

        private void BindCategories()
        {
            ddlCategory.Items.Clear();
            var placeholderItem = new ListItem("Select Category", string.Empty);
            placeholderItem.Attributes["class"] = "bg-surface-container";
            ddlCategory.Items.Add(placeholderItem);

            List<Category> categories;
            try
            {
                categories = CategoryService.GetAllCategories() ?? new List<Category>();
            }
            catch
            {
                ShowError("Unable to load categories. Please refresh and try again.");
                return;
            }

            foreach (var category in categories
                .Where(c => c != null && c.Id > 0 && !string.IsNullOrWhiteSpace(c.Name))
                .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
            {
                var categoryItem = new ListItem(
                    category.Name.Trim(),
                    category.Id.ToString(CultureInfo.InvariantCulture));
                categoryItem.Attributes["class"] = "bg-surface-container";
                ddlCategory.Items.Add(categoryItem);
            }

            if (ddlCategory.Items.Count == 1)
            {
                ShowError("No categories found. Add categories before creating products.");
            }
        }

        private bool TrySaveUploadedImage(out string relativeImagePath)
        {
            relativeImagePath = string.Empty;

            var postedFile = fuImage.PostedFile;
            if (postedFile == null || postedFile.ContentLength <= 0)
            {
                ShowError("Please upload a product image.");
                return false;
            }

            if (postedFile.ContentLength > MaxUploadBytes)
            {
                ShowError("Image exceeds the 10 MB upload limit.");
                return false;
            }

            var extension = Path.GetExtension(postedFile.FileName ?? string.Empty) ?? string.Empty;
            if (!AllowedImageExtensions.Contains(extension))
            {
                ShowError("Only JPG, JPEG, PNG, and WEBP images are allowed.");
                return false;
            }

            try
            {
                var uploadsFolderPhysicalPath = Server.MapPath("~/Assets/Images/Products");
                if (string.IsNullOrWhiteSpace(uploadsFolderPhysicalPath))
                {
                    ShowError("Image storage path could not be resolved.");
                    return false;
                }

                Directory.CreateDirectory(uploadsFolderPhysicalPath);

                var normalizedExtension = extension.ToLowerInvariant();
                var fileName = string.Concat(
                    "product_",
                    DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                    "_",
                    Guid.NewGuid().ToString("N").Substring(0, 8),
                    normalizedExtension);

                var savedFilePath = Path.Combine(uploadsFolderPhysicalPath, fileName);
                postedFile.SaveAs(savedFilePath);

                relativeImagePath = "/Assets/Images/Products/" + fileName;
                return true;
            }
            catch
            {
                ShowError("Image upload failed. Please try a different file.");
                return false;
            }
        }

        private static string NormalizeOrDefault(string value, string fallback)
        {
            var normalized = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(normalized) ? fallback : normalized;
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageProduct/AddProductModal.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowError(string message)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = "mb-6 border border-error/40 bg-error-container/30 px-4 py-3 text-xs uppercase tracking-widest text-error";
            litMessage.Text = HttpUtility.HtmlEncode(message ?? "Something went wrong.");

        }
    }
}