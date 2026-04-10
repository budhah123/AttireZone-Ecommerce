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
    public class CategoryService
    {
        public static List<Category> GetAllCategories()
        {
            const string sql = @"
SELECT
    [id],
    [name],
    [description],
    [created_date]
FROM [dbo].[Categories]
ORDER BY [id] DESC;";

            DataTable dt = DBHelper.ExecuteDataTable(sql);
            var categories = new List<Category>(dt.Rows.Count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                categories.Add(MapCategory(dt.Rows[i]));
            }

            return categories;
        }

        public static Category GetCategoryById(int id)
        {
            const string sql = @"
SELECT
    [id],
    [name],
    [description],
    [created_date]
FROM [dbo].[Categories]
WHERE [id] = @Id;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            DataTable dt = DBHelper.ExecuteDataTable(sql, parameters);
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return MapCategory(dt.Rows[0]);
        }

        public static int CreateCategory(Category category)
        {
            if (category == null)
            {
                return 0;
            }

            const string sql = @"
INSERT INTO [dbo].[Categories]
(
    [name],
    [description],
    [created_date]
)
VALUES
(
    @Name,
    @Description,
    @CreatedDate
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            SqlParameter[] parameters = BuildCategoryParameters(category, includeId: false);
            object result = DBHelper.ExecuteScalar(sql, parameters);

            return Convert.ToInt32(result ?? 0);
        }

        public static bool UpdateCategory(Category category)
        {
            if (category == null || category.Id <= 0)
            {
                return false;
            }

            const string sql = @"
UPDATE [dbo].[Categories]
SET
    [name] = @Name,
    [description] = @Description
WHERE [id] = @Id;";

            SqlParameter[] parameters = BuildCategoryParameters(category, includeId: true);
            int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);

            return rowsAffected > 0;
        }

        public static bool DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            const string sql = "DELETE FROM [dbo].[Categories] WHERE [id] = @Id;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            int rowsAffected = DBHelper.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        public static bool CategoryNameExists(string categoryName, int? excludeId = null)
        {
            var normalizedName = (categoryName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return false;
            }

            const string sql = @"
SELECT COUNT(1)
FROM [dbo].[Categories]
WHERE LOWER(LTRIM(RTRIM([name]))) = LOWER(LTRIM(RTRIM(@Name)))
  AND (@ExcludeId IS NULL OR [id] <> @ExcludeId);";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Name", SqlDbType.NVarChar, 150) { Value = normalizedName },
                new SqlParameter("@ExcludeId", SqlDbType.Int)
                {
                    Value = excludeId.HasValue && excludeId.Value > 0
                        ? (object)excludeId.Value
                        : DBNull.Value
                }
            };

            object result = DBHelper.ExecuteScalar(sql, parameters);
            var count = result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result, CultureInfo.InvariantCulture);

            return count > 0;
        }

        public static int GetProductCountByCategoryId(int categoryId)
        {
            if (categoryId <= 0)
            {
                return 0;
            }

            const string sql = @"
SELECT COUNT(1)
FROM [dbo].[Products]
WHERE [CategoryId] = @CategoryId;";

            SqlParameter[] parameters =
            {
                new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId }
            };

            object result = DBHelper.ExecuteScalar(sql, parameters);
            return result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result, CultureInfo.InvariantCulture);
        }

        public static Dictionary<int, int> GetProductCountsByCategoryIds(IEnumerable<int> categoryIds)
        {
            var ids = (categoryIds ?? Enumerable.Empty<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<int, int>();
            }

            var sqlParameters = new List<SqlParameter>(ids.Count);
            var parameterNames = new List<string>(ids.Count);

            for (var i = 0; i < ids.Count; i++)
            {
                var parameterName = "@CategoryId" + i.ToString(CultureInfo.InvariantCulture);
                parameterNames.Add(parameterName);
                sqlParameters.Add(new SqlParameter(parameterName, SqlDbType.Int) { Value = ids[i] });
            }

            var sql = @"
SELECT [CategoryId], COUNT(1) AS [ProductCount]
FROM [dbo].[Products]
WHERE [CategoryId] IN (" + string.Join(",", parameterNames) + @")
GROUP BY [CategoryId];";

            var counts = ids.ToDictionary(id => id, _ => 0);
            var table = DBHelper.ExecuteDataTable(sql, sqlParameters.ToArray());

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var categoryId = row["CategoryId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(row["CategoryId"], CultureInfo.InvariantCulture);

                if (categoryId <= 0)
                {
                    continue;
                }

                var productCount = row["ProductCount"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(row["ProductCount"], CultureInfo.InvariantCulture);

                counts[categoryId] = productCount;
            }

            return counts;
        }

        private static SqlParameter[] BuildCategoryParameters(Category category, bool includeId)
        {
            var createdDate = category.CreatedDate == default(DateTime)
                ? DateTime.Now
                : category.CreatedDate;

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar, 150)
                {
                    Value = (object)(category.Name ?? string.Empty)
                },
                new SqlParameter("@Description", SqlDbType.NVarChar)
                {
                    Value = string.IsNullOrWhiteSpace(category.Description) ? (object)DBNull.Value : category.Description
                },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime)
                {
                    Value = createdDate
                }
            };

            if (includeId)
            {
                parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = category.Id });
            }

            return parameters.ToArray();
        }

        private static Category MapCategory(DataRow row)
        {
            return new Category
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"] == DBNull.Value ? string.Empty : row["name"].ToString(),
                Description = row["description"] == DBNull.Value ? string.Empty : row["description"].ToString(),
                CreatedDate = row["created_date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(row["created_date"])
            };
        }
    }
}