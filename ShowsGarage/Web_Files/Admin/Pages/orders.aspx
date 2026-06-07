<%@ Page Title="Manage Orders | Admin" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="orders.aspx.cs" Inherits="ShowsGarage.Web_Files.Admin.admin_orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Manage Orders | Admin Dashboard</title>
    <link href="/Web_Files/Admin/Styles/siteSettings.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Admin/Styles/orders.css?v=3" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="admin-theme-wrapper">
        <main class="admin-content-container universal-wide-layout">
            
            <div class="admin-controls-bar">
                <h1 class="admin-main-heading">Incoming Order Pipeline</h1>
                <span class="admin-pill-badge">Live Store Operations</span>
            </div>

            <div class="admin-filter-strip">
                <div class="filter-controls-group">
                    <span class="filter-strip-label">Pipeline State Filter:</span>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" CssClass="admin-filter-dropdown select-arrow-fix">
                        <asp:ListItem Value="ALL">-- Display Entire Order Log History --</asp:ListItem>
                        <asp:ListItem Value="Awaiting Verification">Awaiting Verification Only</asp:ListItem>
                        <asp:ListItem Value="Approved">Approved / Processing</asp:ListItem>
                        <asp:ListItem Value="Cancelled">Cancelled Logs</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-strip-hint">
                    Serialized logs mapping based on state constraints.
                </div>
            </div>

            <asp:Label ID="lblAdminStatus" runat="server" CssClass="admin-alert-banner" Visible="false"></asp:Label>

            <div class="admin-orders-vertical-stack">
                <asp:Repeater ID="rptAdminOrders" runat="server" OnItemDataBound="rptAdminOrders_ItemDataBound" OnItemCommand="rptAdminOrders_ItemCommand">
                    <ItemTemplate>
                        <div class="admin-order-master-card">
                            
                            <div class="admin-order-card-header">
                                <div class="header-meta-group">
                                    <span class="meta-label">Date Logged</span>
                                    <span class="meta-value"><%# Eval("OrderDate", "{0:dd MMM yyyy HH:mm}") %></span>
                                </div>
                                <div class="header-meta-group">
                                    <span class="meta-label">Total Invoice Value</span>
                                    <span class="meta-value text-green">Rs.<%# string.Format("{0:N2}", Eval("TotalAmount")) %></span></div>
                                <div class="header-meta-group">
                                    <span class="meta-label">Customer ID</span>
                                    <span class="meta-value">User #<%# Eval("UserID") %></span></div>
                                <div class="header-meta-group id-right-align">
                                    <span class="meta-label">Order ID Token</span>
                                    <span class="meta-value id-code-highlight">#<%# Eval("OrderID") %></span></div>
                                <div class="header-status-badge-container">
                                    <asp:Label ID="lblStatusBadge" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </div>
                            </div>

                            <div class="admin-order-card-body">
                                <div class="admin-shipping-logistics-summary">
                                    <strong>Fulfillment Logistics Destination:</strong> <%# Eval("ShippingAddress") %>
                                    
                                    <div class="payment-meta-footer-row">
                                        <span>
                                            Payment Mode: <strong><%# Eval("PaymentMethod") %></strong> | 
                                            Ref / UTR: <strong class="monospace-white"><%# string.IsNullOrEmpty(Eval("TransactionReference").ToString()) ? "N/A" : Eval("TransactionReference") %></strong>
                                        </span>
                                        <asp:LinkButton ID="lnkTogglePaymentEdit" runat="server" CssClass="btn-toggle-edit" CommandName="TogglePaymentEdit" CommandArgument='<%# Container.ItemIndex %>'>Modify Payment Details ✎</asp:LinkButton>
                                    </div>

                                    <asp:Panel ID="pnlInlinePaymentEdit" runat="server" CssClass="payment-inline-edit-box" Visible="false">
                                        <div class="edit-fields-row">
                                            <div class="inline-input-group">
                                                <label>Transaction Reference ID / UTR</label>
                                                <asp:TextBox ID="txtEditTxnRef" runat="server" CssClass="inline-text-box" Text='<%# Eval("TransactionReference") %>'></asp:TextBox>
                                            </div>
                                            <div class="inline-input-group">
                                                <label>Active Override Status</label>
                                                <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="inline-text-box select-arrow-fix">
                                                    <asp:ListItem Value="Awaiting Verification">Awaiting Verification</asp:ListItem>
                                                    <asp:ListItem Value="Approved">Approved</asp:ListItem>
                                                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <asp:Button ID="btnSaveInlinePayment" runat="server" Text="Update" CssClass="btn-inline-save" CommandName="SaveInlinePayment" CommandArgument='<%# Eval("OrderID") %>' />
                                    </asp:Panel>
                                </div>

                                <div class="admin-purchased-items-list">
                                    <h4 class="items-list-section-title">Purchased Manifest Line Items</h4>
                                    <asp:Repeater ID="rptAdminOrderItems" runat="server">
                                        <ItemTemplate>
                                            <div class="admin-item-manifest-row">
                                                <div class="manifest-item-details">
                                                    <span class="manifest-item-title"><%# Eval("Title") %></span>
                                                    <span class="manifest-item-specs">Scale Ratio: <%# Eval("Scale") %> | Brand Label: <%# Eval("BrandName") %></span>
                                                </div>
                                                <div class="manifest-item-pricing">
                                                    <span class="manifest-item-qty">Quantity: <%# Eval("Quantity") %></span>
                                                    <span class="manifest-item-price">Rs.<%# string.Format("{0:N2}", Eval("UnitPrice")) %></span></div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <asp:PlaceHolder ID="phVerificationBlock" runat="server" Visible='<%# Eval("Status").ToString().Equals("Awaiting Verification", StringComparison.OrdinalIgnoreCase) %>'>
                                    <div class="admin-receipt-verification-panel">
                                        <div class="verification-details-side">
                                            <a href='<%# Eval("PaymentScreenshotPath") %>' target="_blank" title="Click to view file attachment in safe terminal screen">
                                                <img src='<%# Eval("PaymentScreenshotPath") %>' class="admin-receipt-thumbnail-preview" alt="Customer Deposit Receipt" />
                                            </a>
                                            <div class="reference-string-group">
                                                <span class="reference-string-label">Customer Declared Reference / UTR Number</span>
                                                <strong class="reference-string-value"><%# Eval("TransactionReference") %></strong>
                                                <p class="verification-instructions-hint">Cross-examine this value metric against your current live merchant UPI / Bank ledger entries before approving delivery clearance.</p>
                                            </div>
                                        </div>
                                        
                                        <div class="verification-actions-side">
                                            <asp:LinkButton ID="lnkApprove" runat="server" CssClass="admin-btn-action btn-action-approve" CommandName="ApprovePayment" CommandArgument='<%# Eval("OrderID") %>'>Approve Transaction</asp:LinkButton>
                                            <asp:LinkButton ID="lnkReject" runat="server" CssClass="admin-btn-action btn-action-reject" CommandName="RejectPayment" CommandArgument='<%# Eval("OrderID") %>' OnClientClick="return confirm('Rejecting this order will return items to stock. Remember to process the manual cash transfer back to the buyer\'s account.');">Reject / Cancel Order</asp:LinkButton>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>

                            </div>

                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>