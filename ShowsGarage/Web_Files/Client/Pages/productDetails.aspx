<%@ Page Title="Product Details | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="productDetails.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.productDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Product Details | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/shop.css?v=2" rel="stylesheet" type="text/css" />
    <link href="/Web_Files/Client/Styles/product-details.css?v=1" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container details-max-width">
            
            <div class="details-back-container">
                <a href="shop.aspx" class="btn-layout-back">← Back to Collection</a>
            </div>

            <asp:Label ID="lblDetailStatus" runat="server" CssClass="admin-alert-banner alert-error" Visible="false" Style="margin-bottom:20px; display:block;"></asp:Label>

            <div class="product-details-layout-container">
                
                <div class="details-image-panel">
                    <div class="main-showcase-image-frame">
                        <asp:Image ID="imgProduct" runat="server" CssClass="details-main-img" />
                    </div>
                </div>

                <div class="details-content-column">
                    <span class="details-brand-label">
                        <asp:Label ID="lblManufacturer" runat="server"></asp:Label>
                    </span>
                    <h1 class="details-product-title">
                        <asp:Label ID="lblProductName" runat="server"></asp:Label>
                    </h1>
                    
                    <div class="details-inventory-status-row">
                        <span class="details-scale-badge">Scale Ratio: <asp:Label ID="lblScaleDisplay" runat="server"></asp:Label></span>
                        <asp:Label ID="lblStockBadge" runat="server" CssClass="details-stock-badge-indicator"></asp:Label>
                    </div>
                    
                    <div class="details-price-display">
                        Rs.<asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </div>

                    <div class="details-description-text">
                        <h4 class="description-section-heading">Model Specifications & Features</h4>
                        <p class="description-paragraph-content">
                            <asp:Label ID="lblDescription" runat="server"></asp:Label>
                        </p>
                    </div>

                    <div class="details-action-row">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn-add-to-cart-large" OnClick="btnAddToCart_Click" />
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>