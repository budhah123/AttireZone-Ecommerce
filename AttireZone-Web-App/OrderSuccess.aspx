<%@ Page Title="Order Success | AttireZone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderSuccess.aspx.cs" Inherits="AttireZone_Web_App.OrderSuccess" %>

<asp:Content ID="OrderSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="max-w-4xl mx-auto min-h-[70vh] px-6 py-16 flex items-center justify-center">
        <div class="w-full bg-surface-container p-10 border border-outline-variant/30 text-center space-y-6">
            <span class="material-symbols-outlined text-[#22c55e] !text-[64px]">check_circle</span>
            <h1 class="text-3xl font-bold uppercase tracking-[0.12em] text-on-background">Payment Successful</h1>
            <p class="text-sm uppercase tracking-[0.12em] text-on-surface-variant">Your order has been placed and confirmed.</p>

            <div class="space-y-2 text-xs uppercase tracking-[0.12em] text-on-surface-variant">
                <p>Transaction ID: <span class="text-on-background"><asp:Literal ID="litTransactionId" runat="server"></asp:Literal></span></p>
                <p>Payment Method: <span class="text-on-background"><asp:Literal ID="litPaymentMethod" runat="server"></asp:Literal></span></p>
            </div>

            <asp:PlaceHolder ID="phEmailNotice" runat="server" Visible="false">
                <div class="bg-[#fff7ed] border border-[#fed7aa] px-4 py-3 text-xs tracking-[0.05em] uppercase text-[#9a3412]">
                    <asp:Literal ID="litEmailNotice" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>

            <div class="flex flex-col sm:flex-row gap-4 justify-center pt-4">
                <a href="<%= ResolveUrl("~/Customer/OrderHistory.aspx") %>" class="bg-secondary text-on-secondary px-8 py-3 text-xs font-bold uppercase tracking-[0.14em]">View Orders</a>
                <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="border border-outline-variant/40 text-on-background px-8 py-3 text-xs font-bold uppercase tracking-[0.14em]">Continue Shopping</a>
            </div>
        </div>
    </main>
</asp:Content>
