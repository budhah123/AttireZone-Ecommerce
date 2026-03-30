using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AttireZone_Web_App
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            try
            {
                // Code that runs on application startup
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                // Log startup error
                System.Diagnostics.Debug.WriteLine("Application_Start Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }

        /*
        // Error handling for application errors
        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine("Application Error: " + ex.Message);
            }
        }
        */
    }
}