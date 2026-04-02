using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AttireZone_Web_App.DataAccess
{
    public class EsewaHelper
    {
        /// <summary>
        /// Generate HMAC-SHA256 signature for eSewa ePay v2
        /// </summary>
        public static string GenerateSignature(string message, string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Build the message string that eSewa expects for signing
        /// Format: "total_amount={amount},transaction_uuid={uuid},product_code={code}"
        /// </summary>
        public static string BuildSignatureMessage(
            decimal totalAmount, string transactionUuid, string productCode)
        {
            return $"total_amount={totalAmount},transaction_uuid={transactionUuid}," +
                   $"product_code={productCode}";
        }
    }
}