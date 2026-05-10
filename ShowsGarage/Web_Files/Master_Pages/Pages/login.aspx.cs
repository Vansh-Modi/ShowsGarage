using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class login : System.Web.UI.Page
    {

        SqlConnection conn;
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] != null)
            {
                Response.Redirect("homePage.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Secure query using Parameters to prevent SQL Injection
                    string query = "SELECT UserID, Role, FullName FROM Users WHERE Email=@email AND PasswordHash=@pass";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        // 1. SET THE SESSIONS (This "wakes up" the Master Page logic)
                        Session["UserID"] = dr["UserID"].ToString();
                        Session["UserRole"] = dr["Role"].ToString();
                        Session["UserName"] = dr["FullName"].ToString();

                        // 2. REDIRECT BASED ON ROLE
                        if (dr["Role"].ToString() == "Admin")
                        {
                            Response.Redirect("AdminDashboard.aspx");
                        }
                        else if(dr["Role"].ToString() == "Client")
                        {
                            Response.Redirect("homePage.aspx");
                        }
                    }
                    else
                    {
                        // Show error if login fails
                        lblError.Text = "Invalid email or password. Please try again.";
                        lblError.ForeColor = System.Drawing.Color.FromArgb(255, 100, 100); // Soft red
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Database Error: " + ex.Message;
            }
        }
    }
}