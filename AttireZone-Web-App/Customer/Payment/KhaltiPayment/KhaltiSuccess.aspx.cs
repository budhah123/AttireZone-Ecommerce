using System;
using System.Globalization;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;

namespace AttireZone_Web_App
{
    public partial class KhaltiSuccess : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSuccessData();
                BindEmailNotice();
                TryClearCart();
                ClearPaymentSession();
            }
        }

        private void BindSuccessData()
        {
            var transactionId = Request.QueryString["txId"];
            var paymentMethod = Request.QueryString["method"];

            litTransactionId.Text = string.IsNullOrWhiteSpace(transactionId) ? "N/A" : transactionId.Trim();
            litPaymentMethod.Text = string.IsNullOrWhiteSpace(paymentMethod) ? "Khalti" : paymentMethod.Trim();
        }

        private void BindEmailNotice()
        {
            var warning = Convert.ToString(Session["PaymentEmailWarning"], CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(warning))
            {
                phEmailNotice.Visible = false;
                litEmailNotice.Text = string.Empty;
                return;
            }

            phEmailNotice.Visible = true;
            litEmailNotice.Text = Server.HtmlEncode(warning.Trim());
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
            Session["PaymentError"] = null;
            Session["PaymentEmailWarning"] = null;
        }
    }
}
