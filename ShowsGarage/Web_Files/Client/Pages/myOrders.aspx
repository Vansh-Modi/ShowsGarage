<%@ Page Title="My Orders | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="my-orders.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.my_orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>My Orders | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/myOrders.css?v=1" rel="stylesheet" type="text/css" />
    <style>
        /* Tracking Timeline Styling Elements */
        .order-tracking-timeline { display: flex; justify-content: space-between; margin: 20px 0 10px 0; padding: 15px; background: #0a0a0a; border: 1px solid #111; border-radius: 6px; position: relative; }
        .timeline-step { flex: 1; text-align: center; position: relative; font-size: 11px; color: #555; font-weight: 600; text-transform: uppercase; }
        .timeline-step .step-dot { width: 10px; height: 10px; border-radius: 50%; background: #222; margin: 0 auto 6px auto; display: block; z-index: 2; position: relative; border: 2px solid #0a0a0a; }
        .timeline-step.completed { color: #0076df; }
        .timeline-step.completed .step-dot { background: #0076df; box-shadow: 0 0 8px #0076df; }
        .timeline-step.active-step { color: #25D366; }
        .timeline-step.active-step .step-dot { background: #25D366; box-shadow: 0 0 8px #25D366; }
        
        /* Information Log Row Badges */
        .verification-audit-trail-log { display: flex; flex-wrap: wrap; gap: 15px; margin-top: 15px; padding-top: 15px; border-top: 1px solid #1a1a1a; font-size: 12px; color: #aaa; }
        .audit-badge { background: #141414; padding: 4px 10px; border-radius: 4px; border: 1px solid #222; }
        .audit-badge strong { color: #fff; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">My Purchase History</h1>
                <a href="shop.aspx" class="btn btn-filter-pill">← Back to Shop</a>
            </div>

            <asp:Label ID="lblStatusMessage" runat="server" CssClass="status-msg-error" Visible="false"></asp:Label>

            <asp:Panel ID="pnlNoOrders" runat="server" Visible="false" CssClass="empty-orders-card">
                <div class="empty-icon">📦</div>
                <h2>No Orders Found</h2>
                <p>You haven't placed any orders in our garage yet.</p>
                <a href="shop.aspx" class="btn-shop-now">Start Shopping</a>
            </asp:Panel>

            <div class="orders-list-wrapper">
                <asp:Repeater ID="rptOrders" runat="server" OnItemDataBound="rptOrders_ItemDataBound">
                    <ItemTemplate>
                        <a href='orderDetails.aspx?id=<%# Eval("OrderID") %>' class="order-card-click-wrapper" style="text-decoration: none; display: block; color: inherit; margin-bottom: 20px;">
                            <div class="order-master-card" style="transition: transform 0.2s, border-color 0.2s; cursor: pointer;" onmouseover="this.style.borderColor='#0076df';" onmouseout="this.style.borderColor='#1a1a1a';">
                                
                                <div class="order-card-header">
                                    <div class="header-meta-group">
                                        <span class="meta-label">Order Placed</span>
                                        <span class="meta-value"><%# Eval("OrderDate", "{0:dd MMM yyyy}") %></span>
                                    </div>
                                    <div class="header-meta-group">
                                        <span class="meta-label">Total Amount</span>
                                        <span class="meta-value text-orange">Rs.<%# string.Format("{0:N0}", Eval("TotalAmount")) %></span></div>
                                    <div class="header-meta-group id-group">
                                        <span class="meta-label">Order ID</span>
                                        <span class="meta-value id-code">#<%# Eval("OrderID") %></span></div>
                                    <div class="header-status-badge">
                                        <asp:Label ID="lblStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                    </div>
                                </div>

                                <div class="order-card-body">
                                    <div class="shipping-details-summary">
                                        <strong>Shipping Address Target:</strong> <%# Eval("ShippingAddress") %>
                                    </div>

                                    <div class="order-tracking-timeline">
                                        <div id="stepPlaced" runat="server" class="timeline-step">
                                            <span class="step-dot"></span>Placed
                                        </div>
                                        <div id="stepVerified" runat="server" class="timeline-step">
                                            <span class="step-dot"></span>Payment Verified
                                        </div>
                                        <div id="stepShipped" runat="server" class="timeline-step">
                                            <span class="step-dot"></span>Sourced & Shipped
                                        </div>
                                    </div>

                                    <div class="order-items-grid">
                                        <asp:Repeater ID="rptOrderItems" runat="server">
                                            <ItemTemplate>
                                                <div class="order-item-row">
                                                    <div class="item-main-details">
                                                        <span class="item-title-text"><%# Eval("Title") %></span>
                                                        <span class="item-meta-specs">Scale: <%# Eval("Scale") %> | Brand: <%# Eval("BrandName") %></span>
                                                    </div>
                                                    <div class="item-pricing-details">
                                                        <span class="item-qty-count">Qty: <%# Eval("Quantity") %></span>
                                                        <span class="item-unit-price">Rs.<%# string.Format("{0:N0}", Eval("UnitPrice")) %></span></div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>

                                    <div class="verification-audit-trail-log">
                                        <div class="audit-badge">
                                            Verification Status: <strong><%# Eval("Status") %></strong>
                                        </div>
                                        <div class="audit-badge">
                                            Bank Txn Ref: <strong><%# string.IsNullOrEmpty(Eval("TransactionReference").ToString()) ? "Pending Upload" : Eval("TransactionReference") %></strong>
                                        </div>
                                        <div class="audit-badge" style="color: #0076df; font-weight: bold; margin-left: auto;">
                                            View Tracking Details →
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>