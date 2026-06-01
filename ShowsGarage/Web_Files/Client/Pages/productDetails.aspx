<%@ Page Title="Product Details | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="productDetails.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.productDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Product Details | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/shop.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container" style="max-width: 960px;">
            
            <div class="details-back-container">
                <a href="shop.aspx" class="btn btn-layout-toggle">← Back to Collection</a>
            </div>

            <div class="product-details-layout-container">
                
                <div class="details-image-panel">
                    <asp:Image ID="imgProduct" runat="server" CssClass="details-main-img" />
                </div>

                <div class="details-content-column">
                    <span class="details-brand-label">
                        <asp:Label ID="lblManufacturer" runat="server"></asp:Label>
                    </span>
                    <h1 class="details-product-title">
                        <asp:Label ID="lblProductName" runat="server"></asp:Label>
                    </h1>
                    
                    <div class="details-price-display">
                        Rs.<asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </div>

                    <!-- Cleaned context block wrapper style -->
                    <div class="details-description-text">
                        <asp:Label ID="lblDescription" runat="server"></asp:Label>
                    </div>

                    <div class="details-action-row">
                        <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn btn-add-to-cart-large" OnClick="btnAddToCart_Click" />
                    </div>
                </div>

            </div>
        </main>
    </div>
</asp:Content>