<%@ Page Title="Payment Failed | AttireZone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EsewaFailure.aspx.cs" Inherits="AttireZone_Web_App.EsewaFailure" %>

<asp:Content ID="EsewaFailureContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="max-w-4xl mx-auto min-h-[70vh] px-6 py-16 flex items-center justify-center">
        <div class="w-full bg-surface-container p-10 border border-outline-variant/30 text-center space-y-6">
            <span class="material-symbols-outlined text-error !text-[64px]">cancel</span>
            <h1 class="text-3xl font-bold uppercase tracking-[0.12em] text-on-background">eSewa Payment Failed</h1>
            <p class="text-sm uppercase tracking-[0.12em] text-on-surface-variant">
                <asp:Literal ID="litFailureMessage" runat="server"></asp:Literal>
            </p>
            <div class="flex flex-col sm:flex-row gap-4 justify-center pt-4">
                <a href="<%= ResolveUrl("~/Customer/Checkout.aspx") %>" class="bg-secondary text-on-secondary px-8 py-3 text-xs font-bold uppercase tracking-[0.14em]">Try Again</a>
                <a href="<%= ResolveUrl("~/Customer/Cart.aspx") %>" class="border border-outline-variant/40 text-on-background px-8 py-3 text-xs font-bold uppercase tracking-[0.14em]">Back to Cart</a>
            </div>
        </div>
    </main>
</asp:Content>
