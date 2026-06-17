using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class checkout : System.Web.UI.Page
    {
        // Global variables to hold dynamic metrics from SiteSettings
        private decimal dynamicPlatformFee = 0;
        private decimal dynamicShippingFee = 0;

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

                // Inject the dynamic configuration columns directly into the data matrix for the Repeater Eval expressions
                LoadSiteSettings();
                InjectFeesIntoDataTable(dtCart);

                BindCheckoutReview(dtCart);
            }
        }

        private void LoadSiteSettings()
        {
            // Query pulls both user-configured admin fields simultaneously from your database
            string query = "SELECT TOP 1 ISNULL(ShippingCharges, 0), ISNULL(PlatformFees, 0.00) FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dynamicShippingFee = Convert.ToDecimal(reader[0]);
                            dynamicPlatformFee = Convert.ToDecimal(reader[1]);
                        }
                        else
                        {
                            // Fallback defaults if table row values are uninitialized
                            dynamicShippingFee = 60.00m;
                            dynamicPlatformFee = 15.00m;
                        }
                    }
                }
            }
        }

        private void InjectFeesIntoDataTable(DataTable dtCart)
        {
            // Appends a temporary column so that your frontend markup can safely parse <%# Eval("PlatformFees") %> without errors
            if (!dtCart.Columns.Contains("PlatformFees"))
            {
                dtCart.Columns.Add("PlatformFees", typeof(decimal));
            }

            foreach (DataRow row in dtCart.Rows)
            {
                row["PlatformFees"] = dynamicPlatformFee;
            }
            dtCart.AcceptChanges();
        }

        private void BindCheckoutReview(DataTable dtCart)
        {
            decimal mrpSubtotal = 0;
            decimal brokerageSubtotal = 0;
            decimal cumulativePlatformFees = 0;
            int itemQuantityCounter = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                decimal sellingPrice = Convert.ToDecimal(row["SellingPrice"]);
                decimal mrpPrice = Convert.ToDecimal(row["MRP"]); // Pulled smoothly from our updated cart query pipeline
                int qty = Convert.ToInt32(row["Quantity"]);

                // Apply your formula: Sourcing = SellingPrice - MRP - PlatformFee
                decimal calculatedBrokeragePerUnit = sellingPrice - mrpPrice - dynamicPlatformFee;

                mrpSubtotal += (mrpPrice * qty);
                brokerageSubtotal += (calculatedBrokeragePerUnit * qty);
                cumulativePlatformFees += (dynamicPlatformFee * qty);
                itemQuantityCounter += qty;
            }

            // Grand Total Summing calculations
            decimal grandTotal = mrpSubtotal + brokerageSubtotal + cumulativePlatformFees + dynamicShippingFee;

            rptCheckoutItems.DataSource = dtCart;
            rptCheckoutItems.DataBind();

            // Format strings cleanly into your updated UI Labels components
            lblMRPSubtotal.Text = string.Format("{0:N0}", mrpSubtotal);
            lblBrokerageSubtotal.Text = string.Format("{0:N0}", brokerageSubtotal);
            lblPlatformFee.Text = string.Format("{0:N0}", cumulativePlatformFees);
            lblShippingFee.Text = string.Format("{0:N0}", dynamicShippingFee);
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
                // Re-load settings metrics to guarantee total sum matches
                LoadSiteSettings();

                decimal totalOrderAmount = 0;
                foreach (DataRow row in dtCart.Rows)
                {
                    totalOrderAmount += Convert.ToDecimal(row["SellingPrice"]) * Convert.ToInt32(row["Quantity"]);
                }

                // Add shipping fee variable to generate final cumulative aggregate total
                totalOrderAmount += dynamicShippingFee;

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // A. Insert tracking row down into [dbo].[Orders]
                            string insertOrderQuery = @"
                                INSERT INTO [dbo].[Orders] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod) 
                                OUTPUT INSERTED.OrderID
                                VALUES (@UserID, @OrderDate, @TotalAmount, @Status, @ShippingAddress, @PaymentMethod)";

                            int generatedOrderId;

                            using (SqlCommand cmdOrder = new SqlCommand(insertOrderQuery, conn, transaction))
                            {
                                cmdOrder.Parameters.AddWithValue("@UserID", userId);
                                cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                                cmdOrder.Parameters.AddWithValue("@TotalAmount", totalOrderAmount);
                                cmdOrder.Parameters.AddWithValue("@Status", "Awaiting Payment");
                                cmdOrder.Parameters.AddWithValue("@ShippingAddress", address + ", " + city + " (Phone: " + phone + ", Name: " + fullName + ")");
                                cmdOrder.Parameters.AddWithValue("@PaymentMethod", "ONLINE");

                                generatedOrderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                            }

                            // B. Insert separate mapped lines elements inside [dbo].[OrderDetails]
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

                            // Track session index down into verification billing module
                            Session["ActiveCheckoutOrderID"] = generatedOrderId;
                            Response.Redirect("payment.aspx");
                        }
                        catch (Exception innerEx)
                        {
                            transaction.Rollback();
                            throw new Exception("SQL Transaction Inner Exception: " + innerEx.Message, innerEx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatusMessage.Text = "❌ Process Error: " + ex.Message + (ex.InnerException != null ? " | Details: " + ex.InnerException.Message : "");
                lblStatusMessage.Visible = true;
            }
        }
    }
}