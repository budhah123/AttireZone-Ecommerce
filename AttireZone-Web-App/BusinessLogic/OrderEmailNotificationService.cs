using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace AttireZone_Web_App.BusinessLogic
{
    public static class OrderEmailNotificationService
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["AttireZone"] == null
            ? string.Empty
            : ConfigurationManager.ConnectionStrings["AttireZone"].ConnectionString;

        private const int DefaultEstimatedDeliveryMinDays = 3;
        private const int DefaultEstimatedDeliveryMaxDays = 5;

        public static EmailNotificationResult SendOrderConfirmationEmail(int orderId, decimal totalAmount, string paymentMethod, string paymentStatus, string gatewayTransactionId)
        {
            if (orderId <= 0)
            {
                return EmailNotificationResult.Fail("Order id is invalid for email notification.");
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                return EmailNotificationResult.Fail("Connection string 'AttireZone' is not configured.");
            }

            try
            {
                var orderSnapshot = LoadOrderSnapshot(orderId);
                if (orderSnapshot == null)
                {
                    return EmailNotificationResult.Fail("Order information could not be found for email notification.");
                }

                if (string.IsNullOrWhiteSpace(orderSnapshot.CustomerEmail) || !IsValidEmail(orderSnapshot.CustomerEmail))
                {
                    return EmailNotificationResult.Fail("Customer email is missing or invalid.");
                }

                var orderItems = LoadOrderItems(orderId);
                if (orderItems.Count == 0)
                {
                    return EmailNotificationResult.Fail("Order items are missing; unable to prepare confirmation email.");
                }

                if (!TryBuildSmtpSettings(out var smtpSettings, out var smtpValidationError))
                {
                    return EmailNotificationResult.Fail(smtpValidationError);
                }

                var normalizedPaymentMethod = string.IsNullOrWhiteSpace(paymentMethod)
                    ? (string.IsNullOrWhiteSpace(orderSnapshot.PaymentMethod) ? "Online Payment" : orderSnapshot.PaymentMethod)
                    : paymentMethod.Trim();
                var normalizedPaymentStatus = string.IsNullOrWhiteSpace(paymentStatus) ? "Successful" : paymentStatus.Trim();

                var subtotal = orderItems.Sum(item => item.LineTotal);
                var resolvedTotalAmount = totalAmount > 0m ? totalAmount : subtotal;
                var resolvedShippingCharge = resolvedTotalAmount - subtotal;
                if (resolvedShippingCharge < 0m)
                {
                    resolvedShippingCharge = 0m;
                }

                var estimatedDeliveryText = BuildEstimatedDeliveryText();
                var subject = BuildEmailSubject(orderId);
                var htmlBody = BuildHtmlBody(
                    orderSnapshot,
                    orderItems,
                    subtotal,
                    resolvedShippingCharge,
                    resolvedTotalAmount,
                    normalizedPaymentMethod,
                    normalizedPaymentStatus,
                    gatewayTransactionId,
                    estimatedDeliveryText);
                var textBody = BuildTextBody(
                    orderSnapshot,
                    orderItems,
                    subtotal,
                    resolvedShippingCharge,
                    resolvedTotalAmount,
                    normalizedPaymentMethod,
                    normalizedPaymentStatus,
                    gatewayTransactionId,
                    estimatedDeliveryText);

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpSettings.FromAddress, smtpSettings.FromDisplayName);
                    mailMessage.To.Add(new MailAddress(orderSnapshot.CustomerEmail));
                    mailMessage.Subject = subject;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = htmlBody;

                    var textView = AlternateView.CreateAlternateViewFromString(textBody, Encoding.UTF8, "text/plain");
                    mailMessage.AlternateViews.Add(textView);

                    using (var smtpClient = new SmtpClient(smtpSettings.Host, smtpSettings.Port))
                    {
                        smtpClient.EnableSsl = smtpSettings.EnableSsl;
                        smtpClient.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Timeout = smtpSettings.TimeoutMilliseconds;

                        if (!smtpSettings.UseDefaultCredentials && !string.IsNullOrWhiteSpace(smtpSettings.Username))
                        {
                            smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password ?? string.Empty);
                        }

                        smtpClient.Send(mailMessage);
                    }
                }

                Debug.WriteLine("[OrderEmail] Confirmation email sent for orderId=" + orderId);
                return EmailNotificationResult.Success();
            }
            catch (SmtpException ex)
            {
                Debug.WriteLine("[OrderEmail] SMTP send failed for orderId=" + orderId + ". Status=" + ex.StatusCode + ". " + ex.Message);
                return EmailNotificationResult.Fail("SMTP send failed: " + ex.StatusCode + " - " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[OrderEmail] Failed to send confirmation email for orderId=" + orderId + ". " + ex.Message);
                return EmailNotificationResult.Fail("Unexpected error while sending order confirmation email: " + ex.Message);
            }
        }

        private static OrderSnapshot LoadOrderSnapshot(int orderId)
        {
            const string sql = @"
SELECT TOP 1
    o.[Id],
    o.[FullName],
    o.[DeliveryAddress],
    o.[PaymentMethod],
    u.[Email]
FROM [dbo].[Orders] o
LEFT JOIN [dbo].[Users] u ON u.[UserId] = o.[UserId]
WHERE o.[Id] = @OrderId;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

                connection.Open();
                using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new OrderSnapshot
                    {
                        OrderId = ReadInt(reader, "Id"),
                        FullName = ReadString(reader, "FullName", "Customer"),
                        DeliveryAddress = ReadString(reader, "DeliveryAddress", string.Empty),
                        PaymentMethod = ReadString(reader, "PaymentMethod", string.Empty),
                        CustomerEmail = ReadString(reader, "Email", string.Empty)
                    };
                }
            }
        }

        private static List<OrderItemSnapshot> LoadOrderItems(int orderId)
        {
            const string sql = @"
SELECT
    [ProductName],
    [SelectedSize],
    [Quantity],
    [UnitPrice]
FROM [dbo].[OrderItems]
WHERE [OrderId] = @OrderId;";

            var items = new List<OrderItemSnapshot>();

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var quantity = ReadInt(reader, "Quantity");
                        if (quantity <= 0)
                        {
                            continue;
                        }

                        var unitPrice = ReadDecimal(reader, "UnitPrice");
                        if (unitPrice < 0m)
                        {
                            unitPrice = 0m;
                        }

                        var productName = ReadString(reader, "ProductName", "Product");
                        var selectedSize = ReadString(reader, "SelectedSize", "N/A");

                        items.Add(new OrderItemSnapshot
                        {
                            ProductName = productName,
                            SelectedSize = selectedSize,
                            Quantity = quantity,
                            UnitPrice = unitPrice
                        });
                    }
                }
            }

            return items;
        }

        private static bool TryBuildSmtpSettings(out SmtpSettings settings, out string validationError)
        {
            settings = null;
            validationError = null;

            var host = ReadSetting("Email:SmtpHost", string.Empty);
            if (string.IsNullOrWhiteSpace(host) || host.IndexOf("your-provider", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                validationError = "SMTP host is not configured. Set appSettings key 'Email:SmtpHost'.";
                return false;
            }

            var fromAddress = ReadSetting("Email:FromAddress", string.Empty);
            if (string.IsNullOrWhiteSpace(fromAddress) || !IsValidEmail(fromAddress))
            {
                validationError = "From email address is missing or invalid. Set appSettings key 'Email:FromAddress'.";
                return false;
            }

            var username = ReadSetting("Email:Username", string.Empty);
            var password = ReadSetting("Email:Password", string.Empty);
            var useDefaultCredentials = ParseBool(ReadSetting("Email:UseDefaultCredentials", "false"), false);

            if (!useDefaultCredentials && string.IsNullOrWhiteSpace(username))
            {
                validationError = "SMTP username is missing. Set appSettings key 'Email:Username' or enable default credentials.";
                return false;
            }

            if (!useDefaultCredentials && string.IsNullOrWhiteSpace(password))
            {
                validationError = "SMTP password is missing for the configured SMTP username.";
                return false;
            }

            if (!useDefaultCredentials && IsKnownPlaceholder(username))
            {
                validationError = "SMTP username still uses a placeholder value. Set appSettings key 'Email:Username' to your real mailbox username.";
                return false;
            }

            if (!useDefaultCredentials && IsKnownPlaceholder(password))
            {
                validationError = "SMTP password still uses a placeholder value. Set appSettings key 'Email:Password' to your real app password.";
                return false;
            }

            settings = new SmtpSettings
            {
                Host = host.Trim(),
                Port = ParseInt(ReadSetting("Email:SmtpPort", "587"), 587),
                EnableSsl = ParseBool(ReadSetting("Email:EnableSsl", "true"), true),
                UseDefaultCredentials = useDefaultCredentials,
                Username = username.Trim(),
                Password = password.Trim(),
                FromAddress = fromAddress.Trim(),
                FromDisplayName = ReadSetting("Email:FromDisplayName", "AttireZone"),
                TimeoutMilliseconds = ParseInt(ReadSetting("Email:SmtpTimeoutMs", "15000"), 15000)
            };

            return true;
        }

        private static string BuildEmailSubject(int orderId)
        {
            var subjectTemplate = ReadSetting("Email:OrderConfirmationSubjectTemplate", "Your order has been successfully placed - Order #{0}");

            try
            {
                return string.Format(CultureInfo.InvariantCulture, subjectTemplate, orderId);
            }
            catch (FormatException)
            {
                return string.Format(CultureInfo.InvariantCulture, "Your order has been successfully placed - Order #{0}", orderId);
            }
        }

        private static string BuildEstimatedDeliveryText()
        {
            var minDays = ParseInt(ReadSetting("Order:EstimatedDeliveryDaysMin", DefaultEstimatedDeliveryMinDays.ToString(CultureInfo.InvariantCulture)), DefaultEstimatedDeliveryMinDays);
            var maxDays = ParseInt(ReadSetting("Order:EstimatedDeliveryDaysMax", DefaultEstimatedDeliveryMaxDays.ToString(CultureInfo.InvariantCulture)), DefaultEstimatedDeliveryMaxDays);

            if (minDays < 1)
            {
                minDays = DefaultEstimatedDeliveryMinDays;
            }

            if (maxDays < minDays)
            {
                maxDays = minDays;
            }

            var start = DateTime.Now.Date.AddDays(minDays);
            var end = DateTime.Now.Date.AddDays(maxDays);

            if (start == end)
            {
                return start.ToString("dddd, dd MMM yyyy", CultureInfo.InvariantCulture);
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} to {1}",
                start.ToString("dddd, dd MMM yyyy", CultureInfo.InvariantCulture),
                end.ToString("dddd, dd MMM yyyy", CultureInfo.InvariantCulture));
        }

        private static string BuildHtmlBody(
            OrderSnapshot order,
            IEnumerable<OrderItemSnapshot> items,
            decimal subtotal,
            decimal shippingCharge,
            decimal totalAmount,
            string paymentMethod,
            string paymentStatus,
            string gatewayTransactionId,
            string estimatedDeliveryText)
        {
            var encodedName = HtmlEncode(string.IsNullOrWhiteSpace(order.FullName) ? "Customer" : order.FullName);
            var encodedAddress = HtmlEncode(string.IsNullOrWhiteSpace(order.DeliveryAddress) ? "Address not provided" : order.DeliveryAddress);
            var encodedPaymentMethod = HtmlEncode(paymentMethod);
            var encodedPaymentStatus = HtmlEncode(paymentStatus);
            var encodedTransactionId = HtmlEncode(string.IsNullOrWhiteSpace(gatewayTransactionId) ? "N/A" : gatewayTransactionId);
            var encodedEstimatedDelivery = HtmlEncode(estimatedDeliveryText);

            var itemRowsBuilder = new StringBuilder();
            foreach (var item in items)
            {
                itemRowsBuilder.Append("<tr>");
                itemRowsBuilder.Append("<td style='padding:10px;border-bottom:1px solid #e5e7eb;color:#111827;font-size:14px;'>");
                itemRowsBuilder.Append(HtmlEncode(item.ProductName));
                itemRowsBuilder.Append("</td>");
                itemRowsBuilder.Append("<td style='padding:10px;border-bottom:1px solid #e5e7eb;color:#374151;font-size:13px;text-align:center;'>");
                itemRowsBuilder.Append(HtmlEncode(item.SelectedSize));
                itemRowsBuilder.Append("</td>");
                itemRowsBuilder.Append("<td style='padding:10px;border-bottom:1px solid #e5e7eb;color:#374151;font-size:13px;text-align:center;'>");
                itemRowsBuilder.Append(item.Quantity.ToString(CultureInfo.InvariantCulture));
                itemRowsBuilder.Append("</td>");
                itemRowsBuilder.Append("<td style='padding:10px;border-bottom:1px solid #e5e7eb;color:#374151;font-size:13px;text-align:right;'>");
                itemRowsBuilder.Append(HtmlEncode(FormatCurrency(item.UnitPrice)));
                itemRowsBuilder.Append("</td>");
                itemRowsBuilder.Append("<td style='padding:10px;border-bottom:1px solid #e5e7eb;color:#111827;font-size:13px;text-align:right;font-weight:600;'>");
                itemRowsBuilder.Append(HtmlEncode(FormatCurrency(item.LineTotal)));
                itemRowsBuilder.Append("</td>");
                itemRowsBuilder.Append("</tr>");
            }

            var builder = new StringBuilder();
            builder.Append("<!DOCTYPE html><html><head><meta charset='utf-8' /></head><body style='margin:0;padding:0;background:#f3f4f6;font-family:Segoe UI,Arial,sans-serif;'>");
            builder.Append("<table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background:#f3f4f6;padding:24px 10px;'>");
            builder.Append("<tr><td align='center'>");
            builder.Append("<table role='presentation' cellpadding='0' cellspacing='0' width='680' style='max-width:680px;background:#ffffff;border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;'>");

            builder.Append("<tr><td style='padding:24px 28px;background:#111827;color:#ffffff;'>");
            builder.Append("<h1 style='margin:0;font-size:22px;line-height:1.3;font-weight:700;'>Your order has been successfully placed</h1>");
            builder.Append("<p style='margin:10px 0 0 0;font-size:14px;line-height:1.5;color:#d1d5db;'>Thanks for shopping with AttireZone, ");
            builder.Append(encodedName);
            builder.Append(". Your payment has been confirmed and your order is now being prepared.</p>");
            builder.Append("</td></tr>");

            builder.Append("<tr><td style='padding:22px 28px 10px 28px;'>");
            builder.Append("<table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;'>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Order ID</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;font-weight:600;'>#");
            builder.Append(order.OrderId.ToString(CultureInfo.InvariantCulture));
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Payment Method</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;'>");
            builder.Append(encodedPaymentMethod);
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Payment Status</td><td style='font-size:13px;color:#16a34a;padding:4px 0;text-align:right;font-weight:700;'>");
            builder.Append(encodedPaymentStatus);
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Transaction ID</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;'>");
            builder.Append(encodedTransactionId);
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Estimated Delivery</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;'>");
            builder.Append(encodedEstimatedDelivery);
            builder.Append("</td></tr>");
            builder.Append("</table>");
            builder.Append("</td></tr>");

            builder.Append("<tr><td style='padding:8px 28px 0 28px;'><h2 style='margin:0;font-size:16px;color:#111827;'>Items Purchased</h2></td></tr>");
            builder.Append("<tr><td style='padding:12px 28px 0 28px;'>");
            builder.Append("<table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;border:1px solid #e5e7eb;'>");
            builder.Append("<tr style='background:#f9fafb;'>");
            builder.Append("<th style='padding:10px;text-align:left;color:#374151;font-size:12px;text-transform:uppercase;letter-spacing:0.04em;'>Item</th>");
            builder.Append("<th style='padding:10px;text-align:center;color:#374151;font-size:12px;text-transform:uppercase;letter-spacing:0.04em;'>Size</th>");
            builder.Append("<th style='padding:10px;text-align:center;color:#374151;font-size:12px;text-transform:uppercase;letter-spacing:0.04em;'>Qty</th>");
            builder.Append("<th style='padding:10px;text-align:right;color:#374151;font-size:12px;text-transform:uppercase;letter-spacing:0.04em;'>Unit Price</th>");
            builder.Append("<th style='padding:10px;text-align:right;color:#374151;font-size:12px;text-transform:uppercase;letter-spacing:0.04em;'>Total</th>");
            builder.Append("</tr>");
            builder.Append(itemRowsBuilder.ToString());
            builder.Append("</table>");
            builder.Append("</td></tr>");

            builder.Append("<tr><td style='padding:16px 28px 4px 28px;'>");
            builder.Append("<table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='border-collapse:collapse;'>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Subtotal</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;'>");
            builder.Append(HtmlEncode(FormatCurrency(subtotal)));
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:13px;color:#6b7280;padding:4px 0;'>Shipping</td><td style='font-size:13px;color:#111827;padding:4px 0;text-align:right;'>");
            builder.Append(HtmlEncode(FormatCurrency(shippingCharge)));
            builder.Append("</td></tr>");
            builder.Append("<tr><td style='font-size:15px;color:#111827;padding:8px 0;font-weight:700;'>Total Amount</td><td style='font-size:15px;color:#111827;padding:8px 0;text-align:right;font-weight:700;'>");
            builder.Append(HtmlEncode(FormatCurrency(totalAmount)));
            builder.Append("</td></tr>");
            builder.Append("</table>");
            builder.Append("</td></tr>");

            builder.Append("<tr><td style='padding:0 28px 22px 28px;'>");
            builder.Append("<p style='margin:10px 0 0 0;font-size:13px;line-height:1.5;color:#4b5563;'><strong>Delivery Address:</strong> ");
            builder.Append(encodedAddress);
            builder.Append("</p>");
            builder.Append("<p style='margin:10px 0 0 0;font-size:12px;line-height:1.5;color:#6b7280;'>If you have questions, reply to this email and our support team will help you.</p>");
            builder.Append("</td></tr>");

            builder.Append("</table>");
            builder.Append("</td></tr></table>");
            builder.Append("</body></html>");

            return builder.ToString();
        }

        private static string BuildTextBody(
            OrderSnapshot order,
            IEnumerable<OrderItemSnapshot> items,
            decimal subtotal,
            decimal shippingCharge,
            decimal totalAmount,
            string paymentMethod,
            string paymentStatus,
            string gatewayTransactionId,
            string estimatedDeliveryText)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Your order has been successfully placed.");
            builder.AppendLine();
            builder.AppendLine("Order Details");
            builder.AppendLine("Order ID: #" + order.OrderId.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("Payment Method: " + paymentMethod);
            builder.AppendLine("Payment Status: " + paymentStatus);
            builder.AppendLine("Transaction ID: " + (string.IsNullOrWhiteSpace(gatewayTransactionId) ? "N/A" : gatewayTransactionId));
            builder.AppendLine("Estimated Delivery: " + estimatedDeliveryText);
            builder.AppendLine();
            builder.AppendLine("Items Purchased");

            foreach (var item in items)
            {
                builder.AppendLine(string.Format(
                    CultureInfo.InvariantCulture,
                    "- {0} | Size: {1} | Qty: {2} | Unit: {3} | Line Total: {4}",
                    item.ProductName,
                    item.SelectedSize,
                    item.Quantity,
                    FormatCurrency(item.UnitPrice),
                    FormatCurrency(item.LineTotal)));
            }

            builder.AppendLine();
            builder.AppendLine("Subtotal: " + FormatCurrency(subtotal));
            builder.AppendLine("Shipping: " + FormatCurrency(shippingCharge));
            builder.AppendLine("Total Amount: " + FormatCurrency(totalAmount));
            builder.AppendLine();
            builder.AppendLine("Delivery Address: " + (string.IsNullOrWhiteSpace(order.DeliveryAddress) ? "Address not provided" : order.DeliveryAddress));

            return builder.ToString();
        }

        private static string ReadSetting(string key, string fallback)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            // Allow secret overrides like Email__Password to avoid checking credentials into source control.
            var envKey = (key ?? string.Empty).Replace(":", "__");
            value = Environment.GetEnvironmentVariable(envKey, EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(value))
            {
                value = Environment.GetEnvironmentVariable(envKey, EnvironmentVariableTarget.User);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                value = Environment.GetEnvironmentVariable(envKey, EnvironmentVariableTarget.Machine);
            }

            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static string ReadString(SqlDataReader reader, string columnName, string fallback)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return fallback;
            }

            var value = Convert.ToString(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static int ReadInt(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return 0;
            }

            return Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static decimal ReadDecimal(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return 0m;
            }

            return Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static bool ParseBool(string rawValue, bool fallback)
        {
            if (bool.TryParse(rawValue, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        private static int ParseInt(string rawValue, int fallback)
        {
            if (int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        private static bool IsKnownPlaceholder(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            var normalized = rawValue.Trim().ToLowerInvariant();
            return normalized.StartsWith("your-", StringComparison.Ordinal) ||
                   string.Equals(normalized, "changeme", StringComparison.Ordinal) ||
                   string.Equals(normalized, "replace-me", StringComparison.Ordinal) ||
                   normalized.IndexOf("your-provider", StringComparison.Ordinal) >= 0;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var parsed = new MailAddress(email);
                return string.Equals(parsed.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string FormatCurrency(decimal amount)
        {
            return string.Format(CultureInfo.InvariantCulture, "NPR {0:0.00}", amount);
        }

        private static string HtmlEncode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }

        private sealed class SmtpSettings
        {
            public string Host { get; set; }

            public int Port { get; set; }

            public bool EnableSsl { get; set; }

            public bool UseDefaultCredentials { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            public string FromAddress { get; set; }

            public string FromDisplayName { get; set; }

            public int TimeoutMilliseconds { get; set; }
        }

        private sealed class OrderSnapshot
        {
            public int OrderId { get; set; }

            public string FullName { get; set; }

            public string DeliveryAddress { get; set; }

            public string PaymentMethod { get; set; }

            public string CustomerEmail { get; set; }
        }

        private sealed class OrderItemSnapshot
        {
            public string ProductName { get; set; }

            public string SelectedSize { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal LineTotal
            {
                get { return Quantity * UnitPrice; }
            }
        }

        public sealed class EmailNotificationResult
        {
            public bool IsSuccess { get; private set; }

            public string ErrorMessage { get; private set; }

            public static EmailNotificationResult Success()
            {
                return new EmailNotificationResult
                {
                    IsSuccess = true,
                    ErrorMessage = string.Empty
                };
            }

            public static EmailNotificationResult Fail(string errorMessage)
            {
                return new EmailNotificationResult
                {
                    IsSuccess = false,
                    ErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
                        ? "Unable to send order confirmation email."
                        : errorMessage.Trim()
                };
            }
        }
    }
}