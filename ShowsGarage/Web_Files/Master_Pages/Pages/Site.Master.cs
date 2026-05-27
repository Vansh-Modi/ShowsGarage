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

            if (!IsPostBack)
            {

                if (Session["UserRole"] == null)
                {
                    pnlAdminNav.Visible = false;
                    pnlUserNav.Visible = true;
                }
                else
                {
                    string userSession = Session["UserRole"].ToString();
                    if (Session["UserRole"].ToString() == "Admin")
                    {
                        pnlAdminNav.Visible = true;
                        pnlUserNav.Visible = false;
                    }
                    else
                    {
                        pnlAdminNav.Visible = false;
                        pnlUserNav.Visible = true;
                    }
                }
            }
        }

        protected void ibUserLogout_Click(object sender, ImageClickEventArgs e)
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("~/homePage.aspx");
        }

        protected void ibAdminLogout_Click(object sender, ImageClickEventArgs e)
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("~/homePage.aspx");
        }
    }

}