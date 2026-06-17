using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private string connString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHeroContent();
                ViewState["CurrentGridMode"] = 2;
                LoadGalleryData();
                BindBlogGrid();
            }
        }

        private void LoadHeroContent()
        {
            string query = "SELECT HeroImg, HeroTitle, HeroSubtitle FROM [dbo].[SiteSettings] WHERE SiteSettingId = 1";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string imageUrl = reader["HeroImg"].ToString();
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    heroSection.Attributes["style"] = $"background-image: linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5)), url('{ResolveUrl(imageUrl)}');";
                                }
                                litHeroTitle.Text = reader["HeroTitle"].ToString();
                                litHeroContent.Text = reader["HeroSubtitle"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        litHeroContent.Text = "Welcome to Show's Garage. System baseline warning details: " + ex.Message;
                    }
                }
            }
        }

        private void LoadGalleryData()
        {
            string query = "SELECT ProductID, ImagePath, BrandName, Title, MRP FROM [dbo].[Products] ORDER BY CreatedAt DESC";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        try
                        {
                            conn.Open();
                            sda.Fill(dt);
                            rptGallery.DataSource = dt;
                            rptGallery.DataBind();
                        }
                        catch { /* Graceful baseline silence */ }
                    }
                }
            }
        }

        protected void btnToggleGrid_Click(object sender, EventArgs e)
        {
            int currentMode = (int)(ViewState["CurrentGridMode"] ?? 2);

            if (currentMode == 2)
            {
                ViewState["CurrentGridMode"] = 3;
                pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-3";
                btnToggleGrid.Text = "View 2 Columns";
            }
            else
            {
                ViewState["CurrentGridMode"] = 2;
                pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-2";
                btnToggleGrid.Text = "View 3 Columns";
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;

            // Redirect smoothly to details mapping parameters token view
            Response.Redirect("~/Web_Files/Client/Pages/productDetails.aspx?id=" + productId);
        }

        private void BindBlogGrid()
        {
            string query = "SELECT TOP 3 BlogId, BlogTitle, Excerpt, BlogImage, PublishDate FROM [dbo].[Blogs] ORDER BY PublishDate DESC";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dtBlogs = new DataTable();
                        try
                        {
                            conn.Open();
                            da.Fill(dtBlogs);
                            rptLatestBlogs.DataSource = dtBlogs;
                            rptLatestBlogs.DataBind();
                        }
                        catch { /* Failure fallback tracking handles checks */ }
                    }
                }
            }
        }
    }
}