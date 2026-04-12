<%@ Page Title="AttireZone | User Management" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="ManageUser.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageUser.ManageUser" %>

<asp:Content
  ID="ManageUserStyles"
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

    .az-manage-user button,
    .az-manage-user input,
    .az-manage-user select {
      border-radius: 0 !important;
    }

    .az-manage-user .hide-scrollbar::-webkit-scrollbar {
      display: none;
    }

    .az-manage-user .hide-scrollbar {
      -ms-overflow-style: none;
      scrollbar-width: none;
    }

    .az-manage-user .az-confirm-modal {
      backdrop-filter: blur(4px);
    }

    .az-manage-user .az-confirm-panel {
      box-shadow: 0 24px 64px rgba(0, 0, 0, 0.35);
    }

    .az-manage-user {
      overflow-x: hidden;
    }

    .az-manage-user .az-break-anywhere {
      overflow-wrap: anywhere;
      word-break: break-word;
    }
  </style>
</asp:Content>

<asp:Content
  ID="ManageUserMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <div
    class="az-manage-user bg-background text-on-background selection:bg-secondary selection:text-on-secondary"
  >
    <nav
      class="fixed top-0 w-full z-50 bg-[#001f3f]/80 dark:bg-[#0e0e0e]/80 backdrop-blur-md shadow-[0_0_40px_rgba(0,0,0,0.08)] flex justify-between items-center px-4 sm:px-6 h-16"
    >
      <div class="flex items-center gap-4 sm:gap-8">
        <span
          class="text-xl font-bold tracking-tighter text-[#e2e2e2] uppercase font-['Inter']"
          >AttireZone</span
        >
        <div
          class="hidden md:flex items-center bg-surface-container-low px-4 py-1.5 gap-3 group border-b border-outline-variant/20 focus-within:border-secondary transition-all"
        >
          <span
            class="material-symbols-outlined text-on-surface-variant text-sm"
            data-icon="search"
            >search</span
          >
          <asp:TextBox
            ID="txtUserSearch"
            runat="server"
            AutoPostBack="true"
            OnTextChanged="txtUserSearch_TextChanged"
            class="bg-transparent border-none focus:ring-0 text-sm w-64 placeholder:text-on-surface-variant/50 text-on-surface"
            placeholder="Search users, roles or activity..."
          ></asp:TextBox>
        </div>
      </div>
      <div class="flex items-center gap-4">
        <button
          type="button"
          class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 flex items-center justify-center p-2"
        >
          <span class="material-symbols-outlined" data-icon="settings"
            >settings</span
          >
        </button>
        <button
          type="button"
          class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300 flex items-center justify-center p-2"
        >
          <span class="material-symbols-outlined" data-icon="account_circle"
            >account_circle</span
          >
        </button>
      </div>
    </nav>

    <aside
      class="hidden lg:flex fixed left-0 top-0 h-screen w-64 z-40 bg-[#131313] dark:bg-[#131313] flex-col pt-20 pb-6 px-0 border-r border-outline-variant/10"
    >
      <div class="px-6 mb-8">
        <h2 class="text-lg font-semibold text-[#e2e2e2] font-['Inter']">
          Admin Console
        </h2>
        <p
          class="text-[10px] text-on-surface-variant uppercase tracking-[0.2em] mt-1"
        >
          AttireZone Portal
        </p>
      </div>

      <nav class="flex-1 flex flex-col">
        <a
          class="flex items-center gap-4 px-6 py-4 text-[#c4c6cf] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 group"
          href="/Admin/Dashboard.aspx"
        >
          <span
            class="material-symbols-outlined group-hover:scale-110 transition-transform"
            data-icon="dashboard"
            >dashboard</span
          >
          <span>Dashboard</span>
        </a>
        <a
          class="flex items-center gap-4 px-6 py-4 text-[#c4c6cf] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 group"
          href="/Admin/ManageProduct/ManageProducts.aspx"
        >
          <span
            class="material-symbols-outlined group-hover:scale-110 transition-transform"
            data-icon="inventory_2"
            >inventory_2</span
          >
          <span>Manage Products</span>
        </a>
        <a
          class="flex items-center gap-4 px-6 py-4 text-[#c4c6cf] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 group"
          href="/Admin/ManageCategories/ManageCategory.aspx"
        >
          <span
            class="material-symbols-outlined group-hover:scale-110 transition-transform"
            data-icon="category"
            >category</span
          >
          <span>Manage Categories</span>
        </a>
        <a
          class="flex items-center gap-4 px-6 py-4 text-[#c4c6cf] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 group"
          href="/Admin/ProcessOrders/ProcessOrders.aspx"
        >
          <span
            class="material-symbols-outlined group-hover:scale-110 transition-transform"
            data-icon="shopping_bag"
            >shopping_bag</span
          >
          <span>Process Orders</span>
        </a>
        <a
          class="flex items-center gap-4 px-6 py-4 text-[#e9c349] border-r-2 border-[#e9c349] bg-[#1f1f1f] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] transition-all duration-200 group"
          href="/Admin/ManageUser/ManageUser.aspx"
        >
          <span
            class="material-symbols-outlined translate-x-1"
            data-icon="group"
            >group</span
          >
          <span>User Management</span>
        </a>
      </nav>

      <div class="px-4 mt-auto">
        <a
          class="w-full flex items-center gap-4 px-6 py-4 text-[#c4c6cf] font-['Inter'] text-sm font-medium uppercase tracking-[0.1em] hover:bg-error-container/10 hover:text-error transition-all duration-200"
          href="/Admin/AdminLogin.aspx"
        >
          <span class="material-symbols-outlined" data-icon="logout"
            >logout</span
          >
          <span>Logout</span>
        </a>
      </div>
    </aside>

    <main
      class="ml-0 lg:ml-64 pt-20 lg:pt-24 pb-10 sm:pb-12 px-4 sm:px-6 lg:px-8 min-h-screen bg-surface-dim hide-scrollbar"
    >
      <header
        class="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-12"
      >
        <div>
          <span
            class="text-secondary font-label text-[11px] font-bold tracking-[0.2em] uppercase"
            >Security &amp; Permissions</span
          >
          <h1
            class="text-3xl sm:text-4xl font-headline font-bold text-on-background mt-2 tracking-tight"
          >
            System Access Control
          </h1>
          <p class="text-on-surface-variant mt-3 max-w-xl leading-relaxed">
            Oversee platform access, update administrative privileges, and
            manage user status across the AttireZone ecosystem.
          </p>
        </div>
        <div class="flex w-full sm:w-auto items-center gap-4">
          <a
            href="/Admin/ManageUser/AddUserModal.aspx"
            class="w-full sm:w-auto justify-center bg-secondary text-on-secondary px-8 py-3 font-label text-xs font-bold uppercase tracking-widest hover:brightness-110 transition-all flex items-center gap-2"
          >
            <span
              class="material-symbols-outlined text-sm"
              data-icon="person_add"
              >person_add</span
            >
            Invite User
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

      <div
        class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4 sm:gap-px bg-transparent sm:bg-outline-variant/10 mb-10 sm:mb-12"
      >
        <div class="bg-surface-container p-6">
          <p
            class="text-[10px] uppercase tracking-widest text-on-surface-variant mb-1"
          >
            Total Users
          </p>
          <p class="text-3xl font-headline font-semibold text-on-surface">
            <asp:Literal ID="litTotalUsers" runat="server" />
          </p>
        </div>
        <div class="bg-surface-container p-6">
          <p
            class="text-[10px] uppercase tracking-widest text-on-surface-variant mb-1"
          >
            Active Now
          </p>
          <div class="flex items-baseline gap-2">
            <p class="text-3xl font-headline font-semibold text-on-surface">
              <asp:Literal ID="litActiveNow" runat="server" />
            </p>
            <span class="text-[10px] text-green-500 font-bold">recent 24h</span>
          </div>
        </div>
        <div class="bg-surface-container p-6">
          <p
            class="text-[10px] uppercase tracking-widest text-on-surface-variant mb-1"
          >
            Administrators
          </p>
          <p class="text-3xl font-headline font-semibold text-on-surface">
            <asp:Literal ID="litAdmins" runat="server" />
          </p>
        </div>
        <div class="bg-surface-container p-6">
          <p
            class="text-[10px] uppercase tracking-widest text-on-surface-variant mb-1"
          >
            Pending Invitations
          </p>
          <p class="text-3xl font-headline font-semibold text-on-surface">
            <asp:Literal ID="litPendingInvitations" runat="server" />
          </p>
        </div>
      </div>

      <div
        class="flex flex-col lg:flex-row lg:items-center justify-between gap-4 mb-6"
      >
        <div class="flex flex-wrap items-center gap-4 sm:gap-6">
          <div class="relative group">
            <asp:DropDownList
              ID="ddlRoleFilter"
              runat="server"
              AutoPostBack="true"
              OnSelectedIndexChanged="ddlRoleFilter_SelectedIndexChanged"
              CssClass="appearance-none bg-transparent pr-6 text-xs font-bold uppercase tracking-widest text-on-surface hover:text-secondary transition-colors focus:outline-none"
            >
              <asp:ListItem Value="">Role: All Roles</asp:ListItem>
              <asp:ListItem Value="Admin">Role: Admin</asp:ListItem>
              <asp:ListItem Value="Customer">Role: Customer</asp:ListItem>
            </asp:DropDownList>
            <span
              class="material-symbols-outlined text-sm absolute right-0 top-1/2 -translate-y-1/2 pointer-events-none"
              data-icon="expand_more"
              >expand_more</span
            >
          </div>
          <div class="relative group">
            <asp:DropDownList
              ID="ddlStatusFilter"
              runat="server"
              AutoPostBack="true"
              OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged"
              CssClass="appearance-none bg-transparent pr-6 text-xs font-bold uppercase tracking-widest text-on-surface hover:text-secondary transition-colors focus:outline-none"
            >
              <asp:ListItem Value="">Status: All</asp:ListItem>
              <asp:ListItem Value="Active">Status: Active</asp:ListItem>
              <asp:ListItem Value="Inactive">Status: Inactive</asp:ListItem>
            </asp:DropDownList>
            <span
              class="material-symbols-outlined text-sm absolute right-0 top-1/2 -translate-y-1/2 pointer-events-none"
              data-icon="expand_more"
              >expand_more</span
            >
          </div>
        </div>
        <p class="text-xs text-on-surface-variant font-medium">
          Showing
          <span class="text-on-surface"
            ><asp:Literal ID="litShown" runat="server"
          /></span>
          of
          <span class="text-on-surface"
            ><asp:Literal ID="litTotal" runat="server"
          /></span>
          users
        </p>
      </div>

      <div class="bg-surface-container overflow-hidden">
        <div class="overflow-x-auto">
          <table class="w-full text-left border-collapse">
            <thead>
              <tr class="border-b border-outline-variant/10">
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  UserId
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  FullName
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  Email
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  CreatedDate
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  LastModifiedDate
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant"
                >
                  Role
                </th>
                <th
                  class="px-6 py-4 text-[10px] font-bold uppercase tracking-widest text-on-surface-variant text-right"
                >
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-outline-variant/5">
              <asp:Repeater
                ID="rptUsers"
                runat="server"
                OnItemCommand="rptUsers_ItemCommand"
              >
                <ItemTemplate>
                  <tr
                    class="hover:bg-surface-container-high transition-colors group"
                  >
                    <td class="px-6 py-5 text-xs text-on-surface">
                      <%#: Eval("UserId") %>
                    </td>
                    <td class="px-6 py-5 text-sm font-semibold text-on-surface">
                      <%#: Eval("FullName") %>
                    </td>
                    <td class="px-6 py-5 text-xs text-on-surface-variant">
                      <%#: Eval("Email") %>
                    </td>
                    <td class="px-6 py-5 text-xs text-on-surface-variant">
                      <%#: Eval("CreatedDateDisplay") %>
                    </td>
                    <td class="px-6 py-5 text-xs text-on-surface-variant">
                      <%#: Eval("LastModifiedDateDisplay") %>
                    </td>
                  <td class="px-6 py-5">
                    <span class='<%#: Eval("RoleBadgeCssClass") %>'
                      ><%#: Eval("Role") %></span
                    >
                  </td>
                    <td class="px-6 py-5 text-right">
                      <div
                        class="flex items-center justify-end gap-3 opacity-100 lg:opacity-0 lg:group-hover:opacity-100 transition-opacity"
                      >
                        <asp:LinkButton
                          ID="btnEditUser"
                          runat="server"
                          CommandName="EditUser"
                          CommandArgument='<%#: Eval("UserId") %>'
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
                          ID="btnDeleteUser"
                          runat="server"
                          CommandName="DeleteUser"
                          CommandArgument='<%#: Eval("UserId") %>'
                          CausesValidation="false"
                          data-user-id='<%#: Eval("UserId") %>'
                          OnClientClick="return showDeleteUserDialog(this.getAttribute('data-user-id'));"
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

        <asp:Panel
          ID="pnlEmptyState"
          runat="server"
          Visible="false"
          CssClass="px-6 py-6 border-t border-outline-variant/10 text-xs uppercase tracking-widest text-on-surface-variant"
        >
          No users found.
        </asp:Panel>

        <div
          class="px-4 sm:px-6 py-4 sm:py-6 border-t border-outline-variant/10 flex flex-col sm:flex-row items-start sm:items-center gap-3 sm:gap-0 justify-between"
        >
          <button
            type="button"
            class="text-xs font-bold uppercase tracking-widest text-on-surface-variant hover:text-secondary flex items-center gap-1 transition-colors"
            disabled="disabled"
          >
            <span
              class="material-symbols-outlined text-sm"
              data-icon="arrow_back"
              >arrow_back</span
            >
            Previous
          </button>
          <div class="flex items-center gap-4">
            <button
              type="button"
              class="w-8 h-8 flex items-center justify-center bg-secondary text-on-secondary text-xs font-bold"
            >
              1
            </button>
          </div>
          <button
            type="button"
            class="text-xs font-bold uppercase tracking-widest text-on-surface-variant hover:text-secondary flex items-center gap-1 transition-colors"
            disabled="disabled"
          >
            Next
            <span
              class="material-symbols-outlined text-sm"
              data-icon="arrow_forward"
              >arrow_forward</span
            >
          </button>
        </div>
      </div>

      <section class="mt-12">
        <h3
          class="text-xs font-bold uppercase tracking-[0.3em] text-on-surface-variant mb-6 flex items-center gap-3"
        >
          <span class="h-px w-8 bg-secondary/30"></span>
          Security Event Log
        </h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div
            class="bg-surface-container-low p-6 border-l-2 border-secondary/20"
          >
            <div class="flex justify-between items-start mb-4">
              <div class="flex items-center gap-3">
                <span
                  class="material-symbols-outlined text-secondary"
                  data-icon="history"
                  >history</span
                >
                <p class="text-sm font-bold text-on-surface">
                  Recent Role Modification
                </p>
              </div>
              <span class="text-[10px] text-on-surface-variant"
                >System event</span
              >
            </div>
            <p class="text-xs text-on-surface-variant leading-relaxed">
              User administration actions are tracked in the application audit
              trail.
            </p>
          </div>
          <div class="bg-surface-container-low p-6 border-l-2 border-error/20">
            <div class="flex justify-between items-start mb-4">
              <div class="flex items-center gap-3">
                <span
                  class="material-symbols-outlined text-error"
                  data-icon="warning"
                  >warning</span
                >
                <p class="text-sm font-bold text-on-surface">Access Alerts</p>
              </div>
              <span class="text-[10px] text-on-surface-variant"
                >Live monitor</span
              >
            </div>
            <p class="text-xs text-on-surface-variant leading-relaxed">
              Unauthorized attempts and suspicious access patterns are monitored
              continuously.
            </p>
          </div>
        </div>
      </section>

      <asp:HiddenField ID="hfDeleteUserId" runat="server" />
      <asp:Button
        ID="btnDeleteUserConfirmed"
        runat="server"
        OnClick="btnDeleteUserConfirmed_Click"
        CausesValidation="false"
        UseSubmitBehavior="false"
        Style="display: none"
      />

      <div
        id="userDeleteDialog"
        class="az-confirm-modal fixed inset-0 z-[80] bg-black/70 hidden items-center justify-center p-4"
        role="dialog"
        aria-modal="true"
        aria-labelledby="userDeleteDialogTitle"
        onclick="
          if (event.target === this) {
            closeDeleteUserDialog();
          }
        "
      >
        <div
          class="az-confirm-panel w-full max-w-md border border-outline-variant/20 bg-surface-container px-6 py-6"
        >
          <h2
            id="userDeleteDialogTitle"
            class="text-lg font-bold tracking-tight text-on-surface"
          >
            Delete User
          </h2>
          <p class="mt-3 text-sm text-on-surface-variant leading-relaxed">
            Are you sure you want to delete this item? This action cannot be
            undone.
          </p>
          <div class="mt-6 flex items-center justify-end gap-3">
            <button
              type="button"
              onclick="closeDeleteUserDialog()"
              class="px-4 py-2 border border-outline-variant/30 text-on-surface-variant text-xs font-bold uppercase tracking-widest hover:text-on-surface hover:border-on-surface/40 transition-colors"
            >
              Cancel
            </button>
            <button
              type="button"
              onclick="confirmDeleteUser()"
              class="px-5 py-2 bg-error text-on-error text-xs font-bold uppercase tracking-widest hover:brightness-110 transition-all"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>

  <script type="text/javascript">
    (function () {
      var dialog = document.getElementById("userDeleteDialog");
      var hiddenId = document.getElementById("<%= hfDeleteUserId.ClientID %>");
      var confirmButton = document.getElementById(
        "<%= btnDeleteUserConfirmed.ClientID %>",
      );

      window.showDeleteUserDialog = function (userId) {
        if (!dialog || !hiddenId) {
          return false;
        }

        hiddenId.value = userId;
        dialog.classList.remove("hidden");
        dialog.classList.add("flex");
        return false;
      };

      window.closeDeleteUserDialog = function () {
        if (!dialog || !hiddenId) {
          return;
        }

        hiddenId.value = "";
        dialog.classList.add("hidden");
        dialog.classList.remove("flex");
      };

      window.confirmDeleteUser = function () {
        if (!confirmButton) {
          return;
        }

        confirmButton.click();
      };

      document.addEventListener("keydown", function (event) {
        if (
          event.key === "Escape" &&
          dialog &&
          !dialog.classList.contains("hidden")
        ) {
          closeDeleteUserDialog();
        }
      });
    })();
  </script>
</asp:Content>
