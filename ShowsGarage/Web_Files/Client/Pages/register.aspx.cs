using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class register : System.Web.UI.Page
    {
        private string connStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnRegister.Enabled = false;
                btnResendOTP.Visible = false;
            }
            else if (Session["tempPass"] != null)
            {
                // Re-hydrate passwords back into fields if a postback occurred to preserve UI state
                txtPassword.Attributes.Add("value", Session["tempPass"].ToString());
                txtConfirmPass.Attributes.Add("value", Session["tempPass"].ToString());
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Verify OTP code matches what was stored in the session memory
                if (Session["GeneratedOTP"] != null && txtOTP.Text.Trim() == Session["GeneratedOTP"].ToString())
                {
                    Session["GeneratedOTP"] = null; // Flush token immediately upon verification success
                    string query = "INSERT INTO Users (FullName, Phone, Email, PasswordHash, Role, IsVerified) VALUES (@name, @phone, @email, @pass, @role, @isVerified)";

                    using (SqlConnection con = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());

                        // Pull securely from session token cache to handle blank disabled control values
                        string securePassword = (Session["tempPass"] != null) ? Session["tempPass"].ToString() : txtPassword.Text.Trim();
                        cmd.Parameters.AddWithValue("@pass", securePassword);

                        cmd.Parameters.AddWithValue("@role", "Client");
                        cmd.Parameters.AddWithValue("@isVerified", true);

                        Session["tempPass"] = null; // Clear password state cache safely
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    lblError.Text = "Registration Successful!";
                    lblError.ForeColor = Color.LightGreen;
                    Response.Redirect("~/homePage.aspx");
                }
                else
                {
                    lblError.Text = "Invalid OTP. Please check and try again.";
                    lblError.ForeColor = Color.Red;

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
                lblError.ForeColor = Color.Red;
            }
        }

        protected void btnGetOTP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                lblError.Text = "Please enter an email address first.";
                lblError.ForeColor = Color.Red;
                return;
            }

            // Lock field values into a safe temporary variable block on original request pass
            if (Session["tempPass"] == null && !string.IsNullOrEmpty(txtPassword.Text))
            {
                Session["tempPass"] = txtPassword.Text;
            }

            if (Session["tempPass"] != null)
            {
                txtPassword.Attributes.Add("value", Session["tempPass"].ToString());
                txtConfirmPass.Attributes.Add("value", Session["tempPass"].ToString());
            }

            string otp = new Random().Next(100000, 999999).ToString();
            Session["GeneratedOTP"] = otp;

            try
            {
                using (MailMessage mail = new MailMessage())
                {
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

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("showsgarage@gmail.com", "gikedyayowpsbhjq");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                // UI Architecture Management: Freeze existing inputs to lock transaction context state
                txtName.Enabled = false;
                txtNumber.Enabled = false;
                txtEmail.Enabled = false;
                txtPassword.Enabled = false;
                txtConfirmPass.Enabled = false;

                btnGetOTP.Visible = false;
                btnResendOTP.Visible = true;
                btnRegister.Enabled = true;

                lblError.Text = "OTP sent successfully to " + txtEmail.Text.Trim() + ". <br/><span style='font-size:12px; font-weight:normal; opacity:0.85;'>Can't find it? Please check your <b>Spam, Junk, or Updates</b> folders!</span>";
                lblError.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblError.Text = "Mail Delivery Error: " + ex.Message;
                lblError.ForeColor = Color.Red;
                btnRegister.Enabled = false;
            }
        }
    }
}