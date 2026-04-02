using System;
using System.Globalization;
using System.Web.UI;

namespace AttireZone_Web_App
{
    public partial class OrderFailed : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var rawError = Convert.ToString(Session["PaymentError"], CultureInfo.InvariantCulture);
                litFriendlyError.Text = MapFriendlyMessage(rawError);
            }
        }

        private static string MapFriendlyMessage(string rawError)
        {
            if (string.IsNullOrWhiteSpace(rawError))
            {
                return "We could not complete your payment. Please try again.";
            }

            var normalized = rawError.Trim().ToLowerInvariant();

            if (normalized.Contains("signature"))
            {
                return "Payment verification failed due to signature mismatch. Please try again.";
            }

            if (normalized.Contains("amount mismatch"))
            {
                return "The gateway returned an unexpected amount. Please retry payment.";
            }

            if (normalized.Contains("canceled") || normalized.Contains("cancelled"))
            {
                return "Payment was cancelled before completion.";
            }

            if (normalized.Contains("timeout") || normalized.Contains("network") || normalized.Contains("lookup"))
            {
                return "Payment gateway is temporarily unavailable. Please try again in a moment.";
            }

            return rawError;
        }
    }
}
