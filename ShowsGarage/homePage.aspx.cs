using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage
{
    public partial class WebForm1 : Page
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
            const string query = "SELECT HeroImg, HeroTitle, HeroSubtitle FROM [dbo].[SiteSettings] WHERE SiteSettingId = 1";

            var conn = new SqlConnection(connString);
            var cmd = new SqlCommand(query, conn);
            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string imageUrl = reader["HeroImg"].ToString();
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Set image cleanly; structural layers are completely managed via CSS setup architecture rules
                        heroSection.Attributes["style"] = $"background-image: url('{ResolveUrl(imageUrl)}');";
                    }
                    litHeroTitle.Text = reader["HeroTitle"].ToString();
                    litHeroContent.Text = reader["HeroSubtitle"].ToString();
                }
            }
            catch (Exception ex)
            {
                litHeroContent.Text = $"Welcome to Show's Garage. System baseline warning details: {ex.Message}";
            }
        }

        private void LoadGalleryData()
        {
            const string query = "SELECT ProductID, ImagePath, BrandName, Title, MRP, SellingPrice FROM [dbo].[Products] ORDER BY CreatedAt DESC";

            var conn = new SqlConnection(connString);
            var cmd = new SqlCommand(query, conn);
            var sda = new SqlDataAdapter(cmd);

            var dt = new DataTable();
            try
            {
                conn.Open();
                sda.Fill(dt);
                rptGallery.DataSource = dt;
                rptGallery.DataBind();
            }
            catch { /* Graceful baseline silence placeholder */ }
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
            if (sender is Button btn)
            {
                string productId = btn.CommandArgument;
                Response.Redirect($"~/Web_Files/Client/Pages/productDetails.aspx?id={productId}");
            }
        }

        private void BindBlogGrid()
        {
            const string query = "SELECT TOP 3 BlogId, BlogTitle, Excerpt, BlogImage, PublishDate FROM [dbo].[Blogs] ORDER BY PublishDate DESC";

            var conn = new SqlConnection(connString);
            var cmd = new SqlCommand(query, conn);
            var da = new SqlDataAdapter(cmd);

            var dtBlogs = new DataTable();
            try
            {
                conn.Open();
                da.Fill(dtBlogs);
                rptLatestBlogs.DataSource = dtBlogs;
                rptLatestBlogs.DataBind();
            }
            catch { /* Failure fallback tracking placeholder */ }
        }

        protected void rptGallery_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Extensible hooks point
        }
    }
}