<%@ Page Title="AttireZone | Manage Products" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="ManageProducts.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageProduct.ManageProducts" %>

<asp:Content
  ID="ManageProductsStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <style type="text/css">
    .az-navbar,
    .az-footer {
      display: none !important;
    }

    .container.body-content {
      max-width: none !important;
      width: 100% !important;
      margin: 0 !important;
      padding-left: 0 !important;
      padding-right: 0 !important;
    }

    .material-symbols-outlined {
      font-weight: 300;
      font-style: normal;
      line-height: 1;
    }

    .az-manage-products button,
    .az-manage-products input,
    .az-manage-products select {
      border-radius: 0 !important;
    }

    .az-manage-products .no-scrollbar::-webkit-scrollbar {
      display: none;
    }

    .az-manage-products .glass-nav {
      backdrop-filter: blur(12px);
    }

    .az-manage-products .premium-border-b {
      border-bottom: 1px solid rgba(142, 145, 152, 0.1);
    }
  </style>
</asp:Content>

<asp:Content
  ID="ManageProductsMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <div
    class="az-manage-products bg-background text-on-background selection:bg-secondary selection:text-on-secondary overflow-hidden"
  >
    <!-- TopNavBar Shell -->
    <nav
      class="fixed top-0 w-full z-50 bg-[#001f3f]/80 dark:bg-[#0e0e0e]/80 backdrop-blur-md shadow-[0_0_40px_rgba(0,0,0,0.08)] flex justify-between items-center px-6 h-16"
    >
      <div class="flex items-center gap-8">
        <span
          class="text-xl font-bold tracking-tighter text-[#e2e2e2] uppercase font-['Inter']"
          >AttireZone</span
        >
        <div
          class="hidden md:flex items-center bg-surface-container-low px-4 py-1.5 gap-3 group border-b border-outline-variant/20 transition-all focus-within:border-secondary"
        >
          <span
            class="material-symbols-outlined text-on-surface-variant text-sm"
            data-icon="search"
            >search</span
          >
          <asp:TextBox
            ID="txtProductSearch"
            runat="server"
            AutoPostBack="true"
            OnTextChanged="txtProductSearch_TextChanged"
            class="bg-transparent border-none focus:ring-0 text-sm w-64 placeholder:text-outline text-on-surface"
            placeholder="Search product catalogue..."
          ></asp:TextBox>
        </div>
      </div>
      <div class="flex items-center gap-6">
        <button
          type="button"
          class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 scale-95 transition-transform"
        >
          <span class="material-symbols-outlined" data-icon="settings"
            >settings</span
          >
        </button>
        <button
          type="button"
          class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 scale-95 transition-transform"
        >
          <span class="material-symbols-outlined" data-icon="account_circle"
            >account_circle</span
          >
        </button>
      </div>
    </nav>

    <!-- SideNavBar Shell -->
    <aside
      class="fixed left-0 top-0 h-screen w-64 z-40 bg-[#131313] dark:bg-[#131313] flex flex-col pt-20 pb-6 px-4"
    >
      <div class="mb-10 px-4">
        <h2 class="text-lg font-semibold text-[#e2e2e2]">Admin Console</h2>
        <p
          class="text-xs text-on-surface-variant uppercase tracking-widest mt-1"
        >
          AttireZone Portal
        </p>
      </div>

      <nav class="flex-1 space-y-1">
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/Dashboard.aspx"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            data-icon="dashboard"
            >dashboard</span
          >
          Dashboard
        </a>
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#e9c349] border-r-2 border-[#e9c349] bg-[#1f1f1f] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/ManageProduct/ManageProducts.aspx"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            data-icon="inventory_2"
            >inventory_2</span
          >
          Manage Products
        </a>
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="#"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            data-icon="shopping_bag"
            >shopping_bag</span
          >
          Process Orders
        </a>
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="#"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            data-icon="history_edu"
            >history_edu</span
          >
          Inventory Logs
        </a>
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/ManageUser/ManageUser.aspx"
        >
          <span class="material-symbols-outlined text-[20px]" data-icon="group"
            >group</span
          >
          User Management
        </a>
      </nav>

      <div class="mt-auto border-t border-outline-variant/10 pt-4">
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/AdminLogin.aspx"
        >
          <span class="material-symbols-outlined text-[20px]" data-icon="logout"
            >logout</span
          >
          Logout
        </a>
      </div>
    </aside>

    <!-- Main Content Canvas -->
    <main
      class="ml-64 pt-16 h-screen overflow-y-auto no-scrollbar bg-surface-dim"
    >
      <div class="p-8 max-w-7xl mx-auto">
        <!-- Page Header -->
        <header
          class="flex flex-col md:flex-row md:items-end justify-between mb-12 gap-6"
        >
          <div>
            <span
              class="text-secondary font-label text-xs font-bold uppercase tracking-[0.2em] mb-2 block"
              >Catalogue Management</span
            >
            <h1
              class="text-4xl font-headline font-bold text-on-surface tracking-tight"
            >
              Manage Products
            </h1>
          </div>

          <div class="flex flex-wrap items-center gap-4">
            <div class="relative group">
              <asp:DropDownList
                ID="ddlCategoryFilter"
                runat="server"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged"
                class="appearance-none bg-surface-container-low border-b border-outline-variant/30 text-on-surface-variant text-sm py-2.5 pl-4 pr-10 focus:outline-none focus:border-secondary transition-colors cursor-pointer min-w-[160px]"
              >
                <asp:ListItem Value="">All Categories</asp:ListItem>
              </asp:DropDownList>
              <span
                class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-sm pointer-events-none"
                data-icon="expand_more"
                >expand_more</span
              >
            </div>
            <a
              href="/Admin/ManageProduct/AddProductModal.aspx"
              class="bg-secondary text-on-secondary px-8 py-2.5 font-bold uppercase text-xs tracking-widest flex items-center gap-2 hover:bg-secondary-container transition-colors shadow-lg shadow-secondary/5"
            >
              <span
                class="material-symbols-outlined text-sm"
                data-icon="add"
                style="font-variation-settings: &quot;wght&quot; 600"
                >add</span
              >
              Add Product
            </a>
          </div>
        </header>

        <asp:Panel
          ID="pnlActionMessage"
          runat="server"
          Visible="false"
          CssClass="mb-8 border border-secondary/30 bg-secondary/10 px-4 py-3 text-xs uppercase tracking-widest text-secondary"
        >
          <asp:Literal ID="litActionMessage" runat="server" />
        </asp:Panel>

        <!-- Stats Overview (Asymmetric Bento Lite) -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-12">
          <div class="bg-surface-container p-6 border-l border-secondary/40">
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Total SKU
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litTotalSku" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Low Stock Alerts
            </p>
            <p class="text-2xl font-bold text-secondary">
              <asp:Literal ID="litLowStockAlerts" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Popular Products
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litInSeason" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Live Collections
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litLiveCollections" runat="server" />
            </p>
          </div>
        </div>

        <!-- Products Table Container -->
        <section class="bg-surface-container-lowest overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr
                  class="premium-border-b text-on-surface-variant uppercase text-[10px] font-bold tracking-[0.2em]"
                >
                  <th class="px-6 py-5 font-bold">Product Name</th>
                  <th class="px-6 py-5 font-bold">Category</th>
                  <th class="px-6 py-5 font-bold text-center">Popular</th>
                  <th class="px-6 py-5 font-bold text-right">Price</th>
                  <th class="px-6 py-5 font-bold text-right">Stock</th>
                  <th class="px-6 py-5 font-bold text-center">Status</th>
                  <th class="px-6 py-5 font-bold text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-outline-variant/5">
                <asp:Repeater
                  ID="rptProducts"
                  runat="server"
                  OnItemCommand="rptProducts_ItemCommand"
                >
                  <ItemTemplate>
                    <tr
                      class="group hover:bg-surface-container-low transition-colors duration-200"
                    >
                      <td class="px-6 py-5">
                        <div class="flex items-center gap-4">
                          <div
                            class="h-12 w-12 bg-surface-container flex-shrink-0"
                          >
                            <img
                              class="h-full w-full object-cover grayscale group-hover:grayscale-0 transition-all duration-500"
                              alt='<%#: Eval("ImageAlt") %>'
                              src='<%#: Eval("ImageUrl") %>'
                            />
                          </div>
                          <div>
                            <p class="text-on-surface font-semibold text-sm">
                              <%#: Eval("ProductName") %>
                            </p>
                            <p class="text-on-surface-variant text-xs">
                              SKU: <%#: Eval("Sku") %>
                            </p>
                          </div>
                        </div>
                      </td>
                      <td class="px-6 py-5 text-on-surface-variant text-sm">
                        <%#: Eval("Category") %>
                      </td>
                      <td class="px-6 py-5 text-center">
                        <span
                          class='<%#: Eval("PopularBadgeCssClass") %>'
                          ><%#: Eval("PopularLabel") %></span
                        >
                      </td>
                      <td
                        class="px-6 py-5 text-on-surface text-sm text-right font-medium"
                      >
                        <%#: Eval("PriceFormatted") %>
                      </td>
                      <td class="px-6 py-5 text-on-surface text-sm text-right">
                        <%#: Eval("StockQuantity") %>
                      </td>
                      <td class="px-6 py-5 text-center">
                        <span
                          class='<%#: Eval("StatusBadgeCssClass") %>'
                          ><%#: Eval("StatusLabel") %></span
                        >
                      </td>
                      <td class="px-6 py-5 text-right">
                        <div
                          class="flex items-center justify-end gap-3 opacity-0 group-hover:opacity-100 transition-opacity"
                        >
                          <asp:LinkButton
                            ID="btnEditProduct"
                            runat="server"
                            CommandName="EditProduct"
                            CommandArgument='<%#: Eval("Id") %>'
                            CausesValidation="false"
                            CssClass="text-on-surface-variant hover:text-secondary"
                          >
                            <span
                              class="material-symbols-outlined text-lg"
                              data-icon="edit"
                              >edit</span
                            >
                          </asp:LinkButton>
                          <asp:LinkButton
                            ID="btnDeleteProduct"
                            runat="server"
                            CommandName="DeleteProduct"
                            CommandArgument='<%#: Eval("Id") %>'
                            CausesValidation="false"
                            OnClientClick="return confirm('Delete this product? This action cannot be undone.');"
                            CssClass="text-on-surface-variant hover:text-error"
                          >
                            <span
                              class="material-symbols-outlined text-lg"
                              data-icon="delete"
                              >delete</span
                            >
                          </asp:LinkButton>
                        </div>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>

          <!-- Pagination Footer -->
          <div
            class="px-6 py-6 border-t border-outline-variant/10 flex items-center justify-between"
          >
            <p class="text-on-surface-variant text-xs font-label">
              Showing
              <asp:Literal ID="litShownFrom" runat="server" />-<asp:Literal
                ID="litShownTo"
                runat="server"
              />
              of
              <asp:Literal ID="litShownTotal" runat="server" />
              products
            </p>
            <div class="flex items-center gap-2">
              <button
                type="button"
                class="p-2 text-on-surface-variant hover:text-secondary disabled:opacity-30"
                disabled="disabled"
              >
                <span class="material-symbols-outlined" data-icon="chevron_left"
                  >chevron_left</span
                >
              </button>
              <div class="flex items-center gap-1">
                <span
                  class="px-3 py-1 bg-secondary text-on-secondary text-xs font-bold"
                  >1</span
                >
                <span
                  class="px-3 py-1 text-on-surface-variant text-xs font-medium hover:text-on-surface cursor-pointer"
                  >2</span
                >
                <span
                  class="px-3 py-1 text-on-surface-variant text-xs font-medium hover:text-on-surface cursor-pointer"
                  >3</span
                >
                <span
                  class="px-3 py-1 text-on-surface-variant text-xs font-medium"
                  >...</span
                >
                <span
                  class="px-3 py-1 text-on-surface-variant text-xs font-medium hover:text-on-surface cursor-pointer"
                  >250</span
                >
              </div>
              <button
                type="button"
                class="p-2 text-on-surface-variant hover:text-secondary"
              >
                <span
                  class="material-symbols-outlined"
                  data-icon="chevron_right"
                  >chevron_right</span
                >
              </button>
            </div>
          </div>
        </section>
      </div>
    </main>

    <!-- Contextual FAB (Only for specific view) -->
    <div class="fixed bottom-8 right-8 z-50">
      <button
        type="button"
        class="h-14 w-14 rounded-full bg-secondary-container text-on-secondary shadow-2xl flex items-center justify-center hover:scale-110 transition-transform"
      >
        <span class="material-symbols-outlined text-2xl" data-icon="help"
          >help</span
        >
      </button>
    </div>
  </div>
</asp:Content>
