using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHeroContent(); 
                
                // This is just copy paste code for grid toggle button
                //ViewState["CurrentGridMode"] = 2;
                //LoadGalleryData();
            }
        }

        //private void LoadGalleryData()
        //{
        //    string connString = ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;
        //    string query = "SELECT HeroImg, HeroTitle, HeroSubtitle FROM SiteSettings"; // Fetches your image records

        //    using (SqlConnection conn = new SqlConnection(connString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                DataTable dt = new DataTable();
        //                sda.Fill(dt);

        //                rptGallery.DataSource = dt;
        //                rptGallery.DataBind();
        //            }
        //        }
        //    }
        //}

        //protected void btnToggleGrid_Click(object sender, EventArgs e)
        //{
        //    int currentMode = (int)ViewState["CurrentGridMode"];

        //    if (currentMode == 2)
        //    {
        //        // Switch layout to 3 columns
        //        ViewState["CurrentGridMode"] = 3;
        //        pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-3";
        //        btnToggleGrid.Text = "View 2 Columns"; // Update button instruction text
        //    }
        //    else
        //    {
        //        // Revert back to 2 columns (Default)
        //        ViewState["CurrentGridMode"] = 2;
        //        pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-2";
        //        btnToggleGrid.Text = "View 3 Columns";
        //    }
        //}

        private void LoadHeroContent()
        {
            string connString = ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;
            string query = "SELECT HeroImg, HeroTitle, HeroSubtitle FROM SiteSettings WHERE SiteSettingId = 1";
            using(SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string imageUrl = reader["HeroImg"].ToString();
                        heroSection.Attributes["style"] = $"background-image: url('{ResolveUrl(imageUrl)}');";
                        litHeroTitle.Text = reader["HeroTitle"].ToString();
                        litHeroContent.Text = reader["HeroSubtitle"].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    litHeroContent.Text = (ex.Message);
                }
            }
        }
    }
}
