<%@ Page Title="My Orders | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="my-orders.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.my_orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>My Orders | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/myOrders.css?v=1" rel="stylesheet" type="text/css" />
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
                        <div class="order-master-card">
                            
                            <div class="order-card-header">
                                <div class="header-meta-group">
                                    <span class="meta-label">Order Placed</span>
                                    <span class="meta-value"><%# Eval("OrderDate", "{0:dd MMM yyyy}") %></span>
                                </div>
                                <div class="header-meta-group">
                                    <span class="meta-label">Total Amount</span>
                                    <span class="meta-value text-orange">Rs.<%# string.Format("{0:N0}", Eval("TotalAmount")) %></span>
                                </div>
                                <div class="header-meta-group id-group">
                                    <span class="meta-label">Order ID</span>
                                    <span class="meta-value id-code">#<%# Eval("OrderID") %></span>
                                </div>
                                <div class="header-status-badge">
                                    <asp:Label ID="lblStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </div>
                            </div>

                            <div class="order-card-body">
                                <div class="shipping-details-summary">
                                    <strong>Shipping To:</strong> <%# Eval("ShippingAddress") %>
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
                                                    <span class="item-unit-price">Rs.<%# string.Format("{0:N0}", Eval("UnitPrice")) %></span>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>