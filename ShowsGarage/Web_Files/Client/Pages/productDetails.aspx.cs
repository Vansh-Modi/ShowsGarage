using System;
using System.Data.SqlClient;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class productDetails : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string productIdStr = Request.QueryString["id"];

                if (string.IsNullOrEmpty(productIdStr))
                {
                    Response.Redirect("shop.aspx");
                    return;
                }

                LoadProductInformation(productIdStr);
            }
        }

        private void LoadProductInformation(string productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT Title, Description, SellingPrice, BrandName, ImagePath FROM Products WHERE ProductID = @prodID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@prodID", productId.Trim());
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblProductName.Text = reader["Title"].ToString();
                                lblDescription.Text = reader["Description"].ToString();
                                lblManufacturer.Text = reader["BrandName"].ToString();

                                decimal price = Convert.ToDecimal(reader["SellingPrice"]);
                                lblPrice.Text = string.Format("{0:N2}", price);

                                string imageUrl = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    imgProduct.ImageUrl = ResolveUrl(imageUrl);
                                }
                                else
                                {
                                    imgProduct.ImageUrl = ResolveUrl("/Assets/images/no-image.png");
                                }
                            }
                            else
                            {
                                Response.Redirect("shop.aspx");
                            }
                        }
                    }
                }
            }
            catch
            {
                Response.Redirect("shop.aspx");
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            // Future Shopping cart integration
        }
    }
}