using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ManageProduct
{
    public partial class ManageProducts : System.Web.UI.Page
    {
        private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");
        private const string DefaultProductImage = "https://lh3.googleusercontent.com/aida-public/AB6AXuBf8LWw_nwQ42K9A7U8s_Dd28P0bGdD4ittey0PkRJg-SnABfZIphC63J2bBqv9zgjHzWeC6esOs78K4kaQILpE2CdoHGvwOWMBc19shqJrIAS5Qp09AbpYB7z91_xu_equYGzj8dcAn7k2UFJPipb5ks6AD6CAPqpzYI9u3wBBrmQiHoTCGViGAmnsZLZh5LCUjfZXWU7tfktJ7C1qI7vLmRHcSa3a0qOv1meyzuK77MANLH5KdufszWZX4QyVQ3flciGzkPMqyvU";

        private sealed class ProductRowVm
        {
            public int Id { get; set; }

            public string ProductName { get; set; }

            public string Sku { get; set; }

            public string Category { get; set; }

            public bool IsPopular { get; set; }

            public string PopularLabel { get; set; }

            public string PopularBadgeCssClass { get; set; }

            public string PriceFormatted { get; set; }

            public int StockQuantity { get; set; }

            public string StatusLabel { get; set; }

            public string StatusBadgeCssClass { get; set; }

            public bool IsLowStockAlert { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }
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
                BindCategoryFilter();
                HandleActionMessage();
                LoadProducts();
            }
        }

        protected void txtProductSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            var commandName = e.CommandName ?? string.Empty;
            if (!commandName.Equals("EditProduct", StringComparison.OrdinalIgnoreCase) &&
                !commandName.Equals("DeleteProduct", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), out var productId) || productId <= 0)
            {
                ShowActionMessage("Invalid product selection.", true);
                return;
            }

            if (commandName.Equals("EditProduct", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Admin/ManageProduct/AddProductModal.aspx?id=" + productId, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            bool deleted;
            try
            {
                deleted = ProductService.DeleteProduct(productId);
            }
            catch
            {
                deleted = false;
            }

            var redirectUrl = deleted
                ? "~/Admin/ManageProduct/ManageProducts.aspx?deleted=1"
                : "~/Admin/ManageProduct/ManageProducts.aspx?deleted=0";

            Response.Redirect(redirectUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private bool HasAdminAccess()
        {
            var adminRole = Convert.ToString(Session["AdminRole"]);
            if (!string.IsNullOrEmpty(adminRole) &&
                adminRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var currentUser = Session["CurrentUser"] as User;
            return currentUser != null &&
                   !string.IsNullOrEmpty(currentUser.Role) &&
                   currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToAdminLogin()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ManageProduct/ManageProducts.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void LoadProducts()
        {
            List<ProductRowVm> rows;
            var selectedCategoryId = ParseSelectedCategoryId();
            var searchTerm = NormalizeSearch(txtProductSearch == null ? null : txtProductSearch.Text);

            try
            {
                var products = ProductService.SearchProducts(searchTerm, selectedCategoryId, "featured") ?? new List<Product>();
                rows = products.Select(MapProduct).ToList();
            }
            catch
            {
                rows = FilterFallbackRows(GetFallbackRows(), searchTerm, selectedCategoryId);
            }

            rptProducts.DataSource = rows;
            rptProducts.DataBind();

            var totalSku = rows.Count;
            var lowStockAlerts = rows.Count(item => item.IsLowStockAlert);
            var popularProducts = rows.Count(item => item.IsPopular);
            var liveCollections = rows
                .Select(item => item.Category)
                .Where(category => !string.IsNullOrWhiteSpace(category))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            litTotalSku.Text = FormatNumber(totalSku);
            litLowStockAlerts.Text = FormatNumber(lowStockAlerts);
            litInSeason.Text = FormatNumber(popularProducts);
            litLiveCollections.Text = FormatNumber(liveCollections);

            litShownFrom.Text = totalSku > 0 ? "1" : "0";
            litShownTo.Text = FormatNumber(totalSku);
            litShownTotal.Text = FormatNumber(totalSku);
        }

        private void ApplyFiltersFromQueryString()
        {
            if (txtProductSearch != null)
            {
                txtProductSearch.Text = NormalizeSearch(Request.QueryString["q"]) ?? string.Empty;
            }
        }

        private void BindCategoryFilter()
        {
            if (ddlCategoryFilter == null)
            {
                return;
            }

            ddlCategoryFilter.Items.Clear();
            ddlCategoryFilter.Items.Add(new ListItem("All Categories", string.Empty));

            try
            {
                var categories = CategoryService.GetAllCategories() ?? new List<Category>();
                foreach (var category in categories
                    .Where(item => item != null && item.Id > 0 && !string.IsNullOrWhiteSpace(item.Name))
                    .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
                {
                    ddlCategoryFilter.Items.Add(new ListItem(category.Name.Trim(), category.Id.ToString(CultureInfo.InvariantCulture)));
                }
            }
            catch
            {
            }

            var requestedCategory = Request.QueryString["category"];
            if (!string.IsNullOrWhiteSpace(requestedCategory))
            {
                var requestedItem = ddlCategoryFilter.Items.FindByValue(requestedCategory.Trim());
                if (requestedItem != null)
                {
                    ddlCategoryFilter.ClearSelection();
                    requestedItem.Selected = true;
                }
            }
        }

        private int? ParseSelectedCategoryId()
        {
            var selectedValue = ddlCategoryFilter == null ? string.Empty : ddlCategoryFilter.SelectedValue;
            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                return null;
            }

            if (!int.TryParse(selectedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var categoryId) || categoryId <= 0)
            {
                return null;
            }

            return categoryId;
        }

        private static string NormalizeSearch(string rawSearch)
        {
            if (string.IsNullOrWhiteSpace(rawSearch))
            {
                return null;
            }

            return rawSearch.Trim();
        }

        private static List<ProductRowVm> FilterFallbackRows(List<ProductRowVm> rows, string searchTerm, int? selectedCategoryId)
        {
            _ = selectedCategoryId;
            var filteredRows = rows ?? new List<ProductRowVm>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredRows = filteredRows
                    .Where(item =>
                        item != null &&
                        (
                            item.ProductName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            item.Category.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            item.Sku.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                        ))
                    .ToList();
            }

            return filteredRows;
        }

        private ProductRowVm MapProduct(Product product)
        {
            var safeProduct = product ?? new Product();
            var statusLabel = NormalizeStatus(safeProduct.Status, safeProduct.StockQuantity);

            return new ProductRowVm
            {
                Id = safeProduct.Id,
                ProductName = string.IsNullOrWhiteSpace(safeProduct.ProductName)
                    ? "Untitled Product"
                    : safeProduct.ProductName.Trim(),
                Sku = BuildSku(safeProduct.Id),
                Category = !string.IsNullOrWhiteSpace(safeProduct.CategoryName)
                    ? safeProduct.CategoryName.Trim()
                    : (string.IsNullOrWhiteSpace(safeProduct.Edition)
                        ? "General"
                        : safeProduct.Edition.Trim()),
                IsPopular = safeProduct.IsPopular,
                PopularLabel = safeProduct.IsPopular ? "Popular" : "Standard",
                PopularBadgeCssClass = GetPopularBadgeCssClass(safeProduct.IsPopular),
                PriceFormatted = safeProduct.Price.ToString("C2", UsCulture),
                StockQuantity = Math.Max(0, safeProduct.StockQuantity),
                StatusLabel = statusLabel,
                StatusBadgeCssClass = GetStatusBadgeCssClass(statusLabel),
                IsLowStockAlert = statusLabel.Equals("Low Stock", StringComparison.OrdinalIgnoreCase) ||
                                  statusLabel.Equals("Out Of Stock", StringComparison.OrdinalIgnoreCase),
                ImageUrl = ResolveImagePath(safeProduct.ImagePath),
                ImageAlt = BuildImageAlt(safeProduct)
            };
        }

        private static string BuildSku(int id)
        {
            return id > 0 ? "AZ-" + id.ToString("000000", CultureInfo.InvariantCulture) : "AZ-000000";
        }

        private string ResolveImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return DefaultProductImage;
            }

            var normalizedPath = imagePath.Trim();
            if (Uri.TryCreate(normalizedPath, UriKind.Absolute, out var absoluteUri))
            {
                return absoluteUri.ToString();
            }

            if (normalizedPath.StartsWith("~/", StringComparison.Ordinal))
            {
                return ResolveUrl(normalizedPath);
            }

            if (normalizedPath.StartsWith("/", StringComparison.Ordinal))
            {
                return normalizedPath;
            }

            return ResolveUrl("~/" + normalizedPath.TrimStart('/'));
        }

        private static string BuildImageAlt(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                return product.Description.Trim();
            }

            if (!string.IsNullOrWhiteSpace(product.ProductName))
            {
                return product.ProductName.Trim() + " product image";
            }

            return "Product image";
        }

        private static string NormalizeStatus(string status, int stockQuantity)
        {
            var normalizedStatus = (status ?? string.Empty).Trim();
            if (normalizedStatus.Equals("InStock", StringComparison.OrdinalIgnoreCase)) return "In Stock";
            if (normalizedStatus.Equals("LowStock", StringComparison.OrdinalIgnoreCase)) return "Low Stock";
            if (normalizedStatus.Equals("OutOfStock", StringComparison.OrdinalIgnoreCase)) return "Out Of Stock";
            if (normalizedStatus.Equals("Low", StringComparison.OrdinalIgnoreCase)) return "Low Stock";
            if (normalizedStatus.Equals("Available", StringComparison.OrdinalIgnoreCase)) return "In Stock";
            if (normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase)) return "In Stock";

            if (!string.IsNullOrWhiteSpace(normalizedStatus))
            {
                return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalizedStatus.ToLowerInvariant());
            }

            if (stockQuantity <= 0) return "Out Of Stock";
            if (stockQuantity <= 10) return "Low Stock";
            return "In Stock";
        }

        private static string GetStatusBadgeCssClass(string status)
        {
            if (status.Equals("Low Stock", StringComparison.OrdinalIgnoreCase))
            {
                return "inline-block px-3 py-1 bg-secondary text-on-secondary text-[10px] font-bold uppercase tracking-widest";
            }

            if (status.Equals("Out Of Stock", StringComparison.OrdinalIgnoreCase))
            {
                return "inline-block px-3 py-1 bg-error text-on-error text-[10px] font-bold uppercase tracking-widest";
            }

            return "inline-block px-3 py-1 bg-surface-container-highest text-on-surface text-[10px] font-bold uppercase tracking-widest";
        }

        private static string GetPopularBadgeCssClass(bool isPopular)
        {
            return isPopular
                ? "inline-block px-3 py-1 bg-secondary/20 text-secondary text-[10px] font-bold uppercase tracking-widest border border-secondary/40"
                : "inline-block px-3 py-1 bg-surface-container-highest text-on-surface-variant text-[10px] font-bold uppercase tracking-widest";
        }

        private static string FormatNumber(int value)
        {
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }

        private void HandleActionMessage()
        {
            var created = Request.QueryString["created"];
            if (string.Equals(created, "1", StringComparison.Ordinal))
            {
                ShowActionMessage("Product created successfully.", false);
                return;
            }

            var updated = Request.QueryString["updated"];
            if (string.Equals(updated, "1", StringComparison.Ordinal))
            {
                ShowActionMessage("Product updated successfully.", false);
                return;
            }

            var deleted = Request.QueryString["deleted"];
            if (string.Equals(deleted, "1", StringComparison.Ordinal))
            {
                ShowActionMessage("Product deleted successfully.", false);
                return;
            }

            if (string.Equals(deleted, "0", StringComparison.Ordinal))
            {
                ShowActionMessage("Product could not be deleted.", true);
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

        private static List<ProductRowVm> GetFallbackRows()
        {
            return new List<ProductRowVm>
            {
                CreateFallbackRow(
                    "Obsidian Wool Overcoat",
                    "AZ-W24-001",
                    "Outerwear",
                    "$895.00",
                    42,
                    "In Stock",
                    "https://lh3.googleusercontent.com/aida-public/AB6AXuBf8LWw_nwQ42K9A7U8s_Dd28P0bGdD4ittey0PkRJg-SnABfZIphC63J2bBqv9zgjHzWeC6esOs78K4kaQILpE2CdoHGvwOWMBc19shqJrIAS5Qp09AbpYB7z91_xu_equYGzj8dcAn7k2UFJPipb5ks6AD6CAPqpzYI9u3wBBrmQiHoTCGViGAmnsZLZh5LCUjfZXWU7tfktJ7C1qI7vLmRHcSa3a0qOv1meyzuK77MANLH5KdufszWZX4QyVQ3flciGzkPMqyvU"),
                CreateFallbackRow(
                    "Heritage Gold Loafer",
                    "AZ-F24-045",
                    "Footwear",
                    "$420.00",
                    8,
                    "Low Stock",
                    "https://lh3.googleusercontent.com/aida-public/AB6AXuApMb8h3c02MhE_J9ub4J2F-khAzttMycRj21VB353SxHbA4h7g-N_LtVkk1kKePB028qKe5gU7CVzHkF84OOU6WlePkotgHZ2GDSALprdPb1v-v6xZue-8Le00FuHPSNkS5uwjsarj4kLVCMfmtscY-iJe868v-KI4VvuZdlPh04T0RYkOyVVwTMMJEbCtqTKN7qQ6jQ1PeCFZ57J01nPlAjflLfVj43G_GZKOeVXD8CIvGLsTv-n504XkoGlj7VDAkUUoOdxrWyY"),
                CreateFallbackRow(
                    "Midnight Tuxedo Set",
                    "AZ-S24-012",
                    "Tailored Suits",
                    "$1,450.00",
                    15,
                    "In Stock",
                    "https://lh3.googleusercontent.com/aida-public/AB6AXuDku9R9t0SZNecbgqA46jfPYLDYQp35-BPqbGTbR1C2KpKmHhb7PgpWS8NeIF5MJApCadLEeQymPCfUXsTEjV29CA8gGhpRgb7OsF-0HUc5KFzoVsR74ySwWP44tMKp3KVjCDxBmfFZKoPiww4sPH_f-VlDNL2VtyscdGruEOfBOa5mFKAWPktyPVrOWg10qwUU84W79uPignu-NJDFdck1Jl5O10MG9s7IVopht3c57_t1KuhBfPb1cnIJoDSo5AUtFgwvJ9ovYt4"),
                CreateFallbackRow(
                    "Chronograph Gilt Watch",
                    "AZ-A24-089",
                    "Accessories",
                    "$2,100.00",
                    3,
                    "Low Stock",
                    "https://lh3.googleusercontent.com/aida-public/AB6AXuDAevGCTxEd4jFU3RJGfuLmMYHgEErpHKk4MhyOzfvUw0gyOkxCrDni0V5nFrAjlYbqt_BvEAShyqIjZ5OBgivhFnq4CvIjfrHmi6O_-BJu-J-U6lm2U-N8HWu4EKYazGtv7w2q9EfyrT5_GKTuRy54Mj4o8M8UoHx-fkNMytqZEq4lQEeUxAKZRrMgjMLwmeViUQ781t_DnemMfcv-RbVwAoLpBpVAARepNFppPtmMIP_Pi6jE-DkggERASgNSMtpL5E6kQmI0vm8"),
                CreateFallbackRow(
                    "Pure Cashmere Crewneck",
                    "AZ-K24-210",
                    "Outerwear",
                    "$325.00",
                    112,
                    "In Stock",
                    "https://lh3.googleusercontent.com/aida-public/AB6AXuAspotNsgWaphSfl7uzDtO5m45AMb8sYjmH6IQG0agDdrLIkriUf_lJgTshSG6N0HQkxmEsHNLGCXFhUhaBK6KDA3CsVmTLBcjK6NwF9Ts8eOxID5bILy3BPWw-G_2pgwyOeh_U_92_AEvh3zEbLzkRu-zqIamcAHAzcfyHiFGPMyN3bMOHTvoMwlI8EObsNwT_xfS2T9O6s68V6WeNBLr2kZ2Wb8JDz137DqfRbg5nMJUPCJFtdyeyc29Pql1F5DrLYWvhe3vOfSI")
            };
        }

        private static ProductRowVm CreateFallbackRow(
            string productName,
            string sku,
            string category,
            string priceFormatted,
            int stockQuantity,
            string statusLabel,
            string imageUrl,
            bool isPopular = false)
        {
            return new ProductRowVm
            {
                ProductName = productName,
                Sku = sku,
                Category = category,
                IsPopular = isPopular,
                PopularLabel = isPopular ? "Popular" : "Standard",
                PopularBadgeCssClass = GetPopularBadgeCssClass(isPopular),
                PriceFormatted = priceFormatted,
                StockQuantity = stockQuantity,
                StatusLabel = statusLabel,
                StatusBadgeCssClass = GetStatusBadgeCssClass(statusLabel),
                IsLowStockAlert = statusLabel.Equals("Low Stock", StringComparison.OrdinalIgnoreCase) ||
                                  statusLabel.Equals("Out Of Stock", StringComparison.OrdinalIgnoreCase),
                ImageUrl = imageUrl,
                ImageAlt = productName + " image"
            };

        }
    }
}