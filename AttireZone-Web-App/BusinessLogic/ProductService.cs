using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class ProductService
    {
        public static List<Product> GetAllProducts()
        {
            const string sql = @"
SELECT
    p.[id],
    p.[product_name],
    p.[price],
    p.[edition],
    p.[CategoryId],
    p.[isPopular],
    p.[selected_size],
    p.[description],
    p.[stock_quantity],
    p.[status],
    p.[image_path],
    c.[name] AS [category_name]
FROM [dbo].[Products] p
LEFT JOIN [dbo].[Categories] c ON c.[id] = p.[CategoryId]
ORDER BY p.[id] DESC;";

            DataTable dt = DBHelper.ExecuteDataTable(sql);
            var products = new List<Product>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                products.Add(MapProduct(dt.Rows[i]));
            }

            return products;
        }

        public static Product GetProductById(int id)
        {
            const string sql = @"
SELECT
    p.[id],
    p.[product_name],
    p.[price],
    p.[edition],
    p.[CategoryId],
    p.[isPopular],
    p.[selected_size],
    p.[description],
    p.[stock_quantity],
    p.[status],
    p.[image_path],
    c.[name] AS [category_name]
FROM [dbo].[Products] p
LEFT JOIN [dbo].[Categories] c ON c.[id] = p.[CategoryId]
WHERE p.[id] = @Id;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapProduct(dt.Rows[0]);
        }

        public static int CreateProduct(Product product)
        {
            if (product == null)
            {
                return 0;
            }

            const string sql = @"
INSERT INTO [dbo].[Products]
(
    [product_name],
    [price],
    [edition],
    [CategoryId],
    [selected_size],
    [description],
    [stock_quantity],
    [isPopular],
    [status],
    [image_path]
)
VALUES
(
    @ProductName,
    @Price,
    @Edition,
    @CategoryId,
    @SelectedSize,
    @Description,
    @StockQuantity,
    @IsPopular,
    @Status,
    @ImagePath
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlParameter[] parameters = BuildProductParameters(product, includeId: false);
            object result = DBHelper.ExecuteScalar(sql, parameters);

            return Convert.ToInt32(result ?? 0);
        }

        public static bool UpdateProduct(Product product)
        {
            if (product == null || product.Id <= 0)
            {
                return false;
            }

            const string sql = @"
UPDATE [dbo].[Products]
SET
    [product_name] = @ProductName,
    [price] = @Price,
    [edition] = @Edition,
    [CategoryId] = @CategoryId,
    [selected_size] = @SelectedSize,
    [description] = @Description,
    [stock_quantity] = @StockQuantity,
    [isPopular] = @IsPopular,
    [status] = @Status,
    [image_path] = @ImagePath
WHERE [id] = @Id;";

            SqlParameter[] parameters = BuildProductParameters(product, includeId: true);
            int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);

            return rowsAffected > 0;
        }

        public static bool DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            const string sql = "DELETE FROM [dbo].[Products] WHERE [id] = @Id;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        private static SqlParameter[] BuildProductParameters(Product product, bool includeId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductName", SqlDbType.NVarChar, 200)
                {
                    Value = (object)(product.ProductName ?? string.Empty)
                },
                new SqlParameter("@Price", SqlDbType.Decimal)
                {
                    Precision = 10,
                    Scale = 2,
                    Value = product.Price
                },
                new SqlParameter("@Edition", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(product.Edition) ? (object)DBNull.Value : product.Edition
                },
                new SqlParameter("@CategoryId", SqlDbType.Int)
                {
                    Value = product.CategoryId.HasValue ? (object)product.CategoryId.Value : DBNull.Value
                },
                new SqlParameter("@SelectedSize", SqlDbType.NVarChar, 50)
                {
                    Value = string.IsNullOrWhiteSpace(product.SelectedSize) ? (object)DBNull.Value : product.SelectedSize
                },
                new SqlParameter("@Description", SqlDbType.NVarChar)
                {
                    Value = string.IsNullOrWhiteSpace(product.Description) ? (object)DBNull.Value : product.Description
                },
                new SqlParameter("@StockQuantity", SqlDbType.Int)
                {
                    Value = product.StockQuantity
                },
                new SqlParameter("@IsPopular", SqlDbType.Bit)
                {
                    Value = product.IsPopular
                },
                new SqlParameter("@Status", SqlDbType.NVarChar, 50)
                {
                    Value = string.IsNullOrWhiteSpace(product.Status) ? (object)DBNull.Value : product.Status
                },
                new SqlParameter("@ImagePath", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrWhiteSpace(product.ImagePath) ? (object)DBNull.Value : product.ImagePath
                }
            };

            if (includeId)
            {
                parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = product.Id });
            }

            return parameters.ToArray();
        }

        private static Product MapProduct(DataRow row)
        {
            return new Product
            {
                Id = Convert.ToInt32(row["id"]),
                ProductName = row["product_name"] == DBNull.Value ? string.Empty : row["product_name"].ToString(),
                Price = row["price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["price"]),
                Edition = row["edition"] == DBNull.Value ? string.Empty : row["edition"].ToString(),
                CategoryId = row.Table.Columns.Contains("CategoryId") && row["CategoryId"] != DBNull.Value
                    ? (int?)Convert.ToInt32(row["CategoryId"])
                    : null,
                CategoryName = row.Table.Columns.Contains("category_name") && row["category_name"] != DBNull.Value
                    ? row["category_name"].ToString()
                    : string.Empty,
                SelectedSize = row["selected_size"] == DBNull.Value ? string.Empty : row["selected_size"].ToString(),
                Description = row["description"] == DBNull.Value ? string.Empty : row["description"].ToString(),
                StockQuantity = row["stock_quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["stock_quantity"]),
                IsPopular = ReadIsPopular(row),
                Status = row["status"] == DBNull.Value ? string.Empty : row["status"].ToString(),
                ImagePath = row["image_path"] == DBNull.Value ? string.Empty : row["image_path"].ToString()
            };
        }

        private static bool ReadIsPopular(DataRow row)
        {
            if (row == null || row.Table == null)
            {
                return false;
            }

            if (row.Table.Columns.Contains("isPopular") && row["isPopular"] != DBNull.Value)
            {
                return Convert.ToBoolean(row["isPopular"]);
            }

            if (row.Table.Columns.Contains("IsPopular") && row["IsPopular"] != DBNull.Value)
            {
                return Convert.ToBoolean(row["IsPopular"]);
            }

            return false;
        }
    }
}
