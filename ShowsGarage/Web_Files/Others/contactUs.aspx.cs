using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Others
{
    public partial class contactUs : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Secure query using Parameters to prevent SQL Injection
                    string query = "INSERT INTO contactUs(name, email, message) VALUES(@name, @email, @message)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtContactName.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtContactEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@message", txtMessage.Text.Trim());

                    con.Open();
                    

                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Database Error: " + ex.Message;
            }
        }
    }
}
