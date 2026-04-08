<%@ Page Title="AttireZone | Process Orders" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="ProcessOrders.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ProcessOrders.ProcessOrders" %>

<asp:Content
  ID="ProcessOrdersStyles"
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

    .az-process-orders button,
    .az-process-orders input,
    .az-process-orders select {
      border-radius: 0 !important;
    }

    .az-process-orders .hide-scrollbar::-webkit-scrollbar {
      display: none;
    }

    .az-process-orders .hide-scrollbar {
      -ms-overflow-style: none;
      scrollbar-width: none;
    }
  </style>
</asp:Content>

<asp:Content
  ID="ProcessOrdersMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <div
    class="az-process-orders bg-background text-on-background selection:bg-secondary selection:text-on-secondary"
  >
    <nav
      class="fixed top-0 w-full z-50 bg-[#001f3f]/80 dark:bg-[#0e0e0e]/80 backdrop-blur-md shadow-[0_0_40px_rgba(0,0,0,0.08)]"
    >
      <div class="flex justify-between items-center px-6 h-16 w-full">
        <div class="flex items-center gap-8">
          <span
            class="text-xl font-bold tracking-tighter text-[#e2e2e2] uppercase font-['Inter']"
            >AttireZone</span
          >
          <div class="hidden md:flex items-center space-x-6">
            <a
              class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 font-['Inter'] tracking-tight text-sm uppercase"
              href="/Admin/Dashboard.aspx"
              >Dashboard</a
            >
            <a
              class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 font-['Inter'] tracking-tight text-sm uppercase"
              href="/Admin/ManageProduct/ManageProducts.aspx"
              >Catalogue</a
            >
          </div>
        </div>

        <div class="flex items-center gap-4">
          <span
            class="material-symbols-outlined text-[#c4c6cf]"
            data-icon="settings"
            >settings</span
          >
          <asp:LinkButton
            ID="btnLogoutTop"
            runat="server"
            OnClick="btnLogout_Click"
            CausesValidation="false"
            CssClass="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300"
          >
            <span class="material-symbols-outlined" data-icon="logout"
              >logout</span
            >
          </asp:LinkButton>
        </div>
      </div>
    </nav>

    <aside
      class="fixed left-0 top-0 h-screen w-64 z-40 bg-[#131313] dark:bg-[#131313] flex flex-col pt-20 pb-6 px-0 border-r border-outline-variant/10"
    >
      <div class="px-6 mb-10">
        <h2 class="text-lg font-semibold text-[#e2e2e2] font-['Inter']">
          Admin Console
        </h2>
        <p
          class="text-[10px] text-on-surface-variant uppercase tracking-[0.2em] mt-1"
        >
          AttireZone Portal
        </p>
      </div>

      <nav class="flex-1 space-y-1">
        <a
          class="flex items-center px-6 py-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] group"
          href="/Admin/Dashboard.aspx"
        >
          <span
            class="material-symbols-outlined mr-4 group-hover:translate-x-1 transition-transform"
            data-icon="dashboard"
            >dashboard</span
          >
          Dashboard
        </a>
        <a
          class="flex items-center px-6 py-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] group"
          href="/Admin/ManageProduct/ManageProducts.aspx"
        >
          <span
            class="material-symbols-outlined mr-4 group-hover:translate-x-1 transition-transform"
            data-icon="inventory_2"
            >inventory_2</span
          >
          Manage Products
        </a>
        <a
          class="flex items-center px-6 py-4 text-[#e9c349] border-r-2 border-[#e9c349] bg-[#1f1f1f] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] group"
          href="/Admin/ProcessOrders/ProcessOrders.aspx"
        >
          <span
            class="material-symbols-outlined mr-4 translate-x-1"
            data-icon="shopping_bag"
            >shopping_bag</span
          >
          Process Orders
        </a>
        <a
          class="flex items-center px-6 py-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] group"
          href="#"
        >
          <span
            class="material-symbols-outlined mr-4 group-hover:translate-x-1 transition-transform"
            data-icon="history_edu"
            >history_edu</span
          >
          Inventory Logs
        </a>
        <a
          class="flex items-center px-6 py-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] group"
          href="/Admin/ManageUser/ManageUser.aspx"
        >
          <span
            class="material-symbols-outlined mr-4 group-hover:translate-x-1 transition-transform"
            data-icon="group"
            >group</span
          >
          User Management
        </a>
      </nav>

      <div class="mt-auto px-6">
        <asp:LinkButton
          ID="btnLogoutSide"
          runat="server"
          OnClick="btnLogout_Click"
          CausesValidation="false"
          CssClass="flex items-center py-4 text-[#c4c6cf] hover:text-error transition-colors font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
        >
          <span class="material-symbols-outlined mr-4" data-icon="logout"
            >logout</span
          >
          Logout
        </asp:LinkButton>
      </div>
    </aside>

    <main class="ml-64 pt-24 min-h-screen bg-surface-dim">
      <div class="max-w-7xl mx-auto px-8 pb-12">
        <div
          class="flex flex-col md:flex-row justify-between items-end mb-12 gap-6"
        >
          <div>
            <span
              class="text-secondary font-label text-xs tracking-[0.2em] uppercase mb-2 block"
              >Order Fulfilment</span
            >
            <h1
              class="text-4xl font-headline font-semibold text-on-surface tracking-tight"
            >
              Process Orders
            </h1>
          </div>

          <div class="flex bg-surface-container-lowest p-1">
            <asp:LinkButton
              ID="btnFilterAll"
              runat="server"
              OnCommand="btnFilter_Command"
              CommandArgument="all"
              CausesValidation="false"
              CssClass="px-6 py-2 text-xs font-label uppercase tracking-widest transition-colors"
              >All</asp:LinkButton
            >
            <asp:LinkButton
              ID="btnFilterPending"
              runat="server"
              OnCommand="btnFilter_Command"
              CommandArgument="pending"
              CausesValidation="false"
              CssClass="px-6 py-2 text-xs font-label uppercase tracking-widest transition-colors"
              >Pending</asp:LinkButton
            >
            <asp:LinkButton
              ID="btnFilterShipped"
              runat="server"
              OnCommand="btnFilter_Command"
              CommandArgument="shipped"
              CausesValidation="false"
              CssClass="px-6 py-2 text-xs font-label uppercase tracking-widest transition-colors"
              >Shipped</asp:LinkButton
            >
            <asp:LinkButton
              ID="btnFilterDelivered"
              runat="server"
              OnCommand="btnFilter_Command"
              CommandArgument="delivered"
              CausesValidation="false"
              CssClass="px-6 py-2 text-xs font-label uppercase tracking-widest transition-colors"
              >Delivered</asp:LinkButton
            >
          </div>
        </div>

        <div
          class="grid grid-cols-1 md:grid-cols-4 gap-0 border border-outline-variant/10 mb-12"
        >
          <div
            class="bg-surface-container p-6 border-r border-outline-variant/10"
          >
            <p
              class="text-on-surface-variant text-[10px] uppercase tracking-widest mb-1"
            >
              Open Orders
            </p>
            <p class="text-2xl font-headline text-secondary">
              <asp:Literal ID="litOpenOrders" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-r border-outline-variant/10"
          >
            <p
              class="text-on-surface-variant text-[10px] uppercase tracking-widest mb-1"
            >
              Pending Shipment
            </p>
            <p class="text-2xl font-headline text-on-surface">
              <asp:Literal ID="litPendingShipment" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-r border-outline-variant/10"
          >
            <p
              class="text-on-surface-variant text-[10px] uppercase tracking-widest mb-1"
            >
              Processing Delay
            </p>
            <p class="text-2xl font-headline text-error">
              <asp:Literal ID="litProcessingDelay" runat="server" />
            </p>
          </div>
          <div class="bg-surface-container p-6">
            <p
              class="text-on-surface-variant text-[10px] uppercase tracking-widest mb-1"
            >
              Revenue (24h)
            </p>
            <p class="text-2xl font-headline text-secondary-fixed-dim">
              <asp:Literal ID="litRevenue24h" runat="server" />
            </p>
          </div>
        </div>

        <div class="bg-surface-container-lowest overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr
                  class="bg-surface-container-low border-b border-outline-variant/10"
                >
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Order ID
                  </th>
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Customer
                  </th>
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Date
                  </th>
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Status
                  </th>
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant text-right"
                  >
                    Total Amount
                  </th>
                  <th
                    class="px-6 py-4 text-[10px] font-label uppercase tracking-[0.2em] text-on-surface-variant text-right"
                  >
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody class="divide-y divide-outline-variant/5">
                <asp:Repeater
                  ID="rptOrders"
                  runat="server"
                  OnItemCommand="rptOrders_ItemCommand"
                  OnItemDataBound="rptOrders_ItemDataBound"
                >
                  <ItemTemplate>
                    <tr
                      class="hover:bg-surface-container transition-colors group"
                    >
                      <td
                        class="px-6 py-6 font-headline text-sm font-medium text-secondary"
                      >
                        <%# Eval("OrderNumber") %>
                      </td>
                      <td class="px-6 py-6">
                        <div class="flex items-center gap-3">
                          <div
                            class="w-8 h-8 bg-surface-container-high flex items-center justify-center text-[10px] font-bold text-on-surface-variant"
                          >
                            <%# Eval("CustomerInitials") %>
                          </div>
                          <div>
                            <p class="text-sm font-medium text-on-surface">
                              <%# Eval("CustomerName") %>
                            </p>
                            <p
                              class="text-[10px] text-on-surface-variant uppercase tracking-tighter"
                            >
                              <%# Eval("CustomerLabel") %>
                            </p>
                          </div>
                        </div>
                      </td>
                      <td class="px-6 py-6 text-sm text-on-surface-variant">
                        <%# Eval("PlacedDateLabel") %>
                      </td>
                      <td class="px-6 py-6">
                        <span
                          class='<%# Eval("StatusBadgeCssClass") %>'
                          ><%# Eval("StatusLabel") %></span
                        >
                      </td>
                      <td
                        class="px-6 py-6 text-sm font-medium text-on-surface text-right"
                      >
                        <%# Eval("TotalLabel") %>
                      </td>
                      <td class="px-6 py-6 text-right">
                        <div class="flex justify-end items-center gap-2">
                          <asp:LinkButton
                            ID="btnViewOrder"
                            runat="server"
                            CommandName="ViewOrder"
                            CommandArgument='<%# Eval("OrderId") %>'
                            CausesValidation="false"
                            CssClass="text-on-surface-variant hover:text-secondary transition-colors"
                            ToolTip="View details"
                          >
                            <span
                              class="material-symbols-outlined text-lg"
                              data-icon="visibility"
                              >visibility</span
                            >
                          </asp:LinkButton>
                          <asp:DropDownList
                            ID="ddlOrderStatus"
                            runat="server"
                            CssClass="bg-surface-container-low border border-outline-variant/20 text-on-surface-variant text-xs py-1 px-2 uppercase tracking-wide min-w-[110px]"
                          >
                            <asp:ListItem Value="Pending">Pending</asp:ListItem>
                            <asp:ListItem Value="Shipped">Shipped</asp:ListItem>
                            <asp:ListItem Value="Delivered"
                              >Delivered</asp:ListItem
                            >
                            <asp:ListItem Value="Cancelled"
                              >Cancelled</asp:ListItem
                            >
                          </asp:DropDownList>
                          <asp:HiddenField
                            ID="hfCurrentStatus"
                            runat="server"
                            Value='<%# Eval("StatusValue") %>'
                          />
                          <asp:LinkButton
                            ID="btnUpdateOrderStatus"
                            runat="server"
                            CommandName="UpdateStatus"
                            CommandArgument='<%# Eval("OrderId") %>'
                            CausesValidation="false"
                            CssClass="text-on-surface-variant hover:text-secondary transition-colors"
                            ToolTip="Update status"
                          >
                            <span
                              class="material-symbols-outlined text-lg"
                              data-icon="edit_square"
                              >edit_square</span
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
        </div>

        <asp:Panel
          ID="pnlOrderDetails"
          runat="server"
          Visible="false"
          CssClass="mt-8 bg-surface-container-lowest border border-outline-variant/10 p-6"
        >
          <div class="flex items-start justify-between gap-4">
            <div>
              <span
                class="text-secondary text-[10px] uppercase tracking-[0.2em]"
                >Selected Order</span
              >
              <h2 class="text-2xl font-headline mt-2 text-on-surface">
                <asp:Literal ID="litSelectedOrderNumber" runat="server" />
              </h2>
            </div>
            <asp:LinkButton
              ID="btnCloseDetails"
              runat="server"
              OnClick="btnCloseDetails_Click"
              CausesValidation="false"
              CssClass="text-on-surface-variant hover:text-secondary transition-colors"
              ToolTip="Close details"
            >
              <span class="material-symbols-outlined">close</span>
            </asp:LinkButton>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
            <div class="space-y-2 text-sm text-on-surface-variant">
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Customer</span
                ><asp:Literal ID="litSelectedCustomer" runat="server" />
              </p>
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Placed</span
                ><asp:Literal ID="litSelectedPlacedDate" runat="server" />
              </p>
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Status</span
                ><asp:Literal ID="litSelectedStatus" runat="server" />
              </p>
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Payment</span
                ><asp:Literal ID="litSelectedPaymentStatus" runat="server" />
              </p>
            </div>
            <div class="space-y-2 text-sm text-on-surface-variant">
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Address</span
                ><asp:Literal ID="litSelectedAddress" runat="server" />
              </p>
              <p>
                <span
                  class="uppercase tracking-widest text-[10px] text-secondary mr-2"
                  >Notes</span
                ><asp:Literal ID="litSelectedNotes" runat="server" />
              </p>
            </div>
          </div>

          <div class="mt-8 overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr
                  class="bg-surface-container-low border-b border-outline-variant/10"
                >
                  <th
                    class="px-4 py-3 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Item
                  </th>
                  <th
                    class="px-4 py-3 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant"
                  >
                    Size
                  </th>
                  <th
                    class="px-4 py-3 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant text-right"
                  >
                    Qty
                  </th>
                  <th
                    class="px-4 py-3 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant text-right"
                  >
                    Unit
                  </th>
                  <th
                    class="px-4 py-3 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant text-right"
                  >
                    Line Total
                  </th>
                </tr>
              </thead>
              <tbody class="divide-y divide-outline-variant/5">
                <asp:Repeater ID="rptOrderItems" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td class="px-4 py-3 text-sm text-on-surface">
                        <%# Eval("ProductName") %>
                      </td>
                      <td
                        class="px-4 py-3 text-xs uppercase tracking-wider text-on-surface-variant"
                      >
                        <%# Eval("SelectedSize") %>
                      </td>
                      <td class="px-4 py-3 text-sm text-on-surface text-right">
                        <%# Eval("Quantity") %>
                      </td>
                      <td class="px-4 py-3 text-sm text-on-surface text-right">
                        <%# Eval("UnitPriceLabel") %>
                      </td>
                      <td class="px-4 py-3 text-sm text-on-surface text-right">
                        <%# Eval("LineTotalLabel") %>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </asp:Panel>

        <div
          class="mt-8 flex items-center justify-between border-t border-outline-variant/10 pt-6"
        >
          <p
            class="text-xs text-on-surface-variant font-label uppercase tracking-widest"
          >
            Showing
            <asp:Literal ID="litShowingFrom" runat="server" />
            to
            <asp:Literal ID="litShowingTo" runat="server" />
            of
            <asp:Literal ID="litShowingTotal" runat="server" />
            entries
          </p>
          <div class="flex gap-1 items-center">
            <asp:LinkButton
              ID="btnPrevPage"
              runat="server"
              OnClick="btnPrevPage_Click"
              CausesValidation="false"
              CssClass="p-2 border border-outline-variant/20 hover:border-secondary text-on-surface-variant hover:text-secondary transition-all"
            >
              <span
                class="material-symbols-outlined text-sm"
                data-icon="chevron_left"
                >chevron_left</span
              >
            </asp:LinkButton>
            <span
              class="w-10 h-10 border border-secondary bg-secondary text-on-secondary text-xs font-bold inline-flex items-center justify-center"
            >
              <asp:Literal ID="litCurrentPage" runat="server" />
            </span>
            <span class="text-xs text-on-surface-variant px-2">
              / <asp:Literal ID="litTotalPages" runat="server" />
            </span>
            <asp:LinkButton
              ID="btnNextPage"
              runat="server"
              OnClick="btnNextPage_Click"
              CausesValidation="false"
              CssClass="p-2 border border-outline-variant/20 hover:border-secondary text-on-surface-variant hover:text-secondary transition-all"
            >
              <span
                class="material-symbols-outlined text-sm"
                data-icon="chevron_right"
                >chevron_right</span
              >
            </asp:LinkButton>
          </div>
        </div>
      </div>
    </main>
  </div>
</asp:Content>
