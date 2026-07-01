using System;
using System.Collections.Generic;
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
            // Security Gate: Check login session parameters
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

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
            // BATCH SQL EXECUTION: Pulls product data and then pulls extra rows from ProductGallery
            string query = @"
                SELECT Title, Description, MRP, SellingPrice, BrandName, Scale, ImagePath, 
                       ISNULL(StockQuantity, 0) AS StockQuantity 
                FROM Products 
                WHERE ProductID = @prodID;
                
                SELECT ImagePath FROM ProductGallery WHERE ProductID = @prodID;";

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
                                // 1. Map Textual Parameters to Labels
                                lblProductName.Text = reader["Title"].ToString();
                                lblManufacturer.Text = reader["BrandName"].ToString();

                                string scaleText = reader["Scale"].ToString();
                                lblScaleDisplay.Text = string.IsNullOrEmpty(scaleText) ? "N/A" : scaleText;

                                lblDescription.Text = string.IsNullOrEmpty(reader["Description"].ToString()) ?
                                    "No technical description constraints provided for this diecast model replica." : reader["Description"].ToString();

                                decimal price = Convert.ToDecimal(reader["SellingPrice"]);
                                lblPrice.Text = string.Format("{0:N0}", price);

                                // Stock Validation Logic
                                int currentStockCount = Convert.ToInt32(reader["StockQuantity"]);

                                if (currentStockCount <= 0)
                                {
                                    lblStockBadge.Text = "Out of Stock";
                                    lblStockBadge.CssClass = "details-stock-badge-indicator stock-out-badge";

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

                                // 2. Dynamic Image Assembly Engine
                                List<string> imageGalleryCollection = new List<string>();

                                string mainImageUrl = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                                if (!string.IsNullOrEmpty(mainImageUrl))
                                {
                                    imgProduct.ImageUrl = ResolveUrl(mainImageUrl);
                                    imageGalleryCollection.Add(mainImageUrl); // Main thumbnail goes first
                                }
                                else
                                {
                                    imgProduct.ImageUrl = ResolveUrl("~/Assets/images/default-model.png");
                                }

                                // Read second result set (ProductGallery rows)
                                if (reader.NextResult())
                                {
                                    while (reader.Read())
                                    {
                                        string galleryPath = reader["ImagePath"].ToString();
                                        if (!string.IsNullOrEmpty(galleryPath))
                                        {
                                            imageGalleryCollection.Add(galleryPath);
                                        }
                                    }
                                }

                                // Bind collection lists to repeater if extra views exist
                                if (imageGalleryCollection.Count > 1)
                                {
                                    rptGallery.DataSource = imageGalleryCollection;
                                    rptGallery.DataBind();
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

                    // Boundary Stock Check before executing manipulation logic routines
                    string verifyStockSql = "SELECT ISNULL(StockQuantity, 0) FROM Products WHERE ProductID = @productID";
                    using (SqlCommand cmdStock = new SqlCommand(verifyStockSql, con))
                    {
                        cmdStock.Parameters.AddWithValue("@productID", currentProductId);
                        int activeWholesaleStock = Convert.ToInt32(cmdStock.ExecuteScalar());

                        if (activeWholesaleStock <= 0)
                        {
                            lblDetailStatus.Text = "⚠️ This item has just sold out and cannot be requested for addition.";
                            lblDetailStatus.Visible = true;

                            btnAddToCart.Text = "Sold Out";
                            btnAddToCart.Enabled = false;
                            btnAddToCart.CssClass = "btn-add-to-cart-large btn-add-disabled";
                            return;
                        }
                    }

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