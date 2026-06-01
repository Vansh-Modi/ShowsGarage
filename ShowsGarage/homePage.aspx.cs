using ShowsGarage.Web_Files.Admin.Pages;
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
        string connString = ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

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
            string query = "SELECT HeroImg, HeroTitle, HeroSubtitle FROM SiteSettings WHERE SiteSettingId = 1";
            using (SqlConnection conn = new SqlConnection(connString))
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

        private void LoadGalleryData()
        {
            string query = "SELECT ProductID, ImagePath, BrandName,Title , SellingPrice FROM Products"; // Fetches your image records

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        rptGallery.DataSource = dt;
                        rptGallery.DataBind();
                    }
                }
            }
        }

        protected void btnToggleGrid_Click(object sender, EventArgs e)
        {
            int currentMode = (int)ViewState["CurrentGridMode"];

            if (currentMode == 2)
            {
                ViewState["CurrentGridMode"] = 3;
                pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-3";
            }
            else
            {
                ViewState["CurrentGridMode"] = 2;
                pnlGalleryGrid.CssClass = "my-gallery-grid gallery-grid-2";
            }
        }
        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;

            if (Session["Cart_ProductID_" + productId] != null)
            {
                int currentQty = Convert.ToInt32(Session["Cart_ProductID_" + productId]);
                Session["Cart_ProductID_" + productId] = currentQty + 1;
            }
            else
            {
                Session["Cart_ProductID_" + productId] = 1;
            }
            Response.Redirect("~/Web_Files/Client/Pages/productDetails.aspx");
        }
        private void BindBlogGrid()
        {
            string query = "SELECT TOP 3 BlogId, BlogTitle, Excerpt, BlogImage, PublishDate, BlogContent FROM Blogs ORDER BY PublishDate DESC";

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
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
