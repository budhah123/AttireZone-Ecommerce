using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class WishlistService
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        public static bool AddToWishlist(int userId, int productId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("A valid user id is required.", nameof(userId));
            }

            if (productId <= 0)
            {
                throw new ArgumentException("A valid product id is required.", nameof(productId));
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                const string existsSql = @"
SELECT TOP 1 [WishlistId]
FROM [dbo].[CustomerWishlists]
WHERE [UserId] = @UserId
  AND [ProductId] = @ProductId;";

                using (var existsCommand = new SqlCommand(existsSql, connection))
                {
                    existsCommand.CommandType = CommandType.Text;
                    existsCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    existsCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

                    var existingId = existsCommand.ExecuteScalar();
                    if (existingId != null && existingId != DBNull.Value)
                    {
                        return false;
                    }
                }

                const string insertSql = @"
INSERT INTO [dbo].[CustomerWishlists]
(
    [UserId],
    [ProductId]
)
VALUES
(
    @UserId,
    @ProductId
);";

                using (var insertCommand = new SqlCommand(insertSql, connection))
                {
                    insertCommand.CommandType = CommandType.Text;
                    insertCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    insertCommand.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    return insertCommand.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool RemoveFromWishlist(int wishlistId, int userId)
        {
            if (wishlistId <= 0 || userId <= 0)
            {
                return false;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
DELETE FROM [dbo].[CustomerWishlists]
WHERE [WishlistId] = @WishlistId
  AND [UserId] = @UserId;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@WishlistId", SqlDbType.Int).Value = wishlistId;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static bool RemoveFromWishlistByProduct(int userId, int productId)
        {
            if (userId <= 0 || productId <= 0)
            {
                return false;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
DELETE FROM [dbo].[CustomerWishlists]
WHERE [UserId] = @UserId
  AND [ProductId] = @ProductId;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }

        public static List<Wishlist> GetWishlistByUserId(int userId)
        {
            if (userId <= 0)
            {
                return new List<Wishlist>();
            }

            var wishlistItems = new List<Wishlist>();

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
SELECT
    [WishlistId],
    [UserId],
    [ProductId],
    [CreatedAt]
FROM [dbo].[CustomerWishlists]
WHERE [UserId] = @UserId
ORDER BY [CreatedAt] DESC, [WishlistId] DESC;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        wishlistItems.Add(new Wishlist
                        {
                            WishlistId = reader.GetInt32(reader.GetOrdinal("WishlistId")),
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                        });
                    }
                }
            }

            return wishlistItems;
        }

        public static bool IsInWishlist(int userId, int productId)
        {
            if (userId <= 0 || productId <= 0)
            {
                return false;
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(@"
SELECT TOP 1 1
FROM [dbo].[CustomerWishlists]
WHERE [UserId] = @UserId
  AND [ProductId] = @ProductId;", connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                command.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;

                connection.Open();
                var result = command.ExecuteScalar();
                return result != null && result != DBNull.Value;
            }
        }
    }
}
