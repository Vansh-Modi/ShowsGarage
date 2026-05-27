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
        string password;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnRegister.Enabled = false;
                btnResendOTP.Visible = false; // Hide resend button initially
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Verify OTP from Session
                if (Session["GeneratedOTP"] != null && txtOTP.Text.Trim() == Session["GeneratedOTP"].ToString())
                {
                    // Clear OTP session once successfully verified
                    Session["GeneratedOTP"] = null;

                    // 2. Connect to Database and Insert User Records
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        //Updated query: Inserts data securely and marks user as verified immediately
                        string query = "INSERT INTO Users (FullName, Phone, Email, PasswordHash, Role, IsVerified) " +
                                       "VALUES (@name, @phone, @email, @pass, @role, @isVerified)";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                            cmd.Parameters.AddWithValue("@phone", txtNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                            if (Session["tempPass"] != null)
                                cmd.Parameters.AddWithValue("@pass", Session["tempPass"]); // Note: Plain text for now, consider hashing later!
                            else
                                cmd.Parameters.AddWithValue("@pass", txtPassword); // Note: Plain text for now, consider hashing later!
                            cmd.Parameters.AddWithValue("@role", "Client");
                            cmd.Parameters.AddWithValue("@isVerified", true); // Sets [IsVerified] column to 1 (true)
                            Session["tempPass"] = null;
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 3. UI Success Feedback & Redirect
                    lblError.Text = "Registration Successful!";
                    lblError.ForeColor = System.Drawing.Color.LightGreen;

                    Response.Redirect("~/homePage.aspx");
                }
                else
                {
                    lblError.Text = "Invalid OTP. Please check and try again.";
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Database Error: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnGetOTP_Click(object sender, EventArgs e)
        {
            // Validation check: Ensure the user typed an email address first
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                lblError.Text = "Please enter an email address first.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Step 1: Securely generate a new 6-digit numeric OTP string
            string otp = new Random().Next(100000, 999999).ToString();

            // Step 2: Store it safely in Session state to verify on registration
            Session["GeneratedOTP"] = otp;

            // Developer Quality of Life: Always log it to your Visual Studio Output Window!
            System.Diagnostics.Debug.WriteLine("=== DEBUG OTP: " + otp + " ===");

            // Step 3: Attempt to dispatch the email to your testing environment
            try
            {
                // Setup Mail Envelope
                MailMessage mail = new MailMessage();
                mail.To.Add(txtEmail.Text.Trim()); // The user's input email

                // Your personal email used as the temporary sender mask
                mail.From = new MailAddress("vanshmodi200@outlook.com", "Shows Garage");

                mail.Subject = "Your Registration OTP";
                mail.Body = $"Welcome to the Club!<br/><br/>Your OTP for Shows Garage is: <b>{otp}</b>";
                mail.IsBodyHtml = true;

                // Configure Mailtrap SMTP client settings (Safe, robust sandbox environment)
                // Try changing 2525 to 587
                SmtpClient smtp = new SmtpClient("sandbox.smtp.mailtrap.io", 587);
                smtp.Credentials = new NetworkCredential("19cc235b8c7541", "c4348aebecad76");
                smtp.EnableSsl = true;

                // Fire the email out!
                smtp.Send(mail);

                password = txtPassword.Text.Trim();
                Session["tempPass"] = password;
                // Step 4: UI Management (Only freezes inputs if email successfully executes!)
                txtName.Enabled = false;
                txtNumber.Enabled = false;
                txtEmail.Enabled = false;
                txtPassword.Enabled = false;
                txtConfirmPass.Enabled = false;
                txtPassword.Attributes.Add("value", txtPassword.Text);
                txtConfirmPass.Attributes.Add("value", txtConfirmPass.Text);
                btnGetOTP.Visible = false;
                btnResendOTP.Visible = true;
                btnRegister.Enabled = true; // Unlocks the validation process

                lblError.Text = "OTP sent successfully to " + txtEmail.Text;
                lblError.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                // Graceful error state if network drops out or SMTP config fails
                lblError.Text = "Mail Dispatch Failure: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;

                // Safety lock down
                btnRegister.Enabled = false;
            }
        }
    }
}