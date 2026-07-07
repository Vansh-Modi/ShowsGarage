using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class shop : System.Web.UI.Page
    {
        private const string BasePillStyle = "flex-shrink: 0 !important; white-space: nowrap !important; display: inline-block !important; background-color: #141414 !important; border: 1px solid #222222 !important; color: #aaaaaa !important; font-size: 13px !important; font-weight: 600 !important; text-transform: uppercase !important; letter-spacing: 0.3px !important; padding: 8px 18px !important; border-radius: 20px !important; text-decoration: none !important; cursor: pointer !important;";
        private const string ActivePillStyle = "flex-shrink: 0 !important; white-space: nowrap !important; display: inline-block !important; background-color: rgba(255, 87, 34, 0.08) !important; border: 1px solid #ff5722 !important; color: #ff5722 !important; font-size: 13px !important; font-weight: 700 !important; text-transform: uppercase !important; letter-spacing: 0.3px !important; padding: 8px 18px !important; border-radius: 20px !important; text-decoration: none !important; cursor: pointer !important;";

        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategories();

                if (Request.QueryString["search"] != null)
                {
                    string searchKeyword = Request.QueryString["search"].ToString().Trim();
                    BindProductsFilteredBySearch(searchKeyword);
                }
                else
                {
                    BindProducts(0);
                }
            }
        }

        private void BindCategories()
        {
            string query = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName ASC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                try
                {
                    con.Open();
                    sda.Fill(dt);
                    rptCategories.DataSource = dt;
                    rptCategories.DataBind();
                }
                catch { /* Quiet fallback catch engine */ }
            }
        }

        private void BindProducts(int categoryId)
        {
            string query = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, 
                                   ISNULL(StockQuantity, 0) AS StockQuantity 
                            FROM Products";

            if (categoryId > 0)
            {
                query += " WHERE CategoryID = @catID";
            }
            query += " ORDER BY ProductID DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (categoryId > 0)
                {
                    cmd.Parameters.AddWithValue("@catID", categoryId);
                }

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        con.Open();
                        sda.Fill(dt);
                        rptProducts.DataSource = dt;
                        rptProducts.DataBind();
                    }
                    catch { /* Quiet fallback catch engine */ }
                }
            }
        }

        private void BindProductsFilteredBySearch(string keyword)
        {
            string query = @"SELECT ProductID, Title, BrandName, MRP, SellingPrice, ImagePath, 
                                   ISNULL(StockQuantity, 0) AS StockQuantity 
                            FROM Products 
                            WHERE Title LIKE @search 
                               OR BrandName LIKE @search 
                            ORDER BY ProductID DESC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        con.Open();
                        sda.Fill(dt);
                        rptProducts.DataSource = dt;
                        rptProducts.DataBind();
                    }
                    catch { /* Quiet fallback catch engine */ }
                }
            }
        }

        protected void CategoryFilter_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int categoryId = Convert.ToInt32(btn.CommandArgument);
            BindProducts(categoryId);

            // Reset default base styles safely using your constants
            lnkAll.Attributes["style"] = BasePillStyle;
            lnkAll.CssClass = "btn-filter-pill-isolated";

            foreach (RepeaterItem item in rptCategories.Items)
            {
                LinkButton lb = (LinkButton)item.FindControl("lnkCat");
                if (lb != null)
                {
                    lb.Attributes["style"] = BasePillStyle;
                    lb.CssClass = "btn-filter-pill-isolated";
                }
            }

            // Bind high performance active style tokens onto selection target
            btn.Attributes["style"] = ActivePillStyle;
            btn.CssClass = "btn-filter-pill-isolated active-pill-isolated";
        }
    }
}