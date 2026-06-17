/* ==========================================================================
   Show's Garage - Dynamic Client Scale Model Details Control Panel Engine
   ========================================================================== */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class productDetails : System.Web.UI.Page
    {
        private string connStr => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Gate: Redirect users to login if they try to access details directly without an active session
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Read straight from the QueryString parameter token on initial load
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
            // FIXED: Added Scale and forced NULL StockQuantity to evaluate as 0 safely
            string query = @"
                SELECT Title, Description, MRP, SellingPrice, BrandName, Scale, ImagePath, 
                       ISNULL(StockQuantity, 0) AS StockQuantity 
                FROM Products 
                WHERE ProductID = @prodID";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@prodID", productId.Trim());
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblProductName.Text = reader["Title"].ToString();
                                lblManufacturer.Text = reader["BrandName"].ToString();

                                string scaleText = reader["Scale"].ToString();
                                lblScaleDisplay.Text = string.IsNullOrEmpty(scaleText) ? "N/A" : scaleText;

                                lblDescription.Text = string.IsNullOrEmpty(reader["Description"].ToString()) ?
                                    "No technical description constraints provided for this diecast model replica." : reader["Description"].ToString();

                                decimal price = Convert.ToDecimal(reader["MRP"]);
                                lblPrice.Text = string.Format("{0:N0}", price);

                                // FIXED: Fetch StockQuantity and explicitly disable Add to Cart if 0
                                int currentStockCount = Convert.ToInt32(reader["StockQuantity"]);

                                if (currentStockCount <= 0)
                                {
                                    lblStockBadge.Text = "Out of Stock";
                                    lblStockBadge.CssClass = "details-stock-badge-indicator stock-out-badge";

                                    // Lock down control properties to freeze user clicks
                                    btnAddToCart.Text = "Sold Out";
                                    btnAddToCart.Enabled = false;
                                    btnAddToCart.CssClass = "btn-add-to-cart-large btn-add-disabled";
                                }
                                else
                                {
                                    lblStockBadge.Text = "In Stock (" + currentStockCount + " Units)";
                                    lblStockBadge.CssClass = "details-stock-badge-indicator stock-available-badge";

                                    btnAddToCart.Text = "Add to Cart";
                                    btnAddToCart.Enabled = true;
                                    btnAddToCart.CssClass = "btn-add-to-cart-large";
                                }

                                string imageUrl = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    imgProduct.ImageUrl = ResolveUrl(imageUrl);
                                }
                                else
                                {
                                    imgProduct.ImageUrl = ResolveUrl("~/Assets/images/default-model.png");
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
            lblDetailStatus.Visible = false;

            if (Session["UserID"] == null)
            {
                string targetUrl = Request.RawUrl;
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx?returnUrl=" + Server.UrlEncode(targetUrl));
                return;
            }

            int currentUserId = Convert.ToInt32(Session["UserID"]);
            string prodIdStr = Request.QueryString["id"];

            if (string.IsNullOrEmpty(prodIdStr) || !int.TryParse(prodIdStr.Trim(), out int currentProductId))
            {
                Response.Redirect("shop.aspx");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // SERVER-SIDE STOCK CHECK: Extra boundary defense check before touching cart updates
                    string verifyStockSql = "SELECT ISNULL(StockQuantity, 0) FROM Products WHERE ProductID = @productID";
                    using (SqlCommand cmdStock = new SqlCommand(verifyStockSql, con))
                    {
                        cmdStock.Parameters.AddWithValue("@productID", currentProductId);
                        int activeWholesaleStock = Convert.ToInt32(cmdStock.ExecuteScalar());

                        if (activeWholesaleStock <= 0)
                        {
                            lblDetailStatus.Text = "⚠️ This item has just sold out and cannot be requested for addition.";
                            lblDetailStatus.Visible = true;

                            // Dynamically mirror state change onto UI controls immediately
                            btnAddToCart.Text = "Sold Out";
                            btnAddToCart.Enabled = false;
                            btnAddToCart.CssClass = "btn-add-to-cart-large btn-add-disabled";
                            return;
                        }
                    }

                    // Check if the item already exists in this specific user's cart records
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
                        // Increment item line tracking quantity cleanly
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
                        // Insert clean row item mapping directly back to database structures layout
                        string insertQuery = "INSERT INTO Cart (UserID, ProductID, Quantity) VALUES (@userID, @productID, 1)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@userID", currentUserId);
                            insertCmd.Parameters.AddWithValue("@productID", currentProductId);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                Response.Redirect("cart.aspx");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Response.Redirect("cart.aspx");
            }
        }
    }
}