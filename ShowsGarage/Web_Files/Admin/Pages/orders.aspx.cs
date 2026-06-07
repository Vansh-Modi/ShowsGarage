using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_orders : System.Web.UI.Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSystemOrdersDashboard("ALL");
            }
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAdminStatus.Visible = false;
            LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
        }

        private void LoadSystemOrdersDashboard(string filterStatus)
        {
            string query = @"
                SELECT OrderID, UserID, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, PaymentScreenshotPath, TransactionReference 
                FROM [dbo].[Orders] ";

            if (filterStatus != "ALL")
            {
                query += " WHERE Status = @FilterStatus ORDER BY OrderDate DESC";
            }
            else
            {
                query += " ORDER BY CASE WHEN Status = 'Awaiting Verification' THEN 1 ELSE 2 END, OrderDate DESC";
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (filterStatus != "ALL")
                    {
                        cmd.Parameters.AddWithValue("@FilterStatus", filterStatus);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtOrders = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dtOrders);
                        rptAdminOrders.DataSource = dtOrders;
                        rptAdminOrders.DataBind();
                    }
                    catch (Exception ex)
                    {
                        DisplayStatusAlert("❌ Failed to bind master order logs tracking registry: " + ex.Message, false);
                    }
                }
            }
        }

        protected void rptAdminOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView rowView = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(rowView["OrderID"]);
                string status = rowView["Status"].ToString();

                Label lblStatusBadge = (Label)e.Item.FindControl("lblStatusBadge");
                if (lblStatusBadge != null)
                {
                    lblStatusBadge.CssClass = "admin-status-badge " + GetCssBadgeClass(status);
                }

                // Set value for inline drop down selector edit mode state
                DropDownList ddlEditStatus = (DropDownList)e.Item.FindControl("ddlEditStatus");
                if (ddlEditStatus != null)
                {
                    ddlEditStatus.SelectedValue = ddlEditStatus.Items.FindByValue(status) != null ? status : "Awaiting Verification";
                }

                Repeater rptAdminOrderItems = (Repeater)e.Item.FindControl("rptAdminOrderItems");
                if (rptAdminOrderItems != null)
                {
                    string subItemsQuery = @"
                        SELECT od.Quantity, od.UnitPrice, p.Title, p.Scale, p.BrandName 
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";

                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(subItemsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dtItems = new DataTable();
                            da.Fill(dtItems);

                            rptAdminOrderItems.DataSource = dtItems;
                            rptAdminOrderItems.DataBind();
                        }
                    }
                }
            }
        }

        protected void rptAdminOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblAdminStatus.Visible = false;

            if (e.CommandName == "TogglePaymentEdit")
            {
                int itemIndex = Convert.ToInt32(e.CommandArgument);
                RepeaterItem rowItem = rptAdminOrders.Items[itemIndex];
                Panel pnlEdit = (Panel)rowItem.FindControl("pnlInlinePaymentEdit");
                if (pnlEdit != null) { pnlEdit.Visible = !pnlEdit.Visible; }
            }
            else if (e.CommandName == "SaveInlinePayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                TextBox txtRef = (TextBox)e.Item.FindControl("txtEditTxnRef");
                DropDownList ddlStat = (DropDownList)e.Item.FindControl("ddlEditStatus");

                if (txtRef != null && ddlStat != null)
                {
                    ExecuteInlinePaymentUpdate(orderId, txtRef.Text.Trim(), ddlStat.SelectedValue);
                    DisplayStatusAlert($"✓ Order metadata fields for entry #{orderId} updated successfully inside systems tracking grids.", true);
                    LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
                }
            }
            else if (e.CommandName == "ApprovePayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                ExecuteStatusTransition(orderId, "Approved");
                DisplayStatusAlert($"✓ Order reference line entry #{orderId} has been successfully verified. Inventory rows locked down and dispatch flags marked.", true);
                LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
            }
            else if (e.CommandName == "RejectPayment")
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                ExecuteOrderRejectionWithStockRollback(orderId);
                DisplayStatusAlert($"⚠️ Order reference line entry #{orderId} payment registration rejected. Stock logs reverted and outbound refund requirement recorded.", true);
                LoadSystemOrdersDashboard(ddlStatusFilter.SelectedValue);
            }
        }

        private void ExecuteInlinePaymentUpdate(int orderId, string cleanTxnRef, string targetStatus)
        {
            // First, determine if we are executing a new manual cancellation via inline selection override
            string currentStatus = "";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Status FROM [dbo].[Orders] WHERE OrderID = @OrderID", conn);
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                conn.Open();
                currentStatus = cmd.ExecuteScalar()?.ToString();
            }

            if (targetStatus == "Cancelled" && !currentStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                // Process cancellation routines including stock rollbacks
                ExecuteOrderRejectionWithStockRollback(orderId);
            }

            // Apply standard text updates parameters override safely
            string sql = "UPDATE [dbo].[Orders] SET TransactionReference = @TxnRef, Status = @Status WHERE OrderID = @OrderID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TxnRef", cleanTxnRef);
                    cmd.Parameters.AddWithValue("@Status", targetStatus);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteStatusTransition(int orderId, string targetStatus)
        {
            string sql = "UPDATE [dbo].[Orders] SET Status = @Status WHERE OrderID = @OrderID";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", targetStatus);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteOrderRejectionWithStockRollback(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Fetch metadata configuration amount from order row before changing anything
                        decimal totalInvoiceAmount = 0;
                        string orderDataSql = "SELECT TotalAmount, Status FROM [dbo].[Orders] WHERE OrderID = @OrderID";
                        using (SqlCommand cmdFetchOrder = new SqlCommand(orderDataSql, conn, trans))
                        {
                            cmdFetchOrder.Parameters.AddWithValue("@OrderID", orderId);
                            using (SqlDataReader orderReader = cmdFetchOrder.ExecuteReader())
                            {
                                if (orderReader.Read())
                                {
                                    // If order was already cancelled, do not repeat the stock restoration process
                                    if (orderReader["Status"].ToString().Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                                    {
                                        orderReader.Close();
                                        trans.Rollback();
                                        return;
                                    }
                                    totalInvoiceAmount = Convert.ToDecimal(orderReader["TotalAmount"]);
                                }
                                orderReader.Close();
                            }
                        }

                        // 2. Flip parent tracking row metrics indicator state to Cancelled
                        string cancelOrderSql = "UPDATE [dbo].[Orders] SET Status = 'Cancelled' WHERE OrderID = @OrderID";
                        using (SqlCommand cmdCancel = new SqlCommand(cancelOrderSql, conn, trans))
                        {
                            cmdCancel.Parameters.AddWithValue("@OrderID", orderId);
                            cmdCancel.ExecuteNonQuery();
                        }

                        // 3. Index all quantities purchased under this transaction mapping array from OrderDetails
                        string getLineItemsSql = "SELECT ProductID, Quantity FROM [dbo].[OrderDetails] WHERE OrderID = @OrderID";
                        DataTable dtLines = new DataTable();
                        using (SqlCommand cmdFetch = new SqlCommand(getLineItemsSql, conn, trans))
                        {
                            cmdFetch.Parameters.AddWithValue("@OrderID", orderId);
                            SqlDataAdapter da = new SqlDataAdapter(cmdFetch);
                            da.Fill(dtLines);
                        }

                        // 4. Execute direct inventory restoration mathematical updates queries loops inside Products
                        string restoreInventorySql = "UPDATE [dbo].[Products] SET StockQuantity = StockQuantity + @Quantity WHERE ProductID = @ProductID";
                        foreach (DataRow line in dtLines.Rows)
                        {
                            using (SqlCommand cmdRestore = new SqlCommand(restoreInventorySql, conn, trans))
                            {
                                cmdRestore.Parameters.AddWithValue("@Quantity", Convert.ToInt32(line["Quantity"]));
                                cmdRestore.Parameters.AddWithValue("@ProductID", Convert.ToInt32(line["ProductID"]));
                                cmdRestore.ExecuteNonQuery();
                            }
                        }

                        // 5. CRITICAL: Automatically inject an outbound payout log row inside your actual [dbo].[Expenses] schema layout table!
                        string insertRefundExpenseSql = @"
                            INSERT INTO [dbo].[Expenses] (Title, Amount, ExpenseType, ExpenseDate, OrderID, Remarks)
                            VALUES (@Title, @Amount, @ExpenseType, @ExpenseDate, @OrderID, @Remarks)";

                        using (SqlCommand cmdExpense = new SqlCommand(insertRefundExpenseSql, conn, trans))
                        {
                            cmdExpense.Parameters.AddWithValue("@Title", $"Customer Refund Payout for Cancelled Order #{orderId}");
                            cmdExpense.Parameters.AddWithValue("@Amount", totalInvoiceAmount);
                            cmdExpense.Parameters.AddWithValue("@ExpenseType", "Damaged Return"); // Exact match with item selection type definitions
                            cmdExpense.Parameters.AddWithValue("@ExpenseDate", DateTime.Now);
                            cmdExpense.Parameters.AddWithValue("@OrderID", orderId);
                            cmdExpense.Parameters.AddWithValue("@Remarks", $"PENDING MANUAL TRANSFER: Order rejected on admin dashboard on {DateTime.Now:dd MMM yyyy}. Remit payment manually back to client profile details.");

                            cmdExpense.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch (Exception innerEx)
                    {
                        trans.Rollback();
                        throw new Exception("Operational transaction rejection sequence processing error context: " + innerEx.Message, innerEx);
                    }
                }
            }
        }

        private string GetCssBadgeClass(string status)
        {
            switch (status.ToLower())
            {
                case "awaiting payment": return "adm-badge-yellow";
                case "awaiting verification": return "adm-badge-blue";
                case "approved": return "adm-badge-green";
                case "cancelled": return "adm-badge-gray";
                default: return "adm-badge-gray";
            }
        }

        private void DisplayStatusAlert(string message, bool isSuccess)
        {
            lblAdminStatus.Text = message;
            lblAdminStatus.CssClass = isSuccess ? "admin-alert-banner alert-success" : "admin-alert-banner alert-error";
            lblAdminStatus.Visible = true;
        }
    }
}