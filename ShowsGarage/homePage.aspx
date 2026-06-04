<%@ Page Title="" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="homePage.aspx.cs" Inherits="ShowsGarage.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Home | Show's Garage</title>
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/homePage.css?v=1" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="heroSection" runat="server" class="my-hero-wrapper">
        <div class="my-hero-overlay">
        </div>

        <div class="my-hero-content-box">
            <h2 class="my-hero-title">
                <asp:Literal ID="litHeroTitle" runat="server"></asp:Literal>
            </h2>
            <p class="my-hero-description">
                <asp:Literal ID="litHeroContent" runat="server"></asp:Literal>
            </p>
            <asp:Button ID="btnHeroImg" runat="server" CssClass="my-hero-button" PostBackUrl="~/Web_Files/Client/Pages/productDetails.aspx" Text="Explore Collection" />
        </div>
    </div>

    <%-- This is just the copy paste code --%>
    <div class="my-gallery-container">
        <div class="my-gallery-header">
            <h3 class="my-gallery-title">Our Featured Collection</h3>
            <asp:Button ID="btnToggleGrid" runat="server" Text="View 3 Columns" OnClick="btnToggleGrid_Click" CssClass="my-gallery-toggle-btn" />
        </div>

        <asp:Panel ID="pnlGalleryGrid" runat="server" CssClass="my-gallery-grid gallery-grid-2">
            <asp:Repeater ID="rptGallery" runat="server">
                <itemtemplate>
                    <a href='<%# "Web_Files/Client/Pages/productDetails.aspx?id=" + Eval("ProductID") %>' class="my-gallery-card-link">
                        <div class="my-gallery-card">
                            <div class="my-gallery-image-wrapper">
                                <img src='<%# ResolveUrl(Eval("ImagePath").ToString()) %>' alt='<%# Eval("Title") %>' class="my-gallery-image" />
                            </div>
                            <div class="my-gallery-card-body">
                                <h4><%# Eval("BrandName") %></h4>
                                <h5><%# Eval("Title") %></h5>
                                <p>Rs.<%# Eval("SellingPrice") %></p>
                                <asp:Button ID="btnAddToCart" runat="server" Text="View" CommandArgument='<%# Eval("ProductID") %>' OnClick="btnAddToCart_Click" OnClientClick="event.stopPropagation();" CssClass="my-gallery-cart-btn" />
                            </div>
                        </div>
                    </a>
                </itemtemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>
    <div class="my-blog-container">
        <div class="my-blog-header">
            <h3 class="my-blog-title">Latest From Our Garage Blog</h3>
        </div>

        <div class="my-blog-grid">
            <asp:Repeater ID="rptLatestBlogs" runat="server">
                <itemtemplate>
                    <article class="my-blog-card">
                        <div class="my-blog-image-wrapper">
                            <img src='<%# ResolveUrl(Eval("BlogImage").ToString()) %>' alt='<%# Eval("BlogTitle") %>' class="my-blog-image" />
                        </div>
                        <div class="my-blog-card-body">
                            <span class="my-blog-category">Featured Post</span>
                            <h4><%# Eval("BlogTitle") %></h4>
                            <h5><%# Eval("Excerpt") %></h5>
                            <p><%# Eval("BlogContent") %></p>
                        </div>
                    </article>
                </itemtemplate>
            </asp:Repeater>
        </div>
        </div>
</asp:Content>