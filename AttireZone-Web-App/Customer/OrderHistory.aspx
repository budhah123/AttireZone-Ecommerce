<%@ Page Title="Order History | AttireZone" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="OrderHistory.aspx.cs"
Inherits="AttireZone_Web_App.Customer.OrderHistory" %>

<asp:Content
  ID="OrderHistoryStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <link
    href="/Assets/CSS/authentication.css"
    rel="stylesheet"
    type="text/css"
  />
</asp:Content>

<asp:Content
  ID="OrderHistoryMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <main
    class="az-order-shell flex-grow max-w-7xl mx-auto w-full px-6 py-12 md:py-20"
  >
    <header class="mb-16">
      <h1
        class="text-5xl md:text-7xl font-bold tracking-tighter uppercase mb-4 text-on-background"
      >
        Order History
      </h1>
      <p
        class="text-on-surface-variant max-w-xl text-lg font-light leading-relaxed"
      >
        Review your curated selections and track the journey of your AttireZone
        pieces.
      </p>
    </header>

    <div class="space-y-12">
      <section
        class="grid grid-cols-1 lg:grid-cols-12 gap-0 border-t border-outline-variant/20 pt-8"
      >
        <div class="lg:col-span-3 mb-4 lg:mb-0">
          <span
            class="block text-xs uppercase tracking-[0.2em] font-bold text-secondary mb-2"
            >Active Shipment</span
          >
          <h3 class="text-2xl font-semibold tracking-tight">Order #AZ-88291</h3>
          <p class="text-sm text-on-surface-variant mt-1">
            Placed: October 24, 2024
          </p>
        </div>
        <div class="lg:col-span-6 flex items-center space-x-6">
          <div class="w-24 h-32 bg-surface-container overflow-hidden group">
            <img
              alt="Luxury Wool Coat"
              class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              src="https://lh3.googleusercontent.com/aida-public/AB6AXuCJPXsnWgmp5koYmtjz-8HtmQw3djkScnKTksHpIMu16Dd9_5RRs7F3D48S4h-aL95oHiG95RmR0rh9S3kPzuPtTaTL9Sev_j0X6n-IkQD58paVX_fT38zu0RZN9rlOrM1rLUBaIfkvmrIN6fGnGg1WeEG9aMmv4aHTIODpM22boKPf-wp07ALWW71W_vAk18HYc7FqzoC_fgMnIz_-5QDxIyrGVF55JLl4_jc6z3QKbOyF8K0k3mWXRQrQ6Kd7-l4AFIbpFuR-ASM"
            />
          </div>
          <div>
            <p
              class="text-on-surface font-medium uppercase text-sm tracking-wide"
            >
              Signature Wool Overcoat
            </p>
            <p
              class="text-xs text-on-surface-variant mt-1 uppercase tracking-widest"
            >
              Size: 48 | Color: Midnight
            </p>
            <div class="mt-4 flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-secondary"></span>
              <span
                class="text-xs uppercase font-bold tracking-widest text-secondary"
                >In Transit - Expected Oct 28</span
              >
            </div>
          </div>
        </div>
        <div class="lg:col-span-3 flex flex-col items-end justify-between">
          <span class="text-2xl font-light">$1,250.00</span>
          <asp:Button
            ID="btnTrackDelivery"
            runat="server"
            Text="Track Delivery"
            CssClass="mt-4 px-8 py-3 bg-secondary text-on-secondary text-xs font-bold uppercase tracking-widest hover:opacity-90 transition-opacity"
            CausesValidation="false"
            UseSubmitBehavior="false"
            OnClientClick="return false;"
          />
        </div>
      </section>

      <asp:GridView
        ID="gvOrderHistory"
        runat="server"
        AutoGenerateColumns="False"
        ShowHeader="False"
        GridLines="None"
        CssClass="az-order-gridview"
      >
        <Columns>
          <asp:TemplateField>
            <ItemTemplate>
              <div class="az-order-row grid grid-cols-1 lg:grid-cols-12 gap-0">
                <div class="lg:col-span-3 mb-4 lg:mb-0">
                  <h3 class="text-xl font-semibold tracking-tight">
                    <%# Eval("OrderNumber") %>
                  </h3>
                  <p class="text-sm text-on-surface-variant mt-1">
                    Placed: <%# Eval("PlacedDate") %>
                  </p>
                </div>
                <div class="lg:col-span-6">
                  <div class="flex flex-wrap gap-4">
                    <asp:Literal
                      ID="litItems"
                      runat="server"
                      Text='<%# Eval("ItemsHtml") %>'
                    ></asp:Literal>
                  </div>
                  <div class="mt-4 flex items-center gap-2">
                    <span
                      class='material-symbols-outlined text-[16px] <%# Eval("StatusCssClass") %>'
                      ><%# Eval("StatusIcon") %></span
                    >
                    <span
                      class='text-xs uppercase font-bold tracking-widest <%# Eval("StatusCssClass") %>'
                      ><%# Eval("StatusLabel") %></span
                    >
                  </div>
                </div>
                <div
                  class="lg:col-span-3 flex flex-col items-end justify-between"
                >
                  <span class="text-xl font-light"><%# Eval("Total") %></span>
                  <asp:Button
                    ID="btnOrderAction"
                    runat="server"
                    Text='<%# Eval("ActionText") %>'
                    CssClass="az-order-action-link"
                    CausesValidation="false"
                    UseSubmitBehavior="false"
                    OnClientClick="return false;"
                  />
                </div>
              </div>
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>

      <div class="border-t border-outline-variant/20 pt-16 flex justify-center">
        <asp:Button
          ID="btnLoadMore"
          runat="server"
          Text="Load More History"
          CssClass="az-order-load-more"
          UseSubmitBehavior="false"
          CausesValidation="false"
          OnClick="btnLoadMore_Click"
        />
      </div>
    </div>
  </main>
</asp:Content>
