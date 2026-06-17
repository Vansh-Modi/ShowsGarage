using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShowsGarage.Web_Files.Master_Pages.Pages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGlobalDynamicBrandingParameters();
                EvaluateUserRoleNavigationDeck();
            }
        }

        /// <summary>
        /// Pulls core branding configuration values directly out of SiteSettings table parameters schema row #1.
        /// </summary>
        private void LoadGlobalDynamicBrandingParameters()
        {
            string query = "SELECT TOP 1 Logo, LogoTitle, Copyright, Email, PhoneNumber FROM [dbo].[SiteSettings] WHERE [SiteSettingId] = 1";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 1. Safe Logo & Brand Title String Extractions
                                object dbLogo = reader["Logo"];
                                object dbLogoTitle = reader["LogoTitle"];

                                ibLogo.ImageUrl = (dbLogo == DBNull.Value || string.IsNullOrEmpty(dbLogo.ToString())) ?
                                    "~/Web_Files/Images/icons/Show's Garage Logo(Master Page BG).png" : dbLogo.ToString();

                                string companyTitle = (dbLogoTitle == DBNull.Value || string.IsNullOrEmpty(dbLogoTitle.ToString())) ?
                                    "Show's Garage" : dbLogoTitle.ToString();

                                ibLogo.AlternateText = companyTitle;
                                litFooterBrandTitle.Text = companyTitle.ToUpper();

                                // 2. Safe Copyright Extraction (This was causing the crash!)
                                // 2. Safe Copyright Extraction with Dynamic {year} Token Replacement
                                object dbCopyright = reader["Copyright"];
                                if (dbCopyright != DBNull.Value && !string.IsNullOrEmpty(dbCopyright.ToString()))
                                {
                                    string copyrightText = dbCopyright.ToString();

                                    // Dynamically replace the {year} string wrapper parameter with the current system year
                                    if (copyrightText.Contains("{year}"))
                                    {
                                        copyrightText = copyrightText.Replace("{year}", DateTime.Now.Year.ToString());
                                    }

                                    litCopyrightDisplay.Text = copyrightText;
                                }
                                else
                                {
                                    // Hardcoded global fallback defaults parameter string layout
                                    litCopyrightDisplay.Text = $"&copy; {DateTime.Now.Year} Shows Garage";
                                }

                                // 3. Safe Email Support Channel Checking
                                object dbEmail = reader["Email"];
                                if (dbEmail != DBNull.Value && !string.IsNullOrEmpty(dbEmail.ToString()))
                                {
                                    lnkFooterEmail.HRef = "mailto:" + dbEmail.ToString().Trim();
                                    lnkFooterEmail.InnerText = dbEmail.ToString().Trim();
                                    lnkFooterEmail.Visible = true;
                                }
                                else
                                {
                                    lnkFooterEmail.Visible = false;
                                }

                                // 4. Safe Phone Number Contact Checking
                                object dbPhone = reader["PhoneNumber"];
                                if (dbPhone != DBNull.Value && !string.IsNullOrEmpty(dbPhone.ToString().Trim()))
                                {
                                    lnkFooterPhone.HRef = "tel:" + dbPhone.ToString().Trim();
                                    lnkFooterPhone.InnerText = "Call: " + dbPhone.ToString().Trim();
                                    lnkFooterPhone.Visible = true;
                                }
                                else
                                {
                                    lnkFooterPhone.Visible = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Fallback catch block prevents the footer line from ever disappearing entirely
                        ibLogo.ImageUrl = "~/Web_Files/Images/icons/Show's Garage Logo(Master Page BG).png";
                        litFooterBrandTitle.Text = "SHOWS GARAGE";
                        litCopyrightDisplay.Text = "&copy; 2026 Shows Garage";
                        lnkFooterEmail.Visible = false;
                        lnkFooterPhone.Visible = false;
                    }
                }
            }
        }

        private void EvaluateUserRoleNavigationDeck()
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                pnlAdminNav.Visible = true;
                pnlUserNav.Visible = false;
                Panel2.Visible = true;
                Panel1.Visible = false;
            }
            else
            {
                pnlAdminNav.Visible = false;
                pnlUserNav.Visible = true;
                Panel2.Visible = false;
                Panel1.Visible = true;
            }
        }

        protected void ibUserLogout_Click(object sender, ImageClickEventArgs e)
        {
            ExecuteGlobalLogoutSequence();
        }

        protected void ibAdminLogout_Click(object sender, ImageClickEventArgs e)
        {
            ExecuteGlobalLogoutSequence();
        }

        private void ExecuteGlobalLogoutSequence()
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/homePage.aspx");
        }

        protected void btnFooterJoin_Click(object sender, EventArgs e)
        {
            txtFooterEmail.Text = string.Empty;
        }

        protected void ibLogo_Click(object sender, ImageClickEventArgs e)
        {
            // FIXED: Added safe null comparison validation criteria check step before string conversion evaluation drops
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin")
            {
                Response.Redirect("~/Web_Files/Admin/Pages/dashboard.aspx");
            }
            else
            {
                Response.Redirect("~/homePage.aspx");
            }
        }
    }
}