<%@ Page Title="Home | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="homePage.aspx.cs" Inherits="ShowsGarage.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Home | Show's Garage</title>
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/homePage.css?v=2" />
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
            <asp:Button ID="btnHeroImg" runat="server" CssClass="my-hero-button" PostBackUrl="~/Web_Files/Client/Pages/shop.aspx" Text="Explore Collection" />
        </div>
    </div>

    <div class="my-gallery-container">
        <div class="my-gallery-header">
            <h3 class="my-gallery-title">Our Featured Collection</h3>
            <asp:Button ID="btnToggleGrid" runat="server" Text="View 3 Columns" OnClick="btnToggleGrid_Click" CssClass="my-gallery-toggle-btn" />
        </div>

        <asp:Panel ID="pnlGalleryGrid" runat="server" CssClass="my-gallery-grid gallery-grid-2">
            <asp:Repeater ID="rptGallery" runat="server" OnItemCommand="rptGallery_ItemCommand">
                <ItemTemplate>
                    <div class="my-gallery-card">
                        <div class="my-gallery-image-wrapper">
                            <img src='<%# ResolveUrl(string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? "~/Assets/images/default-model.png" : "~" + Eval("ImagePath").ToString().Replace("~","")) %>' alt='<%# Eval("Title") %>' class="my-gallery-image" />
                        </div>
                        <div class="my-gallery-card-body">
                            <h4><%# Eval("BrandName") %></h4>
                            <h5><%# Eval("Title") %></h5>
                            <p class="gallery-price-tag">Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></p>
                            
                            <asp:Button ID="btnAddToCart" runat="server" Text="View Model Details" CommandArgument='<%# Eval("ProductID") %>' OnClick="btnAddToCart_Click" CssClass="my-gallery-cart-btn" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>
<div class="my-blog-container">
        <div class="my-blog-header">
            <h3 class="my-blog-title">Latest From Our Garage Blog</h3>
        </div>

        <div class="my-blog-grid">
            <asp:Repeater ID="rptLatestBlogs" runat="server">
                <ItemTemplate>
                    <article class="my-blog-card">
                        <div class="my-blog-image-wrapper">
                            <img src='<%# ResolveUrl(string.IsNullOrEmpty(Eval("BlogImage").ToString()) ? "~/Assets/uploads/blogs/default-blog.png" : "~" + Eval("BlogImage").ToString().Replace("~","")) %>' alt='<%# Eval("BlogTitle") %>' class="my-blog-image" />
                        </div>
                        <div class="my-blog-card-body">
                            <span class="my-blog-category">Featured Post</span>
                            <h4><%# Eval("BlogTitle") %></h4>
                            <p class="blog-excerpt-text"><%# Eval("Excerpt") %></p>
                            <a href='/Web_Files/Client/Pages/blogDetails.aspx?id=<%# Eval("BlogId") %>' class="my-blog-readmore-btn">Read Full Article →</a>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script type="text/javascript">
        function toggleFaqAccordion(headerElement) {
            const item = headerElement.parentElement;
            const contentPanel = item.querySelector('.faq-content-panel');
            const isOpen = item.classList.contains('active');

            document.querySelectorAll('.faq-accordion-item').forEach(el => {
                el.classList.remove('active');
                el.querySelector('.faq-content-panel').style.maxHeight = null;
            });

            if (!isOpen) {
                item.classList.add('active');
                contentPanel.style.maxHeight = contentPanel.scrollHeight + "px";
            }
        }
    </script>
</asp:Content>