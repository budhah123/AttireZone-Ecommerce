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
                var cartItemCount = GetCartItemCount(userId);
                ShowSnackbar("Item added to cart!", "success", cartItemCount);
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

            phProductNotFound.Visible = false;
            phProductContent.Visible = true;
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

        private static int GetCartItemCount(int userId)
        {
            if (userId <= 0)
            {
                return 0;
            }

            try
            {
                return Math.Max(0, CartService.GetCartCount(userId));
            }
            catch
            {
                return 0;
            }
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

        private void ShowSnackbar(string message, string type, int? cartItemCount = null)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");

            var cartBadgeUpdateScript = string.Empty;
            if (cartItemCount.HasValue)
            {
                var normalizedCartCount = Math.Max(0, cartItemCount.Value);
                cartBadgeUpdateScript = string.Concat(
                    "if (window.azCartBadge && typeof window.azCartBadge.setCount === 'function') {",
                    "window.azCartBadge.setCount(",
                    normalizedCartCount.ToString(CultureInfo.InvariantCulture),
                    ");",
                    "}");
            }

            var script = string.Concat(
                "window.setTimeout(function(){",
                "var showInlineSnackbar=function(message,variant){",
                "var host=document.getElementById('az-inline-snackbar-host');",
                "var toast=document.getElementById('az-inline-snackbar');",
                "if(!host||!toast){",
                "host=document.createElement('div');",
                "host.id='az-inline-snackbar-host';",
                "host.style.cssText='position:fixed;top:1.25rem;right:1.25rem;z-index:9999;pointer-events:none;';",
                "toast=document.createElement('div');",
                "toast.id='az-inline-snackbar';",
                "toast.style.cssText='min-width:280px;max-width:420px;padding:0.85rem 1rem;border:1px solid rgba(255,255,255,0.1);background:rgba(20,20,20,0.92);color:#f5f0e8;font-size:0.82rem;letter-spacing:0.03em;box-shadow:0 12px 26px rgba(0,0,0,0.35);backdrop-filter:blur(8px);transform:translateY(-10px);opacity:0;transition:transform 220ms ease,opacity 220ms ease;';",
                "host.appendChild(toast);",
                "document.body.appendChild(host);",
                "}",
                "var accent='#e9c349';",
                "if(variant==='success'){accent='#22c55e';}else if(variant==='error'){accent='#ef4444';}else if(variant==='info'){accent='#60a5fa';}",
                "toast.style.borderColor=accent;",
                "toast.textContent=message||'';",
                "toast.style.opacity='1';",
                "toast.style.transform='translateY(0)';",
                "window.clearTimeout(window.__azInlineSnackbarTimer);",
                "window.__azInlineSnackbarTimer=window.setTimeout(function(){toast.style.opacity='0';toast.style.transform='translateY(-10px)';},3400);",
                "};",
                "if(window.azSnackbar&&typeof window.azSnackbar.show==='function'){window.azSnackbar.show('",
                safeMessage,
                "','",
                safeType,
                "');}else{showInlineSnackbar('",
                safeMessage,
                "','",
                safeType,
                "');}",
                cartBadgeUpdateScript,
                "},0);");

            ClientScript.RegisterStartupScript(GetType(), "productDetailsSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private void ShowProductNotFound()
        {
            phProductContent.Visible = false;
            phProductNotFound.Visible = true;

        }

    }
}