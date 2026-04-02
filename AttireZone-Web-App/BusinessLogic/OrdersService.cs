using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class OrderService
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        public static int PlaceOrder(Order order)
        {
            ValidateOrder(order);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var orderId = InsertOrder(order, connection, transaction);
                        var insertedItems = OrderItemsService.InsertOrderItems(orderId, order.Items, connection, transaction);
                        if (insertedItems <= 0)
                        {
                            throw new InvalidOperationException("Order could not be created because no order items were inserted.");
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static int InsertOrder(Order order, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"
INSERT INTO [dbo].[Orders]
(
    [UserId],
    [FullName],
    [DeliveryAddress],
    [OrderNotes],
    [PaymentMethod],
    [OrderStatus],
    [PaymentStatus]
)
VALUES
(
    @UserId,
    @FullName,
    @DeliveryAddress,
    @OrderNotes,
    @PaymentMethod,
    @OrderStatus,
    @PaymentStatus
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var command = new SqlCommand(sql, connection, transaction))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = order.UserId;
                command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = NormalizeString(order.FullName, 100);
                command.Parameters.Add("@DeliveryAddress", SqlDbType.NVarChar, 500).Value = NormalizeString(order.DeliveryAddress, 500);
                command.Parameters.Add("@OrderNotes", SqlDbType.NVarChar, 500).Value = ToDbNullable(NormalizeString(order.OrderNotes, 500));
                command.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 20).Value = NormalizeString(order.PaymentMethod, 20);
                command.Parameters.Add("@OrderStatus", SqlDbType.NVarChar, 30).Value = NormalizeString(order.OrderStatus, 30, "Pending");
                command.Parameters.Add("@PaymentStatus", SqlDbType.NVarChar, 20).Value = NormalizeString(order.PaymentStatus, 20, "Pending");

                var result = command.ExecuteScalar();
                var orderId = Convert.ToInt32(result ?? 0);
                if (orderId <= 0)
                {
                    throw new InvalidOperationException("Failed to insert order.");
                }

                order.Id = orderId;
                return orderId;
            }
        }

        private static void ValidateOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (order.UserId <= 0)
            {
                throw new ArgumentException("A valid user id is required.", nameof(order));
            }

            if (string.IsNullOrWhiteSpace(order.FullName))
            {
                throw new ArgumentException("Full name is required.", nameof(order));
            }

            if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                throw new ArgumentException("Delivery address is required.", nameof(order));
            }

            if (string.IsNullOrWhiteSpace(order.PaymentMethod))
            {
                throw new ArgumentException("Payment method is required.", nameof(order));
            }

            if (order.Items == null || !order.Items.Any())
            {
                throw new ArgumentException("At least one order item is required.", nameof(order));
            }
        }

        private static object ToDbNullable(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
        }

        private static string NormalizeString(string value, int maxLength, string fallback = "")
        {
            var normalized = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
            if (normalized.Length > maxLength)
            {
                normalized = normalized.Substring(0, maxLength);
            }

            return normalized;
        }
    }
}