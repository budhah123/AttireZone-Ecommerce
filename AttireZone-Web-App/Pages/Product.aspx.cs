using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Pages
{
    public partial class Product : System.Web.UI.Page
    {
        private const string ActiveCategoryCssClass = "text-sm font-medium text-secondary block py-1 border-b border-secondary/30";
        private const string InactiveCategoryCssClass = "text-sm font-medium hover:text-secondary transition-colors block py-1 border-b border-outline-variant/10";
        private const string NewBadgeCssClass = "absolute top-4 left-4 bg-white text-black px-3 py-1 text-[9px] font-black uppercase tracking-widest";
        private const string HiddenBadgeCssClass = "hidden";
        private const string DefaultProductImageVirtualPath = "~/Assets/Images/Hero-Section-Image.png";

        protected string AllCategoryCssClass { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCatalog();
            }

            if (string.IsNullOrWhiteSpace(AllCategoryCssClass))
            {
                AllCategoryCssClass = InactiveCategoryCssClass;
            }
        }

        private void BindCatalog()
        {
            var categories = LoadCategories();
            var allProducts = LoadProducts();

            var selectedCategoryId = ReadSelectedCategoryId();
            if (selectedCategoryId.HasValue && categories.All(category => category.Id != selectedCategoryId.Value))
            {
                selectedCategoryId = null;
            }

            var filteredProducts = FilterProductsByCategory(allProducts, selectedCategoryId);

            BindCategories(categories, selectedCategoryId);
            BindProducts(filteredProducts);

            var totalVisibleProducts = allProducts.Count(product => product != null && !string.IsNullOrWhiteSpace(product.ProductName));
            var shownProducts = filteredProducts.Count(product => product != null && !string.IsNullOrWhiteSpace(product.ProductName));
            litShowingSummary.Text = BuildShowingSummary(shownProducts, totalVisibleProducts);
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

        private static List<Models.Product> LoadProducts()
        {
            try
            {
                return ProductService.GetAllProducts() ?? new List<Models.Product>();
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

        private static List<Models.Product> FilterProductsByCategory(List<Models.Product> allProducts, int? selectedCategoryId)
        {
            var products = allProducts ?? new List<Models.Product>();
            if (!selectedCategoryId.HasValue)
            {
                return products;
            }

            return products
                .Where(product => product != null && product.CategoryId.HasValue && product.CategoryId.Value == selectedCategoryId.Value)
                .ToList();
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
            var pageUrl = ResolveUrl("~/Pages/Product.aspx");
            return string.Concat(pageUrl, "?category=", categoryId.ToString(CultureInfo.InvariantCulture));
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

        private sealed class CategoryFilterVm
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public string CssClass { get; set; }
        }

        private sealed class ProductCardVm
        {
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