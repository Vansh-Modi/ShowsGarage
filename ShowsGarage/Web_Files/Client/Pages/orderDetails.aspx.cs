using System;
using System.Data.SqlClient;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class order_details : System.Web.UI.Page
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
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblErrorMessage.Text = "⚠️ Missing parameters record routing exception. Return to history index panel.";
                    lblErrorMessage.Visible = true;
                    pnlTrackingInfo.Visible = false;
                    return;
                }

                int orderId = Convert.ToInt32(Request.QueryString["id"]);
                LoadTrackingMatrixData(orderId);
            }
        }

        private void LoadTrackingMatrixData(int orderId)
        {
            int currentUserId = Convert.ToInt32(Session["UserID"]);

            string query = @"
                SELECT OrderID, Status, ShippingAddress, TrackingPartner, TrackingNumber, EstimatedDeliveryDate 
                FROM [dbo].[Orders] 
                WHERE OrderID = @OrderID AND UserID = @UserID";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pnlTrackingInfo.Visible = true;
                                lblErrorMessage.Visible = false;

                                lblOrderID.Text = reader["OrderID"].ToString();
                                lblStatus.Text = reader["Status"].ToString();
                                lblDeliveryAddress.Text = reader["ShippingAddress"].ToString();

                                string partner = reader["TrackingPartner"].ToString();
                                string trackNo = reader["TrackingNumber"].ToString();
                                object estDate = reader["EstimatedDeliveryDate"];

                                if (!string.IsNullOrEmpty(partner))
                                    lblCourierName.Text = partner;

                                if (!string.IsNullOrEmpty(trackNo))
                                    lblTrackingNo.Text = trackNo;

                                if (estDate != DBNull.Value && estDate != null)
                                    lblEstDelivery.Text = Convert.ToDateTime(estDate).ToString("dd MMM yyyy (dddd)");
                            }
                            else
                            {
                                lblErrorMessage.Text = "❌ Access Boundary Violation: Record resource not found or missing structural security authorization tokens.";
                                lblErrorMessage.Visible = true;
                                pnlTrackingInfo.Visible = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = "❌ Processing Framework Failure: " + ex.Message;
                        lblErrorMessage.Visible = true;
                        pnlTrackingInfo.Visible = false;
                    }
                }
            }
        }
    }
}