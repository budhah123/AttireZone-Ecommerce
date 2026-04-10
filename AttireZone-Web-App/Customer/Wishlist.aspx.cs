using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;
using WishlistRecord = AttireZone_Web_App.Models.Wishlist;

namespace AttireZone_Web_App.Customer
{
    public partial class Wishlist : System.Web.UI.Page
    {
        private const string FallbackProductImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBAxBDyaXVSNvWBgJguaIJAty6OftEJp3jItW5YP9aLzAUuDue8NkCY2bzqb5Z7eOTthWynxxMx0WcNgjVqgqYCpxy3UolkkqtOWTtCK3yhRrGAehmgXD8JKWcxKcXcGgsA6SkOWm_MA9segGWll4Qnew7vdr3zxzf-tlrENKx_J25p3srsadLJq08x4vXrEGMwSXD1m5BFJAt-YrgmuzqOmiYON0xMVB88UyruC3TPkYP_OxGp9xwybu3niUXtLBqtr-jubX8pihc";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!IsPostBack)
            {
                BindWishlist(userId);
            }
        }

        protected void rptWishlistItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (string.Equals(e.CommandName, "RemoveWishlistItem", StringComparison.OrdinalIgnoreCase))
            {
                RemoveWishlistItem(e.CommandArgument, userId);
                return;
            }

            if (string.Equals(e.CommandName, "AddToCartFromWishlist", StringComparison.OrdinalIgnoreCase))
            {
                AddWishlistItemToCart(e.CommandArgument, userId);
            }
        }

        private void BindWishlist(int userId)
        {
            var wishlistRows = LoadWishlistRows(userId);
            var productsById = LoadProductsById();
            var items = wishlistRows
                .Select(wishlistRow => BuildWishlistItemViewModel(wishlistRow, productsById))
                .ToList();

            rptWishlistItems.DataSource = items;
            rptWishlistItems.DataBind();

            phEmptyWishlist.Visible = items.Count == 0;
            rptWishlistItems.Visible = items.Count > 0;
        }

        private static List<WishlistRecord> LoadWishlistRows(int userId)
        {
            try
            {
                return WishlistService.GetWishlistByUserId(userId) ?? new List<WishlistRecord>();
            }
            catch
            {
                return new List<WishlistRecord>();
            }
        }

        private static Dictionary<int, Product> LoadProductsById()
        {
            try
            {
                return (ProductService.GetAllProducts() ?? new List<Product>())
                    .Where(product => product != null && product.Id > 0)
                    .GroupBy(product => product.Id)
                    .ToDictionary(group => group.Key, group => group.First());
            }
            catch
            {
                return new Dictionary<int, Product>();
            }
        }

        private WishlistItemViewModel BuildWishlistItemViewModel(WishlistRecord wishlistRow, IDictionary<int, Product> productsById)
        {
            if (wishlistRow == null)
            {
                return new WishlistItemViewModel
                {
                    WishlistId = 0,
                    ProductId = 0,
                    ProductName = "Curated Piece",
                    PriceLabel = "$0",
                    ImageUrl = FallbackProductImageUrl,
                    ImageAlt = "Curated product",
                    ProductDetailsUrl = ResolveUrl("~/Pages/Product.aspx")
                };
            }

            productsById.TryGetValue(wishlistRow.ProductId, out var product);

            var productName = string.IsNullOrWhiteSpace(product == null ? null : product.ProductName)
                ? "Curated Piece"
                : product.ProductName.Trim();
            var productPrice = product == null ? 0m : product.Price;
            var productId = product == null ? wishlistRow.ProductId : product.Id;

            return new WishlistItemViewModel
            {
                WishlistId = wishlistRow.WishlistId,
                ProductId = productId,
                ProductName = productName,
                PriceLabel = "$" + productPrice.ToString("0,0.##", CultureInfo.InvariantCulture),
                ImageUrl = ResolveProductImageUrl(product == null ? null : product.ImagePath),
                ImageAlt = productName,
                ProductDetailsUrl = BuildProductDetailsUrl(productId)
            };
        }

        private string ResolveProductImageUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return FallbackProductImageUrl;
            }

            var normalizedPath = imagePath.Trim();
            if (normalizedPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath;
            }

            if (normalizedPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl(normalizedPath);
            }

            if (normalizedPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl("~" + normalizedPath);
            }

            return ResolveUrl("~/" + normalizedPath.TrimStart('/'));
        }

        private string BuildProductDetailsUrl(int productId)
        {
            var baseUrl = ResolveUrl("~/Pages/ProductDetails.aspx");
            if (productId <= 0)
            {
                return baseUrl;
            }

            return string.Concat(
                baseUrl,
                "?id=",
                productId.ToString(CultureInfo.InvariantCulture));
        }

        private void RemoveWishlistItem(object commandArgument, int userId)
        {
            if (!int.TryParse(Convert.ToString(commandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var wishlistId) || wishlistId <= 0)
            {
                ShowSnackbar("Unable to update wishlist right now.", "error");
                return;
            }

            try
            {
                var removed = WishlistService.RemoveFromWishlist(wishlistId, userId);
                if (!removed)
                {
                    ShowSnackbar("Wishlist item not found.", "info");
                    return;
                }

                BindWishlist(userId);
                ShowSnackbar("Item removed from wishlist.", "info");
            }
            catch
            {
                ShowSnackbar("Unable to update wishlist right now.", "error");
            }
        }

        private void AddWishlistItemToCart(object commandArgument, int userId)
        {
            if (!int.TryParse(Convert.ToString(commandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var productId) || productId <= 0)
            {
                ShowSnackbar("Unable to add this item to cart.", "error");
                return;
            }

            try
            {
                CartService.AddToCart(userId, productId, 1, "M");
                ShowSnackbar("Item added to shopping bag.", "success");
            }
            catch
            {
                ShowSnackbar("Unable to add this item to cart.", "error");
            }
        }

        private bool TryReadLoggedInUserId(out int userId)
        {
            userId = 0;

            var sessionUserId = Session["UserId"];
            if (sessionUserId == null)
            {
                return false;
            }

            if (sessionUserId is int directUserId && directUserId > 0)
            {
                userId = directUserId;
                return true;
            }

            return int.TryParse(Convert.ToString(sessionUserId, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out userId) && userId > 0;
        }

        private void RedirectToLoginWithReturnUrl()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Customer/Wishlist.aspx"));
            Response.Redirect("~/Auth/Login.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowSnackbar(string message, string type)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            var script = string.Format(
                CultureInfo.InvariantCulture,
                "window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} }}, 0);",
                safeMessage,
                safeType);

            ScriptManager.RegisterStartupScript(this, GetType(), "wishlistSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class WishlistItemViewModel
        {
            public int WishlistId { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string PriceLabel { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }

            public string ProductDetailsUrl { get; set; }
        }
    }
}