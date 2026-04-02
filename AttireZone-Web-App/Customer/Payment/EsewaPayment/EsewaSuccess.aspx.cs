using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AttireZone_Web_App
{
    public partial class EsewaSuccess : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessSuccessCallback();
        }

        private void ProcessSuccessCallback()
        {
            var encodedData = Request.QueryString["data"];
            if (string.IsNullOrWhiteSpace(encodedData))
            {
                FailFlow("eSewa callback did not include payment data.", null, null, null);
                return;
            }

            JObject callbackJson;
            string transactionUuid = null;
            string transactionCode = null;
            decimal callbackAmount = 0m;

            try
            {
                var jsonText = DecodeBase64Payload(encodedData);
                callbackJson = JObject.Parse(jsonText);

                transactionUuid = Convert.ToString(callbackJson["transaction_uuid"], CultureInfo.InvariantCulture);
                transactionCode = Convert.ToString(callbackJson["transaction_code"], CultureInfo.InvariantCulture);
                callbackAmount = ParseDecimal(Convert.ToString(callbackJson["total_amount"], CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[eSewa] Invalid callback payload => " + ex.Message);
                FailFlow("Unable to parse eSewa callback response.", null, null, null);
                return;
            }

            var paymentRecord = PaymentDbHelper.GetPaymentByTransactionUuid(transactionUuid);
            if (paymentRecord == null)
            {
                FailFlow("Payment record not found for the returned transaction.", transactionUuid, transactionCode, callbackJson);
                return;
            }

            if (string.Equals(paymentRecord.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                RedirectToSuccess(paymentRecord.OrderId, paymentRecord.PaymentMethod, paymentRecord.GatewayTransactionId);
                return;
            }

            var callbackStatus = Convert.ToString(callbackJson["status"], CultureInfo.InvariantCulture);
            var productCode = Convert.ToString(callbackJson["product_code"], CultureInfo.InvariantCulture);
            var signedFieldNames = Convert.ToString(callbackJson["signed_field_names"], CultureInfo.InvariantCulture);
            var receivedSignature = Convert.ToString(callbackJson["signature"], CultureInfo.InvariantCulture);
            var secretKey = ConfigurationManager.AppSettings["eSewa:SecretKey"];

            var signatureFields = BuildSignatureDictionary(callbackJson);
            var signatureValid = EsewaHelper.VerifyResponseSignature(
                signedFieldNames,
                signatureFields,
                receivedSignature,
                secretKey);

            if (!signatureValid)
            {
                FailFlow("eSewa signature verification failed.", transactionUuid, transactionCode, callbackJson, paymentRecord.OrderId);
                return;
            }

            var statusCheckResult = VerifyWithStatusApi(productCode, callbackAmount, transactionUuid);
            var isCallbackComplete = string.Equals(callbackStatus, "COMPLETE", StringComparison.OrdinalIgnoreCase);
            var isStatusApiComplete = string.Equals(statusCheckResult.Status, "COMPLETE", StringComparison.OrdinalIgnoreCase);
            var expectedAmount = paymentRecord.Amount;
            var amountMatches = Math.Abs(expectedAmount - callbackAmount) <= 0.01m;

            if (isCallbackComplete && isStatusApiComplete && amountMatches)
            {
                var mergedResponse = new JObject
                {
                    ["callback"] = callbackJson,
                    ["statusApi"] = statusCheckResult.RawJson
                };

                PaymentDbHelper.UpdatePaymentStatus(
                    transactionUuid,
                    "Completed",
                    string.IsNullOrWhiteSpace(transactionCode) ? statusCheckResult.ReferenceId : transactionCode,
                    mergedResponse.ToString(Formatting.None));
                PaymentDbHelper.UpdateOrderPaymentStatus(paymentRecord.OrderId, "Paid");

                TryClearCart();
                ClearPaymentSession();

                Debug.WriteLine("[eSewa] Payment completed tx=" + transactionUuid);
                RedirectToSuccess(paymentRecord.OrderId, "eSewa", string.IsNullOrWhiteSpace(transactionCode) ? statusCheckResult.ReferenceId : transactionCode);
                return;
            }

            var failureReason = string.Format(
                CultureInfo.InvariantCulture,
                "Payment verification failed. callbackStatus={0}, statusApi={1}, amountMatches={2}",
                callbackStatus,
                statusCheckResult.Status,
                amountMatches);

            FailFlow(failureReason, transactionUuid, transactionCode, callbackJson, paymentRecord.OrderId, statusCheckResult.RawJson);
        }

        private static Dictionary<string, string> BuildSignatureDictionary(JObject callbackJson)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (callbackJson == null)
            {
                return map;
            }

            foreach (var property in callbackJson.Properties())
            {
                map[property.Name] = Convert.ToString(property.Value, CultureInfo.InvariantCulture) ?? string.Empty;
            }

            return map;
        }

        private StatusCheckResult VerifyWithStatusApi(string productCode, decimal totalAmount, string transactionUuid)
        {
            var statusApiBase = ConfigurationManager.AppSettings["eSewa:StatusUrl"];
            var requestUrl = string.Format(
                CultureInfo.InvariantCulture,
                "{0}?product_code={1}&total_amount={2}&transaction_uuid={3}",
                statusApiBase,
                HttpUtility.UrlEncode(productCode),
                HttpUtility.UrlEncode(totalAmount.ToString("0.00", CultureInfo.InvariantCulture)),
                HttpUtility.UrlEncode(transactionUuid));

            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "GET";
            request.Accept = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? Stream.Null))
            {
                var responseText = reader.ReadToEnd();
                var json = JObject.Parse(string.IsNullOrWhiteSpace(responseText) ? "{}" : responseText);
                var status = Convert.ToString(json["status"], CultureInfo.InvariantCulture);
                var referenceId = Convert.ToString(json["transaction_code"], CultureInfo.InvariantCulture);
                if (string.IsNullOrWhiteSpace(referenceId))
                {
                    referenceId = Convert.ToString(json["ref_id"], CultureInfo.InvariantCulture);
                }

                return new StatusCheckResult
                {
                    Status = status,
                    ReferenceId = referenceId,
                    RawJson = json
                };
            }
        }

        private static string DecodeBase64Payload(string encodedData)
        {
            var normalized = (encodedData ?? string.Empty).Trim().Replace(" ", "+");
            var bytes = Convert.FromBase64String(normalized);
            return Encoding.UTF8.GetString(bytes);
        }

        private static decimal ParseDecimal(string rawAmount)
        {
            if (string.IsNullOrWhiteSpace(rawAmount))
            {
                return 0m;
            }

            decimal.TryParse(rawAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out var value);
            return value;
        }

        private void FailFlow(
            string errorMessage,
            string transactionUuid,
            string gatewayTransactionId,
            JObject callbackJson,
            int orderId = 0,
            JObject statusJson = null)
        {
            Debug.WriteLine("[eSewa] FailFlow => " + errorMessage);

            if (!string.IsNullOrWhiteSpace(transactionUuid))
            {
                JObject merged = new JObject();
                if (callbackJson != null)
                {
                    merged["callback"] = callbackJson;
                }

                if (statusJson != null)
                {
                    merged["statusApi"] = statusJson;
                }

                PaymentDbHelper.UpdatePaymentStatus(
                    transactionUuid,
                    "Failed",
                    gatewayTransactionId,
                    merged.HasValues ? merged.ToString(Formatting.None) : null);
            }

            var resolvedOrderId = orderId;
            if (resolvedOrderId <= 0 && !string.IsNullOrWhiteSpace(transactionUuid))
            {
                var payment = PaymentDbHelper.GetPaymentByTransactionUuid(transactionUuid);
                if (payment != null)
                {
                    resolvedOrderId = payment.OrderId;
                }
            }

            if (resolvedOrderId > 0)
            {
                PaymentDbHelper.UpdateOrderPaymentStatus(resolvedOrderId, "Failed");
            }

            Session["PaymentError"] = errorMessage;
            ClearPaymentSession();
            Response.Redirect("~/OrderFailed.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void RedirectToSuccess(int orderId, string paymentMethod, string transactionId)
        {
            var url = string.Format(
                CultureInfo.InvariantCulture,
                "~/OrderSuccess.aspx?orderId={0}&method={1}&txId={2}",
                orderId,
                HttpUtility.UrlEncode(paymentMethod ?? string.Empty),
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

        private sealed class StatusCheckResult
        {
            public string Status { get; set; }

            public string ReferenceId { get; set; }

            public JObject RawJson { get; set; }
        }
    }
}
