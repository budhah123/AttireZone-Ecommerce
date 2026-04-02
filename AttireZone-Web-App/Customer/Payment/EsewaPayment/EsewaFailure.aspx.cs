using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using AttireZone_Web_App.Helpers;

namespace AttireZone_Web_App
{
    public partial class EsewaFailure : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HandleFailure();
            }
        }

        private void HandleFailure()
        {
            var transactionUuid = Request.QueryString["transaction_uuid"];
            if (string.IsNullOrWhiteSpace(transactionUuid))
            {
                transactionUuid = Convert.ToString(Session["Payment.TransactionUuid"], CultureInfo.InvariantCulture);
            }

            var message = "Your eSewa payment was cancelled or could not be completed.";

            if (!string.IsNullOrWhiteSpace(transactionUuid))
            {
                var payment = PaymentDbHelper.GetPaymentByTransactionUuid(transactionUuid);
                if (payment != null)
                {
                    PaymentDbHelper.UpdatePaymentStatus(transactionUuid, "Failed", payment.GatewayTransactionId, payment.GatewayResponse);
                    PaymentDbHelper.UpdateOrderPaymentStatus(payment.OrderId, "Failed");
                }

                Debug.WriteLine("[eSewa] Failure callback tx=" + transactionUuid);
            }

            Session["PaymentError"] = message;
            litFailureMessage.Text = message;
        }
    }
}
