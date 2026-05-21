<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Login | Show's Garage</title>
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/login.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auth-wrapper">
        <div class="auth-card">
            <div class="auth-header">
                <h2>Welcome Back</h2>
                <p>Login to your garage account</p>
            </div>

            <div class="auth-form">
                <div class="input-group">
                    <label>Email Address<asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="Enter Password" ForeColor="Red"></asp:RequiredFieldValidator>
                    </label>
                    &nbsp;<asp:TextBox ID="txtEmail" runat="server" CssClass="auth-input" placeholder="Enter your email" AutoPostBack="True" CausesValidation="True" TextMode="Email"></asp:TextBox>
                </div>

                <div class="input-group">
                    <label>Password<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Enter Password" ForeColor="Red"></asp:RequiredFieldValidator>
                    </label>
                    &nbsp;<asp:TextBox ID="txtPassword" runat="server" CssClass="auth-input" TextMode="Password" placeholder="••••••••" CausesValidation="True"></asp:TextBox>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="LOGIN" CssClass="auth-btn" OnClick="btnLogin_Click" />

                <div class="auth-footer">
                    <p>New to the garage? <a href="/Web_Files/Client/Pages/register.aspx">Create Account</a></p>
                    <asp:Label ID="lblError" runat="server" ForeColor="#FF3333" Font-Size="12px"></asp:Label>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
