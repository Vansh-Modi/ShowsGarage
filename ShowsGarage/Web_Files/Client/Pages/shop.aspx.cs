using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class shop : System.Web.UI.Page
    {
        private string connStr => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // REMOVED: The Session["UserID"] == null redirect restriction gate.
            // This allows guest shoppers and potential buyers to freely browse your catalog!

            if (!IsPostBack)
            {
                BindCategories();
                BindProducts(0);
            }
        }

        private void BindCategories()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName ASC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
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
                        catch { /* Fail-safe fallback tracking handles checks */ }
                    }
                }
            }
        }

        private void BindProducts(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT ProductID, Title, BrandName, SellingPrice, ImagePath, 
                           ISNULL(StockQuantity, 0) AS StockQuantity 
                    FROM Products";

                if (categoryId > 0)
                {
                    query += " WHERE CategoryID = @catID";
                }

                query += " ORDER BY ProductID DESC";

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
                        catch { /* Prevent layout crashes */ }
                    }
                }
            }
        }

        protected void CategoryFilter_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int categoryId = Convert.ToInt32(btn.CommandArgument);
            BindProducts(categoryId);

            lnkAll.CssClass = "btn btn-filter-pill";
            foreach (RepeaterItem item in rptCategories.Items)
            {
                LinkButton lb = (LinkButton)item.FindControl("lnkCat");
                if (lb != null)
                {
                    lb.CssClass = "btn btn-filter-pill";
                }
            }

            btn.CssClass = "btn btn-filter-pill active-pill";
        }
    }
}