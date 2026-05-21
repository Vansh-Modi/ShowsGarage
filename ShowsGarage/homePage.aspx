<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="homePage.aspx.cs" Inherits="ShowsGarage.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Home | Show's Garage</title>
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/homePage.css?v=1" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="heroSection" runat="server" class="my-hero-wrapper">
        <div class="my-hero-overlay"></div>
        
        <div class="my-hero-content-box">
            <h2 class="my-hero-title">
                <asp:Literal ID="litHeroTitle" runat="server"></asp:Literal>
            </h2>
            <p class="my-hero-description">
                <asp:Literal ID="litHeroContent" runat="server"></asp:Literal>
            </p>
            <asp:Button ID="btnHeroImg" runat="server" Text="Explore Collection" CssClass="my-hero-button" PostBackUrl="~/Web_Files/Client/Pages/productDetails.aspx" />
        </div>
    </div>

    <%-- This is just the copy paste code
        <div class="my-gallery-container">
    <div class="my-gallery-header">
        <h3 class="my-gallery-title">Our Featured Collection</h3>
        
        <asp:Button ID="btnToggleGrid" runat="server" Text="View 3 Columns" OnClick="btnToggleGrid_Click" CssClass="my-gallery-toggle-btn" />
    </div>

    <asp:Panel ID="pnlGalleryGrid" runat="server" CssClass="my-gallery-grid gallery-grid-2">
        <asp:Repeater ID="rptGallery" runat="server">
            <ItemTemplate>
                <div class="my-gallery-card">
                    <img src='<%# Eval("HeroImg") %>' alt='<%# Eval("HeroTitle") %>' class="my-gallery-image" />
                    <div class="my-gallery-card-body">
                        <h4><%# Eval("HeroTitle") %></h4>
                        <p><%# Eval("HeroSubtitle") %></p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</div>
        --%>

</asp:Content>