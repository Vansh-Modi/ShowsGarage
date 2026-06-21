using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                string paymentMethod = GetCurrentOrderPaymentMethod();

                if (paymentMethod == "COD")
                {
                    // Configure UI layout for Cash on Delivery
                    pnlOnlinePaymentDetails.Visible = false;
                    pnlOnlineUploadForm.Visible = false;
                    pnlCodConfirmation.Visible = true;
                    btnSubmitProof.Text = "Confirm Cash on Delivery Order ✔";
                }
                else
                {
                    // Configure UI layout for Online Payments
                    pnlOnlinePaymentDetails.Visible = true;
                    pnlOnlineUploadForm.Visible = true;
                    pnlCodConfirmation.Visible = false;
                    btnSubmitProof.Text = "Submit Reference & Complete Order";
                    LoadMerchantDetailsFromSettings();
                }
            }
        }

        private string GetCurrentOrderPaymentMethod()
        {
            int orderId = Convert.ToInt32(Session["ActiveCheckoutOrderID"]);
            string query = "SELECT PaymentMethod FROM [dbo].[Orders] WHERE OrderID = @OrderID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "ONLINE";
                }
            }
        }

        private void LoadMerchantDetailsFromSettings()
        {
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

            string paymentMethod = GetCurrentOrderPaymentMethod();
            string txnRef = "COD-ORDER";
            string relativeDbStringPath = "COD";
            string targetOrderStatus = "Awaiting Verification"; // Admin must approve both paths

            DataTable dtCart = Session["Cart"] as DataTable;
            if (dtCart == null || dtCart.Rows.Count == 0)
            {
                lblStatus.Text = "❌ Process Error: Session cart data has expired or is missing. Please restart checkout.";
                lblStatus.Visible = true;
                return;
            }

            // --- ONLINE METHOD VALIDATIONS & UPLOADS ---
            if (paymentMethod != "COD")
            {
                txnRef = txtTxnReference.Text.Trim();

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

                int maxAllowedBytes = 2 * 1024 * 1024;
                if (fileScreenshot.PostedFile.ContentLength > maxAllowedBytes)
                {
                    lblStatus.Text = "⚠️ Image upload rejected! Screenshot size exceeds the maximum limit of <b>2MB</b>.";
                    lblStatus.Visible = true;
                    return;
                }

                string mimeType = fileScreenshot.PostedFile.ContentType.ToLower();
                if (mimeType != "image/jpeg" && mimeType != "image/jpg" && mimeType != "image/png")
                {
                    lblStatus.Text = "❌ Invalid file type. Only clean graphic images (.jpg, .jpeg, .png) are supported.";
                    lblStatus.Visible = true;
                    return;
                }

                try
                {
                    string extension = Path.GetExtension(fileScreenshot.FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        string folderMapPath = Server.MapPath("~/Assets/uploads/receipts/");
                        if (!Directory.Exists(folderMapPath)) { Directory.CreateDirectory(folderMapPath); }

                        string cleanFileName = "Order_" + orderId + "_" + DateTime.Now.Ticks + extension;
                        string fullServerSavePath = Path.Combine(folderMapPath, cleanFileName);

                        fileScreenshot.SaveAs(fullServerSavePath);
                        relativeDbStringPath = "/Assets/uploads/receipts/" + cleanFileName;
                    }
                    else
                    {
                        lblStatus.Text = "❌ Invalid file selection profiles. Only upload standard image configurations (.jpg, .jpeg, .png).";
                        lblStatus.Visible = true;
                        return;
                    }
                }
                catch (Exception fileEx)
                {
                    lblStatus.Text = "❌ File Save Error: " + fileEx.Message;
                    lblStatus.Visible = true;
                    return;
                }
            }

            // --- UNIFIED DATABASE TRANSACTION (Works for both COD & ONLINE) ---
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // A. Update Status and paths in your [dbo].[Orders] table
                            string updateOrderSql = @"
                                UPDATE [dbo].[Orders] 
                                SET Status = @Status, 
                                    PaymentScreenshotPath = @ImgPath, 
                                    TransactionReference = @TxnRef 
                                WHERE OrderID = @OrderID";

                            using (SqlCommand cmdOrder = new SqlCommand(updateOrderSql, conn, trans))
                            {
                                cmdOrder.Parameters.AddWithValue("@Status", targetOrderStatus);
                                cmdOrder.Parameters.AddWithValue("@ImgPath", relativeDbStringPath);
                                cmdOrder.Parameters.AddWithValue("@TxnRef", txnRef);
                                cmdOrder.Parameters.AddWithValue("@OrderID", orderId);
                                cmdOrder.ExecuteNonQuery();
                            }

                            // B. Deduct stock values from [dbo].[Products]
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

                            // C. Cleanse active user items inside database Cart table
                            string deleteCartSql = "DELETE FROM Cart WHERE UserID = @UserID";
                            using (SqlCommand cmdCart = new SqlCommand(deleteCartSql, conn, trans))
                            {
                                cmdCart.Parameters.AddWithValue("@UserID", userId);
                                cmdCart.ExecuteNonQuery();
                            }

                            trans.Commit();
                        }
                        catch (Exception innerEx)
                        {
                            trans.Rollback();
                            throw new Exception("Database inventory adjustment operation rejected. Rolling back alterations.", innerEx);
                        }
                    }
                }

                // Flush user session details and redirect to success landing page
                Session["Cart"] = null;
                Session["ActiveCheckoutOrderID"] = null;
                Response.Redirect("confirmOrder.aspx?id=" + orderId);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Process Error: " + ex.Message + (ex.InnerException != null ? " | Details: " + ex.InnerException.Message : "");
                lblStatus.Visible = true;
            }
        }
    }
}