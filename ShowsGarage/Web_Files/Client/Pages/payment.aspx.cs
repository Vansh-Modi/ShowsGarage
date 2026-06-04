using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class payment_upload : System.Web.UI.Page
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
            // Security Authorization Gate
            if (Session["UserID"] == null || Session["ActiveCheckoutOrderID"] == null)
            {
                Response.Redirect("cart.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadMerchantDetailsFromSettings();
            }
        }

        private void LoadMerchantDetailsFromSettings()
        {
            // Pulls owner specifications directly from your existing SiteSettings structure
            string query = "SELECT TOP 1 QrCodePath, UpiID, BankAccountDetails FROM [dbo].[SiteSettings]";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Apply settings records directly into frontend text elements
                                imgQrCode.ImageUrl = !string.IsNullOrEmpty(reader["QrCodePath"].ToString()) ? reader["QrCodePath"].ToString() : "/Assets/images/default-qr.png";
                                litUpiId.Text = !string.IsNullOrEmpty(reader["UpiID"].ToString()) ? reader["UpiID"].ToString() : "Not Available";
                                litBankDetails.Text = !string.IsNullOrEmpty(reader["BankAccountDetails"].ToString()) ? reader["BankAccountDetails"].ToString() : "Contact Support for Details";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = "❌ Configuration Read Failure: " + ex.Message;
                        lblStatus.Visible = true;
                    }
                }
            }
        }

        protected void btnSubmitProof_Click(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
            int orderId = Convert.ToInt32(Session["ActiveCheckoutOrderID"]);
            int userId = Convert.ToInt32(Session["UserID"]);
            string txnRef = txtTxnReference.Text.Trim();

            // 1. Front-End Form Input Validation Rules
            if (string.IsNullOrEmpty(txnRef))
            {
                lblStatus.Text = "⚠️ Please enter your transaction UTR or reference number for account lookup.";
                lblStatus.Visible = true;
                return;
            }

            if (!fileScreenshot.HasFile)
            {
                lblStatus.Text = "⚠️ Please upload an image screenshot copy of your payment receipt.";
                lblStatus.Visible = true;
                return;
            }

            // Retrieve active items from session cache memory to process stock calculations
            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                lblStatus.Text = "❌ Process Error: Session cart data has expired or is missing. Please restart checkout.";
                lblStatus.Visible = true;
                return;
            }

            try
            {
                string extension = Path.GetExtension(fileScreenshot.FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    // 2. Establish secure physical server directory target path
                    string folderMapPath = Server.MapPath("~/Assets/uploads/receipts/");
                    if (!Directory.Exists(folderMapPath)) { Directory.CreateDirectory(folderMapPath); }

                    // Generate a distinct non-clashing custom file designation identifier name
                    string cleanFileName = "Order_" + orderId + "_" + DateTime.Now.Ticks + extension;
                    string fullServerSavePath = Path.Combine(folderMapPath, cleanFileName);

                    // Commit image allocation asset onto disk architecture
                    fileScreenshot.SaveAs(fullServerSavePath);
                    string relativeDbStringPath = "/Assets/uploads/receipts/" + cleanFileName;

                    // 3. Execute atomic database transactions updates across Orders, Products, and Cart rows
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                // A. Update Status tracking metric flags in your [dbo].[Orders] table
                                string updateOrderSql = @"
                            UPDATE [dbo].[Orders] 
                            SET Status = @Status, 
                                PaymentScreenshotPath = @ImgPath, 
                                TransactionReference = @TxnRef 
                            WHERE OrderID = @OrderID";

                                using (SqlCommand cmdOrder = new SqlCommand(updateOrderSql, conn, trans))
                                {
                                    cmdOrder.Parameters.AddWithValue("@Status", "Awaiting Verification");
                                    cmdOrder.Parameters.AddWithValue("@ImgPath", relativeDbStringPath);
                                    cmdOrder.Parameters.AddWithValue("@TxnRef", txnRef);
                                    cmdOrder.Parameters.AddWithValue("@OrderID", orderId);
                                    cmdOrder.ExecuteNonQuery();
                                }

                                // B. Loop through each item in the cart and deduct its stock from [dbo].[Products]
                                string deductStockSql = @"
                            UPDATE [dbo].[Products] 
                            SET StockQuantity = StockQuantity - @Quantity 
                            WHERE ProductID = @ProductID";

                                foreach (DataRow row in dtCart.Rows)
                                {
                                    using (SqlCommand cmdStock = new SqlCommand(deductStockSql, conn, trans))
                                    {
                                        cmdStock.Parameters.AddWithValue("@Quantity", Convert.ToInt32(row["Quantity"]));
                                        cmdStock.Parameters.AddWithValue("@ProductID", Convert.ToInt32(row["ProductID"]));

                                        cmdStock.ExecuteNonQuery();
                                    }
                                }

                                // C. Cleanse active User temporary items collection inside your database CartTable
                                string deleteCartSql = "DELETE FROM Cart WHERE UserID = @UserID";
                                using (SqlCommand cmdCart = new SqlCommand(deleteCartSql, conn, trans))
                                {
                                    cmdCart.Parameters.AddWithValue("@UserID", userId);
                                    cmdCart.ExecuteNonQuery();
                                }

                                // Everything passed successfully, commit the transaction locks permanently
                                trans.Commit();
                            }
                            catch (Exception innerEx)
                            {
                                // Rollback everything safely if any single statement throws an exception
                                trans.Rollback();
                                throw new Exception("Database inventory adjustment operation rejected. Rolling back alterations.", innerEx);
                            }
                        }
                    }

                    // 4. Flush user cache allocations properties and route cleanly to success landing page
                    Session["Cart"] = null;
                    Session["ActiveCheckoutOrderID"] = null;
                    Response.Redirect("confirmOrder.aspx?id=" + orderId);
                }
                else
                {
                    lblStatus.Text = "❌ Invalid file selection profiles. Only upload standard image configurations (.jpg, .jpeg, .png).";
                    lblStatus.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Process Error: " + ex.Message + (ex.InnerException != null ? " | Details: " + ex.InnerException.Message : "");
                lblStatus.Visible = true;
            }
        }
    }
}