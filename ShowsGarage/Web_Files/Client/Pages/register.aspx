<%@ Page Title="Show's Garage | Registration" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/Web_Files/Client/Styles/register.css?v=3" rel="stylesheet" type="text/css" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-theme-wrapper">
        <div class="auth-centered-container">
            <div class="auth-card-width">
                
                <div class="auth-header-block">
                    <h2 class="auth-main-heading">Join Club</h2>
                    <p class="auth-subtitle-text">Create your show's garage account</p>
                </div>

                <div class="auth-form-body">
                    <div class="input-group">
                        <label>User Name</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="auth-input" placeholder="Enter User Name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" Display="Dynamic" CssClass="auth-error-message" ErrorMessage="Name Required" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                    </div>

                    <div class="input-group">
                        <label>Phone Number</label>
                        <asp:TextBox ID="txtNumber" runat="server" CssClass="auth-input" TextMode="Phone" placeholder="Enter Phone Number"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNumber" runat="server" ControlToValidate="txtNumber" CssClass="auth-error-message" Display="Dynamic" ErrorMessage="Phone Number Required" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revNumber" runat="server" ControlToValidate="txtNumber" CssClass="auth-error-message" Display="Dynamic" ErrorMessage="Enter Correct Number" ValidationExpression="^[6-9]\d{9}$" ValidationGroup="RegGroup"></asp:RegularExpressionValidator>
                    </div>

                    <div class="input-group">
                        <label>Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="auth-input" placeholder="Enter your email" AutoPostBack="True" TextMode="Email"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="auth-error-message" Display="Dynamic" ErrorMessage="Email Required" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" Display="Dynamic" CssClass="auth-error-message" ErrorMessage="Invalid Email" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="RegGroup" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                    </div>

                    <div class="input-group">
                        <label>Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" CssClass="auth-error-message" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="Password Required" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revPassword" runat="server" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$" ErrorMessage="Password must be 8-20 characters, include a capital letter, a number, and a symbol." ControlToValidate="txtPassword" Display="Dynamic" ValidationGroup="RegGroup" CssClass="auth-error-message"></asp:RegularExpressionValidator>
                    </div>

                    <div class="input-group">
                        <label>Confirm Password</label>
                        <asp:TextBox ID="txtConfirmPass" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPass" runat="server" CssClass="auth-error-message" ControlToValidate="txtConfirmPass" Display="Dynamic" ErrorMessage="Password Required" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvConfirmPass" runat="server" CssClass="auth-error-message" ErrorMessage="Password Mismatch" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPass" Display="Dynamic" ValidationGroup="RegGroup"></asp:CompareValidator>
                    </div>

                    <div class="input-group">
                        <label>OTP</label>
                        <div class="auth-inline-input-row">
                            <asp:TextBox ID="txtOTP" runat="server" CssClass="auth-input auth-flex-input" placeholder="Enter OTP"></asp:TextBox>
                            <asp:Button ID="btnGetOTP" runat="server" Text="Get OTP" CssClass="btn-inline-action" OnClick="btnGetOTP_Click" ValidationGroup="RegGroup" />
                        </div>
                        <div class="auth-resend-link-box">
                            <asp:LinkButton ID="btnResendOTP" runat="server" Visible="false" OnClick="btnGetOTP_Click" CssClass="auth-resend-btn">
                                Didn't get it? Resend OTP
                            </asp:LinkButton>
                        </div>
                    </div>

                    <asp:Button ID="btnRegister" runat="server" Text="REGISTER" CssClass="auth-primary-submit-btn" OnClick="btnRegister_Click" ValidationGroup="RegGroup" />

                    <div class="auth-footer-routing-box">
                        <p class="auth-footer-text">Already a member? <a href="/Web_Files/Master_Pages/Pages/login.aspx" class="auth-redirect-link">Login here</a></p>
                        <asp:Label ID="lblError" runat="server" CssClass="auth-system-alert-text"></asp:Label>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>