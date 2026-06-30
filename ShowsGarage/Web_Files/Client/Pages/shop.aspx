<%@ Page Title="Shop | Show's Garage" Language="C#" MasterPageFile="~/Web_Files/Master_Pages/Pages/Site.Master" AutoEventWireup="true" CodeBehind="shop.aspx.cs" Inherits="ShowsGarage.Web_Files.Client.Pages.shop" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Shop | Show's Garage</title>
    <link href="/Web_Files/Client/Styles/shop.css?v=2" rel="stylesheet" type="text/css" />
    <style>
        /* Out of Stock Styling Modifiers */
        .product-gallery-card.sold-out {
            opacity: 0.75;
        }
        .btn-sold-out {
            background-color: #222222 !important;
            color: #777777 !important;
            border: 1px solid #333333 !important;
            cursor: not-allowed !important;
            pointer-events: none;
            font-weight: 700;
            text-transform: uppercase;
            font-size: 11px;
            letter-spacing: 0.5px;
            display: block;
            width: 100%;
            text-align: center;
            padding: 10px 0;
            border-radius: 4px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="shop-theme-wrapper">
        <main class="shop-content-container">
            
            <div class="shop-controls-bar">
                <h1 class="shop-main-heading">Our Featured Collection</h1>
            </div>

            <div class="final-scroll-box" style="width: 100% !important; max-width: 100% !important; display: block !important; overflow-x: auto !important; overflow-y: hidden !important; margin-bottom: 35px !important; padding-bottom: 15px !important; -webkit-overflow-scrolling: touch !important;">
                <div class="final-scroll-content" style="display: flex !important; flex-direction: row !important; flex-wrap: nowrap !important; justify-content: flex-start !important; align-items: center !important; gap: 12px !important; width: max-content !important;">
                    
                    <asp:LinkButton ID="lnkAll" runat="server" 
                        Style="flex-shrink: 0 !important; white-space: nowrap !important; display: inline-block !important; background-color: #141414 !important; border: 1px solid #222222 !important; color: #aaaaaa !important; font-size: 13px !important; font-weight: 600 !important; text-transform: uppercase !important; letter-spacing: 0.3px !important; padding: 8px 18px !important; border-radius: 20px !important; text-decoration: none !important; cursor: pointer !important;"
                        CssClass="btn-filter-pill-isolated active-pill-isolated" 
                        OnClick="CategoryFilter_Click" 
                        CommandArgument="0">All Collections</asp:LinkButton>
                    
                    <asp:Repeater ID="rptCategories" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkCat" runat="server" 
                                Style="flex-shrink: 0 !important; white-space: nowrap !important; display: inline-block !important; background-color: #141414 !important; border: 1px solid #222222 !important; color: #aaaaaa !important; font-size: 13px !important; font-weight: 600 !important; text-transform: uppercase !important; letter-spacing: 0.3px !important; padding: 8px 18px !important; border-radius: 20px !important; text-decoration: none !important; cursor: pointer !important;"
                                CssClass="btn-filter-pill-isolated" 
                                OnClick="CategoryFilter_Click" 
                                CommandArgument='<%# Eval("CategoryID") %>'><%# Eval("CategoryName") %></asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>

            <div class="products-gallery-grid">
                <asp:Repeater ID="rptProducts" runat="server">
                    <ItemTemplate>
                        <div class='<%# Convert.ToInt32(Eval("StockQuantity")) <= 0 ? "product-gallery-card sold-out" : "product-gallery-card" %>'>
                            <a href='productDetails.aspx?id=<%# Eval("ProductID") %>' class="card-clickable-area">
                                <div class="product-image-frame">
                                    <img src='<%# ResolveUrl(Eval("ImagePath") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath").ToString() : "/Assets/images/no-image.png") %>' alt='<%# Eval("Title") %>' class="product-gallery-img" />
                                </div>
                                <div class="product-details-block">
                                    <h3 class="product-title-text"><%# Eval("BrandName") %></h3>
                                    <p class="product-subtitle-text"><%# Eval("Title") %></p>
                                    <span class="product-price-tag">Rs.<%# string.Format("{0:N0}", Eval("SellingPrice")) %></span></div>
                            </a>
                            <div class="card-action-block">
                                <%# Convert.ToInt32(Eval("StockQuantity")) <= 0 ? 
                                    "<span class='btn-sold-out'>Out of Stock</span>" : 
                                    "<a href='productDetails.aspx?id=" + Eval("ProductID") + "' class='btn btn-add-to-cart'>View Details</a>" 
                                %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>
    </div>
</asp:Content>