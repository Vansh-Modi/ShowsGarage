using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Master_Pages.Pages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    if (Session["UserRole"].ToString() == "Admin")
            //    {
                    pnlAdminNav.Visible = true;
                    pnlUserNav.Visible = false;
            //    }
            //}
        }

        protected void ibUserLogout_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void ibAdminLogout_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
    
}