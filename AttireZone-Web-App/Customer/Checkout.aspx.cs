using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Helpers;
using AttireZone_Web_App.Models;
using CartRecord = AttireZone_Web_App.Models.Cart;

namespace AttireZone_Web_App.Customer
{
    public partial class Checkout : Page
    {
        private const decimal ShippingFee = 25m;
        private const string DefaultPaymentMethod = "eSewa";
        private const string FallbackProductImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCM4rF3A9mZIXTnaNLjx8FVr6etfOJ1uYxaYUzobhic4vXZiCaHNs82pcY8AlzglFNTd2Gi-JzJoUKw_tLSv1wghUhmfrJATuY_3WMFerO8bcoCXwBXU07d96pNKxxvR8o_MEyT_5-AVAa80HpLjmXmQgcBFxeCzrZkN-s7OUBUImRQOxyAHAHhfo7cJ55qflBYnG3TxwmckCPJHUQkvYTlK4sRiJsznvPejL5ifgCnMTfoC-docqGUWsw46AXNnkxh_LBVNaiSZPQ";

        private const string SessionOrderIdKey = "Payment.OrderId";
        private const string SessionOrderAmountKey = "Payment.OrderAmount";
        private const string SessionTransactionUuidKey = "Payment.TransactionUuid";
        private const string SessionPaymentMethodKey = "Payment.Method";
        private const string SessionCustomerNameKey = "Payment.CustomerName";
        private const string SessionCustomerPhoneKey = "Payment.CustomerPhone";
        private const string SessionOrderLabelKey = "Payment.OrderLabel";
        private const string SessionPidxKey = "Payment.Pidx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!IsPostBack)
            {
                rblPaymentMethod.SelectedValue = DefaultPaymentMethod;
                PrefillFullName();
                BindCheckout(userId);
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            var state = BuildCheckoutState(userId);
            if (state.Items.Count == 0)
            {
                BindCheckout(userId);
                ShowSnackbar("Your cart is empty. Add items before placing an order.", "info");
                return;
            }

            var fullName = NormalizeText(txtFullName.Text, 100);
            var phone = NormalizePhone(txtPhone.Text);
            var deliveryAddress = NormalizeText(txtDeliveryAddress.Text, 500);
            var orderNotes = NormalizeText(txtOrderNotes.Text, 500);
            var paymentMethod = NormalizePaymentMethod(rblPaymentMethod.SelectedValue);

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowSnackbar("Please provide your full name.", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowSnackbar("Please provide your phone number.", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(deliveryAddress))
            {
                ShowSnackbar("Please provide a delivery address.", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                ShowSnackbar("Please select a payment method.", "error");
                return;
            }

            var amount = state.Subtotal + ShippingFee;
            var transactionUuid = EsewaHelper.GenerateTransactionUuid();
            var decoratedNotes = BuildOrderNotes(phone, orderNotes);

            var order = new Order
            {
                UserId = userId,
                FullName = fullName,
                DeliveryAddress = deliveryAddress,
                OrderNotes = decoratedNotes,
                PaymentMethod = paymentMethod,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                Items = state.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    SelectedSize = item.SelectedSize,
                    Quantity = item.SelectedQuantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            try
            {
                var orderId = OrderService.PlaceOrder(order);
                if (orderId <= 0)
                {
                    throw new InvalidOperationException("Unable to place order.");
                }

                var paymentId = PaymentDbHelper.InsertPayment(orderId, paymentMethod, transactionUuid, amount);
                if (paymentId <= 0)
                {
                    throw new InvalidOperationException("Unable to create payment record.");
                }

                SetPaymentSession(orderId, amount, transactionUuid, paymentMethod, fullName, phone);

                Debug.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "[Checkout] Order created orderId={0}, paymentId={1}, method={2}, tx={3}",
                    orderId,
                    paymentId,
                    paymentMethod,
                    transactionUuid));

                var paymentRedirectUrl = string.Equals(paymentMethod, "eSewa", StringComparison.OrdinalIgnoreCase)
                    ? "~/Customer/Payment/EsewaPayment/EsewaPayment.aspx"
                    : "~/Customer/Payment/KhaltiPayment/KhaltiPayment.aspx";

                Response.Redirect(paymentRedirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch
            {
                BindCheckout(userId);
                ShowSnackbar("We could not place your order right now. Please try again.", "error");
                Session["PaymentError"] = "Payment initialization failed. Please try again.";
            }
        }

        private void BindCheckout(int userId)
        {
            var state = BuildCheckoutState(userId);

            phCheckoutEmpty.Visible = state.Items.Count == 0;
            phCheckoutReady.Visible = state.Items.Count > 0;

            rptCheckoutItems.DataSource = state.Items;
            rptCheckoutItems.DataBind();

            var shipping = state.Items.Count > 0 ? ShippingFee : 0m;
            var grandTotal = state.Subtotal + shipping;

            litSubtotal.Text = FormatCurrency(state.Subtotal);
            litShipping.Text = FormatCurrency(shipping);
            litGrandTotal.Text = FormatCurrency(grandTotal);
        }

        private CheckoutState BuildCheckoutState(int userId)
        {
            var cartRows = LoadCartRows(userId);
            var productsById = LoadProductsById();

            var items = cartRows
                .Select(cartRow => BuildCheckoutItemViewModel(cartRow, productsById))
                .Where(item => item != null)
                .ToList();

            return new CheckoutState
            {
                Items = items,
                Subtotal = items.Sum(item => item.ItemTotal)
            };
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

        private CheckoutItemViewModel BuildCheckoutItemViewModel(CartRecord cartRow, IDictionary<int, Product> productsById)
        {
            if (cartRow == null || cartRow.ProductId <= 0 || cartRow.SelectedQuantity <= 0)
            {
                return null;
            }

            productsById.TryGetValue(cartRow.ProductId, out var product);

            var productName = string.IsNullOrWhiteSpace(product == null ? null : product.ProductName)
                ? string.Format(CultureInfo.InvariantCulture, "Product #{0}", cartRow.ProductId)
                : product.ProductName.Trim();
            var selectedSize = string.IsNullOrWhiteSpace(cartRow.SelectedSize)
                ? "M"
                : cartRow.SelectedSize.Trim().ToUpperInvariant();
            var unitPrice = product == null ? 0m : product.Price;
            var quantity = cartRow.SelectedQuantity;
            var itemTotal = unitPrice * quantity;

            return new CheckoutItemViewModel
            {
                ProductId = cartRow.ProductId,
                ProductName = productName,
                SelectedSize = selectedSize,
                SelectedQuantity = quantity,
                UnitPrice = unitPrice,
                ItemTotal = itemTotal,
                ItemTotalLabel = FormatCurrency(itemTotal),
                DetailLabel = string.Format(CultureInfo.InvariantCulture, "Qty: {0:00} - Size: {1}", quantity, selectedSize),
                ImageUrl = ResolveProductImageUrl(product == null ? null : product.ImagePath),
                ImageAlt = productName
            };
        }

        private void PrefillFullName()
        {
            if (!string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                return;
            }

            var user = Session["CurrentUser"] as User;
            if (user != null && !string.IsNullOrWhiteSpace(user.FullName))
            {
                txtFullName.Text = user.FullName.Trim();
                return;
            }

            var userName = Convert.ToString(Session["UserName"], CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(userName))
            {
                txtFullName.Text = userName.Trim();
            }
        }

        private static string BuildOrderNotes(string phone, string orderNotes)
        {
            if (string.IsNullOrWhiteSpace(orderNotes))
            {
                return string.Format(CultureInfo.InvariantCulture, "Phone: {0}", phone);
            }

            return string.Format(CultureInfo.InvariantCulture, "Phone: {0} | Notes: {1}", phone, orderNotes);
        }

        private void SetPaymentSession(int orderId, decimal amount, string transactionUuid, string paymentMethod, string customerName, string customerPhone)
        {
            Session[SessionOrderIdKey] = orderId;
            Session[SessionOrderAmountKey] = amount;
            Session[SessionTransactionUuidKey] = transactionUuid;
            Session[SessionPaymentMethodKey] = paymentMethod;
            Session[SessionCustomerNameKey] = customerName;
            Session[SessionCustomerPhoneKey] = customerPhone;
            Session[SessionOrderLabelKey] = string.Format(CultureInfo.InvariantCulture, "AttireZone Order #{0}", orderId);
            Session[SessionPidxKey] = null;
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

        private static string NormalizeText(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var normalized = input.Trim();
            if (normalized.Length > maxLength)
            {
                normalized = normalized.Substring(0, maxLength);
            }

            return normalized;
        }

        private static string NormalizePaymentMethod(string rawPaymentMethod)
        {
            if (string.IsNullOrWhiteSpace(rawPaymentMethod))
            {
                return null;
            }

            var normalized = rawPaymentMethod.Trim();

            if (string.Equals(normalized, "eSewa", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Esewa", StringComparison.OrdinalIgnoreCase))
            {
                return "eSewa";
            }

            if (string.Equals(normalized, "Khalti", StringComparison.OrdinalIgnoreCase))
            {
                return "Khalti";
            }

            return null;
        }

        private static string NormalizePhone(string rawPhone)
        {
            if (string.IsNullOrWhiteSpace(rawPhone))
            {
                return string.Empty;
            }

            var trimmed = rawPhone.Trim();
            if (trimmed.Length > 20)
            {
                trimmed = trimmed.Substring(0, 20);
            }

            return trimmed;
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
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Customer/Checkout.aspx"));
            Response.Redirect("~/Auth/Login.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowSnackbar(string message, string type)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            var script = string.Format(
                CultureInfo.InvariantCulture,
                "window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} else {{ alert('{0}'); }} }}, 0);",
                safeMessage,
                safeType);

            ScriptManager.RegisterStartupScript(this, GetType(), "checkoutSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class CheckoutState
        {
            public List<CheckoutItemViewModel> Items { get; set; }

            public decimal Subtotal { get; set; }
        }

        private sealed class CheckoutItemViewModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string SelectedSize { get; set; }

            public int SelectedQuantity { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal ItemTotal { get; set; }

            public string ItemTotalLabel { get; set; }

            public string DetailLabel { get; set; }

            public string ImageUrl { get; set; }

            public string ImageAlt { get; set; }
        }
    }
}