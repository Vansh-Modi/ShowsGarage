using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class shop : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }
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
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        rptCategories.DataSource = dt;
                        rptCategories.DataBind();
                    }
                }
            }
        }

        private void BindProducts(int categoryId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT ProductID, Title, BrandName, SellingPrice, ImagePath FROM Products";
                if (categoryId > 0)
                {
                    query += " WHERE CategoryID = @catID";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    if (categoryId > 0)
                    {
                        cmd.Parameters.AddWithValue("@catID", categoryId);
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        rptProducts.DataSource = dt;
                        rptProducts.DataBind();
                    }
                }
            }
        }

        protected void CategoryFilter_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int categoryId = Convert.ToInt32(btn.CommandArgument);
            BindProducts(categoryId);

            // Update active pill styling visually
            foreach (RepeaterItem item in rptCategories.Items)
            {
                LinkButton lb = (LinkButton)item.FindControl("lnkCat");
                lb.CssClass = "btn btn-filter-pill";
            }
            lnkAll.CssClass = "btn btn-filter-pill";
            btn.CssClass = "btn btn-filter-pill active-pill";
        }
    }
}