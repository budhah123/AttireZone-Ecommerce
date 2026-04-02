using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;
using CartRecord = AttireZone_Web_App.Models.Cart;

namespace AttireZone_Web_App.Customer
{
    public partial class Cart : Page
    {
        private const decimal EstimatedTaxAmount = 0m;
        private const string FallbackProductImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCM4rF3A9mZIXTnaNLjx8FVr6etfOJ1uYxaYUzobhic4vXZiCaHNs82pcY8AlzglFNTd2Gi-JzJoUKw_tLSv1wghUhmfrJATuY_3WMFerO8bcoCXwBXU07d96pNKxxvR8o_MEyT_5-AVAa80HpLjmXmQgcBFxeCzrZkN-s7OUBUImRQOxyAHAHhfo7cJ55qflBYnG3TxwmckCPJHUQkvYTlK4sRiJsznvPejL5ifgCnMTfoC-docqGUWsw46AXNnkxh_LBVNaiSZPQ";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!IsPostBack)
            {
                BindCart(userId);
            }
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "RemoveItem", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var cartId) || cartId <= 0)
            {
                ShowSnackbar("Unable to remove this item right now.", "error");
                return;
            }

            try
            {
                CartService.RemoveFromCart(cartId);
                BindCart(userId);
                ShowSnackbar("Item removed from cart.", "info");
            }
            catch
            {
                ShowSnackbar("Unable to remove this item right now.", "error");
            }
        }

        private void BindCart(int userId)
        {
            var cartRows = LoadCartRows(userId);
            var productsById = LoadProductsById();
            var cartItems = cartRows
                .Select(cartRow => BuildCartItemViewModel(cartRow, productsById))
                .ToList();

            rptCartItems.DataSource = cartItems;
            rptCartItems.DataBind();

            phEmptyCart.Visible = cartItems.Count == 0;

            var subtotal = cartItems.Sum(item => item.ItemTotal);
            var total = subtotal + EstimatedTaxAmount;

            litBagSubtotal.Text = FormatCurrency(subtotal);
            litSubtotal.Text = FormatCurrency(subtotal);
            litTax.Text = FormatCurrency(EstimatedTaxAmount);
            litGrandTotal.Text = FormatCurrency(total);
        }

        private static List<CartRecord> LoadCartRows(int userId)
        {
            try
            {
                return CartService.GetCartByUserId(userId) ?? new List<CartRecord>();
            }
            catch
            {
                return new List<CartRecord>();
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

        private CartItemViewModel BuildCartItemViewModel(CartRecord cartRow, IDictionary<int, Product> productsById)
        {
            if (cartRow == null)
            {
                return new CartItemViewModel
                {
                    CartId = 0,
                    ProductName = "Curated Piece",
                    ReferenceLabel = "Ref: N/A | Size: M",
                    SelectedQuantity = 1,
                    ItemTotal = 0m,
                    ItemTotalLabel = FormatCurrency(0m),
                    ImageUrl = FallbackProductImageUrl,
                    ImageAlt = "Curated product"
                };
            }

            productsById.TryGetValue(cartRow.ProductId, out var product);

            var productName = string.IsNullOrWhiteSpace(product == null ? null : product.ProductName)
                ? "Curated Piece"
                : product.ProductName.Trim();
            var quantity = cartRow.SelectedQuantity <= 0 ? 1 : cartRow.SelectedQuantity;
            var unitPrice = product == null ? 0m : product.Price;
            var itemTotal = unitPrice * quantity;
            var selectedSize = string.IsNullOrWhiteSpace(cartRow.SelectedSize) ? "M" : cartRow.SelectedSize.Trim().ToUpperInvariant();

            return new CartItemViewModel
            {
                CartId = cartRow.Id,
                ProductName = productName,
                ReferenceLabel = string.Format(CultureInfo.InvariantCulture, "Ref: {0} | Size: {1}", cartRow.ProductId, selectedSize),
                SelectedQuantity = quantity,
                ItemTotal = itemTotal,
                ItemTotalLabel = FormatCurrency(itemTotal),
                ImageUrl = ResolveProductImageUrl(product == null ? null : product.ImagePath),
                ImageAlt = productName
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

        private static string FormatCurrency(decimal amount)
        {
            return amount.ToString("$0.00", CultureInfo.InvariantCulture);
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Customer/Cart.aspx"));
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

            ScriptManager.RegisterStartupScript(this, GetType(), "cartSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class CartItemViewModel
        {
            public int CartId { get; set; }

            public string ProductName { get; set; }

            public string ReferenceLabel { get; set; }

            public int SelectedQuantity { get; set; }

            public decimal ItemTotal { get; set; }

            public string ItemTotalLabel { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }
        }
    }
}
