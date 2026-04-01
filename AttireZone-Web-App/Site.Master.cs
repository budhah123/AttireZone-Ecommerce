using System;
using System.Web;
using System.Web.UI;

namespace AttireZone_Web_App
{
    public partial class SiteMaster : MasterPage
    {
        public bool IsHomePage { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsHomePage =
                IsHomePath(Request.AppRelativeCurrentExecutionFilePath) ||
                IsHomePath(VirtualPathUtility.ToAppRelative(Request.Path)) ||
                Page is _Default;
        }

        private static bool IsHomePath(string appRelativePath)
        {
            if (string.IsNullOrWhiteSpace(appRelativePath))
            {
                return false;
            }

            return
                string.Equals(appRelativePath, "~/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(appRelativePath, "~/Default", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(appRelativePath, "~/Default.aspx", StringComparison.OrdinalIgnoreCase);
        }
    }
}