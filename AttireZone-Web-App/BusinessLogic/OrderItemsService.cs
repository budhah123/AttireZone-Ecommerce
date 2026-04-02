using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class OrderItemsService
    {
        public static int InsertOrderItems(int orderId, IEnumerable<OrderItem> items, SqlConnection connection, SqlTransaction transaction)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("A valid order id is required.", nameof(orderId));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var normalizedItems = (items ?? Enumerable.Empty<OrderItem>())
                .Where(item => item != null)
                .Select(item => NormalizeOrderItem(item))
                .Where(item => item.ProductId > 0 && item.Quantity > 0)
                .ToList();

            if (normalizedItems.Count == 0)
            {
                return 0;
            }

            const string sql = @"
INSERT INTO [dbo].[OrderItems]
(
    [OrderId],
    [ProductId],
    [ProductName],
    [SelectedSize],
    [Quantity],
    [UnitPrice]
)
VALUES
(
    @OrderId,
    @ProductId,
    @ProductName,
    @SelectedSize,
    @Quantity,
    @UnitPrice
);";

            var insertedCount = 0;
            foreach (var item in normalizedItems)
            {
                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200).Value = item.ProductName;
                    command.Parameters.Add("@SelectedSize", SqlDbType.NVarChar, 10).Value = item.SelectedSize;
                    command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;

                    var unitPrice = command.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                    unitPrice.Precision = 10;
                    unitPrice.Scale = 2;
                    unitPrice.Value = item.UnitPrice;

                    insertedCount += command.ExecuteNonQuery();
                }
            }

            return insertedCount;
        }

        private static OrderItem NormalizeOrderItem(OrderItem item)
        {
            var normalizedSize = string.IsNullOrWhiteSpace(item.SelectedSize) ? "M" : item.SelectedSize.Trim().ToUpperInvariant();
            if (normalizedSize.Length > 10)
            {
                normalizedSize = normalizedSize.Substring(0, 10);
            }

            var normalizedName = string.IsNullOrWhiteSpace(item.ProductName)
                ? string.Format("Product #{0}", item.ProductId)
                : item.ProductName.Trim();
            if (normalizedName.Length > 200)
            {
                normalizedName = normalizedName.Substring(0, 200);
            }

            return new OrderItem
            {
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductName = normalizedName,
                SelectedSize = normalizedSize,
                Quantity = item.Quantity <= 0 ? 1 : item.Quantity,
                UnitPrice = item.UnitPrice < 0m ? 0m : item.UnitPrice
            };
        }
    }
}