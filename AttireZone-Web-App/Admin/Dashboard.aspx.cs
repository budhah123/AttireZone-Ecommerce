using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HasAdminAccess())
            {
                RedirectToAdminLogin();
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private sealed class RecentOrderVm
        {
            public string OrderNumber { get; set; }
            public string Description { get; set; }
            public string AmountFormatted { get; set; }
        }

        private sealed class PeriodMetric
        {
            public decimal CurrentPeriod { get; set; }
            public decimal PreviousPeriod { get; set; }
        }

        private sealed class DashboardSchema
        {
            public bool HasUsersTable { get; set; }
            public bool HasOrdersTable { get; set; }
            public bool HasProductsTable { get; set; }
            public bool HasPaymentsTable { get; set; }

            public string UsersCreatedDateColumn { get; set; }
            public string UsersUserIdColumn { get; set; }
            public string UsersFullNameColumn { get; set; }

            public string OrdersIdColumn { get; set; }
            public string OrdersUserIdColumn { get; set; }
            public string OrdersDateColumn { get; set; }
            public string OrdersStatusColumn { get; set; }
            public string OrdersTotalAmountColumn { get; set; }
            public string OrdersCustomerNameColumn { get; set; }

            public string ProductsCreatedDateColumn { get; set; }

            public string PaymentsAmountColumn { get; set; }
            public string PaymentsStatusColumn { get; set; }
            public string PaymentsCreatedAtColumn { get; set; }

            public bool CanJoinUsers { get; set; }
        }

        private void LoadDashboard()
        {
            var schema = SafeDb(ResolveDashboardSchema, new DashboardSchema());

            var totalUsers = schema.HasUsersTable
                ? SafeDb(() => ExecuteInt("SELECT COUNT(1) FROM [dbo].[Users];"), 0)
                : 0;
            var totalOrders = schema.HasOrdersTable
                ? SafeDb(() => ExecuteInt("SELECT COUNT(1) FROM [dbo].[Orders];"), 0)
                : 0;
            var totalProducts = schema.HasProductsTable
                ? SafeDb(() => ExecuteInt("SELECT COUNT(1) FROM [dbo].[Products];"), 0)
                : 0;
            var totalRevenue = SafeDb(() => GetTotalRevenue(schema), 0m);

            litTotalUsers.Text = totalUsers.ToString("N0", CultureInfo.InvariantCulture);
            litTotalOrders.Text = totalOrders.ToString("N0", CultureInfo.InvariantCulture);
            litTotalProducts.Text = totalProducts.ToString("N0", CultureInfo.InvariantCulture);
            litTotalRevenue.Text = totalRevenue.ToString("C0", UsCulture);

            litUsersDelta.Text = SafeDb(() => BuildCountDeltaLabel("Users", schema.UsersCreatedDateColumn, schema.HasUsersTable), "0%");
            litOrdersDelta.Text = SafeDb(() => BuildCountDeltaLabel("Orders", schema.OrdersDateColumn, schema.HasOrdersTable), "0%");
            litProductsDelta.Text = SafeDb(() => BuildCountDeltaLabel("Products", schema.ProductsCreatedDateColumn, schema.HasProductsTable), "0%");
            litRevenueDelta.Text = SafeDb(() => BuildRevenueDeltaLabel(schema), "0%");

            var recentOrders = SafeDb(() => LoadRecentOrders(schema, 3), new List<RecentOrderVm>());
            if (recentOrders.Count == 0)
            {
                recentOrders = BuildNoDataRecentOrders();
            }

            rptRecentOrders.DataSource = recentOrders;
            rptRecentOrders.DataBind();
        }

        private static int ExecuteInt(string sql)
        {
            var result = DBHelper.ExecuteScalar(sql);
            if (result == null || result == DBNull.Value) return 0;
            return Convert.ToInt32(result, CultureInfo.InvariantCulture);
        }

        private static decimal ExecuteDecimal(string sql)
        {
            var result = DBHelper.ExecuteScalar(sql);
            if (result == null || result == DBNull.Value) return 0m;
            return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
        }

        private static DashboardSchema ResolveDashboardSchema()
        {
            var schema = new DashboardSchema
            {
                HasUsersTable = TableExists("Users"),
                HasOrdersTable = TableExists("Orders"),
                HasProductsTable = TableExists("Products"),
                HasPaymentsTable = TableExists("Payments")
            };

            schema.UsersUserIdColumn = ResolveFirstExistingColumn("Users", "UserId", "Id");
            schema.UsersFullNameColumn = ResolveFirstExistingColumn("Users", "FullName", "Name");
            schema.UsersCreatedDateColumn = ResolveFirstExistingColumn("Users", "CreatedDate", "CreatedAt");

            schema.OrdersIdColumn = ResolveFirstExistingColumn("Orders", "OrderId", "Id");
            schema.OrdersUserIdColumn = ResolveFirstExistingColumn("Orders", "UserId");
            schema.OrdersDateColumn = ResolveFirstExistingColumn("Orders", "OrderDate", "CreatedAt", "CreatedDate");
            schema.OrdersStatusColumn = ResolveFirstExistingColumn("Orders", "Status", "OrderStatus");
            schema.OrdersTotalAmountColumn = ResolveFirstExistingColumn("Orders", "TotalAmount", "GrandTotal", "Amount");
            schema.OrdersCustomerNameColumn = ResolveFirstExistingColumn("Orders", "FullName", "CustomerName");

            schema.ProductsCreatedDateColumn = ResolveFirstExistingColumn("Products", "CreatedAt", "CreatedDate", "created_date");

            schema.PaymentsAmountColumn = ResolveFirstExistingColumn("Payments", "Amount", "TotalAmount");
            schema.PaymentsStatusColumn = ResolveFirstExistingColumn("Payments", "Status", "PaymentStatus");
            schema.PaymentsCreatedAtColumn = ResolveFirstExistingColumn("Payments", "CreatedAt", "VerifiedAt", "PaymentDate", "CreatedDate");

            schema.CanJoinUsers = schema.HasOrdersTable &&
                                  schema.HasUsersTable &&
                                  !string.IsNullOrWhiteSpace(schema.OrdersUserIdColumn) &&
                                  !string.IsNullOrWhiteSpace(schema.UsersUserIdColumn);

            return schema;
        }

        private static string BuildCountDeltaLabel(string tableName, string dateColumn, bool hasTable)
        {
            if (!hasTable || string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(dateColumn))
            {
                return "0%";
            }

            var dateExpr = "t." + WrapIdentifier(dateColumn);
            var sql = string.Concat(
                "SELECT ",
                "ISNULL(SUM(CASE WHEN ", dateExpr, " >= DATEADD(day,-30,GETDATE()) THEN 1 ELSE 0 END),0) AS CurrentPeriod,",
                "ISNULL(SUM(CASE WHEN ", dateExpr, " < DATEADD(day,-30,GETDATE()) AND ", dateExpr, " >= DATEADD(day,-60,GETDATE()) THEN 1 ELSE 0 END),0) AS PreviousPeriod ",
                "FROM [dbo].[", tableName, "] t;");

            var metric = ExecutePeriodMetric(sql);
            return BuildDeltaLabel(metric.CurrentPeriod, metric.PreviousPeriod);
        }

        private static string BuildRevenueDeltaLabel(DashboardSchema schema)
        {
            if (schema == null)
            {
                return "0%";
            }

            if (schema.HasPaymentsTable &&
                !string.IsNullOrWhiteSpace(schema.PaymentsAmountColumn) &&
                !string.IsNullOrWhiteSpace(schema.PaymentsCreatedAtColumn))
            {
                var dateExpr = "p." + WrapIdentifier(schema.PaymentsCreatedAtColumn);
                var amountExpr = BuildDecimalExpression("p." + WrapIdentifier(schema.PaymentsAmountColumn));
                var paidPredicate = BuildPaidPaymentPredicate(schema.PaymentsStatusColumn, "p");

                var sql = string.Concat(
                    "SELECT ",
                    "ISNULL(SUM(CASE WHEN ", paidPredicate, " AND ", dateExpr, " >= DATEADD(day,-30,GETDATE()) THEN ", amountExpr, " ELSE 0 END),0) AS CurrentPeriod,",
                    "ISNULL(SUM(CASE WHEN ", paidPredicate, " AND ", dateExpr, " < DATEADD(day,-30,GETDATE()) AND ", dateExpr, " >= DATEADD(day,-60,GETDATE()) THEN ", amountExpr, " ELSE 0 END),0) AS PreviousPeriod ",
                    "FROM [dbo].[Payments] p;");

                var paymentMetric = ExecutePeriodMetric(sql);
                return BuildDeltaLabel(paymentMetric.CurrentPeriod, paymentMetric.PreviousPeriod);
            }

            if (schema.HasOrdersTable &&
                !string.IsNullOrWhiteSpace(schema.OrdersTotalAmountColumn) &&
                !string.IsNullOrWhiteSpace(schema.OrdersDateColumn))
            {
                var dateExpr = "o." + WrapIdentifier(schema.OrdersDateColumn);
                var amountExpr = BuildDecimalExpression("o." + WrapIdentifier(schema.OrdersTotalAmountColumn));

                var sql = string.Concat(
                    "SELECT ",
                    "ISNULL(SUM(CASE WHEN ", dateExpr, " >= DATEADD(day,-30,GETDATE()) THEN ", amountExpr, " ELSE 0 END),0) AS CurrentPeriod,",
                    "ISNULL(SUM(CASE WHEN ", dateExpr, " < DATEADD(day,-30,GETDATE()) AND ", dateExpr, " >= DATEADD(day,-60,GETDATE()) THEN ", amountExpr, " ELSE 0 END),0) AS PreviousPeriod ",
                    "FROM [dbo].[Orders] o;");

                var orderMetric = ExecutePeriodMetric(sql);
                return BuildDeltaLabel(orderMetric.CurrentPeriod, orderMetric.PreviousPeriod);
            }

            return "0%";
        }

        private static decimal GetTotalRevenue(DashboardSchema schema)
        {
            if (schema == null)
            {
                return 0m;
            }

            if (schema.HasPaymentsTable && !string.IsNullOrWhiteSpace(schema.PaymentsAmountColumn))
            {
                var amountExpr = BuildDecimalExpression("p." + WrapIdentifier(schema.PaymentsAmountColumn));
                var paidPredicate = BuildPaidPaymentPredicate(schema.PaymentsStatusColumn, "p");
                var sql = string.Concat(
                    "SELECT ISNULL(SUM(CASE WHEN ", paidPredicate, " THEN ", amountExpr, " ELSE 0 END),0) ",
                    "FROM [dbo].[Payments] p;");

                return ExecuteDecimal(sql);
            }

            if (schema.HasOrdersTable && !string.IsNullOrWhiteSpace(schema.OrdersTotalAmountColumn))
            {
                var amountExpr = BuildDecimalExpression("o." + WrapIdentifier(schema.OrdersTotalAmountColumn));
                var sql = string.Concat(
                    "SELECT ISNULL(SUM(", amountExpr, "),0) FROM [dbo].[Orders] o;");

                return ExecuteDecimal(sql);
            }

            return 0m;
        }

        private static List<RecentOrderVm> LoadRecentOrders(DashboardSchema schema, int take)
        {
            if (schema == null || !schema.HasOrdersTable || string.IsNullOrWhiteSpace(schema.OrdersIdColumn))
            {
                return new List<RecentOrderVm>();
            }

            var orderIdExpr = "o." + WrapIdentifier(schema.OrdersIdColumn);
            var dateExpr = string.IsNullOrWhiteSpace(schema.OrdersDateColumn)
                ? "NULL"
                : "o." + WrapIdentifier(schema.OrdersDateColumn);
            var amountExpr = string.IsNullOrWhiteSpace(schema.OrdersTotalAmountColumn)
                ? "CAST(0 AS DECIMAL(18,2))"
                : BuildDecimalExpression("o." + WrapIdentifier(schema.OrdersTotalAmountColumn));
            var statusExpr = string.IsNullOrWhiteSpace(schema.OrdersStatusColumn)
                ? "'Pending'"
                : "o." + WrapIdentifier(schema.OrdersStatusColumn);

            var customerExpr = BuildCustomerNameExpression(schema);

            var joinUsers = string.Empty;
            if (schema.CanJoinUsers)
            {
                joinUsers = string.Concat(
                    " LEFT JOIN [dbo].[Users] u ON u.",
                    WrapIdentifier(schema.UsersUserIdColumn),
                    " = o.",
                    WrapIdentifier(schema.OrdersUserIdColumn),
                    " ");
            }

            var orderByExpr = string.IsNullOrWhiteSpace(schema.OrdersDateColumn)
                ? orderIdExpr
                : dateExpr;

            var sql = string.Concat(
                "SELECT TOP (@Take) ",
                orderIdExpr, " AS [OrderId],",
                customerExpr, " AS [CustomerName],",
                statusExpr, " AS [OrderStatus],",
                amountExpr, " AS [TotalAmount],",
                dateExpr, " AS [OrderDate] ",
                "FROM [dbo].[Orders] o",
                joinUsers,
                "ORDER BY ", orderByExpr, " DESC, ", orderIdExpr, " DESC;");

            var dt = DBHelper.ExecuteDataTable(sql, new[]
            {
                new SqlParameter("@Take", SqlDbType.Int) { Value = take <= 0 ? 3 : take }
            });

            if (dt.Rows.Count == 0)
            {
                return new List<RecentOrderVm>();
            }

            var items = new List<RecentOrderVm>(dt.Rows.Count);
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var orderId = ToInt(row["OrderId"]);
                var customerName = ReadString(row["CustomerName"], "Customer");
                var normalizedStatus = NormalizeStatus(ReadString(row["OrderStatus"], "Pending"));
                var totalAmount = ToDecimal(row["TotalAmount"]);

                var description = customerName + " - " + normalizedStatus;
                items.Add(new RecentOrderVm
                {
                    OrderNumber = orderId <= 0 ? "N/A" : orderId.ToString(CultureInfo.InvariantCulture),
                    Description = description,
                    AmountFormatted = totalAmount.ToString("C2", UsCulture)
                });
            }

            return items;
        }

        private static string BuildCustomerNameExpression(DashboardSchema schema)
        {
            if (schema == null)
            {
                return "'Customer'";
            }

            if (!string.IsNullOrWhiteSpace(schema.OrdersCustomerNameColumn))
            {
                var orderNameExpr = "NULLIF(LTRIM(RTRIM(o." + WrapIdentifier(schema.OrdersCustomerNameColumn) + ")), '')";
                if (schema.CanJoinUsers && !string.IsNullOrWhiteSpace(schema.UsersFullNameColumn))
                {
                    return "COALESCE(" + orderNameExpr + ", NULLIF(LTRIM(RTRIM(u." + WrapIdentifier(schema.UsersFullNameColumn) + ")), ''), 'Customer')";
                }

                return "COALESCE(" + orderNameExpr + ", 'Customer')";
            }

            if (schema.CanJoinUsers && !string.IsNullOrWhiteSpace(schema.UsersFullNameColumn))
            {
                return "COALESCE(NULLIF(LTRIM(RTRIM(u." + WrapIdentifier(schema.UsersFullNameColumn) + ")), ''), 'Customer')";
            }

            return "'Customer'";
        }

        private static string NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return "Pending";
            }

            var normalized = status.Trim().ToLowerInvariant();

            if (normalized.Contains("deliver") || normalized.Contains("complete") || normalized.Contains("success"))
            {
                return "Delivered";
            }

            if (normalized.Contains("ship") || normalized.Contains("transit"))
            {
                return "Shipped";
            }

            if (normalized.Contains("cancel") || normalized.Contains("fail"))
            {
                return "Cancelled";
            }

            if (normalized.Contains("process") || normalized.Contains("confirm") || normalized.Contains("pending"))
            {
                return "Pending";
            }

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
        }

        private static List<RecentOrderVm> BuildNoDataRecentOrders()
        {
            return new List<RecentOrderVm>
            {
                new RecentOrderVm
                {
                    OrderNumber = "N/A",
                    Description = "No recent orders available.",
                    AmountFormatted = 0m.ToString("C2", UsCulture)
                }
            };
        }

        private static PeriodMetric ExecutePeriodMetric(string sql)
        {
            var dt = DBHelper.ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                return new PeriodMetric();
            }

            return new PeriodMetric
            {
                CurrentPeriod = ToDecimal(dt.Rows[0]["CurrentPeriod"]),
                PreviousPeriod = ToDecimal(dt.Rows[0]["PreviousPeriod"])
            };
        }

        private static string BuildDeltaLabel(decimal current, decimal previous)
        {
            if (previous <= 0m)
            {
                if (current <= 0m)
                {
                    return "0%";
                }

                return "+100%";
            }

            var percent = ((current - previous) / previous) * 100m;
            var rounded = (int)Math.Round(percent, 0, MidpointRounding.AwayFromZero);

            if (rounded == 0)
            {
                return "0%";
            }

            return rounded > 0 ? $"+{rounded}%" : $"{rounded}%";
        }

        private static string BuildPaidPaymentPredicate(string paymentStatusColumn, string tableAlias)
        {
            if (string.IsNullOrWhiteSpace(paymentStatusColumn))
            {
                return "1 = 1";
            }

            var field = tableAlias + "." + WrapIdentifier(paymentStatusColumn);
            var normalized = "LOWER(LTRIM(RTRIM(CAST(" + field + " AS NVARCHAR(50)))))";
            return normalized + " IN ('paid','completed','complete','success','succeeded','verified','captured')";
        }

        private static string BuildDecimalExpression(string fieldExpression)
        {
            return "TRY_CONVERT(DECIMAL(18,2), " + fieldExpression + ")";
        }

        private static bool TableExists(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            const string sql = @"
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName;";

            var result = DBHelper.ExecuteScalar(sql, new[]
            {
                new SqlParameter("@TableName", SqlDbType.NVarChar, 128) { Value = tableName.Trim() }
            });

            return ToInt(result) > 0;
        }

        private static string ResolveFirstExistingColumn(string tableName, params string[] candidates)
        {
            if (!TableExists(tableName) || candidates == null || candidates.Length == 0)
            {
                return null;
            }

            const string sql = @"
SELECT TOP 1 COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = @TableName
  AND COLUMN_NAME = @ColumnName;";

            for (var i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                var result = DBHelper.ExecuteScalar(sql, new[]
                {
                    new SqlParameter("@TableName", SqlDbType.NVarChar, 128) { Value = tableName.Trim() },
                    new SqlParameter("@ColumnName", SqlDbType.NVarChar, 128) { Value = candidate.Trim() }
                });

                var value = Convert.ToString(result, CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return null;
        }

        private static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            if (value is int directInt)
            {
                return directInt;
            }

            return int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : 0;
        }

        private static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0m;
            }

            if (value is decimal directDecimal)
            {
                return directDecimal;
            }

            return decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : 0m;
        }

        private static string ReadString(object value, string fallback)
        {
            var parsed = value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture);

            return string.IsNullOrWhiteSpace(parsed) ? fallback : parsed.Trim();
        }

        private static string WrapIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return string.Empty;
            }

            var trimmed = identifier.Trim();
            if (trimmed.StartsWith("[", StringComparison.Ordinal) && trimmed.EndsWith("]", StringComparison.Ordinal))
            {
                return trimmed;
            }

            return "[" + trimmed.Replace("]", string.Empty) + "]";
        }

        private static T SafeDb<T>(Func<T> query, T fallback)
        {
            try
            {
                return query == null ? fallback : query();
            }
            catch
            {
                return fallback;
            }
        }

        private bool HasAdminAccess()
        {
            var adminRole = Convert.ToString(Session["AdminRole"]);
            if (!string.IsNullOrEmpty(adminRole) &&
                adminRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var currentUser = Session["CurrentUser"] as User;
            return currentUser != null &&
                   !string.IsNullOrEmpty(currentUser.Role) &&
                   currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToAdminLogin()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/Dashboard.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}