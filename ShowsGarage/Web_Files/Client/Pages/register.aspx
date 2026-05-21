<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Show's Garage | Registration </title>
    <link href="/Web_Files/Client/Styles/register.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-wrapper">
        <div class="auth-card">
            <div class="auth-header">
                <h2>Join Club</h2>
                <p>Create your show's garage account</p>
            </div>

            <div class="auth-form">
                <div class="input-group">
                    <label>User Name</label>
                    &nbsp;<asp:TextBox ID="txtName" runat="server" CssClass="auth-input" placeholder="Enter User Name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" Display="Dynamic" CssClass="error-text" ErrorMessage="Name Required" ForeColor="Red" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                </div>

                <div class="input-group">
                    <label>Phone Number</label>
                    &nbsp;<asp:TextBox ID="txtNumber" runat="server" CssClass="auth-input" TextMode="Phone" placeholder="Enter Phone Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNumber" runat="server" ControlToValidate="txtNumber" CssClass="error-text" Display="Dynamic" ErrorMessage="Phone Number Required" ForeColor="Red" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revNumber" runat="server" ControlToValidate="txtNumber" CssClass="error-text" Display="Dynamic" ErrorMessage="Enter Correct Number" ForeColor="Red" ValidationExpression="^[6-9]\d{9}$" ValidationGroup="RegGroup"></asp:RegularExpressionValidator>
                </div>

                <div class="input-group">
                    <label>Email Address</label>
                    &nbsp;<asp:TextBox ID="txtEmail" runat="server" CssClass="auth-input" placeholder="Enter your email" AutoPostBack="True" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="error-text" Display="Dynamic" ErrorMessage="Email Required" ForeColor="Red" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" Display="Dynamic" CssClass="error-text" ErrorMessage="Invalid Email" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="RegGroup" ControlToValidate="txtEmail"></asp:RegularExpressionValidator>
                </div>

                <div class="input-group">
                    <label>Password</label>
                    &nbsp;<asp:TextBox ID="txtPassword" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" CssClass="error-text" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="Password Required" ForeColor="Red" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$" ErrorMessage="Password must be 8-20 characters, include a capital letter, a number, and a symbol (@$!%*?&)." ControlToValidate="txtPassword" Display="Dynamic" ForeColor="Red" ValidationGroup="RegGroup" CssClass="error-text"></asp:RegularExpressionValidator>
                </div>

                <div class="input-group">
                    <label>Confirm Password</label>
                    &nbsp;<asp:TextBox ID="txtConfirmPass" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvConfirmPass" runat="server" CssClass="error-text" ControlToValidate="txtConfirmPass" Display="Dynamic" ErrorMessage="Password Required" ForeColor="Red" ValidationGroup="RegGroup"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvConfirmPass" runat="server" CssClass="error-text" ErrorMessage="Password Missmatch" ControlToCompare="txtPassword" ControlToValidate="txtConfirmPass" Display="Dynamic" ForeColor="Red" ValidationGroup="RegGroup"></asp:CompareValidator>
                </div>

                <div class="input-group">
                    <label>OTP</label>
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <asp:TextBox ID="txtOTP" runat="server" CssClass="auth-input" placeholder="Enter OTP" Style="flex: 2;"></asp:TextBox>

                        <asp:Button ID="btnGetOTP" runat="server" Text="Get OTP" CssClass="auth-btn"
                            Style="flex: 1; margin-top: 0; padding: 10px;"
                            OnClick="btnGetOTP_Click" ValidationGroup="RegGroup" />
                    </div>
                    <div style="margin-top: 10px; text-align: right;">
                        <asp:LinkButton ID="btnResendOTP" runat="server" Visible="false"
                            OnClick="btnGetOTP_Click" Style="color: #FAEAB1; font-size: 11px; text-decoration: underline;">
                            Didn't get it? Resend OTP
                        </asp:LinkButton>
                    </div>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="REGISTER" CssClass="auth-btn" OnClick="btnRegister_Click" ValidationGroup="RegGroup" />

                <div class="auth-footer">
                    <p>Already a member? <a href="\Web_Files\Master_Pages\Pages\login.aspx">Login here</a></p>
                    <asp:Label ID="lblError" runat="server" ForeColor="#FF3333" Font-Size="12px"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
