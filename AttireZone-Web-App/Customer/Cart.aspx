<%@ Page Title="Cart | ATTIREZONE" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="AttireZone_Web_App.Customer.Cart" %>

<asp:Content ID="CartStyles" ContentPlaceHolderID="StylesPlaceholder" runat="server">
  <link href="https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@600;700&display=swap" rel="stylesheet" />
  <style type="text/css">
    .container.body-content {
      max-width: 100% !important;
      width: 100% !important;
      margin: 0 !important;
      padding: 0 !important;
    }

    .az-cart-page .material-symbols-outlined {
      font-variation-settings: 'FILL' 0, 'wght' 200, 'GRAD' 0, 'opsz' 24;
      font-size: 20px;
    }
  </style>
</asp:Content>

<asp:Content ID="CartMain" ContentPlaceHolderID="MainContent" runat="server">
  <div class="az-cart-page">
    <main class="min-h-screen pt-20 pb-20 px-6 md:px-12 max-w-7xl mx-auto">
      <h1 class="font-serif text-4xl md:text-5xl mb-12 tracking-tight text-on-background">Your Selection</h1>
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-16">
        <div class="lg:col-span-8 space-y-8">
          <asp:PlaceHolder ID="phEmptyCart" runat="server" Visible="false">
            <div class="flex flex-col md:flex-row gap-6 pb-8 border-b border-surface-container-high group">
              <div class="flex-grow">
                <h3 class="font-serif text-2xl text-on-background">Your shopping cart is currently empty.</h3>
                <p class="text-sm text-on-surface-variant font-label uppercase tracking-widest mt-3">Start curating your selection from the latest collection.</p>
              </div>
            </div>
          </asp:PlaceHolder>

          <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
            <ItemTemplate>
              <div class="flex flex-col md:flex-row gap-6 pb-8 border-b border-surface-container-high group">
                <div class="w-20 h-20 bg-surface-container overflow-hidden shrink-0">
                  <img class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" src="<%#: Eval("ImageUrl") %>" alt="<%#: Eval("ImageAlt") %>" />
                </div>
                <div class="flex-grow flex flex-col justify-between">
                  <div class="flex justify-between items-start">
                    <div>
                      <h3 class="font-serif text-2xl text-on-background"><%#: Eval("ProductName") %></h3>
                      <p class="text-sm text-on-surface-variant font-label uppercase tracking-widest mt-1"><%#: Eval("ReferenceLabel") %></p>
                    </div>
                    <asp:LinkButton ID="btnRemoveItem" runat="server" CommandName="RemoveItem" CommandArgument='<%# Eval("CartId") %>' CssClass="text-on-surface-variant hover:text-error transition-colors" aria-label="Remove item">
                      <span class="material-symbols-outlined">delete</span>
                    </asp:LinkButton>
                  </div>
                  <div class="flex justify-between items-end mt-4">
                    <div class="flex items-center bg-[#111] border border-[#2a2a2a] px-3 py-1 gap-4">
                      <button type="button" class="text-on-surface-variant hover:text-secondary">-</button>
                      <span class="text-sm font-medium w-4 text-center"><%#: Eval("SelectedQuantity") %></span>
                      <button type="button" class="text-on-surface-variant hover:text-secondary">+</button>
                    </div>
                    <div class="text-right">
                      <p class="text-secondary font-medium tracking-tighter"><%#: Eval("ItemTotalLabel") %></p>
                    </div>
                  </div>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>

          <div class="pt-4 flex justify-between items-center">
            <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="group flex items-center gap-2 text-on-surface-variant hover:text-on-background transition-all">
              <span class="material-symbols-outlined transition-transform group-hover:-translate-x-1">west</span>
              <span class="font-label text-xs tracking-widest uppercase">Continue Shopping</span>
            </a>
            <div class="text-right">
              <span class="text-on-surface-variant text-xs uppercase tracking-widest">Bag Subtotal</span>
              <p class="text-2xl font-bold text-on-background tracking-tighter mt-1"><asp:Literal ID="litBagSubtotal" runat="server"></asp:Literal></p>
            </div>
          </div>
        </div>

        <div class="lg:col-span-4">
          <div class="sticky top-40 bg-surface-container p-8 space-y-8 shadow-[0px_40px_80px_rgba(0,0,0,0.2)]">
            <h2 class="text-xl font-bold uppercase tracking-[0.2em] text-on-background">Order Summary</h2>
            <div class="space-y-4 text-sm tracking-wide">
              <div class="flex justify-between">
                <span class="text-on-surface-variant uppercase">Subtotal</span>
                <span class="text-on-background"><asp:Literal ID="litSubtotal" runat="server"></asp:Literal></span>
              </div>
              <div class="flex justify-between">
                <span class="text-on-surface-variant uppercase">Shipping</span>
                <span class="text-[#10b981] font-medium uppercase tracking-widest">Free</span>
              </div>
              <div class="flex justify-between">
                <span class="text-on-surface-variant uppercase">Tax (Est.)</span>
                <span class="text-on-background"><asp:Literal ID="litTax" runat="server"></asp:Literal></span>
              </div>
            </div>
            <div class="border-t border-surface-container-high pt-6">
              <div class="flex justify-between items-baseline mb-8">
                <span class="text-on-background font-bold uppercase tracking-widest">Total</span>
                <span class="text-3xl font-bold text-secondary tracking-tighter"><asp:Literal ID="litGrandTotal" runat="server"></asp:Literal></span>
              </div>
              <div class="space-y-3">
                <asp:Button ID="btnProceedToCheckout" runat="server" Text="Proceed to Checkout" CssClass="w-full bg-secondary text-on-secondary py-5 font-bold uppercase tracking-[0.15em] hover:brightness-110 active:scale-[0.98] transition-all" CausesValidation="false" UseSubmitBehavior="false" PostBackUrl="~/Customer/Checkout.aspx" />
                <div class="flex items-center gap-2 justify-center py-4 text-[10px] text-on-surface-variant uppercase tracking-widest">
                  <span class="material-symbols-outlined !text-[14px]">lock</span>
                  Secure checkout with SSL encryption
                </div>
              </div>
            </div>
            <div class="pt-4">
              <label class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2">Have a Promotional Code?</label>
              <div class="flex border-b border-outline-variant focus-within:border-secondary transition-colors pb-1">
                <input class="bg-transparent border-none text-on-background placeholder:text-surface-variant focus:ring-0 w-full text-sm" placeholder="Enter Code" type="text" />
                <button class="text-secondary font-bold text-xs uppercase tracking-widest px-2" type="button">Apply</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</asp:Content>
