<%@ Page Title="AttireZone | Admin Dashboard" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="Dashboard.aspx.cs" Inherits="AttireZone_Web_App.Admin.Dashboard" %>

<asp:Content
  ID="DashboardStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <style type="text/css">
    .body-content {
      max-width: none !important;
      width: 100% !important;
      padding-left: 0 !important;
      padding-right: 0 !important;
      margin: 0 !important;
    }
  </style>
</asp:Content>

<asp:Content
  ID="DashboardMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <main class="max-w-[1920px] mx-auto px-5 sm:px-6 py-8 md:py-12">
    <!-- Header Section -->
    <header class="mb-10">
      <h1
        class="text-3xl md:text-5xl font-extrabold tracking-tighter mb-3 uppercase"
      >
        Executive Dashboard
      </h1>
      <p class="text-on-surface-variant font-medium tracking-tight">
        System overview and commercial performance metrics.
      </p>
    </header>

    <!-- Bento Grid Stats -->
    <div
      class="grid grid-cols-1 md:grid-cols-4 gap-0 mb-14 border border-outline-variant/20"
    >
      <div
        class="p-6 border-r border-b md:border-b-0 border-outline-variant/20 bg-surface-container-low transition-colors hover:bg-surface-container"
      >
        <div class="flex justify-between items-start mb-8">
          <span class="material-symbols-outlined text-secondary text-2xl"
            >person</span
          >
          <span
            class="text-xs font-bold tracking-widest uppercase text-on-surface-variant"
            ><asp:Literal ID="litUsersDelta" runat="server"
          /></span>
        </div>
        <h3
          class="text-sm font-bold uppercase tracking-widest text-on-surface-variant mb-2"
        >
          Total Users
        </h3>
        <p class="text-3xl md:text-4xl font-bold tracking-tighter">
          <asp:Literal ID="litTotalUsers" runat="server" />
        </p>
      </div>

      <div
        class="p-6 border-r border-b md:border-b-0 border-outline-variant/20 bg-surface-container-low transition-colors hover:bg-surface-container"
      >
        <div class="flex justify-between items-start mb-8">
          <span class="material-symbols-outlined text-secondary text-2xl"
            >shopping_cart</span
          >
          <span
            class="text-xs font-bold tracking-widest uppercase text-on-surface-variant"
            ><asp:Literal ID="litOrdersDelta" runat="server"
          /></span>
        </div>
        <h3
          class="text-sm font-bold uppercase tracking-widest text-on-surface-variant mb-2"
        >
          Total Orders
        </h3>
        <p class="text-3xl md:text-4xl font-bold tracking-tighter">
          <asp:Literal ID="litTotalOrders" runat="server" />
        </p>
      </div>

      <div
        class="p-6 border-r border-b md:border-b-0 border-outline-variant/20 bg-surface-container-low transition-colors hover:bg-surface-container"
      >
        <div class="flex justify-between items-start mb-8">
          <span class="material-symbols-outlined text-secondary text-2xl"
            >inventory_2</span
          >
          <span
            class="text-xs font-bold tracking-widest uppercase text-on-surface-variant"
            ><asp:Literal ID="litProductsDelta" runat="server"
          /></span>
        </div>
        <h3
          class="text-sm font-bold uppercase tracking-widest text-on-surface-variant mb-2"
        >
          Total Products
        </h3>
        <p class="text-3xl md:text-4xl font-bold tracking-tighter">
          <asp:Literal ID="litTotalProducts" runat="server" />
        </p>
      </div>

      <div
        class="p-6 bg-surface-container-low transition-colors hover:bg-surface-container"
      >
        <div class="flex justify-between items-start mb-8">
          <span class="material-symbols-outlined text-secondary text-2xl"
            >payments</span
          >
          <span
            class="text-xs font-bold tracking-widest uppercase text-secondary"
            ><asp:Literal ID="litRevenueDelta" runat="server"
          /></span>
        </div>
        <h3
          class="text-sm font-bold uppercase tracking-widest text-on-surface-variant mb-2"
        >
          Total Revenue
        </h3>
        <p class="text-3xl md:text-4xl font-bold tracking-tighter">
          <asp:Literal ID="litTotalRevenue" runat="server" />
        </p>
      </div>
    </div>

    <!-- Quick Actions & Recent Activity -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      <!-- Management Controls -->
      <div class="lg:col-span-4 space-y-8">
        <section>
          <h2
            class="text-xl font-bold uppercase tracking-widest mb-6 border-l-4 border-secondary pl-3"
          >
            Management
          </h2>
          <div class="space-y-3">
            <div
              class="w-full flex justify-between items-center p-4 border border-secondary/40 bg-secondary text-on-secondary"
              aria-current="page"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Dashboard</span
              >
              <span class="material-symbols-outlined">dashboard</span>
            </div>
            <a
              href="/Admin/ManageProduct/ManageProducts.aspx"
              class="w-full flex justify-between items-center p-4 border border-outline-variant/20 bg-surface-container-low hover:bg-secondary hover:text-on-secondary group transition-all"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Manage Products</span
              >
              <span
                class="material-symbols-outlined group-hover:translate-x-2 transition-transform"
                >arrow_forward</span
              >
            </a>
            <a
              href="/Admin/ManageCategories/ManageCategory.aspx"
              class="w-full flex justify-between items-center p-4 border border-outline-variant/20 bg-surface-container-low hover:bg-secondary hover:text-on-secondary group transition-all"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Manage Categories</span
              >
              <span
                class="material-symbols-outlined group-hover:translate-x-2 transition-transform"
                >arrow_forward</span
              >
            </a>
            <a
              href="/Admin/ManageUser/ManageUser.aspx"
              class="w-full flex justify-between items-center p-4 border border-outline-variant/20 bg-surface-container-low hover:bg-secondary hover:text-on-secondary group transition-all"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Manage Users</span
              >
              <span
                class="material-symbols-outlined group-hover:translate-x-2 transition-transform"
                >arrow_forward</span
              >
            </a>
            <a
              href="/Admin/ProcessOrders/ProcessOrders.aspx"
              class="w-full flex justify-between items-center p-4 border border-outline-variant/20 bg-surface-container-low hover:bg-secondary hover:text-on-secondary group transition-all"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Process Orders</span
              >
              <span
                class="material-symbols-outlined group-hover:translate-x-2 transition-transform"
                >arrow_forward</span
              >
            </a>
            <button
              type="button"
              class="w-full flex justify-between items-center p-4 border border-outline-variant/20 bg-surface-container-low hover:bg-secondary hover:text-on-secondary group transition-all"
            >
              <span class="font-bold uppercase tracking-widest text-sm"
                >Inventory Log</span
              >
              <span
                class="material-symbols-outlined group-hover:translate-x-2 transition-transform"
                >arrow_forward</span
              >
            </button>
          </div>
        </section>

        <section class="bg-primary-container p-6">
          <h3
            class="text-secondary font-bold uppercase tracking-widest text-xs mb-3"
          >
            System Status
          </h3>
          <div class="flex items-center gap-4 text-on-primary-container">
            <div class="w-2 h-2 rounded-full bg-green-500 animate-pulse"></div>
            <span class="text-sm font-medium">Global CDN: Operational</span>
          </div>
        </section>
      </div>

      <!-- Visual Content & List -->
      <div class="lg:col-span-8">
        <h2 class="text-xl font-bold uppercase tracking-widest mb-6">
          Recent Revenue Streams
        </h2>
        <div class="bg-surface-container p-1 overflow-hidden">
          <div class="relative h-[320px] md:h-[360px] w-full mb-6 group">
            <img
              alt="Luxury Retail Interior"
              data-alt="Modern high-end fashion boutique interior with minimal racks and moody spotlights on dark surfaces"
              class="w-full h-full object-cover grayscale brightness-50 group-hover:grayscale-0 transition-all duration-700"
              src="https://lh3.googleusercontent.com/aida-public/AB6AXuCTRKOAl5m0-SGm66ASg4_54XN0iOUIeAyBrwQ4xBlXAn8IWL0RgL5oifUv3udKMKrSgi-uuiJ4EwDq_wPlSrADVCCQjjMIP0abGXheSdUUArsdp_qntJ1TiH6MZ7SZXwR8uzmMlkQcmtN7MyqyP1L8n10qOVUHtA8j4S4LZGyoXPCkBghQZcPInzPi0kRnrE44bzJWp9Ks_V_2qeigzTS5t-BpcDxFO88QE29vXDGvxZO1l-0hrpGTYKsKohLoY-9nDFgM59pg2D0"
            />
            <div
              class="absolute inset-0 flex flex-col justify-end p-8 md:p-10 bg-gradient-to-t from-surface-container-lowest via-transparent"
            >
              <p
                class="text-secondary font-bold uppercase tracking-widest text-xs mb-2"
              >
                Quarterly Projection
              </p>
              <h4
                class="text-2xl md:text-3xl font-extrabold tracking-tighter uppercase"
              >
                +18% expected growth in Q4 apparel
              </h4>
            </div>
          </div>

          <div class="p-6 space-y-4">
            <asp:Repeater ID="rptRecentOrders" runat="server">
              <ItemTemplate>
                <div
                  class="flex items-center justify-between pb-4 border-b border-outline-variant/10 last:border-b-0 last:pb-0"
                >
                  <div class="flex items-center gap-4">
                    <div
                      class="w-10 h-10 bg-surface-container-highest flex items-center justify-center"
                    >
                      <span class="material-symbols-outlined text-secondary"
                        >check_circle</span
                      >
                    </div>
                    <div>
                      <p class="font-bold uppercase text-xs tracking-widest">
                        Order #<%#: Eval("OrderNumber") %>
                      </p>
                      <p class="text-on-surface-variant text-sm">
                        <%#: Eval("Description") %>
                      </p>
                    </div>
                  </div>
                  <span class="font-bold tracking-tighter text-base md:text-lg"
                    ><%#: Eval("AmountFormatted") %></span
                  >
                </div>
              </ItemTemplate>
            </asp:Repeater>
          </div>
        </div>
      </div>
    </div>
  </main>

  <script type="text/javascript">
    (function () {
      var refreshIntervalMs = 60000;

      window.setInterval(function () {
        if (document.hidden) {
          return;
        }

        window.location.reload();
      }, refreshIntervalMs);
    })();
  </script>
</asp:Content>
