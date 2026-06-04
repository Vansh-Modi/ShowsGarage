<%@ Page Title="Submit Payment | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="payment-upload.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.payment_upload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title> Payment | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/checkout.css?v=2" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container" style="max-width: 650px; margin: 0 auto;">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Complete Your Payment</h1>
            </div>

            <!-- Error/Status Messages Label -->
            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <div class="checkout-form-card" style="text-align: center;">
                <h2 class="form-section-title">Scan QR or Transfer Direct</h2>
                <p style="color: #aaaaaa; font-size: 14px; margin-bottom: 25px; line-height: 1.4; text-align: center;">
                    Please scan the merchant QR code or transfer the exact order amount to the bank account listed below.
                </p>
                
                <!-- Dynamic QR Image Area -->
                <asp:Image ID="imgQrCode" runat="server" Style="max-width: 220px; width: 100%; border: 4px solid #222; background-color: #ffffff; padding: 6px; border-radius: 8px; margin-bottom: 20px;" AlternateText="Merchant Payment QR Code" />
                
                <!-- Credentials Information Blocks -->
                <div style="background-color: #111111; padding: 18px; border-radius: 6px; text-align: left; border: 1px solid #222222; margin-bottom: 30px;">
                    <div style="margin-bottom: 12px;">
                        <span style="color: #666666; font-size: 11px; text-transform: uppercase; display: block; font-weight: 600; letter-spacing: 0.5px;">Merchant UPI ID</span>
                        <span style="font-size: 16px; font-weight: 700; color: #0076df; word-break: break-all;"><asp:Literal ID="litUpiId" runat="server"></asp:Literal></span>
                    </div>
                    <div style="border-top: 1px solid #222222; padding-top: 12px;">
                        <span style="color: #666666; font-size: 11px; text-transform: uppercase; display: block; font-weight: 600; letter-spacing: 0.5px;">Direct Bank Wire Credentials</span>
                        <p style="margin: 6px 0 0 0; color: #cccccc; font-size: 14px; line-height: 1.5; white-space: pre-line; font-family: monospace;"><asp:Literal ID="litBankDetails" runat="server"></asp:Literal></p>
                    </div>
                </div>

                <!-- Upfront Manual Verification Warning Info Box -->
                <div class="checkout-info-banner">
                    <span class="info-banner-title">⚠️ Secure Verification Process</span>
                    <p class="info-banner-text">
                        Please double-check that your typed Reference Number matches your bank statement receipt exactly. The owner will cross-verify this reference manually before dispatching your package. False submissions will result in immediate cancellation.
                    </p>
                </div>

                <!-- Proof Submission Fieldsets -->
                <h2 class="form-section-title">Upload Transaction Proof</h2>
                <div class="form-grid" style="text-align: left;">
                    <div class="form-group full-width">
                        <label class="form-label">Transaction Reference Number / UTR Id</label>
                        <asp:TextBox ID="txtTxnReference" runat="server" CssClass="form-input" placeholder="Enter 12-Digit UTR or Bank Ref ID"></asp:TextBox>
                    </div>

                    <div class="form-group full-width">
                        <label class="form-label">Upload Transfer Screenshot Receipt (JPG / PNG)</label>
                        <asp:FileUpload ID="fileScreenshot" runat="server" CssClass="form-input" style="padding: 8px;" />
                    </div>

                    <div class="summary-actions-block full-width">
                        <asp:Button ID="btnSubmitProof" runat="server" Text="Submit Reference & Complete Order" CssClass="btn btn-checkout-large" OnClick="btnSubmitProof_Click" />
                    </div>
                </div>
            </div>
        </main>
    </div>
</asp:Content>