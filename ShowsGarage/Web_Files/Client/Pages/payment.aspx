<%@ Page Title="Submit Payment | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="payment-upload.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.payment_upload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Payment | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/checkout.css?v=9" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
        /* ==========================================================================
           🏁 SHOW'S GARAGE - MIDNIGHT DARK MODE RESET FRAMEWORK
           ========================================================================== */
        
        /* Constrain the main container over your Master Page teal background */
        .payment-upload-layout {
            max-width: 600px !important;
            margin: 0 auto !important;
            width: 100% !important;
            box-sizing: border-box !important;
            padding: 10px !important;
        }

        /* Restored to your original premium midnight dashboard background */
        .checkout-form-card {
            background-color: #141414 !important;
            border: 1px solid #222222 !important;
            border-radius: 8px !important;
            padding: 25px !important;
            box-shadow: 0 4px 20px rgba(0,0,0,0.4) !important;
            color: #ffffff !important; /* Flipped text to pure white */
            box-sizing: border-box !important;
            width: 100% !important;
            overflow: hidden !important;
        }

        /* Strict Image Correction & Containment */
        .checkout-form-card img, 
        .payment-upload-layout .payment-qr-img {
            display: block !important;
            max-width: 240px !important;
            width: 100% !important;
            height: auto !important;
            margin: 20px auto !important;
            border: 4px solid #222 !important;
            background-color: #ffffff !important;
            padding: 8px !important;
            border-radius: 6px !important;
            box-sizing: border-box !important;
        }

        /* Force fields to flatten vertically so they NEVER clip past the borders */
        .form-grid {
            display: flex !important;
            flex-direction: column !important;
            gap: 16px !important;
            width: 100% !important;
            box-sizing: border-box !important;
        }

        .form-group {
            display: flex !important;
            flex-direction: column !important;
            gap: 6px !important;
            width: 100% !important;
            box-sizing: border-box !important;
        }

        .form-label {
            color: #888888 !important; /* Metallic silver-gray text label */
            font-size: 11px !important;
            font-weight: 700 !important;
            text-transform: uppercase !important;
            letter-spacing: 0.5px !important;
        }

        /* Upgraded Input Controls - Midnight Theme */
        .form-input {
            background-color: #1a1a1a !important; /* Deep dark dashboard finish */
            border: 1px solid #2d2d2d !important;
            border-radius: 6px !important;
            color: #ffffff !important;
            padding: 12px 14px !important;
            font-size: 14px !important;
            width: 100% !important;
            max-width: 100% !important;
            box-sizing: border-box !important;
            outline: none !important;
            transition: all 0.2s ease !important;
            box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.4) !important;
        }

        .form-input:focus {
            border-color: #0076df !important; /* High performance focus accent link glow */
            background-color: #1f1f1f !important;
            box-shadow: 0 0 0 3px rgba(0, 118, 223, 0.2) !important;
        }

        /* Notice Verification Info Banner */
        .checkout-info-banner {
            background-color: rgba(255, 204, 0, 0.05) !important;
            border-left: 4px solid #ffcc00 !important; /* Safety yellow accent marker */
            padding: 15px !important;
            margin: 20px 0 !important;
            border-radius: 4px !important;
        }

        .info-banner-title {
            color: #ffcc00 !important;
            font-weight: 700 !important;
            display: block;
            margin-bottom: 5px;
            font-size: 12px;
            text-transform: uppercase;
        }

        .info-banner-text {
            color: #cccccc !important;
            font-size: 13px !important;
            margin: 0 !important;
            line-height: 1.5 !important;
        }

        /* Billing Price Highlight Panel Box */
        .price-summary-container {
            background: #161616 !important;
            padding: 15px !important;
            border-radius: 6px !important;
            border: 1px dashed #333333 !important;
            margin-bottom: 20px !important;
            text-align: center !important;
        }

        .price-summary-value {
            font-size: 24px !important;
            font-weight: 800 !important;
            color: #00ff66 !important; /* Classic bright green display numbers */
        }

        .bank-details-box {
            background-color: #111111 !important;
            padding: 16px !important;
            border-radius: 6px !important;
            border: 1px solid #222222 !important;
            margin-bottom: 20px !important;
        }

        .bank-row-value.values-highlight {
            color: #0076df !important;
            font-weight: 700;
        }

        .bank-monospace-text {
            color: #cccccc !important;
            font-family: monospace !important;
            white-space: pre-line !important;
            word-break: break-all !important;
            margin: 6px 0 0 0 !important;
        }

        /* Upgraded Premium Dashboard Style Button */
        .btn-checkout-large {
            background: linear-gradient(135deg, #1c1c1c 0%, #111111 100%) !important;
            color: #ffffff !important;
            border: 1px solid #333333 !important;
            border-radius: 6px !important;
            padding: 15px 20px !important;
            font-size: 14px !important;
            font-weight: 700 !important;
            width: 100% !important;
            max-width: 100% !important;
            cursor: pointer !important;
            text-transform: uppercase !important;
            letter-spacing: 1px !important;
            box-sizing: border-box !important;
            transition: all 0.2s ease !important;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3) !important;
        }

        .btn-checkout-large:hover {
            background: linear-gradient(135deg, #262626 0%, #1a1a1a 100%) !important;
            border-color: #0076df !important;
            box-shadow: 0 6px 16px rgba(0, 118, 223, 0.15) !important;
        }

        .btn-checkout-large:active {
            transform: scale(0.99) !important;
        }

        .alignment-center-wrapper { text-align: center !important; }
        .alignment-left-wrapper { text-align: left !important; }
        .text-center { text-align: center !important; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container payment-upload-layout">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Complete Your Order</h1>
            </div>

            <asp:Label ID="lblStatus" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <div class="checkout-form-card alignment-center-wrapper">
                
                <div class="price-summary-container">
                    <span class="price-summary-label" style="color: #aaa; font-size: 13px;">Total Payment Amount Due:</span>
                    <span class="price-summary-value">Rs. <asp:Literal ID="litPaymentDue" runat="server"></asp:Literal></span>
                </div>

                <asp:Panel ID="pnlOnlinePaymentDetails" runat="server">
                    <h2 class="form-section-title text-center" style="font-size: 16px; font-weight: 700; text-transform: uppercase; margin-bottom: 15px; color: #ffffff;">Scan QR or Transfer Direct</h2>
                    <p class="section-description-text" style="color: #aaaaaa; font-size: 13px; line-height: 1.4; margin-bottom: 20px;">
                        Please scan the merchant QR code or transfer the exact order amount to the bank account listed below.
                    </p>
                    
                    <asp:Image ID="imgQrCode" runat="server" CssClass="payment-qr-img" AlternateText="Merchant Payment QR Code" />
                    
                    <div class="bank-details-box alignment-left-wrapper">
                        <div class="bank-row-item">
                            <span class="bank-row-label">Merchant UPI ID</span>
                            <span class="bank-row-value values-highlight"><asp:Literal ID="litUpiId" runat="server"></asp:Literal></span>
                        </div>
                        <div style="border-top: 1px solid #222222; padding-top: 12px; margin-top: 12px;">
                            <span class="bank-row-label">Direct Bank Wire Credentials</span>
                            <p class="bank-monospace-text"><asp:Literal ID="litBankDetails" runat="server"></asp:Literal></p>
                        </div>
                    </div>

                    <div class="checkout-info-banner alignment-left-wrapper">
                        <span class="info-banner-title">⚠️ Secure Verification Process</span>
                        <p class="info-banner-text">
                            Please double-check that your typed Reference Number matches your bank statement receipt exactly. The owner will cross-verify this reference manually before dispatching your package. False submissions will result in immediate cancellation.
                        </p>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlCodConfirmation" runat="server" Visible="false">
                    <div class="cod-confirmation-box" style="text-align: center; padding: 20px 0;">
                        <div style="font-size: 40px; margin-bottom: 10px;">📦</div>
                        <h2 class="form-section-title" style="color: #ffffff; font-size: 16px;">Cash on Delivery Verification</h2>
                        <p style="color: #cccccc; font-size: 13px; line-height: 1.5;">
                            Thank you for opting for Cash on Delivery. To avoid fraudulent checkouts, your order will remain flagged as <span style="color: #0076df; font-weight: 600;">"Awaiting Verification"</span>.
                        </p>
                    </div>
                </asp:Panel>

                <div class="form-grid alignment-left-wrapper">
                    
                    <asp:Panel ID="pnlOnlineUploadForm" runat="server" Style="display: contents;">
                        <div class="form-group">
                            <label class="form-label">Transaction Reference Number / UTR Id</label>
                            <asp:TextBox ID="txtTxnReference" runat="server" CssClass="form-input" placeholder="Enter 12-Digit UTR or Bank Ref ID"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Upload Transfer Screenshot Receipt <span style="color: #e5ba6b; font-size: 11px;">(Max 2MB)</span></label>
                            <asp:FileUpload ID="fileScreenshot" runat="server" CssClass="form-input" accept=".jpg,.jpeg,.png" />
                        </div>
                    </asp:Panel>

                    <div class="summary-actions-block" style="width: 100%; margin-top: 10px;">
                        <asp:Button ID="btnSubmitProof" runat="server" Text="Submit Reference & Complete Order" CssClass="btn btn-checkout-large" OnClick="btnSubmitProof_Click" />
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>