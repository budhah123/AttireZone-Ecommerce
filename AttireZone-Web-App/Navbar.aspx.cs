using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App
{
    public partial class Navbar : System.Web.UI.Page
    {
        protected bool IsHomePage { get; private set; }

        protected string CartNavigationUrl { get; private set; }

        protected int CartItemCount { get; private set; }

        protected string ProfileNavigationUrl { get; private set; }

        protected string ProfileIconCssClass { get; private set; }

        protected string ProfileIconInlineStyle { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsHomePage = IsHomePath(Request.AppRelativeCurrentExecutionFilePath) || IsHomePath(VirtualPathUtility.ToAppRelative(Request.Path));
            CartNavigationUrl = ResolveUrl(GetCartNavigationPath());
            CartItemCount = ResolveCartItemCount();
            ProfileNavigationUrl = ResolveUrl(GetProfileNavigationPath());

            var profileBaseCss = "material-symbols-outlined relative z-[1600] inline-flex h-10 w-10 items-center justify-center rounded-full cursor-pointer pointer-events-auto transition-colors duration-200";

            if (IsCurrentPath("~/Customer/Profile.aspx"))
            {
                ProfileIconCssClass = profileBaseCss + " text-amber-500 border border-amber-500/60";
                ProfileIconInlineStyle = "font-variation-settings: 'FILL' 1;";
                return;
            }

            ProfileIconCssClass = profileBaseCss + " text-slate-600 dark:text-slate-400 hover:text-amber-500";
            ProfileIconInlineStyle = string.Empty;
        }

        private int ResolveCartItemCount()
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                return 0;
            }

            try
            {
                return Math.Max(0, CartService.GetCartCount(userId));
            }
            catch
            {
                return 0;
            }
        }

        private string GetCartNavigationPath()
        {
            return "~/Customer/Cart.aspx";
        }

        private string GetProfileNavigationPath()
        {
            if (IsUserLoggedIn())
            {
                return "~/Customer/Profile.aspx";
            }

            var encodedReturnUrl = HttpUtility.UrlEncode(ResolveUrl("~/Customer/Profile.aspx"));
            return "~/Auth/Login.aspx?returnUrl=" + encodedReturnUrl;
        }

        private bool IsCurrentPath(string appRelativePath)
        {
            var currentPath = VirtualPathUtility.ToAppRelative(Request.Path);
            return string.Equals(currentPath, appRelativePath, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHomePath(string appRelativePath)
        {
            if (string.IsNullOrWhiteSpace(appRelativePath))
            {
                return false;
            }

            return string.Equals(appRelativePath, "~/", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(appRelativePath, "~/Default", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(appRelativePath, "~/Default.aspx", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsUserLoggedIn()
        {
            return TryReadLoggedInUserId(out _);
        }

        private bool TryReadLoggedInUserId(out int userId)
        {
            userId = 0;

            if (Session["CurrentUser"] is User currentUser && currentUser.UserId > 0)
            {
                userId = currentUser.UserId;
                return true;
            }

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

            return int.TryParse(Convert.ToString(sessionUserId, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out userId) && userId > 0;
        }
    }
}
