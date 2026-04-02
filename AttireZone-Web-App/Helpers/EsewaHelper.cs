using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AttireZone_Web_App.Helpers
{
    public static class EsewaHelper
    {
        public static string GenerateSignature(string message, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("A message is required.", nameof(message));
            }

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentException("A secret key is required.", nameof(secretKey));
            }

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string BuildPaymentSignatureMessage(decimal totalAmount, string transactionUuid, string productCode)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "total_amount={0},transaction_uuid={1},product_code={2}",
                totalAmount.ToString("0.00", CultureInfo.InvariantCulture),
                (transactionUuid ?? string.Empty).Trim(),
                (productCode ?? string.Empty).Trim());
        }

        public static string BuildResponseSignatureMessage(string signedFieldNames, IDictionary<string, string> fields)
        {
            if (string.IsNullOrWhiteSpace(signedFieldNames))
            {
                throw new ArgumentException("signedFieldNames is required.", nameof(signedFieldNames));
            }

            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var names = signedFieldNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .Where(name => name.Length > 0)
                .ToList();

            var parts = new List<string>(names.Count);
            foreach (var name in names)
            {
                fields.TryGetValue(name, out var value);
                parts.Add(name + "=" + (value ?? string.Empty));
            }

            return string.Join(",", parts);
        }

        public static bool VerifyResponseSignature(
            string signedFieldNames,
            IDictionary<string, string> fields,
            string receivedSignature,
            string secretKey)
        {
            if (string.IsNullOrWhiteSpace(receivedSignature) || string.IsNullOrWhiteSpace(secretKey))
            {
                return false;
            }

            var message = BuildResponseSignatureMessage(signedFieldNames, fields);
            var computedSignature = GenerateSignature(message, secretKey);

            var isValid = ConstantTimeEquals(computedSignature, receivedSignature);
            Debug.WriteLine("[eSewa] VerifyResponseSignature => " + isValid);
            return isValid;
        }

        public static string GenerateTransactionUuid()
        {
            var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                DateTime.UtcNow.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture),
                randomSuffix);
        }

        private static bool ConstantTimeEquals(string leftValue, string rightValue)
        {
            var left = (leftValue ?? string.Empty).Trim();
            var right = (rightValue ?? string.Empty).Trim();

            if (left.Length != right.Length)
            {
                return false;
            }

            var mismatch = 0;
            for (var i = 0; i < left.Length; i++)
            {
                mismatch |= left[i] ^ right[i];
            }

            return mismatch == 0;
        }
    }
}
