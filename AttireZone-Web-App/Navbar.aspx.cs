using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App
{
    public partial class Navbar : System.Web.UI.Page
    {
        protected string CartNavigationUrl { get; private set; }

        protected string ProfileNavigationUrl { get; private set; }

        protected string ProfileIconCssClass { get; private set; }

        protected string ProfileIconInlineStyle { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CartNavigationUrl = ResolveUrl(GetCartNavigationPath());
            ProfileNavigationUrl = ResolveUrl(GetProfileNavigationPath());

            if (IsCurrentPath("~/Customer/Profile.aspx"))
            {
                ProfileIconCssClass = "material-symbols-outlined text-amber-500 border-b-2 border-amber-500 pb-1 transition-colors duration-200";
                ProfileIconInlineStyle = "font-variation-settings: 'FILL' 1;";
                return;
            }

            ProfileIconCssClass = "material-symbols-outlined text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200";
            ProfileIconInlineStyle = string.Empty;
        }

        private string GetCartNavigationPath()
        {
            if (IsUserLoggedIn())
            {
                return "~/Customer/Cart.aspx";
            }

            var encodedReturnUrl = HttpUtility.UrlEncode(ResolveUrl("~/Customer/Cart.aspx"));
            return "~/Auth/Login.aspx?returnUrl=" + encodedReturnUrl;
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

        private bool IsUserLoggedIn()
        {
            if (Session["CurrentUser"] is User)
            {
                return true;
            }

            var sessionUserId = Session["UserId"];
            if (sessionUserId == null)
            {
                return false;
            }

            return int.TryParse(Convert.ToString(sessionUserId, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId) && userId > 0;
        }
    }
}
