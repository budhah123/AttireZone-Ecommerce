using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class CartService
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        public static void AddToCart(int userId, int productId, int selectedQuantity, string selectedSize)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user id is required.", nameof(userId));
            }

            if (productId <= 0)
            {
                throw new ArgumentException("A valid product id is required.", nameof(productId));
            }

            var normalizedSize = NormalizeSelectedSize(selectedSize);
            var quantityToAdd = selectedQuantity <= 0 ? 1 : selectedQuantity;

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                const string selectSql = @"
SELECT TOP 1 [Id], [SelectedQuantity]
FROM [dbo].[Cart]
WHERE [UserId] = @UserId
  AND [ProductId] = @ProductId
  AND [SelectedSize] = @SelectedSize;";

                using (var selectCommand = new SqlCommand(selectSql, connection))
                {
                    selectCommand.CommandType = CommandType.Text;
                    selectCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    selectCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    selectCommand.Parameters.Add("@SelectedSize", SqlDbType.NVarChar, 10).Value = normalizedSize;

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var cartId = Convert.ToInt32(reader["Id"]);
                            var existingQuantity = Convert.ToInt32(reader["SelectedQuantity"]);
                            reader.Close();

                            const string updateSql = @"
UPDATE [dbo].[Cart]
SET [SelectedQuantity] = @UpdatedQuantity
WHERE [Id] = @Id;";

                            using (var updateCommand = new SqlCommand(updateSql, connection))
                            {
                                updateCommand.CommandType = CommandType.Text;
                                updateCommand.Parameters.Add("@UpdatedQuantity", SqlDbType.Int).Value = existingQuantity + quantityToAdd;
                                updateCommand.Parameters.Add("@Id", SqlDbType.Int).Value = cartId;
                                updateCommand.ExecuteNonQuery();
                            }

                            return;
                        }
                    }
                }

                const string insertSql = @"
INSERT INTO [dbo].[Cart]
(
    [UserId],
    [ProductId],
    [SelectedQuantity],
    [SelectedSize]
)
VALUES
(
    @UserId,
    @ProductId,
    @SelectedQuantity,
    @SelectedSize
);";

                using (var insertCommand = new SqlCommand(insertSql, connection))
                {
                    insertCommand.CommandType = CommandType.Text;
                    insertCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    insertCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    insertCommand.Parameters.Add("@SelectedQuantity", SqlDbType.Int).Value = quantityToAdd;
                    insertCommand.Parameters.Add("@SelectedSize", SqlDbType.NVarChar, 10).Value = normalizedSize;
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public static List<Cart> GetCartByUserId(int userId)
        {
            if (userId <= 0)
            {
                return new List<Cart>();
            }

            var items = new List<Cart>();

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
SELECT
    [Id],
    [UserId],
    [ProductId],
    [SelectedQuantity],
    [SelectedSize],
    [CreatedAt]
FROM [dbo].[Cart]
WHERE [UserId] = @UserId
ORDER BY [CreatedAt] DESC, [Id] DESC;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Cart
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            SelectedQuantity = reader.GetInt32(reader.GetOrdinal("SelectedQuantity")),
                            SelectedSize = reader["SelectedSize"] == DBNull.Value ? string.Empty : Convert.ToString(reader["SelectedSize"]),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                        });
                    }
                }
            }

            return items;
        }

        public static bool RemoveFromCart(int cartId)
        {
            if (cartId <= 0)
            {
                return false;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("DELETE FROM [dbo].[Cart] WHERE [Id] = @Id;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@Id", SqlDbType.Int).Value = cartId;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static int ClearCart(int userId)
        {
            if (userId <= 0)
            {
                return 0;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("DELETE FROM [dbo].[Cart] WHERE [UserId] = @UserId;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int GetCartCount(int userId)
        {
            if (userId <= 0)
            {
                return 0;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
SELECT ISNULL(SUM([SelectedQuantity]), 0)
FROM [dbo].[Cart]
WHERE [UserId] = @UserId;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result ?? 0);
            }
        }

        private static string NormalizeSelectedSize(string selectedSize)
        {
            if (string.IsNullOrWhiteSpace(selectedSize))
            {
                return "M";
            }

            var normalized = selectedSize.Trim();
            if (normalized.Length > 10)
            {
                normalized = normalized.Substring(0, 10);
            }

            return normalized.ToUpperInvariant();
        }
    }
}