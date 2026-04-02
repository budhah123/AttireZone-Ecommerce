using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace AttireZone_Web_App.Helpers
{
    public static class PaymentDbHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        public static int InsertPayment(int orderId, string paymentMethod, string transactionUuid, decimal amount)
        {
            const string sql = @"
INSERT INTO [dbo].[Payments]
(
    [OrderId],
    [TransactionUuid],
    [PaymentMethod],
    [Amount],
    [Status]
)
VALUES
(
    @OrderId,
    @TransactionUuid,
    @PaymentMethod,
    @Amount,
    'Pending'
);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                command.Parameters.Add("@TransactionUuid", SqlDbType.NVarChar, 100).Value = Normalize(transactionUuid, 100);
                command.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 20).Value = Normalize(paymentMethod, 20);

                var amountParameter = command.Parameters.Add("@Amount", SqlDbType.Decimal);
                amountParameter.Precision = 10;
                amountParameter.Scale = 2;
                amountParameter.Value = amount;

                connection.Open();
                var result = command.ExecuteScalar();
                var paymentId = Convert.ToInt32(result ?? 0, CultureInfo.InvariantCulture);
                Debug.WriteLine("[PaymentDb] InsertPayment orderId=" + orderId + ", tx=" + transactionUuid + ", paymentId=" + paymentId);
                return paymentId;
            }
        }

        public static bool UpdatePaymentStatus(string transactionUuid, string status, string gatewayTransactionId, string gatewayResponse)
        {
            const string sql = @"
UPDATE [dbo].[Payments]
SET
    [Status] = CASE
        WHEN [Status] = 'Completed' AND @Status <> 'Completed' THEN [Status]
        ELSE @Status
    END,
    [GatewayTransactionId] = CASE
        WHEN @GatewayTransactionId IS NULL OR LTRIM(RTRIM(@GatewayTransactionId)) = '' THEN [GatewayTransactionId]
        ELSE @GatewayTransactionId
    END,
    [GatewayResponse] = @GatewayResponse,
    [VerifiedAt] = CASE WHEN @Status = 'Completed' THEN GETDATE() ELSE [VerifiedAt] END
WHERE [TransactionUuid] = @TransactionUuid;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@TransactionUuid", SqlDbType.NVarChar, 100).Value = Normalize(transactionUuid, 100);
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = Normalize(status, 20, "Pending");
                command.Parameters.Add("@GatewayTransactionId", SqlDbType.NVarChar, 200).Value = ToDbNullable(Normalize(gatewayTransactionId, 200));
                command.Parameters.Add("@GatewayResponse", SqlDbType.NVarChar).Value = ToDbNullable(gatewayResponse);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine("[PaymentDb] UpdatePaymentStatus tx=" + transactionUuid + ", status=" + status + ", rows=" + rowsAffected);
                return rowsAffected > 0;
            }
        }

        public static bool UpdateOrderPaymentStatus(int orderId, string paymentStatus)
        {
            const string sql = @"
UPDATE [dbo].[Orders]
SET [PaymentStatus] = @PaymentStatus
WHERE [Id] = @OrderId;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                command.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 20).Value = Normalize(paymentStatus, 20, "Pending");

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                Debug.WriteLine("[PaymentDb] UpdateOrderPaymentStatus orderId=" + orderId + ", status=" + paymentStatus + ", rows=" + rowsAffected);
                return rowsAffected > 0;
            }
        }

        public static PaymentRecord GetPaymentByTransactionUuid(string transactionUuid)
        {
            const string sql = @"
SELECT TOP 1
    [Id],
    [OrderId],
    [TransactionUuid],
    [GatewayTransactionId],
    [PaymentMethod],
    [Amount],
    [Status],
    [GatewayResponse],
    [CreatedAt],
    [VerifiedAt]
FROM [dbo].[Payments]
WHERE [TransactionUuid] = @TransactionUuid;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@TransactionUuid", SqlDbType.NVarChar, 100).Value = Normalize(transactionUuid, 100);

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapPaymentRecord(reader);
                }
            }
        }

        public static PaymentRecord GetLatestPaymentByOrderId(int orderId)
        {
            const string sql = @"
SELECT TOP 1
    [Id],
    [OrderId],
    [TransactionUuid],
    [GatewayTransactionId],
    [PaymentMethod],
    [Amount],
    [Status],
    [GatewayResponse],
    [CreatedAt],
    [VerifiedAt]
FROM [dbo].[Payments]
WHERE [OrderId] = @OrderId
ORDER BY [Id] DESC;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapPaymentRecord(reader);
                }
            }
        }

        private static PaymentRecord MapPaymentRecord(SqlDataReader reader)
        {
            return new PaymentRecord
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                TransactionUuid = reader["TransactionUuid"] == DBNull.Value ? string.Empty : Convert.ToString(reader["TransactionUuid"], CultureInfo.InvariantCulture),
                GatewayTransactionId = reader["GatewayTransactionId"] == DBNull.Value ? string.Empty : Convert.ToString(reader["GatewayTransactionId"], CultureInfo.InvariantCulture),
                PaymentMethod = reader["PaymentMethod"] == DBNull.Value ? string.Empty : Convert.ToString(reader["PaymentMethod"], CultureInfo.InvariantCulture),
                Amount = reader["Amount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                Status = reader["Status"] == DBNull.Value ? string.Empty : Convert.ToString(reader["Status"], CultureInfo.InvariantCulture),
                GatewayResponse = reader["GatewayResponse"] == DBNull.Value ? string.Empty : Convert.ToString(reader["GatewayResponse"], CultureInfo.InvariantCulture),
                CreatedAt = reader["CreatedAt"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedAt"], CultureInfo.InvariantCulture),
                VerifiedAt = reader["VerifiedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VerifiedAt"], CultureInfo.InvariantCulture)
            };
        }

        private static object ToDbNullable(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }

        private static string Normalize(string value, int maxLength, string fallback = "")
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
            if (normalized.Length > maxLength)
            {
                normalized = normalized.Substring(0, maxLength);
            }

            return normalized;
        }

        public sealed class PaymentRecord
        {
            public int Id { get; set; }

            public int OrderId { get; set; }

            public string TransactionUuid { get; set; }

            public string GatewayTransactionId { get; set; }

            public string PaymentMethod { get; set; }

            public decimal Amount { get; set; }

            public string Status { get; set; }

            public string GatewayResponse { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? VerifiedAt { get; set; }
        }
    }
}
