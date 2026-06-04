using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class cart : System.Web.UI.Page
    {
        // Centralized property to retrieve your web config connection string securely
        private string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCartFromDatabase();
                CalculateAndBindCart();
            }
        }

        private void LoadCartFromDatabase()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            DataTable dtCart = new DataTable();

            string query = @"
                SELECT c.ProductID, p.BrandName, p.Title, p.SellingPrice, c.Quantity, p.ImagePath
                FROM Cart c 
                INNER JOIN Products p ON c.ProductID = p.ProductID 
                WHERE c.UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtCart);
                    }
                }
            }

            Session["Cart"] = dtCart;
        }

        private void CalculateAndBindCart()
        {
            DataTable dtCart = Session["Cart"] as DataTable;

            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                lblEmptyMessage.Visible = true;
                divSummary.Visible = false;
                rptCartItems.DataSource = null;
                rptCartItems.DataBind();
                return;
            }

            lblEmptyMessage.Visible = false;
            divSummary.Visible = true;

            decimal cartTotal = 0;
            int totalItemsCount = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                decimal price = Convert.ToDecimal(row["SellingPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                cartTotal += (price * qty);
                totalItemsCount += qty;
            }

            rptCartItems.DataSource = dtCart;
            rptCartItems.DataBind();

            lblTotalItems.Text = totalItemsCount.ToString();
            lblCartTotal.Text = string.Format("{0:N0}", cartTotal);
        }

        protected void rptCartItems_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null) return;

            int userId = Convert.ToInt32(Session["UserID"]);

            if (e.CommandName == "UpdateQty")
            {
                string[] args = e.CommandArgument.ToString().Split('|');
                int targetProductId = Convert.ToInt32(args[0]);
                string operationalAction = args[1];

                foreach (DataRow row in dtCart.Rows)
                {
                    if (Convert.ToInt32(row["ProductID"]) == targetProductId)
                    {
                        int currentQty = Convert.ToInt32(row["Quantity"]);
                        int newQty = currentQty;

                        if (operationalAction == "plus")
                        {
                            newQty = currentQty + 1;
                        }
                        else if (operationalAction == "minus" && currentQty > 1)
                        {
                            newQty = currentQty - 1;
                        }

                        // Only write to database if quantity actually changed
                        if (newQty != currentQty)
                        {
                            // 1. Permanently update the Database row
                            UpdateQuantityInDatabase(userId, targetProductId, newQty);

                            // 2. Synchronize memory row to match DB adjustments
                            row["Quantity"] = newQty;
                        }
                        break;
                    }
                }
            }
            else if (e.CommandName == "RemoveItem")
            {
                int targetProductId = Convert.ToInt32(e.CommandArgument);

                // 1. Permanently delete the item from SQL database
                RemoveItemFromDatabase(userId, targetProductId);

                // 2. Filter local memory rows matching standard primary structures
                for (int i = dtCart.Rows.Count - 1; i >= 0; i--)
                {
                    if (Convert.ToInt32(dtCart.Rows[i]["ProductID"]) == targetProductId)
                    {
                        dtCart.Rows[i].Delete();
                        break;
                    }
                }
                dtCart.AcceptChanges();
            }

            // Save state collections and rebind front-end repeater
            Session["Cart"] = dtCart;
            CalculateAndBindCart();
        }

        // Helper Method: Handles SQL UPDATE queries safely
        private void UpdateQuantityInDatabase(int userId, int productId, int newQuantity)
        {
            string query = "UPDATE Cart SET Quantity = @Quantity WHERE UserID = @UserID AND ProductID = @ProductID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Quantity", newQuantity);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Helper Method: Handles SQL DELETE queries safely
        private void RemoveItemFromDatabase(int userId, int productId)
        {
            string query = "DELETE FROM Cart WHERE UserID = @UserID AND ProductID = @ProductID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart != null && dtCart.Rows.Count > 0)
            {
                Response.Redirect("checkout.aspx");
            }
        }
    }
}