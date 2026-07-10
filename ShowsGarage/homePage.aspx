<%@ Page Title="Home | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="homePage.aspx.cs" Inherits="ShowsGarage.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="/Web_Files/Master_Pages/Styles/homePage.css?v=3" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Hero Showcase Block Framework Container -->
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

    <!-- Product Inventory Gallery Matrix Showcase Block -->
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

    <!-- Sourcing Trust Badges & Fulfillment Metrics Row Block -->
    <div class="my-gallery-container">
        <div class="shipping-badges-row">
            <div class="badge-box-item">
                <svg viewBox="0 0 24 24"><path d="M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66 1.34 3 3 3s3-1.34 3-3h6c0 1.66 1.34 3 3 3s3-1.34 3-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm12 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm1.5-6H17V9h2.5l2 3h-2z"/></svg>
                <h4>Pan India Shipping</h4>
                <p>Secure premium logistics tracking networks deployed straight to your collection garage room.</p>
            </div>
            <div class="badge-box-item">
                <svg viewBox="0 0 24 24"><path d="M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm-2 16l-4-4 1.41-1.41L10 14.17l6.59-6.59L18 9l-8 8z"/></svg>
                <h4>Precision Quality Inspect</h4>
                <p>Every single model sample receives complete inspection before being dispatched from logistics lines.</p>
            </div>
            <div class="badge-box-item">
                <svg viewBox="0 0 24 24"><path d="M18 7l-1.41-1.41-6.59 6.59-2.59-2.59L6 11l4 4zM12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8z"/></svg>
                <h4>100% Authentic Sourcing</h4>
                <p>Direct supply loops straight from verified licensed diecast manufacturers and original hobbyist providers.</p>
            </div>
        </div>
    </div>

    <!-- Interactive FAQ Accordion Module Block Box -->
    <div class="my-faq-container">
        <div class="my-faq-header">
            <h3 class="my-faq-title">Frequently Asked Questions</h3>
        </div>
        <div class="faq-accordion-stack">
            <div class="faq-accordion-item">
                <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                    <div class="faq-title-text">
                        <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 16h-2v-2h1v2zm1.07-7.75l-.9.92C12.45 11.9 12 12.5 12 14h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H7c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.04-.42 1.99-1.07 2.25z"/></svg>
                        What are the standard delivery transit times?
                    </div>
                    <span class="faq-arrow-icon">▼</span>
                </div>
                <div class="faq-content-panel">
                    <div class="faq-inner-text">
                        Orders are packaged in double-walled secure boxes and processed within 24–48 working hours. Standard shipping takes 3–6 business days across India depending on delivery pincode proximity metrics.
                    </div>
                </div>
            </div>
            <div class="faq-accordion-item">
                <div class="faq-trigger-header" onclick="toggleFaqAccordion(this)">
                    <div class="faq-title-text">
                        <svg viewBox="0 0 24 24"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 16h-2v-2h1v2zm1.07-7.75l-.9.92C12.45 11.9 12 12.5 12 14h-2v-.5c0-1.1.45-2.1 1.17-2.83l1.24-1.26c.37-.36.59-.86.59-1.41 0-1.1-.9-2-2-2s-2 .9-2 2H7c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.04-.42 1.99-1.07 2.25z"/></svg>
                        How do I inspect and trace the status of my order?
                    </div>
                    <span class="faq-arrow-icon">▼</span>
                </div>
                <div class="faq-content-panel">
                    <div class="faq-inner-text">
                        Once dispatch validation routines finalize cleanly, an explicit tracking UTR identifier number tracking token will update inside your customer account panel interface instantly.
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Blog Presentation Content Grid -->
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