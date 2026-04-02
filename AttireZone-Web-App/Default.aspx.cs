using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App
{
    public partial class _Default : Page
    {
        private const int CuratedProductCount = 4;
        private const string FallbackProductImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCM4rF3A9mZIXTnaNLjx8FVr6etfOJ1uYxaYUzobhic4vXZiCaHNs82pcY8AlzglFNTd2Gi-JzJoUKw_tLSv1wghUhmfrJATuY_3WMFerO8bcoCXwBXU07d96pNKxxvR8o_MEyT_5-AVAa80HpLjmXmQgcBFxeCzrZkN-s7OUBUImRQOxyAHAHhfo7cJ55qflBYnG3TxwmckCPJHUQkvYTlK4sRiJsznvPejL5ifgCnMTfoC-docqGUWsw46AXNnkxh_LBVNaiSZPQ";

        public string ProfileNavigationUrl => ResolveUrl(GetProfileNavigationPath());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCuratedProducts();
            }
        }

        protected void txtHomeSearch_TextChanged(object sender, EventArgs e)
        {
            RedirectToCatalogueSearch(txtHomeSearch == null ? null : txtHomeSearch.Text);
        }

        protected void btnHomeSearch_Click(object sender, EventArgs e)
        {
            RedirectToCatalogueSearch(txtHomeSearch == null ? null : txtHomeSearch.Text);
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

        private string GetProfileNavigationPath()
        {
            var currentUser = Session["CurrentUser"] as User;
            if (currentUser == null)
            {
                return "~/Auth/Login.aspx";
            }

            if (string.Equals(currentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return "~/Admin/Dashboard.aspx";
            }

            return "~/Customer/OrderHistory.aspx";
        }

        protected bool ShowBadge(object badgeText)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(badgeText));
        }

        private void BindCuratedProducts()
        {
            List<Product> allProducts;

            try
            {
                allProducts = ProductService.GetAllProducts();
            }
            catch
            {
                allProducts = new List<Product>();
            }

            List<CuratedProductCardViewModel> curatedProducts = allProducts
                .Where(product => product != null && !string.IsNullOrWhiteSpace(product.ProductName) && product.IsPopular)
                .OrderByDescending(product => product.Id)
                .Take(CuratedProductCount)
                .Select(BuildCuratedProductCard)
                .ToList();

            rptCuratedProducts.DataSource = curatedProducts;
            rptCuratedProducts.DataBind();

            phNoCuratedProducts.Visible = curatedProducts.Count == 0;
        }

        private CuratedProductCardViewModel BuildCuratedProductCard(Product product)
        {
            bool isLimitedEdition =
                !string.IsNullOrWhiteSpace(product.Edition) &&
                product.Edition.IndexOf("limited", StringComparison.OrdinalIgnoreCase) >= 0;

            return new CuratedProductCardViewModel
            {
                ProductName = product.ProductName,
                CategoryLabel = string.IsNullOrWhiteSpace(product.CategoryName) ? "Essentials" : product.CategoryName,
                PriceLabel = product.Price.ToString("$0.00", CultureInfo.InvariantCulture),
                ImageUrl = ResolveProductImageUrl(product.ImagePath),
                ImageAlt = BuildImageAltText(product),
                BadgeText = isLimitedEdition ? "Limited Edition" : (product.IsPopular ? "New Arrival" : string.Empty),
                BadgeCssClass = isLimitedEdition
                    ? "bg-error-container text-on-error-container px-3 py-1 text-[10px] font-bold tracking-widest uppercase"
                    : (product.IsPopular
                        ? "bg-secondary text-on-secondary px-3 py-1 text-[10px] font-bold tracking-widest uppercase"
                        : string.Empty)
            };
        }

        private void RedirectToCatalogueSearch(string rawSearch)
        {
            var searchTerm = string.IsNullOrWhiteSpace(rawSearch) ? string.Empty : rawSearch.Trim();
            var targetUrl = ResolveUrl("~/Pages/Product.aspx");

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                targetUrl = string.Concat(targetUrl, "?q=", HttpUtility.UrlEncode(searchTerm));
            }

            Response.Redirect(targetUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private string BuildImageAltText(Product product)
        {
            string productName = string.IsNullOrWhiteSpace(product.ProductName) ? "Curated product" : product.ProductName;
            string category = string.IsNullOrWhiteSpace(product.CategoryName) ? "fashion" : product.CategoryName;

            return string.Format(CultureInfo.InvariantCulture, "{0} in {1} collection", productName, category);
        }

        private string ResolveProductImageUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return FallbackProductImageUrl;
            }

            string cleanedPath = imagePath.Trim();

            if (Uri.IsWellFormedUriString(cleanedPath, UriKind.Absolute))
            {
                return cleanedPath;
            }

            if (cleanedPath.StartsWith("~/", StringComparison.Ordinal))
            {
                return ResolveUrl(cleanedPath);
            }

            if (cleanedPath.StartsWith("/", StringComparison.Ordinal))
            {
                return ResolveUrl("~" + cleanedPath);
            }

            return ResolveUrl("~/" + cleanedPath.TrimStart('/'));
        }

        private sealed class CuratedProductCardViewModel
        {
            public string ProductName { get; set; }

            public string CategoryLabel { get; set; }

            public string PriceLabel { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }

            public string BadgeText { get; set; }

            public string BadgeCssClass { get; set; }
        }
    }
}