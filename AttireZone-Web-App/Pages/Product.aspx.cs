using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Pages
{
    public partial class Product : System.Web.UI.Page
    {
        private const string SearchQueryKey = "q";
        private const string SortQueryKey = "sort";
        private const string ActiveCategoryCssClass = "text-sm font-medium text-secondary block py-1 border-b border-secondary/30";
        private const string InactiveCategoryCssClass = "text-sm font-medium hover:text-secondary transition-colors block py-1 border-b border-outline-variant/10";
        private const string NewBadgeCssClass = "absolute top-4 left-4 bg-white text-black px-3 py-1 text-[9px] font-black uppercase tracking-widest";
        private const string HiddenBadgeCssClass = "hidden";
        private const string DefaultProductImageVirtualPath = "~/Assets/Images/Hero-Section-Image.png";

        protected string AllCategoryCssClass { get; private set; }

        protected string AllCategoryUrl { get; private set; }

        private string CurrentSearchTerm { get; set; }

        private string CurrentSortOption { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ApplyQueryParametersToControls();
                BindCatalog();
            }

            if (string.IsNullOrWhiteSpace(AllCategoryCssClass))
            {
                AllCategoryCssClass = InactiveCategoryCssClass;
            }

            if (string.IsNullOrWhiteSpace(AllCategoryUrl))
            {
                AllCategoryUrl = ResolveUrl("~/Pages/Product.aspx");
            }
        }

        protected void txtSearchProducts_TextChanged(object sender, EventArgs e)
        {
            RedirectWithCurrentFilters();
        }

        protected void btnSearchProducts_Click(object sender, EventArgs e)
        {
            RedirectWithCurrentFilters();
        }

        protected void ddlSortProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedirectWithCurrentFilters();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string> GetSearchSuggestions(string term)
        {
            try
            {
                return ProductService.GetSearchSuggestions(term, 8);
            }
            catch
            {
                return new List<string>();
            }
        }

        protected void rptProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "AddToCart", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var productId) || productId <= 0)
            {
                ShowSnackbar("Unable to add this item right now.", "error");
                return;
            }

            var sizeSelector = e.Item.FindControl("ddlSelectedSize") as DropDownList;
            var quantityInput = e.Item.FindControl("txtSelectedQuantity") as TextBox;

            var selectedSize = sizeSelector == null ? "M" : sizeSelector.SelectedValue;
            var selectedQuantity = ParseSelectedQuantity(quantityInput == null ? null : quantityInput.Text);

            try
            {
                CartService.AddToCart(userId, productId, selectedQuantity, selectedSize);
                ShowSnackbar("Item added to cart!", "success");
            }
            catch
            {
                ShowSnackbar("Unable to add item to cart right now.", "error");
            }
        }

        private void BindCatalog()
        {
            var categories = LoadCategories();

            var selectedCategoryId = ReadSelectedCategoryId();
            if (selectedCategoryId.HasValue && categories.All(category => category.Id != selectedCategoryId.Value))
            {
                selectedCategoryId = null;
            }

            CurrentSearchTerm = NormalizeSearchTerm(txtSearchProducts == null ? null : txtSearchProducts.Text);
            CurrentSortOption = NormalizeSortOption(ddlSortProducts == null ? null : ddlSortProducts.SelectedValue);
            AllCategoryUrl = BuildCatalogUrl(null, CurrentSearchTerm, CurrentSortOption);

            var filteredProducts = LoadProducts(CurrentSearchTerm, selectedCategoryId, CurrentSortOption);

            BindCategories(categories, selectedCategoryId);
            BindProducts(filteredProducts);

            var shownProducts = filteredProducts.Count(product => product != null && !string.IsNullOrWhiteSpace(product.ProductName));
            litShowingSummary.Text = BuildShowingSummary(shownProducts, shownProducts);
        }

        private static List<Category> LoadCategories()
        {
            try
            {
                return CategoryService.GetAllCategories() ?? new List<Category>();
            }
            catch
            {
                return new List<Category>();
            }
        }

        private static List<Models.Product> LoadProducts(string searchTerm, int? selectedCategoryId, string sortOption)
        {
            try
            {
                return ProductService.SearchProducts(searchTerm, selectedCategoryId, sortOption) ?? new List<Models.Product>();
            }
            catch
            {
                return new List<Models.Product>();
            }
        }

        private int? ReadSelectedCategoryId()
        {
            var rawCategoryValue = Request.QueryString["category"];
            if (string.IsNullOrWhiteSpace(rawCategoryValue))
            {
                return null;
            }

            if (!int.TryParse(rawCategoryValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var categoryId) || categoryId <= 0)
            {
                return null;
            }

            return categoryId;
        }

        private void BindCategories(List<Category> categories, int? selectedCategoryId)
        {
            var categoryFilters = (categories ?? new List<Category>())
                .Where(category => category != null && category.Id > 0 && !string.IsNullOrWhiteSpace(category.Name))
                .OrderBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
                .Select(category => new CategoryFilterVm
                {
                    Name = category.Name.Trim(),
                    Url = BuildCategoryFilterUrl(category.Id),
                    CssClass = selectedCategoryId.HasValue && selectedCategoryId.Value == category.Id
                        ? ActiveCategoryCssClass
                        : InactiveCategoryCssClass
                })
                .ToList();

            AllCategoryCssClass = selectedCategoryId.HasValue ? InactiveCategoryCssClass : ActiveCategoryCssClass;

            rptCategories.DataSource = categoryFilters;
            rptCategories.DataBind();
        }

        private string BuildCategoryFilterUrl(int categoryId)
        {
            return BuildCatalogUrl(categoryId, CurrentSearchTerm, CurrentSortOption);
        }

        private void ApplyQueryParametersToControls()
        {
            if (txtSearchProducts != null)
            {
                txtSearchProducts.Text = NormalizeSearchTerm(Request.QueryString[SearchQueryKey]) ?? string.Empty;
            }

            if (ddlSortProducts == null)
            {
                return;
            }

            var requestedSort = NormalizeSortOption(Request.QueryString[SortQueryKey]);
            var selectedItem = ddlSortProducts.Items.FindByValue(requestedSort);
            if (selectedItem != null)
            {
                ddlSortProducts.ClearSelection();
                selectedItem.Selected = true;
            }
        }

        private void RedirectWithCurrentFilters()
        {
            var selectedCategoryId = ReadSelectedCategoryId();
            var searchTerm = NormalizeSearchTerm(txtSearchProducts == null ? null : txtSearchProducts.Text);
            var sortOption = NormalizeSortOption(ddlSortProducts == null ? null : ddlSortProducts.SelectedValue);

            var url = BuildCatalogUrl(selectedCategoryId, searchTerm, sortOption);
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private string BuildCatalogUrl(int? categoryId, string searchTerm, string sortOption)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query["category"] = categoryId.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query[SearchQueryKey] = searchTerm;
            }

            var normalizedSort = NormalizeSortOption(sortOption);
            if (!normalizedSort.Equals("featured", StringComparison.OrdinalIgnoreCase))
            {
                query[SortQueryKey] = normalizedSort;
            }

            var baseUrl = ResolveUrl("~/Pages/Product.aspx");
            var queryString = query.ToString();

            return string.IsNullOrWhiteSpace(queryString)
                ? baseUrl
                : string.Concat(baseUrl, "?", queryString);
        }

        private static string NormalizeSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return null;
            }

            return searchTerm.Trim();
        }

        private static string NormalizeSortOption(string sortOption)
        {
            if (string.IsNullOrWhiteSpace(sortOption))
            {
                return "featured";
            }

            var normalized = sortOption.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "featured":
                case "newest":
                case "price_asc":
                case "price_desc":
                case "name_asc":
                case "name_desc":
                    return normalized;
                default:
                    return "featured";
            }
        }

        private void BindProducts(List<Models.Product> products)
        {
            var cards = (products ?? new List<Models.Product>())
                .Where(product => product != null && !string.IsNullOrWhiteSpace(product.ProductName))
                .OrderByDescending(product => product.IsPopular)
                .ThenByDescending(product => product.Id)
                .Select(MapProductCard)
                .ToList();

            rptProducts.DataSource = cards;
            rptProducts.DataBind();

            var hasProducts = cards.Count > 0;
            rptProducts.Visible = hasProducts;
            phNoProducts.Visible = !hasProducts;
        }

        private ProductCardVm MapProductCard(Models.Product product)
        {
            var categoryLabel = string.IsNullOrWhiteSpace(product.CategoryName)
                ? "AttireZone Studio"
                : product.CategoryName.Trim();

            return new ProductCardVm
            {
                ProductId = product.Id,
                ProductName = product.ProductName.Trim(),
                CategoryLabel = categoryLabel,
                PriceLabel = product.Price.ToString("$0.00", CultureInfo.InvariantCulture),
                ImageUrl = ResolveProductImageUrl(product.ImagePath),
                ImageAlt = BuildImageAlt(product.ProductName, categoryLabel),
                BadgeCssClass = product.IsPopular ? NewBadgeCssClass : HiddenBadgeCssClass,
                ViewDetailsUrl = BuildProductDetailsUrl(product.Id)
            };
        }

        private string BuildProductDetailsUrl(int productId)
        {
            var baseUrl = ResolveUrl("~/Pages/ProductDetails.aspx");
            if (productId <= 0)
            {
                return baseUrl;
            }

            return string.Concat(baseUrl, "?id=", productId.ToString(CultureInfo.InvariantCulture));
        }

        private string ResolveProductImageUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return ResolveUrl(DefaultProductImageVirtualPath);
            }

            var normalizedPath = imagePath.Trim();
            if (normalizedPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath;
            }

            if (normalizedPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl(normalizedPath);
            }

            return ResolveUrl("~/" + normalizedPath.TrimStart('/'));
        }

        private static string BuildImageAlt(string productName, string categoryName)
        {
            var safeProductName = string.IsNullOrWhiteSpace(productName) ? "AttireZone product" : productName.Trim();
            var safeCategoryName = string.IsNullOrWhiteSpace(categoryName) ? "curated collection" : categoryName.Trim();
            return string.Concat(safeProductName, " - ", safeCategoryName);
        }

        private static string BuildShowingSummary(int shownCount, int totalCount)
        {
            if (shownCount <= 0 || totalCount <= 0)
            {
                return string.Concat(
                    "Showing 0 of ",
                    Math.Max(totalCount, 0).ToString(CultureInfo.InvariantCulture),
                    " Items");
            }

            return string.Concat(
                "Showing 1-",
                shownCount.ToString(CultureInfo.InvariantCulture),
                " of ",
                totalCount.ToString(CultureInfo.InvariantCulture),
                " Items");
        }

        private static int ParseSelectedQuantity(string rawQuantity)
        {
            if (!int.TryParse(rawQuantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity <= 0)
            {
                return 1;
            }

            return quantity;
        }

        private bool TryReadLoggedInUserId(out int userId)
        {
            userId = 0;

            var userIdFromSession = Session["UserId"];
            if (userIdFromSession == null)
            {
                return false;
            }

            if (userIdFromSession is int directUserId && directUserId > 0)
            {
                userId = directUserId;
                return true;
            }

            return int.TryParse(Convert.ToString(userIdFromSession, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out userId) && userId > 0;
        }

        private void RedirectToLoginWithReturnUrl()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Pages/Product.aspx"));
            Response.Redirect("~/Auth/Login.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowSnackbar(string message, string type)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            var script = string.Format(
                "window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} else {{ alert('{0}'); }} }}, 0);",
                safeMessage,
                safeType);

            ClientScript.RegisterStartupScript(GetType(), "catalogSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class CategoryFilterVm
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public string CssClass { get; set; }
        }

        private sealed class ProductCardVm
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string CategoryLabel { get; set; }

            public string PriceLabel { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }

            public string BadgeCssClass { get; set; }

            public string ViewDetailsUrl { get; set; }

        }
    }
}