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
            {
                btnRegister.Enabled = false;
                btnResendOTP.Visible = false;
            }
            else
            {
                // Re-hydrate passwords back into fields if a postback occurred
                if (Session["tempPass"] != null)
                {
                    txtPassword.Attributes.Add("value", Session["tempPass"].ToString());
                    txtConfirmPass.Attributes.Add("value", Session["tempPass"].ToString());
                }
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
                        string query = "INSERT INTO Users (FullName, Phone, Email, PasswordHash, Role, IsVerified) " +
                                       "VALUES (@name, @phone, @email, @pass, @role, @isVerified)";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                            cmd.Parameters.AddWithValue("@phone", txtNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                            // Pull safely from your persistent Session object
                            if (Session["tempPass"] != null)
                                cmd.Parameters.AddWithValue("@pass", Session["tempPass"].ToString());
                            else
                                cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());

                            cmd.Parameters.AddWithValue("@role", "Client");
                            cmd.Parameters.AddWithValue("@isVerified", true);

                            Session["tempPass"] = null; // Flush clean out on successful creation
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    lblError.Text = "Registration Successful!";
                    lblError.ForeColor = System.Drawing.Color.LightGreen;

                    Response.Redirect("~/homePage.aspx");
                }
                else
                {
                    lblError.Text = "Invalid OTP. Please check and try again.";
                    lblError.ForeColor = System.Drawing.Color.Red;

                    // Maintain password visuals even on failed registration postbacks
                    if (Session["tempPass"] != null)
                    {
                        txtPassword.Attributes.Add("value", Session["tempPass"].ToString());
                        txtConfirmPass.Attributes.Add("value", Session["tempPass"].ToString());
                    }
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
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                lblError.Text = "Please enter an email address first.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // CRITICAL FIX: Only read text fields if Session["tempPass"] doesn't already hold the token.
            // This stops resend clicks (which return blank inputs since the controls are disabled) from wiping your data.
            if (Session["tempPass"] == null)
            {
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    Session["tempPass"] = txtPassword.Text;
                }
            }

            // Ensure attributes are appended regardless of click counts so CSS stays clean
            if (Session["tempPass"] != null)
            {
                txtPassword.Attributes.Add("value", Session["tempPass"].ToString());
                txtConfirmPass.Attributes.Add("value", Session["tempPass"].ToString());
            }

            string otp = new Random().Next(100000, 999999).ToString();
            Session["GeneratedOTP"] = otp;

            System.Diagnostics.Debug.WriteLine("=== DEBUG OTP: " + otp + " ===");

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(txtEmail.Text.Trim());

                mail.From = new MailAddress("showsgarage@gmail.com", "Show's Garage");
                mail.Subject = "Your Verification Security Code";

                mail.Body = $@"
                    <div style='font-family: sans-serif; padding: 25px; background-color: #141414; color: #ffffff; border-radius: 8px; max-width: 480px;'>
                        <h2 style='color: #ff5722; margin-top: 0;'>Show's Garage</h2>
                        <p style='color: #aaaaaa; font-size: 14px;'>Welcome to the club! Use the verification security code below to activate your account profile logs:</p>
                        <div style='background-color: #1c1c1c; border: 1px solid #2d2d2d; padding: 15px; text-align: center; font-size: 26px; font-weight: 800; color: #2ebd59; letter-spacing: 5px; border-radius: 4px; margin: 20px 0;'>
                            {otp}
                        </div>
                        <p style='font-size: 11px; color: #666666; margin: 0;'>If you did not request this code, you can safely ignore this email validation trace.</p>
                    </div>";
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("showsgarage@gmail.com", "gikedyayowpsbhjq");
                smtp.EnableSsl = true;

                smtp.Send(mail);

                // Step 4: UI Management (Freeze fields on success execution)
                txtName.Enabled = false;
                txtNumber.Enabled = false;
                txtEmail.Enabled = false;
                txtPassword.Enabled = false;
                txtConfirmPass.Enabled = false;

                btnGetOTP.Visible = false;
                btnResendOTP.Visible = true;
                btnRegister.Enabled = true;

                lblError.Text = "OTP resent successfully to " + txtEmail.Text.Trim() + ". <br/><span style='font-size:12px; font-weight:normal; opacity:0.85;'>Can't find it? Please check your <b>Spam, Junk, or Updates</b> folders!</span>";
                lblError.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblError.Text = "Mail Delivery Error: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
                btnRegister.Enabled = false;
            }
        }
    }
}