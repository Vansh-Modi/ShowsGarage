using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class checkout : System.Web.UI.Page
    {
        private decimal dynamicPlatformFee = 15.00m; // Default backup platform metric fallback

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

                LoadPlatformFeesFromSettings();
                InjectFeesIntoDataTable(dtCart);
                BindCheckoutReview(dtCart);
            }
        }

        private void LoadPlatformFeesFromSettings()
        {
            string query = "SELECT TOP 1 ISNULL(PlatformFees, 15.00) FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        object feeResult = cmd.ExecuteScalar();
                        if (feeResult != null)
                        {
                            dynamicPlatformFee = Convert.ToDecimal(feeResult);
                        }
                    }
                    catch
                    {
                        dynamicPlatformFee = 15.00m;
                    }
                }
            }
        }

        private void InjectFeesIntoDataTable(DataTable dtCart)
        {
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
            decimal itemsSubtotal = 0;
            int itemQuantityCounter = 0;

            foreach (DataRow row in dtCart.Rows)
            {
                decimal sellingPrice = Convert.ToDecimal(row["SellingPrice"]);
                int qty = Convert.ToInt32(row["Quantity"]);

                itemsSubtotal += (sellingPrice * qty);
                itemQuantityCounter += qty;
            }

            // 🔥 CRITICAL RULE: ONLINE = Rs. 120, COD = Rs. 0
            decimal appliedShippingFee = 120.00m;
            if (ddlPaymentMode.SelectedValue == "COD")
            {
                appliedShippingFee = 0.00m;
            }

            decimal grandTotal = itemsSubtotal + appliedShippingFee;

            Repeater1.DataSource = dtCart;
            Repeater1.DataBind();

            // Push fresh figures onto front-end labels
            Label1.Text = itemQuantityCounter.ToString();
            Label2.Text = string.Format("{0:N0}", itemsSubtotal);
            Label3.Text = string.Format("{0:N0}", appliedShippingFee);
            Label4.Text = string.Format("{0:N0}", grandTotal);
        }

        protected void txtCity_TextChanged(object sender, EventArgs e)
        {
            string cityInput = txtCity.Text.Trim().ToLower();
            lblPaymentWarning.Visible = false;

            if (cityInput != "surat" && !string.IsNullOrEmpty(cityInput))
            {
                ddlPaymentMode.SelectedValue = "ONLINE";

                ListItem codItem = ddlPaymentMode.Items.FindByValue("COD");
                if (codItem != null)
                {
                    ddlPaymentMode.Items.Remove(codItem);
                }

                lblPaymentWarning.Text = "⚠️ Cash-on-Delivery is only available within Surat city limits.";
                lblPaymentWarning.Visible = true;
            }
            else
            {
                if (ddlPaymentMode.Items.FindByValue("COD") == null)
                {
                    ddlPaymentMode.Items.Add(new ListItem("Cash-on-Delivery", "COD"));
                }
            }

            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart != null)
            {
                BindCheckoutReview(dtCart);
            }

            updMainCheckoutLayout.Update();
        }

        protected void ddlPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart != null)
            {
                BindCheckoutReview(dtCart);
            }
            updMainCheckoutLayout.Update();
        }

        protected void btnPageNavigation_Click(object sender, EventArgs e)
        {
            lblStatusMessage.Visible = false;

            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string city = txtCity.Text.Trim();
            string selectedPaymentMode = ddlPaymentMode.SelectedValue;
            int userId = Convert.ToInt32(Session["UserID"]);

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(city))
            {
                lblStatusMessage.Text = "⚠️ Please fill in all required delivery information fields before submitting.";
                lblStatusMessage.Visible = true;
                return;
            }

            if (city.Trim().ToLower() != "surat" && selectedPaymentMode == "COD")
            {
                lblStatusMessage.Text = "❌ Validation Breach: Cash-on-Delivery (COD) services are strictly closed for delivery destinations outside Surat city limits.";
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
                decimal totalOrderAmount = 0;
                foreach (DataRow row in dtCart.Rows)
                {
                    totalOrderAmount += Convert.ToDecimal(row["SellingPrice"]) * Convert.ToInt32(row["Quantity"]);
                }

                // Apply correct delivery charges modifier rule to final database total record
                if (selectedPaymentMode != "COD")
                {
                    totalOrderAmount += 120.00m;
                }

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
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
                                cmdOrder.Parameters.AddWithValue("@PaymentMethod", selectedPaymentMode);

                                generatedOrderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                            }

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
                lblStatusMessage.Text = "❌ Process Error: " + ex.Message;
                lblStatusMessage.Visible = true;
            }
        }
    }
}