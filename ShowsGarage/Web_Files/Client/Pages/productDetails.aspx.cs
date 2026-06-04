using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
//using System.Windows.Forms;


namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class productDetails : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        public object MessageBox { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Read straight from the Request stream inside the initial execution block
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
            // 1. GATEWAY: Enforce strict authentication before interacting with the database
            if (Session["UserID"] == null)
            {
                string targetUrl = Request.RawUrl;
                Response.Redirect("~/Web_Files/Client/Pages/login.aspx?returnUrl=" + Server.UrlEncode(targetUrl));
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserID"]);

            // 2. THE FIX: Pull the ID directly from the Request QueryString during the postback event execution
            string prodIdStr = Request.QueryString["id"];

            if (string.IsNullOrEmpty(prodIdStr) || !int.TryParse(prodIdStr.Trim(), out int currentProductId))
            {
                Response.Redirect("shop.aspx");
                return;
            }

            // 3. DATABASE OPERATIONS: Direct mapping to your [dbo].[Cart] table schema
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Check if the item already exists in this user's cart
                    string checkQuery = "SELECT CartID, Quantity FROM Cart WHERE UserID = @userID AND ProductID = @productID";
                    int existingCartId = 0;
                    int currentQuantity = 0;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@userID", currentUserId);
                        checkCmd.Parameters.AddWithValue("@productID", currentProductId);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingCartId = Convert.ToInt32(reader["CartID"]);
                                currentQuantity = Convert.ToInt32(reader["Quantity"]);
                            }
                        }
                    }

                    if (existingCartId > 0)
                    {
                        // MATCH FOUND: Increment the quantity structurally
                        string updateQuery = "UPDATE Cart SET Quantity = @quantity WHERE CartID = @cartID";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@quantity", currentQuantity + 1);
                            updateCmd.Parameters.AddWithValue("@cartID", existingCartId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // FRESH PIECE: Insert a brand new row mapping exactly to your table schema
                        // Let IDENTITY handle CartID and GETDATE() handle AddedAt automatically
                        string insertQuery = "INSERT INTO Cart (UserID, ProductID, Quantity) VALUES (@userID, @productID, 1)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@userID", currentUserId);
                            insertCmd.Parameters.AddWithValue("@productID", currentProductId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                // 4. PIPELINE ROUTING: Advance to Step 1 of the checkout funnel
                Response.Redirect("cart.aspx");
            }
            catch (Exception ax)
            {
                // Fail-safe protection: log the exception if needed and keep user safely on details page or redirect
                Debug.WriteLine(ax);
                Response.Redirect("cart.aspx");
            }
        }
    }
}