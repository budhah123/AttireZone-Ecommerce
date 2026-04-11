using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Helpers;

namespace AttireZone_Web_App
{
    public partial class KhaltiCallback : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessKhaltiCallback();
            }
        }

        private void ProcessKhaltiCallback()
        {
            var pidx = Request.QueryString["pidx"];
            var callbackStatus = Request.QueryString["status"];
            var purchaseOrderIdRaw = Request.QueryString["purchase_order_id"];
            var tidx = Request.QueryString["tidx"];

            Debug.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                "[Khalti] Callback received pidx={0}, status={1}, purchase_order_id={2}, tidx={3}",
                pidx,
                callbackStatus,
                purchaseOrderIdRaw,
                tidx));

            var payment = ResolvePaymentRecord(purchaseOrderIdRaw);
            if (payment == null)
            {
                FailAndRedirect("Unable to locate payment record for Khalti callback.", null, tidx, BuildCallbackSnapshot());
                return;
            }

            if (string.Equals(payment.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                RedirectToSuccess(payment.OrderId, "Khalti", payment.GatewayTransactionId);
                return;
            }

            if (string.Equals(callbackStatus, "User canceled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(callbackStatus, "Cancelled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(callbackStatus, "Canceled", StringComparison.OrdinalIgnoreCase))
            {
                FailAndRedirect("Khalti payment was canceled by the user.", payment, tidx, BuildCallbackSnapshot());
                return;
            }

            if (string.IsNullOrWhiteSpace(pidx))
            {
                FailAndRedirect("Khalti callback did not include pidx.", payment, tidx, BuildCallbackSnapshot());
                return;
            }

            var lookupResult = KhaltiHelper.LookupPayment(pidx);
            if (!lookupResult.IsSuccess)
            {
                FailAndRedirect(
                    string.IsNullOrWhiteSpace(lookupResult.ErrorMessage)
                        ? "Khalti lookup failed."
                        : lookupResult.ErrorMessage,
                    payment,
                    tidx,
                    lookupResult.RawResponse);
                return;
            }

            if (string.Equals(lookupResult.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                PaymentDbHelper.UpdatePaymentStatus(payment.TransactionUuid, "Pending", pidx, lookupResult.RawResponse);
                litStatusMessage.Text = "Your payment is still pending confirmation. Please refresh after a short while.";
                return;
            }

            var expectedPaisa = Convert.ToInt64(decimal.Round(payment.Amount * 100m, 0, MidpointRounding.AwayFromZero), CultureInfo.InvariantCulture);
            if (lookupResult.TotalAmount != expectedPaisa)
            {
                FailAndRedirect(
                    string.Format(CultureInfo.InvariantCulture, "Amount mismatch detected. Expected {0} paisa, received {1} paisa.", expectedPaisa, lookupResult.TotalAmount),
                    payment,
                    tidx,
                    lookupResult.RawResponse);
                return;
            }

            if (string.Equals(lookupResult.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                var gatewayTxId = string.IsNullOrWhiteSpace(lookupResult.TransactionId) ? tidx : lookupResult.TransactionId;
                PaymentDbHelper.UpdatePaymentStatus(payment.TransactionUuid, "Completed", gatewayTxId, lookupResult.RawResponse);
                PaymentDbHelper.UpdateOrderPaymentStatus(payment.OrderId, "Paid");

                var emailResult = OrderEmailNotificationService.SendOrderConfirmationEmail(
                    payment.OrderId,
                    payment.Amount,
                    "Khalti",
                    "Successful",
                    gatewayTxId);
                if (!emailResult.IsSuccess)
                {
                    Debug.WriteLine("[OrderEmail] Khalti orderId=" + payment.OrderId + " send failed. " + emailResult.ErrorMessage);
                    Session["PaymentEmailWarning"] = BuildEmailWarningMessage(emailResult.ErrorMessage);
                }
                else
                {
                    Session["PaymentEmailWarning"] = null;
                }

                TryClearCart();
                ClearPaymentSession();

                RedirectToSuccess(payment.OrderId, "Khalti", gatewayTxId);
                return;
            }

            FailAndRedirect(
                "Khalti payment was not completed. Current status: " + lookupResult.Status,
                payment,
                tidx,
                lookupResult.RawResponse);
        }

        private string BuildEmailWarningMessage(string emailError)
        {
            const string genericMessage = "Payment was successful, but we could not send your confirmation email right now.";

            if (Context == null || !Context.IsDebuggingEnabled || string.IsNullOrWhiteSpace(emailError))
            {
                return genericMessage;
            }

            return genericMessage + " Reason: " + emailError.Trim();
        }

        private PaymentDbHelper.PaymentRecord ResolvePaymentRecord(string purchaseOrderIdRaw)
        {
            var transactionUuid = Convert.ToString(Session["Payment.TransactionUuid"], CultureInfo.InvariantCulture);
            if (!string.IsNullOrWhiteSpace(transactionUuid))
            {
                var byTransaction = PaymentDbHelper.GetPaymentByTransactionUuid(transactionUuid);
                if (byTransaction != null)
                {
                    return byTransaction;
                }
            }

            if (int.TryParse(purchaseOrderIdRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderIdFromQuery) && orderIdFromQuery > 0)
            {
                return PaymentDbHelper.GetLatestPaymentByOrderId(orderIdFromQuery);
            }

            var orderIdFromSession = Convert.ToInt32(Session["Payment.OrderId"] ?? 0, CultureInfo.InvariantCulture);
            if (orderIdFromSession > 0)
            {
                return PaymentDbHelper.GetLatestPaymentByOrderId(orderIdFromSession);
            }

            return null;
        }

        private string BuildCallbackSnapshot()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "pidx={0};status={1};purchase_order_id={2};total_amount={3};tidx={4}",
                Request.QueryString["pidx"],
                Request.QueryString["status"],
                Request.QueryString["purchase_order_id"],
                Request.QueryString["total_amount"],
                Request.QueryString["tidx"]);
        }

        private void FailAndRedirect(string errorMessage, PaymentDbHelper.PaymentRecord payment, string gatewayTransactionId, string gatewayResponse)
        {
            Debug.WriteLine("[Khalti] FailAndRedirect => " + errorMessage);

            if (payment != null)
            {
                PaymentDbHelper.UpdatePaymentStatus(payment.TransactionUuid, "Failed", gatewayTransactionId, gatewayResponse);
                PaymentDbHelper.UpdateOrderPaymentStatus(payment.OrderId, "Failed");
            }

            Session["PaymentError"] = errorMessage;
            ClearPaymentSession();
            Response.Redirect("~/Customer/Payment/KhaltiPayment/KhaltiFailure.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void RedirectToSuccess(int orderId, string method, string transactionId)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "~/Customer/Payment/KhaltiPayment/KhaltiSuccess.aspx?orderId={0}&method={1}&txId={2}",
                orderId,
                HttpUtility.UrlEncode(method ?? string.Empty),
                HttpUtility.UrlEncode(transactionId ?? string.Empty));

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void TryClearCart()
        {
            var sessionUserId = Session["UserId"];
            if (sessionUserId == null)
            {
                return;
            }

            if (sessionUserId is int directUserId && directUserId > 0)
            {
                CartService.ClearCart(directUserId);
                return;
            }

            if (int.TryParse(Convert.ToString(sessionUserId, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId) && userId > 0)
            {
                CartService.ClearCart(userId);
            }
        }

        private void ClearPaymentSession()
        {
            Session["Payment.OrderId"] = null;
            Session["Payment.OrderAmount"] = null;
            Session["Payment.TransactionUuid"] = null;
            Session["Payment.Method"] = null;
            Session["Payment.CustomerName"] = null;
            Session["Payment.CustomerPhone"] = null;
            Session["Payment.OrderLabel"] = null;
            Session["Payment.Pidx"] = null;
        }
    }
}
