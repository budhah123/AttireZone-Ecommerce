<%@ Page Title="Khalti Callback | AttireZone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="KhaltiCallBack.aspx.cs" Inherits="AttireZone_Web_App.KhaltiCallback" %>

<asp:Content ID="KhaltiCallbackContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="max-w-4xl mx-auto min-h-[70vh] px-6 py-16 flex items-center justify-center">
        <div class="w-full bg-surface-container p-10 border border-outline-variant/30 text-center space-y-6">
            <span class="material-symbols-outlined text-secondary !text-[64px]">payments</span>
            <h1 class="text-3xl font-bold uppercase tracking-[0.12em] text-on-background">Khalti Payment Status</h1>
            <p class="text-sm uppercase tracking-[0.12em] text-on-surface-variant">
                <asp:Literal ID="litStatusMessage" runat="server"></asp:Literal>
            </p>
            <div class="pt-4">
                <a href="<%= ResolveUrl("~/Customer/Checkout.aspx") %>" class="border border-outline-variant/40 text-on-background px-8 py-3 text-xs font-bold uppercase tracking-[0.14em]">Back to Checkout</a>
            </div>
        </div>
    </main>
</asp:Content>
