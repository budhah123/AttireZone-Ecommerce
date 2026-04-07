using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Customer
{
    public partial class Profile : Page
    {
        private const string ProfilePagePath = "~/Customer/Profile.aspx";

        protected string CurrentUserFullName { get; private set; }

        protected string CurrentUserEmail { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLogin();
                return;
            }

            if (!IsPostBack)
            {
                BindProfile(userId);
            }
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect(ResolveUrl("~/Auth/Login.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void BindProfile(int userId)
        {
            var user = Session["CurrentUser"] as User;
            if (user == null || user.UserId != userId)
            {
                user = new UserRepository().GetById(userId);
                if (user != null)
                {
                    Session["CurrentUser"] = user;
                    Session["UserName"] = user.FullName;
                    Session["UserEmail"] = user.Email;
                }
            }

            var fullName = user != null && !string.IsNullOrWhiteSpace(user.FullName)
                ? user.FullName
                : Convert.ToString(Session["UserName"], CultureInfo.InvariantCulture);

            var email = user != null && !string.IsNullOrWhiteSpace(user.Email)
                ? user.Email
                : Convert.ToString(Session["UserEmail"], CultureInfo.InvariantCulture);

            CurrentUserFullName = string.IsNullOrWhiteSpace(fullName) ? "AttireZone Member" : fullName.Trim();
            CurrentUserEmail = string.IsNullOrWhiteSpace(email) ? "member@attirezone.com" : email.Trim();
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

            return int.TryParse(Convert.ToString(sessionUserId, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out userId) && userId > 0;
        }

        private void RedirectToLogin()
        {
            var encodedReturnUrl = HttpUtility.UrlEncode(ResolveUrl(ProfilePagePath));
            Response.Redirect("~/Auth/Login.aspx?returnUrl=" + encodedReturnUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}