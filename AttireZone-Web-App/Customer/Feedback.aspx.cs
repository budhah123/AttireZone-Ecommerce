using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;
using ProductModel = AttireZone_Web_App.Models.Product;
using FeedbackModel = AttireZone_Web_App.Models.Feedback;

namespace AttireZone_Web_App.Customer
{
    public partial class Feedback : System.Web.UI.Page
    {
        private const string DefaultProductImageVirtualPath = "~/Assets/Images/Hero-Section-Image.png";
        private const int MinimumCommentLength = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out _))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!TryReadProductId(out var productId))
            {
                ShowFeedbackUnavailable("Please choose a valid product before submitting feedback.");
                return;
            }

            if (!IsPostBack)
            {
                BindFeedbackContext(productId);
            }
        }

        protected void btnSubmitFeedback_Click(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLoginWithReturnUrl();
                return;
            }

            if (!TryReadProductId(out var productId))
            {
                ShowFormMessage("Please select a valid product before submitting a review.", true);
                return;
            }

            var product = LoadProduct(productId);
            if (product == null)
            {
                ShowFeedbackUnavailable("The selected product could not be found.");
                return;
            }

            if (!TryParseRating(ddlRating == null ? null : ddlRating.SelectedValue, out var rating))
            {
                ShowFormMessage("Please choose a rating between 1 and 5 stars.", true);
                return;
            }

            var normalizedComment = NormalizeComment(txtComment == null ? null : txtComment.Text);
            if (string.IsNullOrWhiteSpace(normalizedComment) || normalizedComment.Length < MinimumCommentLength)
            {
                ShowFormMessage("Please write a short comment with at least 5 characters.", true);
                return;
            }

            try
            {
                var feedback = new FeedbackModel
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating,
                    Comment = normalizedComment
                };

                if (!FeedbackService.AddFeedback(feedback))
                {
                    ShowFormMessage("Unable to submit your review right now. Please try again.", true);
                    return;
                }

                var redirectUrl = string.Concat(
                    ResolveUrl("~/Pages/ProductDetails.aspx"),
                    "?id=",
                    productId.ToString(CultureInfo.InvariantCulture),
                    "&review=submitted");

                Response.Redirect(redirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                ShowFormMessage("Unable to submit your review right now. Please try again.", true);
            }
        }

        private void BindFeedbackContext(int productId)
        {
            var product = LoadProduct(productId);
            if (product == null)
            {
                ShowFeedbackUnavailable("The selected product could not be found.");
                return;
            }

            var safeProductName = string.IsNullOrWhiteSpace(product.ProductName)
                ? "Curated Product"
                : product.ProductName.Trim();

            litProductName.Text = HttpUtility.HtmlEncode(safeProductName);
            litProductPrice.Text = HttpUtility.HtmlEncode(product.Price.ToString("$0.00", CultureInfo.InvariantCulture));
            imgProductPreview.ImageUrl = ResolveProductImageUrl(product.ImagePath);
            imgProductPreview.AlternateText = safeProductName;

            var productDetailsUrl = BuildProductDetailsUrl(productId);
            lnkBackToProduct.NavigateUrl = productDetailsUrl;
            lnkCancelFeedback.NavigateUrl = productDetailsUrl;

            lblFeedbackMessage.Visible = false;
            phFeedbackUnavailable.Visible = false;
            phFeedbackForm.Visible = true;
        }

        private ProductModel LoadProduct(int productId)
        {
            if (productId <= 0)
            {
                return null;
            }

            try
            {
                return ProductService.GetProductById(productId);
            }
            catch
            {
                return null;
            }
        }

        private bool TryReadProductId(out int productId)
        {
            productId = 0;

            var rawProductId = Request.QueryString["productId"];
            if (string.IsNullOrWhiteSpace(rawProductId))
            {
                rawProductId = Request.QueryString["id"];
            }

            if (string.IsNullOrWhiteSpace(rawProductId))
            {
                return false;
            }

            return int.TryParse(rawProductId, NumberStyles.Integer, CultureInfo.InvariantCulture, out productId) && productId > 0;
        }

        private bool TryReadLoggedInUserId(out int userId)
        {
            userId = 0;

            var sessionUserId = Session["UserId"];
            if (sessionUserId == null)
            {
                return false;
            }

            if (sessionUserId is int directUserId && directUserId > 0)
            {
                userId = directUserId;
                return true;
            }

            return int.TryParse(
                Convert.ToString(sessionUserId, CultureInfo.InvariantCulture),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out userId)
                && userId > 0;
        }

        private static bool TryParseRating(string rawRating, out int rating)
        {
            rating = 0;
            if (!int.TryParse(rawRating, NumberStyles.Integer, CultureInfo.InvariantCulture, out rating))
            {
                return false;
            }

            return rating >= 1 && rating <= 5;
        }

        private static string NormalizeComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return string.Empty;
            }

            var normalized = comment.Trim();
            if (normalized.Length > 1000)
            {
                normalized = normalized.Substring(0, 1000).TrimEnd();
            }

            return normalized;
        }

        private void RedirectToLoginWithReturnUrl()
        {
            var returnUrl = HttpUtility.UrlEncode(Request.RawUrl ?? ResolveUrl("~/Customer/Feedback.aspx"));
            Response.Redirect("~/Auth/Login.aspx?returnUrl=" + returnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowFeedbackUnavailable(string message)
        {
            litUnavailableMessage.Text = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(message)
                ? "This review page is unavailable right now."
                : message);

            phFeedbackForm.Visible = false;
            phFeedbackUnavailable.Visible = true;
        }

        private void ShowFormMessage(string message, bool isError)
        {
            var safeMessage = HttpUtility.HtmlEncode(message ?? string.Empty);
            lblFeedbackMessage.Text = safeMessage;
            lblFeedbackMessage.Visible = true;
            lblFeedbackMessage.CssClass = isError
                ? "block px-4 py-3 text-sm border border-error/70 text-error bg-error/10"
                : "block px-4 py-3 text-sm border border-secondary/60 text-secondary bg-secondary/10";
        }

        private string BuildProductDetailsUrl(int productId)
        {
            var baseUrl = ResolveUrl("~/Pages/ProductDetails.aspx");
            if (productId <= 0)
            {
                return baseUrl;
            }

            return string.Concat(baseUrl, "?id=", productId.ToString(CultureInfo.InvariantCulture));
        }

        private string ResolveProductImageUrl(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return ResolveUrl(DefaultProductImageVirtualPath);
            }

            var normalizedPath = imagePath.Trim();
            if (normalizedPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath;
            }

            if (normalizedPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl(normalizedPath);
            }

            return ResolveUrl("~/" + normalizedPath.TrimStart('/'));

        }
    }
}