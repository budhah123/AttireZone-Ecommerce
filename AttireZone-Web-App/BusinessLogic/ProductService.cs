using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.BusinessLogic
{
    public class ProductService
    {
        public static List<string> GetSearchSuggestions(string searchTerm, int take = 8)
        {
            var normalizedSearch = NormalizeSearch(searchTerm);
            if (string.IsNullOrWhiteSpace(normalizedSearch))
            {
                return new List<string>();
            }

            var safeTake = take <= 0 ? 8 : Math.Min(take, 20);

            const string sql = @"
SELECT TOP (@Take)
    p.[product_name],
    c.[name] AS [category_name],
    p.[edition]
FROM [dbo].[Products] p
LEFT JOIN [dbo].[Categories] c ON c.[id] = p.[CategoryId]
WHERE
    p.[product_name] LIKE '%' + @SearchTerm + '%'
    OR c.[name] LIKE '%' + @SearchTerm + '%'
    OR p.[edition] LIKE '%' + @SearchTerm + '%'
ORDER BY
    CASE WHEN p.[product_name] LIKE @SearchPrefix THEN 0 ELSE 1 END,
    p.[isPopular] DESC,
    p.[id] DESC;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Take", SqlDbType.Int) { Value = safeTake },
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 200) { Value = normalizedSearch },
                new SqlParameter("@SearchPrefix", SqlDbType.NVarChar, 200) { Value = normalizedSearch + "%" }
            };

            var dt = DBHelper.ExecuteDataTable(sql, parameters);
            var suggestions = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var tokens = normalizedSearch.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var productName = row["product_name"] == DBNull.Value ? string.Empty : Convert.ToString(row["product_name"], CultureInfo.InvariantCulture);
                if (string.IsNullOrWhiteSpace(productName))
                {
                    continue;
                }

                var normalizedProductName = productName.Trim();
                if (tokens.Length > 0 && !tokens.All(token => normalizedProductName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    continue;
                }

                if (!seen.Add(normalizedProductName))
                {
                    continue;
                }

                suggestions.Add(normalizedProductName);
                if (suggestions.Count >= safeTake)
                {
                    break;
                }
            }

            return suggestions;
        }

        public static List<Product> SearchProducts(string searchTerm, int? categoryId, string sortOption)
        {
            var normalizedSearch = NormalizeSearch(searchTerm);
            var normalizedSort = NormalizeSort(sortOption);

            const string sqlTemplate = @"
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
WHERE
    (@SearchTerm IS NULL
        OR p.[product_name] LIKE '%' + @SearchTerm + '%'
        OR p.[edition] LIKE '%' + @SearchTerm + '%'
        OR p.[status] LIKE '%' + @SearchTerm + '%'
        OR c.[name] LIKE '%' + @SearchTerm + '%')
    AND (@CategoryId IS NULL OR p.[CategoryId] = @CategoryId)
ORDER BY {0};";

            var sql = string.Format(CultureInfo.InvariantCulture, sqlTemplate, BuildSortClause(normalizedSort));

            SqlParameter[] parameters =
            {
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 200)
                {
                    Value = string.IsNullOrWhiteSpace(normalizedSearch) ? (object)DBNull.Value : normalizedSearch
                },
                new SqlParameter("@CategoryId", SqlDbType.Int)
                {
                    Value = categoryId.HasValue && categoryId.Value > 0 ? (object)categoryId.Value : DBNull.Value
                }
            };

            DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
            var products = new List<Product>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                products.Add(MapProduct(dt.Rows[i]));
            }

            return products;
        }

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

        private static string NormalizeSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return null;
            }

            return searchTerm.Trim();
        }

        private static string NormalizeSort(string sortOption)
        {
            if (string.IsNullOrWhiteSpace(sortOption))
            {
                return "featured";
            }

            var normalized = sortOption.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "featured":
                case "newest":
                case "price_asc":
                case "price_desc":
                case "name_asc":
                case "name_desc":
                    return normalized;
                default:
                    return "featured";
            }
        }

        private static string BuildSortClause(string sortOption)
        {
            switch (sortOption)
            {
                case "newest":
                    return "p.[id] DESC";
                case "price_asc":
                    return "p.[price] ASC, p.[id] DESC";
                case "price_desc":
                    return "p.[price] DESC, p.[id] DESC";
                case "name_asc":
                    return "p.[product_name] ASC, p.[id] DESC";
                case "name_desc":
                    return "p.[product_name] DESC, p.[id] DESC";
                case "featured":
                default:
                    return "p.[isPopular] DESC, p.[id] DESC";
            }
        }
    }
}
