using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Admin.ProcessOrders
{
    public partial class ProcessOrders : Page
    {
        private const string DefaultFilter = "all";
        private const string FilterStateKey = "ProcessOrders.Filter";
        private const string PageStateKey = "ProcessOrders.Page";
        private const string SelectedOrderStateKey = "ProcessOrders.SelectedOrderId";
        private const int PageSize = 8;
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
                SetCurrentFilter(NormalizeFilter(Request.QueryString["status"]));
                SetCurrentPage(1);
                SetSelectedOrderId(null);
                BindProcessOrders();
            }
        }

        protected void btnFilter_Command(object sender, CommandEventArgs e)
        {
            var selectedFilter = NormalizeFilter(e == null ? null : Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture));
            SetCurrentFilter(selectedFilter);
            SetCurrentPage(1);
            SetSelectedOrderId(null);

            BindProcessOrders();
        }

        protected void btnPrevPage_Click(object sender, EventArgs e)
        {
            var currentPage = GetCurrentPage();
            SetCurrentPage(Math.Max(1, currentPage - 1));
            BindProcessOrders();
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            var currentPage = GetCurrentPage();
            SetCurrentPage(currentPage + 1);
            BindProcessOrders();
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            SetSelectedOrderId(null);
            pnlOrderDetails.Visible = false;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Remove("AdminRole");
            Session.Remove("CurrentUser");
            Session.Remove("UserId");
            Session.Remove("UserName");
            Session.Remove("UserEmail");

            Response.Redirect("~/Admin/AdminLogin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = e.Item.DataItem as OrderRowVm;
            var ddlStatus = e.Item.FindControl("ddlOrderStatus") as DropDownList;
            if (row == null || ddlStatus == null)
            {
                return;
            }

            var statusItem = ddlStatus.Items.FindByValue(row.StatusValue);
            if (statusItem == null)
            {
                return;
            }

            ddlStatus.ClearSelection();
            statusItem.Selected = true;
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderId) || orderId <= 0)
            {
                ShowSnackbar("Unable to identify the selected order.", "error");
                return;
            }

            if (string.Equals(e.CommandName, "ViewOrder", StringComparison.OrdinalIgnoreCase))
            {
                SetSelectedOrderId(orderId);
                BindProcessOrders();
                return;
            }

            if (!string.Equals(e.CommandName, "UpdateStatus", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var ddlStatus = e.Item.FindControl("ddlOrderStatus") as DropDownList;
            var selectedStatus = NormalizeEditableStatus(ddlStatus == null ? null : ddlStatus.SelectedValue);
            if (string.IsNullOrWhiteSpace(selectedStatus))
            {
                ShowSnackbar("Please choose a valid status.", "error");
                return;
            }

            try
            {
                var updated = UpdateOrderStatus(orderId, selectedStatus);
                if (!updated)
                {
                    ShowSnackbar("Order status could not be updated.", "error");
                    return;
                }

                SetSelectedOrderId(orderId);
                ShowSnackbar("Order status updated to " + selectedStatus + ".", "success");
                BindProcessOrders();
            }
            catch
            {
                ShowSnackbar("Unable to update the order status right now.", "error");
            }
        }

        private void BindProcessOrders()
        {
            ApplyFilterButtonStyles();

            List<OrderRowVm> allOrders;
            try
            {
                allOrders = LoadOrdersFromDatabase();
            }
            catch
            {
                allOrders = new List<OrderRowVm>();
                ShowSnackbar("Unable to load orders from the database.", "error");
            }

            BindStats(allOrders);

            var filter = GetCurrentFilter();
            var filteredOrders = ApplyStatusFilter(allOrders, filter);
            var totalCount = filteredOrders.Count;

            var totalPages = totalCount <= 0
                ? 1
                : (int)Math.Ceiling(totalCount / (double)PageSize);

            var currentPage = GetCurrentPage();
            if (currentPage > totalPages)
            {
                currentPage = totalPages;
                SetCurrentPage(currentPage);
            }

            if (currentPage <= 0)
            {
                currentPage = 1;
                SetCurrentPage(1);
            }

            var skip = (currentPage - 1) * PageSize;
            var pageRows = filteredOrders.Skip(skip).Take(PageSize).ToList();

            rptOrders.DataSource = pageRows;
            rptOrders.DataBind();

            var shownFrom = totalCount == 0 ? 0 : skip + 1;
            var shownTo = totalCount == 0 ? 0 : Math.Min(skip + PageSize, totalCount);

            litShowingFrom.Text = shownFrom.ToString(CultureInfo.InvariantCulture);
            litShowingTo.Text = shownTo.ToString(CultureInfo.InvariantCulture);
            litShowingTotal.Text = totalCount.ToString(CultureInfo.InvariantCulture);
            litCurrentPage.Text = currentPage.ToString(CultureInfo.InvariantCulture);
            litTotalPages.Text = totalPages.ToString(CultureInfo.InvariantCulture);

            var paginationBaseCss = "p-2 border border-outline-variant/20 hover:border-secondary text-on-surface-variant hover:text-secondary transition-all";
            btnPrevPage.Enabled = currentPage > 1;
            btnNextPage.Enabled = currentPage < totalPages;
            btnPrevPage.CssClass = btnPrevPage.Enabled ? paginationBaseCss : paginationBaseCss + " opacity-40 pointer-events-none";
            btnNextPage.CssClass = btnNextPage.Enabled ? paginationBaseCss : paginationBaseCss + " opacity-40 pointer-events-none";

            var selectedOrderId = GetSelectedOrderId();
            if (selectedOrderId.HasValue)
            {
                BindOrderDetails(selectedOrderId.Value, allOrders);
            }
            else
            {
                pnlOrderDetails.Visible = false;
            }
        }

        private void BindOrderDetails(int orderId, IReadOnlyCollection<OrderRowVm> allOrders)
        {
            var order = (allOrders ?? Array.Empty<OrderRowVm>()).FirstOrDefault(item => item.OrderId == orderId);
            if (order == null)
            {
                pnlOrderDetails.Visible = false;
                SetSelectedOrderId(null);
                return;
            }

            var orderItems = LoadOrderItems(orderId);

            litSelectedOrderNumber.Text = HttpUtility.HtmlEncode(order.OrderNumber);
            litSelectedCustomer.Text = HttpUtility.HtmlEncode(order.CustomerName);
            litSelectedPlacedDate.Text = HttpUtility.HtmlEncode(order.PlacedDateLabel);
            litSelectedStatus.Text = HttpUtility.HtmlEncode(order.StatusLabel);
            litSelectedPaymentStatus.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(order.PaymentStatus) ? "Pending" : order.PaymentStatus);
            litSelectedAddress.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(order.Address) ? "N/A" : order.Address);
            litSelectedNotes.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(order.Notes) ? "No notes provided." : order.Notes);

            rptOrderItems.DataSource = orderItems;
            rptOrderItems.DataBind();

            pnlOrderDetails.Visible = true;
        }

        private void BindStats(IReadOnlyCollection<OrderRowVm> allOrders)
        {
            var source = allOrders ?? Array.Empty<OrderRowVm>();

            var openOrders = source.Count(item => !string.Equals(item.StatusGroup, "delivered", StringComparison.OrdinalIgnoreCase) && !string.Equals(item.StatusGroup, "cancel", StringComparison.OrdinalIgnoreCase));
            var pendingShipment = source.Count(item => string.Equals(item.StatusGroup, "pending", StringComparison.OrdinalIgnoreCase));
            var processingDelay = source.Count(item => string.Equals(item.StatusGroup, "pending", StringComparison.OrdinalIgnoreCase) && item.PlacedDate.HasValue && item.PlacedDate.Value < DateTime.Now.AddDays(-2));
            var revenue24h = source.Where(item => item.PlacedDate.HasValue && item.PlacedDate.Value >= DateTime.Now.AddHours(-24)).Sum(item => item.TotalAmount);

            litOpenOrders.Text = openOrders.ToString(CultureInfo.InvariantCulture);
            litPendingShipment.Text = pendingShipment.ToString(CultureInfo.InvariantCulture);
            litProcessingDelay.Text = processingDelay.ToString(CultureInfo.InvariantCulture);
            litRevenue24h.Text = revenue24h.ToString("C2", UsCulture);
        }

        private List<OrderRowVm> LoadOrdersFromDatabase()
        {
            var schema = ResolveOrdersSchema();

            var orderIdExpr = "o." + WrapIdentifier(schema.OrderIdColumn);
            var placedDateExpr = string.IsNullOrWhiteSpace(schema.OrderDateColumn)
                ? "NULL"
                : "o." + WrapIdentifier(schema.OrderDateColumn);
            var totalAmountExpr = string.IsNullOrWhiteSpace(schema.TotalAmountColumn)
                ? "CAST(0 AS DECIMAL(18,2))"
                : "o." + WrapIdentifier(schema.TotalAmountColumn);
            var statusExpr = string.IsNullOrWhiteSpace(schema.StatusColumn)
                ? "'Pending'"
                : "o." + WrapIdentifier(schema.StatusColumn);
            var paymentStatusExpr = string.IsNullOrWhiteSpace(schema.PaymentStatusColumn)
                ? "'Pending'"
                : "o." + WrapIdentifier(schema.PaymentStatusColumn);
            var addressExpr = string.IsNullOrWhiteSpace(schema.AddressColumn)
                ? "''"
                : "o." + WrapIdentifier(schema.AddressColumn);
            var notesExpr = string.IsNullOrWhiteSpace(schema.NotesColumn)
                ? "''"
                : "o." + WrapIdentifier(schema.NotesColumn);

            var joinUsers = string.Empty;
            if (schema.CanJoinUsers)
            {
                joinUsers = string.Concat(
                    " LEFT JOIN [dbo].[Users] u ON u.",
                    WrapIdentifier(schema.UsersUserIdColumn),
                    " = o.",
                    WrapIdentifier(schema.UserIdColumn),
                    " ");
            }

            var customerNameExpr = BuildCustomerNameExpression(schema);
            var customerLabelExpr = BuildCustomerLabelExpression(schema);

            var sql = string.Concat(
                "SELECT ",
                orderIdExpr, " AS [OrderId],",
                customerNameExpr, " AS [CustomerName],",
                customerLabelExpr, " AS [CustomerLabel],",
                placedDateExpr, " AS [PlacedDate],",
                statusExpr, " AS [OrderStatus],",
                paymentStatusExpr, " AS [PaymentStatus],",
                totalAmountExpr, " AS [TotalAmount],",
                addressExpr, " AS [ShipAddress],",
                notesExpr, " AS [OrderNotes] ",
                "FROM [dbo].[Orders] o",
                joinUsers,
                "ORDER BY ",
                string.IsNullOrWhiteSpace(schema.OrderDateColumn) ? orderIdExpr : placedDateExpr,
                " DESC, ",
                orderIdExpr,
                " DESC;");

            var dt = DBHelper.ExecuteDataTable(sql);
            var rows = new List<OrderRowVm>(dt.Rows.Count);

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var orderId = ToInt(row["OrderId"]);
                if (orderId <= 0)
                {
                    continue;
                }

                var customerName = ReadString(row["CustomerName"], "Customer");
                var normalizedStatus = NormalizeStatusLabel(ReadString(row["OrderStatus"], "Pending"));
                var statusGroup = ResolveStatusGroup(normalizedStatus);
                var placedDate = ToDateTimeNullable(row["PlacedDate"]);
                var totalAmount = ToDecimal(row["TotalAmount"]);

                rows.Add(new OrderRowVm
                {
                    OrderId = orderId,
                    OrderNumber = BuildOrderNumber(orderId),
                    CustomerName = customerName,
                    CustomerInitials = BuildCustomerInitials(customerName),
                    CustomerLabel = ReadString(row["CustomerLabel"], "Registered Customer"),
                    PlacedDate = placedDate,
                    PlacedDateLabel = placedDate.HasValue
                        ? placedDate.Value.ToString("MMM dd, yyyy", UsCulture)
                        : "N/A",
                    StatusLabel = normalizedStatus,
                    StatusValue = NormalizeEditableStatus(normalizedStatus),
                    StatusGroup = statusGroup,
                    StatusBadgeCssClass = BuildStatusBadgeCss(normalizedStatus),
                    TotalAmount = totalAmount,
                    TotalLabel = totalAmount.ToString("C2", UsCulture),
                    PaymentStatus = NormalizePaymentStatus(ReadString(row["PaymentStatus"], "Pending")),
                    Address = ReadString(row["ShipAddress"], string.Empty),
                    Notes = ReadString(row["OrderNotes"], string.Empty)
                });
            }

            return rows;
        }

        private List<OrderItemVm> LoadOrderItems(int orderId)
        {
            if (orderId <= 0)
            {
                return new List<OrderItemVm>();
            }

            var schema = ResolveOrderItemsSchema();
            if (!schema.HasOrderItemsTable || string.IsNullOrWhiteSpace(schema.OrderIdColumn))
            {
                return new List<OrderItemVm>();
            }

            var quantityExpr = string.IsNullOrWhiteSpace(schema.QuantityColumn)
                ? "CAST(1 AS INT)"
                : "oitem." + WrapIdentifier(schema.QuantityColumn);
            var unitPriceExpr = string.IsNullOrWhiteSpace(schema.UnitPriceColumn)
                ? "CAST(0 AS DECIMAL(18,2))"
                : "oitem." + WrapIdentifier(schema.UnitPriceColumn);

            var productNameParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(schema.ProductNameColumn))
            {
                productNameParts.Add("NULLIF(LTRIM(RTRIM(oitem." + WrapIdentifier(schema.ProductNameColumn) + ")), '')");
            }

            var joinProducts = string.Empty;
            if (schema.CanJoinProducts)
            {
                joinProducts = string.Concat(
                    " LEFT JOIN [dbo].[Products] p ON p.",
                    WrapIdentifier(schema.ProductsProductIdColumn),
                    " = oitem.",
                    WrapIdentifier(schema.ProductIdColumn),
                    " ");

                productNameParts.Add("NULLIF(LTRIM(RTRIM(p." + WrapIdentifier(schema.ProductsNameColumn) + ")), '')");
            }

            if (!string.IsNullOrWhiteSpace(schema.ProductIdColumn))
            {
                productNameParts.Add("'Product #' + CAST(oitem." + WrapIdentifier(schema.ProductIdColumn) + " AS NVARCHAR(30))");
            }

            productNameParts.Add("'Item'");
            var productNameExpr = "COALESCE(" + string.Join(",", productNameParts) + ")";

            var sizeExpr = string.IsNullOrWhiteSpace(schema.SelectedSizeColumn)
                ? "'N/A'"
                : "COALESCE(NULLIF(LTRIM(RTRIM(oitem." + WrapIdentifier(schema.SelectedSizeColumn) + ")), ''), 'N/A')";

            var orderByExpr = string.IsNullOrWhiteSpace(schema.OrderItemIdColumn)
                ? (string.IsNullOrWhiteSpace(schema.ProductIdColumn) ? quantityExpr : "oitem." + WrapIdentifier(schema.ProductIdColumn))
                : "oitem." + WrapIdentifier(schema.OrderItemIdColumn);

            var sql = string.Concat(
                "SELECT ",
                productNameExpr, " AS [ProductName],",
                sizeExpr, " AS [SelectedSize],",
                quantityExpr, " AS [Quantity],",
                unitPriceExpr, " AS [UnitPrice] ",
                "FROM [dbo].[OrderItems] oitem",
                joinProducts,
                "WHERE oitem.",
                WrapIdentifier(schema.OrderIdColumn),
                " = @OrderId ",
                "ORDER BY ",
                orderByExpr,
                " ASC;");

            var dt = DBHelper.ExecuteDataTable(sql, new[]
            {
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }
            });

            var items = new List<OrderItemVm>(dt.Rows.Count);
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                var quantity = Math.Max(1, ToInt(row["Quantity"]));
                var unitPrice = ToDecimal(row["UnitPrice"]);
                var lineTotal = quantity * unitPrice;

                items.Add(new OrderItemVm
                {
                    ProductName = ReadString(row["ProductName"], "Item"),
                    SelectedSize = ReadString(row["SelectedSize"], "N/A"),
                    Quantity = quantity,
                    UnitPriceLabel = unitPrice.ToString("C2", UsCulture),
                    LineTotalLabel = lineTotal.ToString("C2", UsCulture)
                });
            }

            return items;
        }

        private bool UpdateOrderStatus(int orderId, string status)
        {
            var schema = ResolveOrdersSchema();
            if (string.IsNullOrWhiteSpace(schema.OrderIdColumn) || string.IsNullOrWhiteSpace(schema.StatusColumn))
            {
                return false;
            }

            var sql = string.Concat(
                "UPDATE [dbo].[Orders] SET ",
                WrapIdentifier(schema.StatusColumn),
                " = @Status WHERE ",
                WrapIdentifier(schema.OrderIdColumn),
                " = @OrderId;");

            var rows = DBHelper.ExecuteNonQuery(sql, new[]
            {
                new SqlParameter("@Status", SqlDbType.NVarChar, 30) { Value = status },
                new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId }
            });

            return rows > 0;
        }

        private List<OrderRowVm> ApplyStatusFilter(IEnumerable<OrderRowVm> source, string filter)
        {
            var normalizedFilter = NormalizeFilter(filter);
            var list = (source ?? Enumerable.Empty<OrderRowVm>()).ToList();

            if (normalizedFilter == "all")
            {
                return list;
            }

            return list
                .Where(item => item != null && string.Equals(item.StatusGroup, normalizedFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private void ApplyFilterButtonStyles()
        {
            const string activeCss = "px-6 py-2 text-xs font-label uppercase tracking-widest text-on-secondary bg-secondary";
            const string inactiveCss = "px-6 py-2 text-xs font-label uppercase tracking-widest text-on-surface-variant hover:text-secondary transition-colors";

            var filter = GetCurrentFilter();

            btnFilterAll.CssClass = filter == "all" ? activeCss : inactiveCss;
            btnFilterPending.CssClass = filter == "pending" ? activeCss : inactiveCss;
            btnFilterShipped.CssClass = filter == "out-for-delivery" ? activeCss : inactiveCss;
            btnFilterCancelled.CssClass = filter == "cancel" ? activeCss : inactiveCss;
            btnFilterDelivered.CssClass = filter == "delivered" ? activeCss : inactiveCss;
        }

        private static string BuildCustomerNameExpression(OrdersSchema schema)
        {
            if (!string.IsNullOrWhiteSpace(schema.CustomerNameColumn))
            {
                var orderNameExpr = "NULLIF(LTRIM(RTRIM(o." + WrapIdentifier(schema.CustomerNameColumn) + ")), '')";
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

        private static string BuildCustomerLabelExpression(OrdersSchema schema)
        {
            if (schema.CanJoinUsers && !string.IsNullOrWhiteSpace(schema.UsersUserIdColumn))
            {
                return "CASE WHEN u." + WrapIdentifier(schema.UsersUserIdColumn) + " IS NULL THEN 'Guest Checkout' ELSE 'Registered Customer' END";
            }

            return "'Registered Customer'";
        }

        private static OrdersSchema ResolveOrdersSchema()
        {
            var schema = new OrdersSchema
            {
                OrderIdColumn = ResolveFirstExistingColumn("Orders", "OrderId", "Id"),
                UserIdColumn = ResolveFirstExistingColumn("Orders", "UserId"),
                OrderDateColumn = ResolveFirstExistingColumn("Orders", "OrderDate", "CreatedAt", "CreatedDate"),
                StatusColumn = ResolveFirstExistingColumn("Orders", "Status", "OrderStatus"),
                TotalAmountColumn = ResolveFirstExistingColumn("Orders", "TotalAmount", "GrandTotal", "Amount"),
                CustomerNameColumn = ResolveFirstExistingColumn("Orders", "FullName", "CustomerName"),
                AddressColumn = ResolveFirstExistingColumn("Orders", "DeliveryAddress", "ShipAddress", "Address"),
                NotesColumn = ResolveFirstExistingColumn("Orders", "OrderNotes", "Notes"),
                PaymentStatusColumn = ResolveFirstExistingColumn("Orders", "PaymentStatus")
            };

            if (string.IsNullOrWhiteSpace(schema.OrderIdColumn))
            {
                throw new InvalidOperationException("Orders table does not contain a supported primary key column.");
            }

            schema.UsersUserIdColumn = ResolveFirstExistingColumn("Users", "UserId", "Id");
            schema.UsersFullNameColumn = ResolveFirstExistingColumn("Users", "FullName", "Name");
            schema.CanJoinUsers = !string.IsNullOrWhiteSpace(schema.UserIdColumn) && !string.IsNullOrWhiteSpace(schema.UsersUserIdColumn) && TableExists("Users");

            return schema;
        }

        private static OrderItemsSchema ResolveOrderItemsSchema()
        {
            var schema = new OrderItemsSchema
            {
                HasOrderItemsTable = TableExists("OrderItems"),
                OrderIdColumn = ResolveFirstExistingColumn("OrderItems", "OrderId"),
                OrderItemIdColumn = ResolveFirstExistingColumn("OrderItems", "OrderItemId", "Id"),
                ProductIdColumn = ResolveFirstExistingColumn("OrderItems", "ProductId"),
                ProductNameColumn = ResolveFirstExistingColumn("OrderItems", "ProductName", "Name"),
                SelectedSizeColumn = ResolveFirstExistingColumn("OrderItems", "SelectedSize", "Size"),
                QuantityColumn = ResolveFirstExistingColumn("OrderItems", "Quantity", "Qty"),
                UnitPriceColumn = ResolveFirstExistingColumn("OrderItems", "UnitPrice", "Price")
            };

            schema.ProductsProductIdColumn = ResolveFirstExistingColumn("Products", "ProductId", "id", "Id");
            schema.ProductsNameColumn = ResolveFirstExistingColumn("Products", "Name", "product_name", "ProductName");
            schema.CanJoinProducts = TableExists("Products") &&
                                     !string.IsNullOrWhiteSpace(schema.ProductIdColumn) &&
                                     !string.IsNullOrWhiteSpace(schema.ProductsProductIdColumn) &&
                                     !string.IsNullOrWhiteSpace(schema.ProductsNameColumn);

            return schema;
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

        private bool HasAdminAccess()
        {
            var adminRole = Convert.ToString(Session["AdminRole"]);
            if (!string.IsNullOrWhiteSpace(adminRole) && adminRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var currentUser = Session["CurrentUser"] as User;
            return currentUser != null &&
                   !string.IsNullOrWhiteSpace(currentUser.Role) &&
                   currentUser.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        private void RedirectToAdminLogin()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? "/Admin/ProcessOrders/ProcessOrders.aspx");
            Response.Redirect("~/Admin/AdminLogin.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private static string NormalizeFilter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefaultFilter;
            }

            var normalized = value.Trim().ToLowerInvariant();
            if (normalized == "shipped" || normalized == "in-transit" || normalized == "in transit" || normalized == "out_for_delivery" || normalized == "out for delivery")
            {
                return "out-for-delivery";
            }

            if (normalized == "cancelled" || normalized == "canceled")
            {
                return "cancel";
            }

            if (normalized == "pending" || normalized == "out-for-delivery" || normalized == "delivered" || normalized == "cancel")
            {
                return normalized;
            }

            return DefaultFilter;
        }

        private static string NormalizeStatusLabel(string rawStatus)
        {
            if (string.IsNullOrWhiteSpace(rawStatus))
            {
                return "Pending";
            }

            var normalized = rawStatus.Trim();
            var lowered = normalized.ToLowerInvariant();

            if (lowered.Contains("cancel") || lowered.Contains("fail"))
            {
                return "Cancel";
            }

            if (lowered.Contains("out for delivery") || lowered.Contains("out-for-delivery") || lowered.Contains("in transit") || lowered.Contains("in-transit") || lowered.Contains("ship") || lowered.Contains("transit") || lowered.Contains("dispatch"))
            {
                return "Out for Delivery";
            }

            if (lowered.Contains("deliver") || lowered.Contains("complete") || lowered.Contains("receive"))
            {
                return "Delivered";
            }

            if (lowered.Contains("pending") || lowered.Contains("process") || lowered.Contains("confirm"))
            {
                return "Pending";
            }

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lowered);
        }

        private static string NormalizeEditableStatus(string rawStatus)
        {
            var normalized = NormalizeStatusLabel(rawStatus);

            if (string.Equals(normalized, "Pending", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Out for Delivery", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Delivered", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(normalized, "Cancel", StringComparison.OrdinalIgnoreCase))
            {
                return normalized;
            }

            return "Pending";
        }

        private static string NormalizePaymentStatus(string rawStatus)
        {
            if (string.IsNullOrWhiteSpace(rawStatus))
            {
                return "Pending";
            }

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(rawStatus.Trim().ToLowerInvariant());
        }

        private static string ResolveStatusGroup(string statusLabel)
        {
            var normalized = NormalizeStatusLabel(statusLabel);

            if (string.Equals(normalized, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                return "pending";
            }

            if (string.Equals(normalized, "Out for Delivery", StringComparison.OrdinalIgnoreCase))
            {
                return "out-for-delivery";
            }

            if (string.Equals(normalized, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                return "delivered";
            }

            if (string.Equals(normalized, "Cancel", StringComparison.OrdinalIgnoreCase))
            {
                return "cancel";
            }

            return "pending";
        }

        private static string BuildStatusBadgeCss(string statusLabel)
        {
            var normalized = NormalizeStatusLabel(statusLabel);

            if (string.Equals(normalized, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                return "inline-flex items-center px-2 py-0.5 text-[10px] font-label uppercase tracking-widest bg-on-primary-container/20 text-primary";
            }

            if (string.Equals(normalized, "Out for Delivery", StringComparison.OrdinalIgnoreCase))
            {
                return "inline-flex items-center px-2 py-0.5 text-[10px] font-label uppercase tracking-widest bg-secondary-container/20 text-secondary";
            }

            if (string.Equals(normalized, "Cancel", StringComparison.OrdinalIgnoreCase))
            {
                return "inline-flex items-center px-2 py-0.5 text-[10px] font-label uppercase tracking-widest bg-error-container/20 text-error";
            }

            return "inline-flex items-center px-2 py-0.5 text-[10px] font-label uppercase tracking-widest bg-surface-container-high text-on-surface-variant";
        }

        private static string BuildOrderNumber(int orderId)
        {
            if (orderId <= 0)
            {
                return "#AZ-0000";
            }

            return string.Format(CultureInfo.InvariantCulture, "#AZ-{0:0000}", orderId);
        }

        private static string BuildCustomerInitials(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return "AZ";
            }

            var tokens = customerName
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .ToArray();

            if (tokens.Length == 0)
            {
                return "AZ";
            }

            if (tokens.Length == 1)
            {
                var token = tokens[0].Trim();
                return token.Length >= 2
                    ? token.Substring(0, 2).ToUpperInvariant()
                    : token.Substring(0, 1).ToUpperInvariant() + "Z";
            }

            return string.Concat(tokens[0][0], tokens[1][0]).ToUpperInvariant();
        }

        private static string ReadString(object value, string fallback)
        {
            var parsed = value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture);

            return string.IsNullOrWhiteSpace(parsed) ? fallback : parsed.Trim();
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

        private static DateTime? ToDateTimeNullable(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is DateTime directDateTime)
            {
                return directDateTime;
            }

            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
            {
                return parsed;
            }

            return null;
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

        private string GetCurrentFilter()
        {
            return NormalizeFilter(Convert.ToString(ViewState[FilterStateKey], CultureInfo.InvariantCulture));
        }

        private void SetCurrentFilter(string filter)
        {
            ViewState[FilterStateKey] = NormalizeFilter(filter);
        }

        private int GetCurrentPage()
        {
            var value = Convert.ToString(ViewState[PageStateKey], CultureInfo.InvariantCulture);
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var page) && page > 0)
            {
                return page;
            }

            return 1;
        }

        private void SetCurrentPage(int page)
        {
            ViewState[PageStateKey] = page <= 0 ? 1 : page;
        }

        private int? GetSelectedOrderId()
        {
            var value = Convert.ToString(ViewState[SelectedOrderStateKey], CultureInfo.InvariantCulture);
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderId) && orderId > 0)
            {
                return orderId;
            }

            return null;
        }

        private void SetSelectedOrderId(int? orderId)
        {
            if (!orderId.HasValue || orderId.Value <= 0)
            {
                ViewState[SelectedOrderStateKey] = null;
                return;
            }

            ViewState[SelectedOrderStateKey] = orderId.Value;
        }

        private void ShowSnackbar(string message, string type)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            var script = string.Concat(
                "window.setTimeout(function(){",
                "var showInlineSnackbar=function(message,variant){",
                "var host=document.getElementById('az-inline-snackbar-host');",
                "var toast=document.getElementById('az-inline-snackbar');",
                "if(!host||!toast){",
                "host=document.createElement('div');",
                "host.id='az-inline-snackbar-host';",
                "host.style.cssText='position:fixed;top:1.25rem;right:1.25rem;z-index:9999;pointer-events:none;';",
                "toast=document.createElement('div');",
                "toast.id='az-inline-snackbar';",
                "toast.style.cssText='min-width:280px;max-width:420px;padding:0.85rem 1rem;border:1px solid rgba(255,255,255,0.1);background:rgba(20,20,20,0.92);color:#f5f0e8;font-size:0.82rem;letter-spacing:0.03em;box-shadow:0 12px 26px rgba(0,0,0,0.35);backdrop-filter:blur(8px);transform:translateY(-10px);opacity:0;transition:transform 220ms ease,opacity 220ms ease;';",
                "host.appendChild(toast);",
                "document.body.appendChild(host);",
                "}",
                "var accent='#e9c349';",
                "if(variant==='success'){accent='#22c55e';}else if(variant==='error'){accent='#ef4444';}else if(variant==='info'){accent='#60a5fa';}",
                "toast.style.borderColor=accent;",
                "toast.textContent=message||'';",
                "toast.style.opacity='1';",
                "toast.style.transform='translateY(0)';",
                "window.clearTimeout(window.__azInlineSnackbarTimer);",
                "window.__azInlineSnackbarTimer=window.setTimeout(function(){toast.style.opacity='0';toast.style.transform='translateY(-10px)';},3400);",
                "};",
                "if(window.azSnackbar&&typeof window.azSnackbar.show==='function'){window.azSnackbar.show('",
                safeMessage,
                "','",
                safeType,
                "');}else{showInlineSnackbar('",
                safeMessage,
                "','",
                safeType,
                "');}",
                "},0);");

            ScriptManager.RegisterStartupScript(this, GetType(), "processOrdersSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class OrdersSchema
        {
            public string OrderIdColumn { get; set; }

            public string UserIdColumn { get; set; }

            public string OrderDateColumn { get; set; }

            public string StatusColumn { get; set; }

            public string TotalAmountColumn { get; set; }

            public string CustomerNameColumn { get; set; }

            public string AddressColumn { get; set; }

            public string NotesColumn { get; set; }

            public string PaymentStatusColumn { get; set; }

            public string UsersUserIdColumn { get; set; }

            public string UsersFullNameColumn { get; set; }

            public bool CanJoinUsers { get; set; }
        }

        private sealed class OrderItemsSchema
        {
            public bool HasOrderItemsTable { get; set; }

            public string OrderIdColumn { get; set; }

            public string OrderItemIdColumn { get; set; }

            public string ProductIdColumn { get; set; }

            public string ProductNameColumn { get; set; }

            public string SelectedSizeColumn { get; set; }

            public string QuantityColumn { get; set; }

            public string UnitPriceColumn { get; set; }

            public string ProductsProductIdColumn { get; set; }

            public string ProductsNameColumn { get; set; }

            public bool CanJoinProducts { get; set; }
        }

        private sealed class OrderRowVm
        {
            public int OrderId { get; set; }

            public string OrderNumber { get; set; }

            public string CustomerName { get; set; }

            public string CustomerInitials { get; set; }

            public string CustomerLabel { get; set; }

            public DateTime? PlacedDate { get; set; }

            public string PlacedDateLabel { get; set; }

            public string StatusLabel { get; set; }

            public string StatusValue { get; set; }

            public string StatusGroup { get; set; }

            public string StatusBadgeCssClass { get; set; }

            public decimal TotalAmount { get; set; }

            public string TotalLabel { get; set; }

            public string PaymentStatus { get; set; }

            public string Address { get; set; }

            public string Notes { get; set; }
        }

        private sealed class OrderItemVm
        {
            public string ProductName { get; set; }

            public string SelectedSize { get; set; }

            public int Quantity { get; set; }

            public string UnitPriceLabel { get; set; }

            public string LineTotalLabel { get; set; }
        }
    }
}