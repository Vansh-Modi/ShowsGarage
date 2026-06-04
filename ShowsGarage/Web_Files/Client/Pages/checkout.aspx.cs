using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class checkout : System.Web.UI.Page
    {
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
                DataTable dtCart = Session["Cart"] as DataTable;
                if (dtCart == null || dtCart.Rows.Count == 0)
                {
                    Response.Redirect("cart.aspx");
                    return;
                }

                BindCheckoutReview(dtCart);
            }
        }

        private decimal GetShippingCharges()
        {
            decimal shippingFee = 0;
            string query = "SELECT TOP 1 ISNULL(ShippingCharges, 0) FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        shippingFee = Convert.ToDecimal(result);
                    }
                }
            }
            return shippingFee;
        }

        private void BindCheckoutReview(DataTable dtCart)
        {
            decimal itemsSubtotal = 0;
            int itemQuantityCounter = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                decimal price = Convert.ToDecimal(row["SellingPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);
                itemsSubtotal += (price * qty);
                itemQuantityCounter += qty;
            }

            decimal shippingFee = GetShippingCharges();
            decimal grandTotal = itemsSubtotal + shippingFee;

            rptCheckoutItems.DataSource = dtCart;
            rptCheckoutItems.DataBind();

            lblShippingFee.Text = string.Format("{0:N0}", shippingFee);
            lblCheckoutItemsCount.Text = itemQuantityCounter.ToString();
            lblCheckoutGrandTotal.Text = string.Format("{0:N0}", grandTotal);
        }

        protected void btnPageNavigation_Click(object sender, EventArgs e)
        {
            lblStatusMessage.Visible = false;

            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string city = txtCity.Text.Trim();
            int userId = Convert.ToInt32(Session["UserID"]);

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(city))
            {
                lblStatusMessage.Text = "⚠️ Please fill in all required delivery information fields before submitting.";
                lblStatusMessage.Visible = true;
                return;
            }

            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                Response.Redirect("cart.aspx");
                return;
            }

            try
            {
                // 1. Calculate order cost + live shipping fee rate
                decimal orderAmount = 0;
                foreach (DataRow row in dtCart.Rows)
                {
                    orderAmount += Convert.ToDecimal(row["SellingPrice"]) * Convert.ToInt32(row["Quantity"]);
                }

                decimal shippingFee = GetShippingCharges();
                orderAmount += shippingFee; // Final amount with shipping included

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // A. Insert master tracking details into [dbo].[Orders]
                            string insertOrderQuery = @"
                                INSERT INTO [dbo].[Orders] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod) 
                                OUTPUT INSERTED.OrderID
                                VALUES (@UserID, @OrderDate, @TotalAmount, @Status, @ShippingAddress, @PaymentMethod)";

                            int generatedOrderId;

                            using (SqlCommand cmdOrder = new SqlCommand(insertOrderQuery, conn, transaction))
                            {
                                cmdOrder.Parameters.AddWithValue("@UserID", userId);
                                cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                                cmdOrder.Parameters.AddWithValue("@TotalAmount", orderAmount);
                                cmdOrder.Parameters.AddWithValue("@Status", "Awaiting Payment");
                                cmdOrder.Parameters.AddWithValue("@ShippingAddress", address + ", " + city + " (Phone: " + phone + ", Name: " + fullName + ")");
                                cmdOrder.Parameters.AddWithValue("@PaymentMethod", "ONLINE");

                                generatedOrderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                            }

                            // B. Insert child items mapping details array into [dbo].[OrderDetails]
                            string insertItemsQuery = @"
                                INSERT INTO [dbo].[OrderDetails] (OrderID, ProductID, Quantity, UnitPrice) 
                                VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";

                            foreach (DataRow row in dtCart.Rows)
                            {
                                using (SqlCommand cmdItem = new SqlCommand(insertItemsQuery, conn, transaction))
                                {
                                    cmdItem.Parameters.AddWithValue("@OrderID", generatedOrderId);
                                    cmdItem.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row["ProductID"]));
                                    cmdItem.Parameters.AddWithValue("@Quantity", Convert.ToInt32(row["Quantity"]));
                                    cmdItem.Parameters.AddWithValue("@UnitPrice", Convert.ToDecimal(row["SellingPrice"]));

                                    cmdItem.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            // Pass configuration indexing values to your manual verification terminal page
                            Session["ActiveCheckoutOrderID"] = generatedOrderId;
                            Response.Redirect("payment.aspx");
                        }
                        catch (Exception innerEx)
                        {
                            transaction.Rollback();
                            // Captures explicit low-level database constraints failure logs for review
                            throw new Exception("SQL Transaction Inner Exception: " + innerEx.Message, innerEx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Outputs exact error path breakdown string directly to front-end status label component
                lblStatusMessage.Text = "❌ Process Error: " + ex.Message + (ex.InnerException != null ? " | Details: " + ex.InnerException.Message : "");
                lblStatusMessage.Visible = true;
            }
        }
    }
}