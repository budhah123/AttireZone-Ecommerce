using System;
using System.Web;
using System.Web.UI;
using AttireZone_Web_App.BusinessLogic;
using AttireZone_Web_App.Models;

namespace AttireZone_Web_App.Auth
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate("LoginForm");

            if (!Page.IsValid)
            {
                ShowSnackbar("Please enter your email and password.", "error");
                return;
            }

            string email = txtLoginEmail.Text.Trim();
            string password = txtLoginPassword.Text;

            try
            {
                User user = UserService.AuthenticateUser(email, password);
                if (user == null)
                {
                    ShowSnackbar("Invalid email or password.", "error");
                    return;
                }

                Session["CurrentUser"] = user;
                Session["UserId"] = user.UserId;
                Session["UserName"] = user.FullName;
                Session["UserEmail"] = user.Email;

                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                ShowSnackbar("Something went wrong while signing you in.", "error");
            }
        }

        private void ShowSnackbar(string message, string type)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            string safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            string script = string.Format("window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} }}, 0);", safeMessage, safeType);
            ScriptManager.RegisterStartupScript(this, GetType(), "loginSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }
    }
}