using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AttireZone_Web_App.Helpers
{
    public static class KhaltiHelper
    {
        private const string PlaceholderSecret = "PASTE_TEST_ADMIN_KHALTI_SECRET_KEY_HERE";

        private static string SecretKey => ConfigurationManager.AppSettings["Khalti:SecretKey"];
        private static string InitiateUrl => ConfigurationManager.AppSettings["Khalti:InitiateUrl"];
        private static string LookupUrl => ConfigurationManager.AppSettings["Khalti:LookupUrl"];

        public static KhaltiInitiateResult InitiatePayment(
            int orderId,
            string orderName,
            long amountInPaisa,
            string returnUrl,
            string websiteUrl,
            string customerName,
            string customerPhone)
        {
            try
            {
                var payload = new JObject
                {
                    ["return_url"] = returnUrl,
                    ["website_url"] = websiteUrl,
                    ["amount"] = amountInPaisa,
                    ["purchase_order_id"] = orderId.ToString(CultureInfo.InvariantCulture),
                    ["purchase_order_name"] = string.IsNullOrWhiteSpace(orderName) ? "AttireZone Order" : orderName
                };

                var customerInfo = new JObject();
                if (!string.IsNullOrWhiteSpace(customerName))
                {
                    customerInfo["name"] = customerName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(customerPhone))
                {
                    customerInfo["phone"] = customerPhone.Trim();
                }

                if (customerInfo.HasValues)
                {
                    payload["customer_info"] = customerInfo;
                }

                var responseBody = PostJson(InitiateUrl, payload.ToString(Formatting.None));
                var json = JObject.Parse(responseBody ?? "{}");

                var pidx = Convert.ToString(json["pidx"], CultureInfo.InvariantCulture);
                var paymentUrl = Convert.ToString(json["payment_url"], CultureInfo.InvariantCulture);

                Debug.WriteLine("[Khalti] Initiate success pidx=" + pidx);

                return new KhaltiInitiateResult
                {
                    IsSuccess = !string.IsNullOrWhiteSpace(pidx) && !string.IsNullOrWhiteSpace(paymentUrl),
                    Pidx = pidx,
                    PaymentUrl = paymentUrl,
                    RawResponse = responseBody
                };
            }
            catch (WebException webException)
            {
                var errorMessage = ReadWebException(webException);
                Debug.WriteLine("[Khalti] Initiate web exception => " + errorMessage);
                return new KhaltiInitiateResult { IsSuccess = false, ErrorMessage = errorMessage };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Khalti] Initiate exception => " + ex.Message);
                return new KhaltiInitiateResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public static KhaltiLookupResult LookupPayment(string pidx)
        {
            try
            {
                var payload = new JObject
                {
                    ["pidx"] = pidx
                };

                var responseBody = PostJson(LookupUrl, payload.ToString(Formatting.None));
                var json = JObject.Parse(responseBody ?? "{}");

                var status = Convert.ToString(json["status"], CultureInfo.InvariantCulture);
                var totalAmount = json["total_amount"] == null ? 0L : Convert.ToInt64(json["total_amount"], CultureInfo.InvariantCulture);
                var transactionId = Convert.ToString(json["transaction_id"], CultureInfo.InvariantCulture);

                Debug.WriteLine("[Khalti] Lookup success pidx=" + pidx + ", status=" + status);

                return new KhaltiLookupResult
                {
                    IsSuccess = true,
                    Status = status,
                    TotalAmount = totalAmount,
                    TransactionId = transactionId,
                    RawResponse = responseBody
                };
            }
            catch (WebException webException)
            {
                var errorMessage = ReadWebException(webException);
                Debug.WriteLine("[Khalti] Lookup web exception => " + errorMessage);
                return new KhaltiLookupResult { IsSuccess = false, ErrorMessage = errorMessage };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Khalti] Lookup exception => " + ex.Message);
                return new KhaltiLookupResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private static string PostJson(string url, string requestBody)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException("Khalti URL is not configured.");
            }

            var normalizedSecretKey = GetNormalizedSecretKey();

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers["Authorization"] = "key " + normalizedSecretKey;

            var bytes = Encoding.UTF8.GetBytes(requestBody ?? "{}");
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream ?? Stream.Null))
            {
                return reader.ReadToEnd();
            }
        }

        private static string ReadWebException(WebException webException)
        {
            if (webException == null)
            {
                return "Unknown network error";
            }

            try
            {
                using (var response = webException.Response as HttpWebResponse)
                using (var stream = response == null ? null : response.GetResponseStream())
                using (var reader = stream == null ? null : new StreamReader(stream))
                {
                    var responseBody = reader == null ? string.Empty : reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(responseBody))
                    {
                        if (response != null &&
                            response.StatusCode == HttpStatusCode.Unauthorized &&
                            responseBody.IndexOf("Invalid token", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return "Khalti authentication failed (Invalid token). Set Web.config appSettings Khalti:SecretKey to your sandbox key from https://test-admin.khalti.com and save only the raw token (without the 'key ' prefix).";
                        }

                        return responseBody;
                    }
                }
            }
            catch
            {
                // Ignore and fall back to exception message.
            }

            return webException.Message;
        }

        private static string GetNormalizedSecretKey()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                throw new InvalidOperationException("Khalti secret key is not configured. Set appSettings Khalti:SecretKey in Web.config.");
            }

            var normalized = SecretKey.Trim().Trim('"', '\'');
            if (string.Equals(normalized, PlaceholderSecret, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Khalti secret key is still placeholder text. Replace Khalti:SecretKey in Web.config with your sandbox key from https://test-admin.khalti.com.");
            }

            if (normalized.StartsWith("key ", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(4).Trim();
            }

            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidOperationException("Khalti secret key is empty after normalization. Set a valid sandbox key from https://test-admin.khalti.com.");
            }

            return normalized;
        }

        public sealed class KhaltiInitiateResult
        {
            public bool IsSuccess { get; set; }

            public string Pidx { get; set; }

            public string PaymentUrl { get; set; }

            public string RawResponse { get; set; }

            public string ErrorMessage { get; set; }
        }

        public sealed class KhaltiLookupResult
        {
            public bool IsSuccess { get; set; }

            public string Status { get; set; }

            public long TotalAmount { get; set; }

            public string TransactionId { get; set; }

            public string RawResponse { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}
