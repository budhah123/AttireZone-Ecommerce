using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using AttireZone_Web_App.Helpers;

namespace AttireZone_Web_App
{
    public partial class KhaltiPayment : Page
    {
        private const string SessionOrderIdKey = "Payment.OrderId";
        private const string SessionOrderAmountKey = "Payment.OrderAmount";
        private const string SessionTransactionUuidKey = "Payment.TransactionUuid";
        private const string SessionPaymentMethodKey = "Payment.Method";
        private const string SessionCustomerNameKey = "Payment.CustomerName";
        private const string SessionCustomerPhoneKey = "Payment.CustomerPhone";
        private const string SessionOrderLabelKey = "Payment.OrderLabel";
        private const string SessionPidxKey = "Payment.Pidx";

        protected bool ShowError { get; private set; }

        protected string ErrorMessage { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitiateKhalti();
            }
        }

        private void InitiateKhalti()
        {
            try
            {
                var paymentMethod = Convert.ToString(Session[SessionPaymentMethodKey], CultureInfo.InvariantCulture);
                if (!string.Equals(paymentMethod, "Khalti", StringComparison.OrdinalIgnoreCase))
                {
                    ShowErrorState("Khalti payment session is missing or expired.");
                    return;
                }

                var orderId = Convert.ToInt32(Session[SessionOrderIdKey] ?? 0, CultureInfo.InvariantCulture);
                var orderAmount = Convert.ToDecimal(Session[SessionOrderAmountKey] ?? 0m, CultureInfo.InvariantCulture);
                var transactionUuid = Convert.ToString(Session[SessionTransactionUuidKey], CultureInfo.InvariantCulture);
                var customerName = Convert.ToString(Session[SessionCustomerNameKey], CultureInfo.InvariantCulture);
                var customerPhone = Convert.ToString(Session[SessionCustomerPhoneKey], CultureInfo.InvariantCulture);
                var orderLabel = Convert.ToString(Session[SessionOrderLabelKey], CultureInfo.InvariantCulture);

                if (orderId <= 0 || orderAmount <= 0m || string.IsNullOrWhiteSpace(transactionUuid))
                {
                    ShowErrorState("Order payment details are incomplete.");
                    return;
                }

                var amountInPaisa = Convert.ToInt64(decimal.Round(orderAmount * 100m, 0, MidpointRounding.AwayFromZero), CultureInfo.InvariantCulture);
                var returnUrl = BuildAbsoluteUrl("~/Customer/Payment/KhaltiPayment/KhaltiCallBack.aspx");
                var websiteUrl = BuildAbsoluteUrl("~/");

                var initiateResult = KhaltiHelper.InitiatePayment(
                    orderId,
                    string.IsNullOrWhiteSpace(orderLabel) ? ("AttireZone Order #" + orderId.ToString(CultureInfo.InvariantCulture)) : orderLabel,
                    amountInPaisa,
                    returnUrl,
                    websiteUrl,
                    customerName,
                    customerPhone);

                if (!initiateResult.IsSuccess)
                {
                    ShowErrorState(string.IsNullOrWhiteSpace(initiateResult.ErrorMessage)
                        ? "Khalti initiate API returned an unknown error."
                        : initiateResult.ErrorMessage);
                    return;
                }

                Session[SessionPidxKey] = initiateResult.Pidx;
                PaymentDbHelper.UpdatePaymentStatus(transactionUuid, "Pending", initiateResult.Pidx, initiateResult.RawResponse);

                Debug.WriteLine("[Khalti] Redirecting to payment_url for orderId=" + orderId + ", pidx=" + initiateResult.Pidx);

                Response.Redirect(initiateResult.PaymentUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Khalti] InitiateKhalti failed => " + ex.Message);
                ShowErrorState("Unable to initialize Khalti payment at this moment.");
            }
        }

        private string BuildAbsoluteUrl(string relativePath)
        {
            var authority = Request.Url == null ? string.Empty : Request.Url.GetLeftPart(UriPartial.Authority);
            return authority + ResolveUrl(relativePath);
        }

        private void ShowErrorState(string message)
        {
            ShowError = true;
            ErrorMessage = message;
            Session["PaymentError"] = message;
        }
    }
}
