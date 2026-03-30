using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AttireZone_Web_App.BusinessLogic;

namespace AttireZone_Web_App.Auth
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void cvTerms_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkTerms.Checked;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblFormMessage.Text = string.Empty;
            lblFormMessage.CssClass = "sr-only";

            Page.Validate("RegisterForm");

            if (!Page.IsValid)
            {
                ShowSnackbar("Please correct the highlighted fields.", "error");
                return;
            }

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                RegistrationResult result = UserService.RegisterUser(fullName, email, password);

                if (!result.IsSuccess)
                {
                    ShowSnackbar(result.Message, "error");
                    return;
                }

                ShowSnackbar(result.Message, "success");

                txtFullName.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;
                chkTerms.Checked = false;
            }
            catch
            {
                ShowSnackbar("Something went wrong while creating your account.", "error");
            }
        }

        private void ShowSnackbar(string message, string type)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            string safeType = HttpUtility.JavaScriptStringEncode(type ?? "info");
            string script = string.Format("window.setTimeout(function(){{ if (window.azSnackbar && window.azSnackbar.show) {{ window.azSnackbar.show('{0}', '{1}'); }} }}, 0);", safeMessage, safeType);
            ScriptManager.RegisterStartupScript(this, GetType(), "registerSnackbar_" + Guid.NewGuid().ToString("N"), script, true);
        }
    }
}