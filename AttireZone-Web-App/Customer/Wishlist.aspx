<%@ Page Title="My Wishlist | ATTIREZONE" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Wishlist.aspx.cs" Inherits="AttireZone_Web_App.Customer.Wishlist" %>

<asp:Content ID="WishlistStyles" ContentPlaceHolderID="StylesPlaceholder" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@400;500;600&amp;display=swap" rel="stylesheet" />
    <style type="text/css">
        .container.body-content {
            max-width: 100% !important;
            width: 100% !important;
            margin: 0 !important;
            padding: 0 !important;
        }

        .serif-headline {
            font-family: 'Cormorant Garamond', serif;
        }

        .az-wishlist-page .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 200, 'GRAD' 0, 'opsz' 24;
        }
    </style>
</asp:Content>

<asp:Content ID="WishlistMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="az-wishlist-page">
        <main class="pt-32 pb-24 px-6 md:px-12 max-w-[1440px] mx-auto min-h-screen">
            <header class="mb-20 text-center md:text-left">
                <h1 class="serif-headline text-5xl md:text-7xl text-on-surface mb-4 font-medium tracking-tight">My Wishlist</h1>
                <p class="text-on-surface-variant text-lg md:text-xl font-light tracking-wide">Save your favorite items for later</p>
            </header>

            <asp:PlaceHolder ID="phEmptyWishlist" runat="server" Visible="false">
                <div class="flex flex-col items-center justify-center py-40 text-center">
                    <span class="material-symbols-outlined text-8xl text-surface-container-highest mb-8">favorite</span>
                    <h2 class="serif-headline text-4xl text-on-surface mb-4">Your wishlist is quiet</h2>
                    <p class="text-on-surface-variant max-w-md mb-12">Discover our curated collections and save the pieces that speak to your style.</p>
                    <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="px-10 py-4 bg-secondary text-on-secondary font-bold text-sm tracking-[0.2em] uppercase transition-all hover:opacity-90">Continue Shopping</a>
                </div>
            </asp:PlaceHolder>

            <asp:Repeater ID="rptWishlistItems" runat="server" OnItemCommand="rptWishlistItems_ItemCommand">
                <HeaderTemplate>
                    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-16">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="group relative flex flex-col">
                        <div class="relative aspect-[3/4] overflow-hidden bg-surface-container-low mb-6 transition-all duration-500 hover:shadow-[0_20px_50px_rgba(0,0,0,0.3)]">
                            <a href="<%#: Eval("ProductDetailsUrl") %>">
                                <img alt="<%#: Eval("ImageAlt") %>" class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="<%#: Eval("ImageUrl") %>" />
                            </a>
                            <asp:LinkButton
                                ID="btnRemoveWishlistItem"
                                runat="server"
                                CommandName="RemoveWishlistItem"
                                CommandArgument='<%# Eval("WishlistId") %>'
                                CausesValidation="false"
                                UseSubmitBehavior="false"
                                CssClass="absolute top-4 right-4 z-10 w-10 h-10 flex items-center justify-center bg-surface-container/40 backdrop-blur-md rounded-full text-secondary hover:bg-surface-container transition-colors"
                                aria-label="Remove from wishlist"
                            >
                                <span class="material-symbols-outlined text-xl" style="font-variation-settings: 'FILL' 1;">favorite</span>
                            </asp:LinkButton>
                        </div>
                        <div class="flex flex-col flex-grow">
                            <h3 class="text-on-surface text-xl font-semibold mb-2 tracking-tight"><%#: Eval("ProductName") %></h3>
                            <p class="text-secondary text-lg font-medium mb-6"><%#: Eval("PriceLabel") %></p>
                            <asp:LinkButton
                                ID="btnAddToCartFromWishlist"
                                runat="server"
                                CommandName="AddToCartFromWishlist"
                                CommandArgument='<%# Eval("ProductId") %>'
                                CausesValidation="false"
                                UseSubmitBehavior="false"
                                CssClass="mt-auto w-full py-4 bg-secondary text-on-secondary font-bold text-sm tracking-[0.15em] uppercase hover:opacity-90 active:scale-[0.98] transition-all duration-300 text-center"
                            >
                                ADD TO CART
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </main>
    </div>
</asp:Content>
