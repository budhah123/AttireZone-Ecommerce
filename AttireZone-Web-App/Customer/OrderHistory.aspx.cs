using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace AttireZone_Web_App.Customer
{
    public partial class OrderHistory : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOrderHistory();
            }
        }

        protected void btnLoadMore_Click(object sender, EventArgs e)
        {
            ShowSnackbar("No more orders found in your history.", "info");
        }

        private void BindOrderHistory()
        {
            var orders = new List<OrderHistoryItem>
            {
                new OrderHistoryItem
                {
                    OrderNumber = "Order #AZ-87102",
                    PlacedDate = "Sept 12, 2024",
                    ItemsHtml = BuildItemMarkup("Silk Scarf (x2)", "Italian Leather Boots"),
                    StatusIcon = "check_circle",
                    StatusLabel = "Delivered",
                    StatusCssClass = "text-green-400",
                    Total = "$890.00",
                    ActionText = "View Details"
                },
                new OrderHistoryItem
                {
                    OrderNumber = "Order #AZ-86004",
                    PlacedDate = "August 05, 2024",
                    ItemsHtml = BuildItemMarkup("Cashmere Crewneck"),
                    StatusIcon = "cancel",
                    StatusLabel = "Cancelled",
                    StatusCssClass = "text-error",
                    Total = "$340.00",
                    ActionText = "Reorder Items"
                }
            };

            gvOrderHistory.DataSource = orders;
            gvOrderHistory.DataBind();
        }

        private static string BuildItemMarkup(params string[] items)
        {
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                builder.Append("<div class=\"px-3 py-1 bg-surface-container-low border border-outline-variant/10 text-[10px] uppercase tracking-tighter\">");
                builder.Append(HttpUtility.HtmlEncode(item));
                builder.Append("</div>");
            }

            return builder.ToString();
        }

        private void ShowSnackbar(string message, string type)
        {
            var safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            var safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            var script = string.Format(
                "window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} }}, 0);",
                safeMessage,
                safeType);

            ScriptManager.RegisterStartupScript(this, GetType(), "orderHistorySnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }

        private sealed class OrderHistoryItem
        {
            public string OrderNumber { get; set; }

            public string PlacedDate { get; set; }

            public string ItemsHtml { get; set; }

            public string StatusIcon { get; set; }

            public string StatusLabel { get; set; }

            public string StatusCssClass { get; set; }

            public string Total { get; set; }

            public string ActionText { get; set; }
        }
    }
}
