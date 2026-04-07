using System;
using System.Globalization;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.DataAccess;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Customer
{
    public partial class Profile : Page
    {
        private const string ProfilePagePath = "~/Customer/Profile.aspx";
        private const string ProfileSnackbarMessageSessionKey = "ProfileSnackbarMessage";
        private const string ProfileSnackbarTypeSessionKey = "ProfileSnackbarType";

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

            ShowQueuedSnackbarIfAvailable();
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect(ResolveUrl("~/Auth/Login.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnUpdateInformation_Click(object sender, EventArgs e)
        {
            if (!TryReadLoggedInUserId(out var userId))
            {
                RedirectToLogin();
                return;
            }

            var fullName = (txtFullName == null ? string.Empty : txtFullName.Text).Trim();
            var email = NormalizeEmail(txtEmail == null ? null : txtEmail.Text);

            if (string.IsNullOrWhiteSpace(fullName))
            {
                ShowSnackbar("Please enter your full name.", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                ShowSnackbar("Please enter a valid email address.", "error");
                return;
            }

            try
            {
                var repository = new UserRepository();

                if (repository.EmailExistsForAnotherUser(userId, email))
                {
                    ShowSnackbar("This email address is already in use.", "error");
                    return;
                }

                var user = repository.GetById(userId);
                if (user == null)
                {
                    ShowSnackbar("Unable to load your profile right now.", "error");
                    return;
                }

                user.FullName = fullName;
                user.Email = email;

                repository.UpdateProfile(user);

                Session["CurrentUser"] = user;
                Session["UserName"] = user.FullName;
                Session["UserEmail"] = user.Email;

                CurrentUserFullName = user.FullName;
                CurrentUserEmail = user.Email;

                if (txtFullName != null)
                {
                    txtFullName.Text = CurrentUserFullName;
                }

                if (txtEmail != null)
                {
                    txtEmail.Text = CurrentUserEmail;
                }

                QueueSnackbar("Profile updated successfully.", "success");
                Response.Redirect(ResolveUrl(ProfilePagePath), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                ShowSnackbar("Unable to update your profile right now.", "error");
            }
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
            CurrentUserEmail = string.IsNullOrWhiteSpace(email) ? "member@attirezone.com" : NormalizeEmail(email);

            if (txtFullName != null)
            {
                txtFullName.Text = CurrentUserFullName;
            }

            if (txtEmail != null)
            {
                txtEmail.Text = CurrentUserEmail;
            }
        }

        private static string NormalizeEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? string.Empty
                : email.Trim().ToLowerInvariant();
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
                return string.Equals(parsed.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
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

        private void QueueSnackbar(string message, string type)
        {
            Session[ProfileSnackbarMessageSessionKey] = message ?? string.Empty;
            Session[ProfileSnackbarTypeSessionKey] = string.IsNullOrWhiteSpace(type) ? "info" : type.Trim().ToLowerInvariant();
        }

        private void ShowQueuedSnackbarIfAvailable()
        {
            var queuedMessage = Convert.ToString(Session[ProfileSnackbarMessageSessionKey], CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(queuedMessage))
            {
                return;
            }

            var queuedType = Convert.ToString(Session[ProfileSnackbarTypeSessionKey], CultureInfo.InvariantCulture);
            Session.Remove(ProfileSnackbarMessageSessionKey);
            Session.Remove(ProfileSnackbarTypeSessionKey);

            ShowSnackbar(queuedMessage, string.IsNullOrWhiteSpace(queuedType) ? "info" : queuedType);
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

            ScriptManager.RegisterStartupScript(this, GetType(), "profileSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }
    }
}