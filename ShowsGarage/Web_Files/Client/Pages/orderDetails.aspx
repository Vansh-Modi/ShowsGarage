<%@ Page Title="Order Tracking | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="order-details.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.order_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Order Tracking | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/myOrders.css?v=1" rel="stylesheet" type="text/css" />
    <style>
        .tracking-details-card { background: #070707; border: 1px solid #1a1a1a; border-radius: 8px; padding: 25px; margin-top: 20px; font-family: Arial, sans-serif; color: #fff; }
        .tracking-header-block { border-bottom: 1px solid #222; padding-bottom: 15px; margin-bottom: 20px; display: flex; justify-content: space-between; align-items: center; }
        .tracking-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 25px; }
        .tracking-data-group { background: #0f0f0f; padding: 15px; border-radius: 6px; border: 1px solid #222; }
        .tracking-data-group label { display: block; font-size: 11px; text-transform: uppercase; color: #666; margin-bottom: 5px; font-weight: bold; letter-spacing: 0.5px; }
        .tracking-data-group span { font-size: 15px; font-weight: 600; color: #fff; }
        .tracking-data-group span.highlight-blue { color: #0076df; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container" style="max-width: 800px; margin: 0 auto; padding: 20px;">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Shipment Tracking Hub</h1>
                <a href="myOrders.aspx" class="btn btn-filter-pill">← Back to History</a>
            </div>

            <asp:Label ID="lblErrorMessage" runat="server" CssClass="status-msg-error" Visible="false" Style="display:block; margin: 15px 0;"></asp:Label>

            <asp:Panel ID="pnlTrackingInfo" runat="server" class="tracking-details-card">
                <div class="tracking-header-block">
                    <div>
                        <h2 style="margin:0; font-size: 18px; color:#fff;">Order Tracking Data Matrix</h2>
                        <span style="color:#666; font-size:12px;">System Reference Mapping Sequence</span>
                    </div>
                    <span style="background: #111; border:1px solid #333; padding: 6px 12px; border-radius: 20px; font-size: 12px; font-weight: bold; color: #25D366;">
                        Status: <asp:Label ID="lblStatus" runat="server" />
                    </span>
                </div>

                <div class="tracking-grid">
                    <div class="tracking-data-group">
                        <label>Order Reference ID</label>
                        <span class="highlight-blue">#<asp:Label ID="lblOrderID" runat="server" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Logistics Courier Partner</label>
                        <span><asp:Label ID="lblCourierName" runat="server" Text="Awaiting Dispatch Allocation" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Docket Tracking Waybill Number</label>
                        <span><asp:Label ID="lblTrackingNo" runat="server" Text="Pending Dispatch Assignment" /></span>
                    </div>
                    <div class="tracking-data-group">
                        <label>Estimated Delivery Arrival</label>
                        <span><asp:Label ID="lblEstDelivery" runat="server" Text="Awaiting Sourcing Clearance" /></span>
                    </div>
                </div>

                <div class="shipping-details-summary" style="background:#0f0f0f; padding:15px; border-radius:6px; border:1px solid #222;">
                    <label style="display:block; font-size: 11px; text-transform: uppercase; color: #666; margin-bottom: 5px; font-weight: bold;">Delivery Dispatch Address Link</label>
                    <p style="margin:0; font-size:14px; color:#ccc; line-height:1.5;"><asp:Label ID="lblDeliveryAddress" runat="server" /></p>
                </div>
            </asp:Panel>

        </main>
    </div>
</asp:Content>