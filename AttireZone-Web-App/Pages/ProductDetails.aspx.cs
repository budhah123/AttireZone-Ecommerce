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
    public partial class ProductDetails : System.Web.UI.Page
    {
        private const string DefaultProductImageVirtualPath = "~/Assets/Images/Hero-Section-Image.png";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProductDetails();
            }
        }

        private void BindProductDetails()
        {
            if (!TryReadProductId(out var productId))
            {
                ShowProductNotFound();
                return;
            }

            Models.Product product;
            try
            {
                product = ProductService.GetProductById(productId);
            }
            catch
            {
                product = null;
            }

            if (product == null)
            {
                ShowProductNotFound();
                return;
            }

            PopulateProductContent(product);
        }

        private bool TryReadProductId(out int productId)
        {
            productId = 0;

            var rawId = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(rawId))
            {
                return false;
            }

            return int.TryParse(rawId, NumberStyles.Integer, CultureInfo.InvariantCulture, out productId) && productId > 0;
        }

        private void PopulateProductContent(Models.Product product)
        {
            var safeProductName = string.IsNullOrWhiteSpace(product.ProductName)
                ? "Curated Product"
                : product.ProductName.Trim();
            var safeCategory = string.IsNullOrWhiteSpace(product.CategoryName)
                ? "Collections"
                : product.CategoryName.Trim();
            var safeDescription = string.IsNullOrWhiteSpace(product.Description)
                ? "A masterclass in modern tailoring and elevated craftsmanship for your signature wardrobe."
                : product.Description.Trim();

            litBreadcrumbCategory.Text = HttpUtility.HtmlEncode(safeCategory);
            litBreadcrumbProductName.Text = HttpUtility.HtmlEncode(safeProductName);

            litProductName.Text = HttpUtility.HtmlEncode(safeProductName);
            litPrice.Text = HttpUtility.HtmlEncode(product.Price.ToString("$0.00", CultureInfo.InvariantCulture));
            litDescription.Text = HttpUtility.HtmlEncode(safeDescription);

            var mainImage = ResolveProductImageUrl(product.ImagePath);
            imgProductMain.ImageUrl = mainImage;

            var isLimited = (!string.IsNullOrWhiteSpace(product.Edition) &&
                             product.Edition.IndexOf("limited", StringComparison.OrdinalIgnoreCase) >= 0) ||
                            product.IsPopular;
            phLimitedBadge.Visible = isLimited;

            BindSimilarProducts(product);

            phProductNotFound.Visible = false;
            phProductContent.Visible = true;
        }

        private void BindSimilarProducts(Models.Product currentProduct)
        {
            var similarProducts = new List<SimilarProductViewModel>();

            try
            {
                var allProducts = ProductService.GetAllProducts() ?? new List<Models.Product>();
                similarProducts = allProducts
                    .Where(product => product != null && product.Id != currentProduct.Id)
                    .OrderByDescending(product => product.IsPopular)
                    .ThenByDescending(product => product.Id)
                    .Take(3)
                    .Select(product => new SimilarProductViewModel
                    {
                        ProductName = string.IsNullOrWhiteSpace(product.ProductName) ? "Curated Product" : product.ProductName.Trim(),
                        Category = string.IsNullOrWhiteSpace(product.CategoryName) ? "Collections" : product.CategoryName.Trim(),
                        Price = product.Price.ToString("$0.00", CultureInfo.InvariantCulture),
                        Status = string.IsNullOrWhiteSpace(product.Status) ? "Available" : product.Status.Trim(),
                        ImageUrl = ResolveProductImageUrl(product.ImagePath),
                        DetailsUrl = ResolveUrl("~/Pages/ProductDetails.aspx?id=" + product.Id.ToString(CultureInfo.InvariantCulture))
                    })
                    .ToList();
            }
            catch
            {
                similarProducts = new List<SimilarProductViewModel>();
            }

            while (similarProducts.Count < 3)
            {
                similarProducts.Add(new SimilarProductViewModel
                {
                    ProductName = "Curated Product",
                    Category = "Collections",
                    Price = "$0.00",
                    Status = "Available",
                    ImageUrl = ResolveUrl(DefaultProductImageVirtualPath),
                    DetailsUrl = ResolveUrl("~/Pages/Product.aspx")
                });
            }

            rptSimilarProducts.DataSource = similarProducts;
            rptSimilarProducts.DataBind();
        }

        private Tuple<string, string> GetGalleryImages(int currentProductId, string fallbackImage)
        {
            try
            {
                var allProducts = ProductService.GetAllProducts() ?? new List<Models.Product>();
                var gallery = allProducts
                    .Where(product => product != null &&
                                      product.Id != currentProductId &&
                                      !string.IsNullOrWhiteSpace(product.ImagePath))
                    .OrderByDescending(product => product.IsPopular)
                    .ThenByDescending(product => product.Id)
                    .Take(2)
                    .Select(product => ResolveProductImageUrl(product.ImagePath))
                    .ToList();

                var firstImage = gallery.Count > 0 ? gallery[0] : fallbackImage;
                var secondImage = gallery.Count > 1 ? gallery[1] : firstImage;

                return Tuple.Create(firstImage, secondImage);
            }
            catch
            {
                return Tuple.Create(fallbackImage, fallbackImage);
            }
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

        private void ShowProductNotFound()
        {
            phProductContent.Visible = false;
            phProductNotFound.Visible = true;

        }

        private sealed class SimilarProductViewModel
        {
            public string ProductName { get; set; }
            public string Category { get; set; }
            public string Price { get; set; }
            public string Status { get; set; }
            public string ImageUrl { get; set; }
            public string DetailsUrl { get; set; }
        }
    }
}