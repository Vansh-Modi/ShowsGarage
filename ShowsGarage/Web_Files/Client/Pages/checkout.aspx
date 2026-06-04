<%@ Page Title="Secure Checkout | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="checkout.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Secure Checkout | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/checkout.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container">

            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Secure Checkout</h1>
                <a href="cart.aspx" class="btn btn-filter-pill">← Return to Cart</a>
            </div>

            <!-- Validation/Status Messages Block -->
            <asp:Label ID="lblStatusMessage" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <!-- Main Checkout Structural Container -->
            <div class="cart-layout-container">

                <!-- Left Panel: Billing & Shipping Forms -->
                <div class="cart-items-column">
                    <div class="checkout-form-card">
                        <h2 class="form-section-title">Shipping & Billing Information</h2>

                        <div class="form-grid">
                            <div class="form-group full-width">
                                <label class="form-label">Full Name</label>
                                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-input" placeholder="Enter Name"></asp:TextBox>
                            </div>

                            <div class="form-group phone-width">
                                <label class="form-label">Phone Number</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input" placeholder="+91 xxxxx xxxxx" MaxLength="10"></asp:TextBox>
                            </div>

                            <div class="form-group full-width">
                                <label class="form-label">Shipping Address</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-input text-area" TextMode="MultiLine" Rows="3" placeholder="House/Apartment number, Street Name, Area..."></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" placeholder="Surat / Mumbai / Bangalore"></asp:TextBox>
                            </div>

                            <div class="form-group half-width">
                                <label class="form-label">Payment Mode</label>
                                <asp:TextBox ID="txtPaymentDisplay" runat="server" CssClass="form-input" Text="Online Transfer (UPI / QR / Bank)" Enabled="false" Style="background-color: #1a1a1a; color: #0076df; font-weight: 600; border-color: #222;"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Right Panel: Order Review & Sticky Summary Block -->
                <div class="cart-summary-column">
                    <div class="summary-card">
                        <h2 class="summary-card-title">Review Your Order</h2>

                        <!-- Mini Order Review List -->
                        <div class="checkout-items-review-list">
                            <asp:Repeater ID="rptCheckoutItems" runat="server">
                                <ItemTemplate>
                                    <div class="checkout-mini-item">
                                        <div class="mini-item-info">
                                            <span class="mini-item-title"><%# Eval("Title") %></span>
                                            <span class="mini-item-meta">Qty: <%# Eval("Quantity") %> × Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></span>
                                        </div>
                                        <span class="mini-item-subtotal">Rs.<%# string.Format("{0:N0}", Convert.ToDecimal(Eval("SellingPrice")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <!-- Mathematical Summary Aggregations -->
                        <div class="summary-data-row pt-15-border">
                            <span>Total Unique Items:</span>
                            <asp:Label ID="lblCheckoutItemsCount" runat="server" Font-Bold="true" Text="0"></asp:Label>
                        </div>
                        <div class="summary-data-row">
                            <span>Shipping & Handling:</span>
                            <span class="shipping-cost-text" style="color: #ffffff;">Rs.<asp:Label ID="lblShippingFee" runat="server" Text="0"></asp:Label></span>
                        </div>

                        <div class="summary-data-row total-highlight-row">
                            <span>Grand Total:</span>
                            <span class="total-green-price">Rs.<asp:Label ID="lblCheckoutGrandTotal" runat="server" Text="0"></asp:Label></span>
                        </div>
                        <div style="background-color: rgba(0, 118, 223, 0.05); border-left: 3px solid #0076df; padding: 12px; margin-bottom: 15px; border-radius: 4px; text-align: left;">
                            <span style="color: #0076df; font-size: 11px; font-weight: 700; text-transform: uppercase; display: block; letter-spacing: 0.5px; margin-bottom: 3px;">Important Notice</span>
                            <p style="color: #cccccc; font-size: 12px; margin: 0; line-height: 1.4;">
                                Payment is verified manually by our garage team. Your order will sit as <span style="color: #ffffff; font-weight: 600;">"Awaiting Verification"</span> until the transaction hits our statement ledger.
                            </p>
                        </div>

                        <div class="summary-actions-block">
                            <asp:Button ID="btnPageNavigation" runat="server" Text="Continue to Payment →" CssClass="btn btn-checkout-large" OnClick="btnPageNavigation_Click" />
                        </div>
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>
