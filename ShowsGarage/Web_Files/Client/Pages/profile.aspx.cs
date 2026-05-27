using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Client.Pages
{
    public partial class profile : System.Web.UI.Page
    {
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["UserEmail"] == null)
            {
                Response.Redirect("~/Web_Files/Master_Pages/Pages/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT FullName, Phone, Email FROM Users WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["FullName"].ToString();
                                txtName.Text = fullName;
                                txtNumber.Text = reader["Phone"].ToString();
                                txtEmail.Text = reader["Email"].ToString();

                                // Render the lowercase welcome display matching reference layout
                                litWelcomeName.Text = fullName.ToLower();

                                txtEmail.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading profile data: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "UPDATE Users SET FullName = @name, Phone = @phone WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblStatus.Text = "Profile text updates saved successfully!";
                            lblStatus.ForeColor = System.Drawing.Color.LightGreen;

                            // Dynamically sync greeting block
                            litWelcomeName.Text = txtName.Text.Trim().ToLower();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Profile Save Error: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string checkQuery = "SELECT PasswordHash FROM Users WHERE Email = @email";
                    string currentDbPassword = "";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());
                        object result = checkCmd.ExecuteScalar();
                        if (result != null)
                        {
                            currentDbPassword = result.ToString();
                        }
                    }

                    if (txtCurrentPassword.Text != currentDbPassword)
                    {
                        lblStatus.Text = "Incorrect current password. Identity authentication check failed.";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    string updateQuery = "UPDATE Users SET PasswordHash = @newPassword WHERE Email = @email";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@newPassword", txtNewPassword.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblStatus.Text = "Password updated securely and successfully!";
                            lblStatus.ForeColor = System.Drawing.Color.LightGreen;

                            txtCurrentPassword.Text = string.Empty;
                            txtNewPassword.Text = string.Empty;
                            txtConfirmNewPassword.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Credentials Sync Failure: " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}