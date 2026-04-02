using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using AttireZone_Web_App.Helpers;

namespace AttireZone_Web_App
{
    public partial class EsewaPayment : Page
    {
        private const string SessionOrderIdKey = "Payment.OrderId";
        private const string SessionOrderAmountKey = "Payment.OrderAmount";
        private const string SessionTransactionUuidKey = "Payment.TransactionUuid";
        private const string SessionPaymentMethodKey = "Payment.Method";

        protected string PaymentUrl { get; private set; }

        protected string AmountText { get; private set; }

        protected string TransactionUuid { get; private set; }

        protected string ProductCode { get; private set; }

        protected string Signature { get; private set; }

        protected string SuccessUrl { get; private set; }

        protected string FailureUrl { get; private set; }

        protected bool ShowError { get; private set; }

        protected string ErrorMessage { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeEsewaForm();
            }
        }

        private void InitializeEsewaForm()
        {
            try
            {
                var paymentMethod = Convert.ToString(Session[SessionPaymentMethodKey], CultureInfo.InvariantCulture);
                if (!string.Equals(paymentMethod, "eSewa", StringComparison.OrdinalIgnoreCase))
                {
                    ShowErrorState("eSewa payment session is missing or expired.");
                    return;
                }

                var orderId = Convert.ToInt32(Session[SessionOrderIdKey] ?? 0, CultureInfo.InvariantCulture);
                var orderAmount = Convert.ToDecimal(Session[SessionOrderAmountKey] ?? 0m, CultureInfo.InvariantCulture);
                var transactionUuid = Convert.ToString(Session[SessionTransactionUuidKey], CultureInfo.InvariantCulture);

                if (orderId <= 0 || orderAmount <= 0m || string.IsNullOrWhiteSpace(transactionUuid))
                {
                    ShowErrorState("Order payment details are incomplete.");
                    return;
                }

                var secretKey = ConfigurationManager.AppSettings["eSewa:SecretKey"];
                ProductCode = ConfigurationManager.AppSettings["eSewa:ProductCode"];
                PaymentUrl = ConfigurationManager.AppSettings["eSewa:PaymentUrl"];

                if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(ProductCode) || string.IsNullOrWhiteSpace(PaymentUrl))
                {
                    ShowErrorState("eSewa configuration is missing.");
                    return;
                }

                AmountText = orderAmount.ToString("0.00", CultureInfo.InvariantCulture);
                TransactionUuid = transactionUuid.Trim();

                var signatureMessage = EsewaHelper.BuildPaymentSignatureMessage(orderAmount, TransactionUuid, ProductCode);
                Signature = EsewaHelper.GenerateSignature(signatureMessage, secretKey);

                SuccessUrl = BuildAbsoluteUrl("~/Customer/Payment/EsewaPayment/EsewaSuccess.aspx");
                FailureUrl = BuildAbsoluteUrl("~/Customer/Payment/EsewaPayment/EsewaFailure.aspx");

                Debug.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "[eSewa] Prepared payment form orderId={0}, tx={1}, amount={2}",
                    orderId,
                    TransactionUuid,
                    AmountText));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[eSewa] InitializeEsewaForm failed => " + ex.Message);
                ShowErrorState("Unable to initialize eSewa payment at this moment.");
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
