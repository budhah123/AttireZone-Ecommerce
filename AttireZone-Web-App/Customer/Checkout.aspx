<%@ Page Title="Checkout | AttireZone" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="AttireZone_Web_App.Customer.Checkout" %>

<asp:Content ID="CheckoutStyles" ContentPlaceHolderID="StylesPlaceholder" runat="server">
    <style type="text/css">
        .container.body-content {
            max-width: 100% !important;
            width: 100% !important;
            margin: 0 !important;
            padding: 0 !important;
        }

        .az-checkout-page .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 300, 'GRAD' 0, 'opsz' 24;
        }

        .az-checkout-page ::-webkit-scrollbar {
            width: 4px;
        }

        .az-checkout-page ::-webkit-scrollbar-track {
            background: #0e0e0e;
        }

        .az-checkout-page ::-webkit-scrollbar-thumb {
            background: #af8d11;
        }

        .az-radio-list td {
            display: block;
            margin-bottom: 0.75rem;
        }

        .az-radio-list input[type="radio"] {
            margin-right: 0.5rem;
            accent-color: #e9c349;
        }

        .az-radio-list label {
            font-size: 0.75rem;
            letter-spacing: 0.12em;
            text-transform: uppercase;
            color: #e2e2e2;
        }
    </style>
</asp:Content>

<asp:Content ID="CheckoutMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="az-checkout-page">
        <main class="pt-32 pb-24 px-6 md:px-12 max-w-7xl mx-auto">
            <asp:PlaceHolder ID="phCheckoutEmpty" runat="server" Visible="false">
                <div class="max-w-3xl mx-auto bg-surface-container-low p-10 md:p-14 text-center space-y-6">
                    <h1 class="text-4xl md:text-5xl font-bold tracking-tight text-on-background">Checkout</h1>
                    <p class="text-on-surface-variant uppercase tracking-[0.18em] text-xs">Your bag is currently empty. Add pieces to continue.</p>
                    <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="inline-flex items-center gap-2 bg-secondary text-on-secondary py-4 px-8 font-bold uppercase tracking-[0.18em] text-xs hover:brightness-110 transition-all">
                        <span class="material-symbols-outlined !text-[18px]">west</span>
                        Continue Shopping
                    </a>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phCheckoutReady" runat="server">
                <div class="grid grid-cols-1 lg:grid-cols-5 gap-16 items-start">
                    <div class="lg:col-span-3 space-y-12">
                        <section>
                            <h1 class="text-4xl font-bold tracking-tight mb-12 text-on-background">Checkout</h1>
                            <div class="bg-surface-container-low border-none p-10 space-y-8">
                                <h2 class="text-xs font-bold tracking-[0.2em] uppercase text-secondary">Shipping Details</h2>
                                <div class="space-y-6">
                                    <div class="group relative">
                                        <label class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2" for="<%= txtFullName.ClientID %>">Full Name</label>
                                        <asp:TextBox ID="txtFullName" runat="server" MaxLength="100" CssClass="w-full bg-transparent border-none border-b border-outline-variant/30 py-3 text-on-background focus:ring-0 focus:border-secondary transition-all placeholder:text-surface-container-highest uppercase text-sm tracking-wider" placeholder="ALEXANDER VOGUE"></asp:TextBox>
                                        <div class="absolute bottom-0 left-0 w-0 h-[1px] bg-secondary transition-all duration-500 group-focus-within:w-full"></div>
                                    </div>
                                    <div class="group relative">
                                        <label class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2" for="<%= txtPhone.ClientID %>">Phone</label>
                                        <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" CssClass="w-full bg-transparent border-none border-b border-outline-variant/30 py-3 text-on-background focus:ring-0 focus:border-secondary transition-all placeholder:text-surface-container-highest uppercase text-sm tracking-wider" placeholder="9800000000"></asp:TextBox>
                                        <div class="absolute bottom-0 left-0 w-0 h-[1px] bg-secondary transition-all duration-500 group-focus-within:w-full"></div>
                                    </div>
                                    <div class="group relative">
                                        <label class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2" for="<%= txtDeliveryAddress.ClientID %>">Delivery Address</label>
                                        <asp:TextBox ID="txtDeliveryAddress" runat="server" MaxLength="500" TextMode="MultiLine" Rows="3" CssClass="w-full bg-transparent border-none border-b border-outline-variant/30 py-3 text-on-background focus:ring-0 focus:border-secondary transition-all placeholder:text-surface-container-highest uppercase text-sm tracking-wider resize-none" placeholder="STREET, SUITE, CITY, POSTAL CODE"></asp:TextBox>
                                        <div class="absolute bottom-0 left-0 w-0 h-[1px] bg-secondary transition-all duration-500 group-focus-within:w-full"></div>
                                    </div>
                                    <div class="group relative">
                                        <label class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2" for="<%= txtOrderNotes.ClientID %>">Order Notes (Optional)</label>
                                        <asp:TextBox ID="txtOrderNotes" runat="server" MaxLength="500" CssClass="w-full bg-transparent border-none border-b border-outline-variant/30 py-3 text-on-background focus:ring-0 focus:border-secondary transition-all placeholder:text-surface-container-highest uppercase text-sm tracking-wider" placeholder="SPECIAL INSTRUCTIONS FOR DELIVERY"></asp:TextBox>
                                        <div class="absolute bottom-0 left-0 w-0 h-[1px] bg-secondary transition-all duration-500 group-focus-within:w-full"></div>
                                    </div>
                                </div>
                            </div>
                        </section>

                        <section>
                            <div class="bg-surface-container-low p-10 space-y-8">
                                <h2 class="text-xs font-bold tracking-[0.2em] uppercase text-secondary">Payment Method</h2>
                                <div class="bg-surface-container p-6 border border-outline-variant/20">
                                    <asp:RadioButtonList
                                        ID="rblPaymentMethod"
                                        runat="server"
                                        CssClass="az-radio-list"
                                        RepeatDirection="Vertical"
                                        RepeatLayout="Table"
                                        CellPadding="0"
                                        CellSpacing="0">
                                        <asp:ListItem Value="eSewa" Selected="True">eSewa</asp:ListItem>
                                        <asp:ListItem Value="Khalti">Khalti</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                        </section>
                    </div>

                    <aside class="lg:col-span-2 lg:sticky lg:top-32 space-y-8">
                        <div class="bg-surface-container-high p-8 shadow-2xl">
                            <h3 class="text-lg font-bold tracking-[0.1em] uppercase mb-8 border-b border-outline-variant/10 pb-4">Your Order</h3>

                            <div class="max-h-[400px] overflow-y-auto pr-4 space-y-6">
                                <asp:Repeater ID="rptCheckoutItems" runat="server">
                                    <ItemTemplate>
                                        <div class="flex gap-4 items-center">
                                            <div class="w-20 h-24 bg-surface-container-highest flex-shrink-0">
                                                <img class="w-full h-full object-cover" src="<%#: Eval("ImageUrl") %>" alt="<%#: Eval("ImageAlt") %>" />
                                            </div>
                                            <div class="flex-grow">
                                                <p class="text-xs font-bold uppercase tracking-wider text-on-background"><%#: Eval("ProductName") %></p>
                                                <p class="text-[10px] text-on-surface-variant uppercase mt-1"><%#: Eval("DetailLabel") %></p>
                                                <p class="text-sm font-medium text-secondary mt-2"><%#: Eval("ItemTotalLabel") %></p>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div class="mt-8 pt-8 border-t border-outline-variant/10 space-y-4">
                                <div class="flex justify-between text-[10px] uppercase tracking-[0.2em] text-on-surface-variant">
                                    <span>Subtotal</span>
                                    <span><asp:Literal ID="litSubtotal" runat="server"></asp:Literal></span>
                                </div>
                                <div class="flex justify-between text-[10px] uppercase tracking-[0.2em] text-on-surface-variant">
                                    <span>Shipping</span>
                                    <span><asp:Literal ID="litShipping" runat="server"></asp:Literal></span>
                                </div>
                                <div class="flex justify-between text-lg font-bold uppercase tracking-widest text-on-background mt-4">
                                    <span>Total</span>
                                    <span class="text-secondary"><asp:Literal ID="litGrandTotal" runat="server"></asp:Literal></span>
                                </div>
                            </div>

                            <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" OnClick="btnPlaceOrder_Click" CssClass="w-full bg-secondary text-on-secondary py-5 mt-10 font-bold uppercase tracking-[0.3em] text-xs hover:brightness-110 active:scale-[0.98] transition-all duration-300" UseSubmitBehavior="false" />
                            <p class="text-[9px] text-center text-on-surface-variant uppercase tracking-widest mt-6">Secure SSL Encrypted Checkout</p>
                        </div>

                        <div class="bg-primary-container p-6 flex items-center gap-4">
                            <span class="material-symbols-outlined text-secondary" data-icon="auto_awesome">auto_awesome</span>
                            <p class="text-[10px] uppercase tracking-widest text-on-primary-container leading-relaxed">
                                Complimentary white-glove delivery included for orders above $2,000.
                            </p>
                        </div>
                    </aside>
                </div>
            </asp:PlaceHolder>
        </main>
    </div>
</asp:Content>
