using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AttireZone_Web_App
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Data binding is intentionally commented out when no DB connection is configured.
            // If you have a product repository, uncomment and adapt the code below.
            //if (!IsPostBack)
            //{
            //    var catRepo = new CategoryRepository();
            //    var prodRepo = new ProductRepository();

            //    rptCategories.DataSource = catRepo.GetAll();
            //    rptCategories.DataBind();

            //    rptFeatured.DataSource = prodRepo.GetFeatured();
            //    rptFeatured.DataBind();
            //}
        }
    }
}