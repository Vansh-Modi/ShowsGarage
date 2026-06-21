<%@ Page Title="Secure Checkout | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="checkout.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Secure Checkout | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/checkout.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <div class="shop-theme-wrapper">
        <main class="shop-content-container">

            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Secure Checkout</h1>
                <a href="cart.aspx" class="btn btn-filter-pill">← Return to Cart</a>
            </div>

            <asp:Label ID="lblStatusMessage" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <div class="cart-layout-container">
                <asp:UpdatePanel ID="updMainCheckoutLayout" runat="server" UpdateMode="Conditional" RenderMode="Block" style="display: contents;">
                    <ContentTemplate>

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
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" placeholder="Surat / Mumbai / Bangalore" AutoPostBack="True" OnTextChanged="txtCity_TextChanged"></asp:TextBox>
                                    </div>

                                    <div class="form-group half-width">
                                        <label class="form-label">Payment Mode</label>
                                        <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-input" Style="background-color: #1a1a1a; color: #0076df; font-weight: 600; border-color: #222;" AutoPostBack="True" OnSelectedIndexChanged="ddlPaymentMode_SelectedIndexChanged">
                                            <asp:ListItem Value="ONLINE" Selected="True">Online Transfer (UPI / QR / Bank)</asp:ListItem>
                                            <asp:ListItem Value="COD">Cash-on-Delivery</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblPaymentWarning" runat="server" ForeColor="#ffcc00" Font-Size="10px" Style="display:block; margin-top:4px;" Visible="false" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="cart-summary-column">
                            <div class="summary-card">
                                <h2 class="summary-card-title">Review Your Order</h2>

                                <div class="checkout-items-review-list">
                                    <asp:Repeater ID="Repeater1" runat="server">
                                        <ItemTemplate>
                                            <div class="checkout-mini-item" style="border-bottom: 1px solid #222; padding-bottom: 10px; margin-bottom: 10px;">
                                                <div class="mini-item-info">
                                                    <span class="mini-item-title" style="display: block; font-weight: 600;"><%# Eval("Title") %></span>
                                                    <span class="mini-item-meta" style="font-size: 11px; color: #888; display: block; margin-top: 2px;">Qty: <%# Eval("Quantity") %></span>
                                                </div>
                                                <span class="mini-item-subtotal" style="font-weight: 600; color: #ffffff;">Rs.<%# string.Format("{0:N0}", Convert.ToDecimal(Eval("SellingPrice")) * Convert.ToInt32(Eval("Quantity"))) %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <div class="summary-data-row pt-15-border">
                                    <span>Total Unique Items:</span>
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="0"></asp:Label>
                                </div>

                                <div class="summary-data-row" style="color: #aaa;">
                                    <span>Subtotal (Items Total):</span>
                                    <span>Rs.<asp:Label ID="Label2" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="summary-data-row">
                                    <span>Delivery Partner Fee:</span>
                                    <span class="shipping-cost-text" style="color: #ffffff;">Rs.<asp:Label ID="Label3" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="summary-data-row total-highlight-row">
                                    <span>Grand Total / To Pay:</span>
                                    <span class="total-green-price">Rs.<asp:Label ID="Label4" runat="server" Text="0"></asp:Label></span>
                                </div>

                                <div class="mandate-box" style="margin: 15px 0; font-size: 11px; color: #cccccc; display: flex; align-items: flex-start; background: #111; padding: 10px; border-radius: 4px;">
                                    <input type="checkbox" id="chkAgree" required style="margin-top: 3px; margin-right: 8px;" />
                                    <label for="chkAgree" style="line-height: 1.4;">
                                        I authorize Show's Garage to act as my sourcing broker. I explicitly agree to pay the listed retail MRP alongside the specified brokerage commission for procurement.
                                    </label>
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

                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </main>
    </div>
</asp:Content>