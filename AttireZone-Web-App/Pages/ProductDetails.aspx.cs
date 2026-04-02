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

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!TryReadProductId(out var productId))
            {
                ShowSnackbar("Unable to add this item right now.", "error");
                return;
            }

            var selectedSize = ddlSelectedSize == null ? "M" : ddlSelectedSize.SelectedValue;
            var selectedQuantity = ParseSelectedQuantity(txtSelectedQuantity == null ? null : txtSelectedQuantity.Text);

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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Pages/ProductDetails.aspx"));
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

            ClientScript.RegisterStartupScript(GetType(), "productDetailsSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
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