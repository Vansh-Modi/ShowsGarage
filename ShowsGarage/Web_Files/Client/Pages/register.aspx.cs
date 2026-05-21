using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Mail;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class register : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                btnRegister.Enabled = false;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOTP.Text.Trim() == Session["GeneratedOTP"]?.ToString())
                {
                    // SUCCESS: Run your SQL Insert Code here
                    lblError.Text = "Registration Successful!";
                }
                else
                {
                    lblError.Text = "Invalid OTP. Please check and try again.";
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Secure query using Parameters to prevent SQL Injection
                    //string query = "INTO INTO Users UserID, Role, FullName FROM Users WHERE Email=@email AND PasswordHash=@pass";

                    //SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    //cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                    //con.Open();
                    //SqlDataReader dr = cmd.ExecuteReader();

                    //if (dr.Read())
                    //{
                    //    // 1. SET THE SESSIONS (This "wakes up" the Master Page logic)
                    //    Session["UserID"] = dr["UserID"].ToString();
                    //    Session["UserRole"] = dr["Role"].ToString();
                    //    Session["UserName"] = dr["FullName"].ToString();

                    //    // 2. REDIRECT BASED ON ROLE
                    //    if (dr["Role"].ToString() == "Admin")
                    //    {
                    //        Response.Redirect("AdminDashboard.aspx");
                    //    }
                    //    else if (dr["Role"].ToString() == "Client")
                    //    {
                    //        Response.Redirect("homePage.aspx");
                    //    }
                    //}
                    //else
                    //{
                    //    // Show error if login fails
                    //    lblError.Text = "Invalid email or password. Please try again.";
                    //    lblError.ForeColor = System.Drawing.Color.FromArgb(255, 100, 100); // Soft red
                    //}
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Database Error: " + ex.Message;
            }
        }

        protected void btnGetOTP_Click(object sender, EventArgs e)
        {
            // 1. Generate the Code
            string otp = new Random().Next(100000, 999999).ToString();
            Session["GeneratedOTP"] = otp;

            // 2. UI Updates
            txtName.Enabled = false;
            txtNumber.Enabled = false;
            txtEmail.Enabled = false;
            txtPassword.Enabled = false;
            txtConfirmPass.Enabled = false;

            btnGetOTP.Visible = false;
            btnResendOTP.Visible = true;

            btnRegister.Enabled = true;

            // 3. Feedback
            lblError.Text = "OTP sent to your email!";
            lblError.ForeColor = System.Drawing.Color.LightGreen;

            // DEBUG: Show the OTP for your trial and error
            System.Diagnostics.Debug.WriteLine("DEBUG OTP: " + otp);

            try
            {
                // 2. Setup the Mail Message
                MailMessage mail = new MailMessage();
                mail.To.Add(txtEmail.Text.Trim());
                mail.From = new MailAddress("vanshmodi200@outlook.com", "Shows Garage");
                mail.Subject = "Your Registration OTP";
                mail.Body = $"Welcome to the Club! Your OTP for Shows Garage is: {otp}";
                mail.IsBodyHtml = true;

                // 3. Setup the SMTP Client (The Server)
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                // IMPORTANT: Use an 'App Password', not your regular login password
                smtp.Credentials = new NetworkCredential("vanshmodi200@outlook.com", "spninemxuhkiewlx");

                //IMPORTANT NOTE: Code given for the replacement of the above as the credentials are not working

                //SmtpClient smtp = new SmtpClient("sandbox.smtp.mailtrap.io", 2525);
                //smtp.Credentials = new System.Net.NetworkCredential("your_mailtrap_username", "your_mailtrap_password");
                //smtp.EnableSsl = true;

                // 4. Send it!
                smtp.Send(mail);

                lblError.Text = "OTP sent to " + txtEmail.Text;
                lblError.ForeColor = System.Drawing.Color.LightGreen;
                btnRegister.Enabled = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "Mail Error: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}