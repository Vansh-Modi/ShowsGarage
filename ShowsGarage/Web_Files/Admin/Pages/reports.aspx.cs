using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace ShowsGarage.Web_Files.Admin
{
    public partial class admin_pl_report : System.Web.UI.Page
    {
        private string ConnectionString => System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        // Public properties used to dynamically seed data strings directly into frontend chart engines
        public decimal ValueGrossSales { get; set; } = 0;
        public decimal ValueSourcingCost { get; set; } = 0;
        public decimal ValueTotalExpenses { get; set; } = 0;
        public decimal ValueShippingCollected { get; set; } = 0;

        public int CountApproved { get; set; } = 0;
        public int CountAwaitingVerify { get; set; } = 0;
        public int CountAwaitingPay { get; set; } = 0;
        public int CountCancelled { get; set; } = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CalculateGarageProfitLossStatement();
            }
        }

        private void CalculateGarageProfitLossStatement()
        {
            lblMessage.Visible = false;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    // 1. Gather Order-based metrics variables (Revenue, Shipping, Counts)
                    string orderMetricsSql = @"
                        SELECT 
                            ISNULL(SUM(CASE WHEN Status = 'Approved' THEN TotalAmount ELSE 0 END), 0) AS GrossSales,
                            COUNT(OrderID) AS TotalOrders,
                            ISNULL(SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END), 0) AS ApprovedCount,
                            ISNULL(SUM(CASE WHEN Status = 'Awaiting Verification' THEN 1 ELSE 0 END), 0) AS VerifyCount,
                            ISNULL(SUM(CASE WHEN Status = 'Awaiting Payment' THEN 1 ELSE 0 END), 0) AS PayCount,
                            ISNULL(SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END), 0) AS CancelledCount
                        FROM [dbo].[Orders]";

                    using (SqlCommand cmd = new SqlCommand(orderMetricsSql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ValueGrossSales = Convert.ToDecimal(reader["GrossSales"]);
                                litTotalOrdersCount.Text = reader["TotalOrders"].ToString();

                                CountApproved = Convert.ToInt32(reader["ApprovedCount"]);
                                CountAwaitingVerify = Convert.ToInt32(reader["VerifyCount"]);
                                CountAwaitingPay = Convert.ToInt32(reader["PayCount"]);
                                CountCancelled = Convert.ToInt32(reader["CancelledCount"]);
                            }
                        }
                    }

                    // 2. Compute dynamic wholesale Product Cost Price outlays linked to total sold quantities matrix
                    string sourcingCostSql = @"
                        SELECT ISNULL(SUM(od.Quantity * p.CostPrice), 0)
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Products] p ON od.ProductID = p.ProductID
                        INNER JOIN [dbo].[Orders] o ON od.OrderID = o.OrderID
                        WHERE o.Status = 'Approved'";

                    using (SqlCommand cmdCost = new SqlCommand(sourcingCostSql, conn))
                    {
                        ValueSourcingCost = Convert.ToDecimal(cmdCost.ExecuteScalar());
                        litTotalInventorySourcingCost.Text = string.Format("{0:N2}", ValueSourcingCost);
                    }

                    // 3. Fallback tracking computation to isolate shipping rules collected configurations parameters
                    // (Takes approved gross order totals minus direct retail values to isolate dynamic shipping charges sums)
                    string retailSumSql = @"
                        SELECT ISNULL(SUM(od.Quantity * od.UnitPrice), 0)
                        FROM [dbo].[OrderDetails] od
                        INNER JOIN [dbo].[Orders] o ON od.OrderID = o.OrderID
                        WHERE o.Status = 'Approved'";

                    using (SqlCommand cmdRetail = new SqlCommand(retailSumSql, conn))
                    {
                        decimal retailItemsTotal = Convert.ToDecimal(cmdRetail.ExecuteScalar());
                        ValueShippingCollected = Math.Max(0, ValueGrossSales - retailItemsTotal);
                        litTotalShippingFees.Text = string.Format("{0:N2}", ValueShippingCollected);
                    }

                    // 4. Query global operational expenses outflow ledger totals
                    string expensesQuery = "SELECT ISNULL(SUM(Amount), 0) FROM [dbo].[Expenses]";
                    using (SqlCommand cmdExp = new SqlCommand(expensesQuery, conn))
                    {
                        ValueTotalExpenses = Convert.ToDecimal(cmdExp.ExecuteScalar());
                    }

                    // Update structural overview widgets card UI interface labels indicators
                    lblTotalRevenue.Text = string.Format("{0:N2}", ValueGrossSales);
                    lblTotalExpenses.Text = string.Format("{0:N2}", ValueTotalExpenses);

                    decimal netMargin = ValueGrossSales - ValueTotalExpenses;
                    lblNetMargin.Text = string.Format("{0:N2}", Math.Abs(netMargin));

                    if (netMargin >= 0)
                    {
                        divNetProfitCard.Attributes["class"] = "pl-stat-card card-net-margin state-profit";
                        lblNetStatusIcon.Text = "🟢 Profit";
                    }
                    else
                    {
                        divNetProfitCard.Attributes["class"] = "pl-stat-card card-net-margin state-loss";
                        lblNetStatusIcon.Text = "🔴 Loss Deficit";
                        lblNetMargin.Text = "-" + lblNetMargin.Text;
                    }

                    // 5. Build categorized breakdown repeater items list
                    string breakdownQuery = @"
                        SELECT ExpenseType, SUM(Amount) AS CategoryTotal 
                        FROM [dbo].[Expenses] 
                        GROUP BY ExpenseType 
                        ORDER BY CategoryTotal DESC";

                    using (SqlCommand cmdBreakdown = new SqlCommand(breakdownQuery, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmdBreakdown);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        rptExpenseTypeBreakdown.DataSource = dt;
                        rptExpenseTypeBreakdown.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "❌ Financial Ledger Analytics Compilation Failure: " + ex.Message;
                    lblMessage.Visible = true;
                }
            }
        }
    }
}