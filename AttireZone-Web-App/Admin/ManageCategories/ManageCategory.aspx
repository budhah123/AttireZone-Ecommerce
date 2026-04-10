<%@ Page Title="AttireZone | Manage Categories" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="ManageCategory.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageCategories.ManageCategory" %>

<asp:Content
  ID="ManageCategoriesStyles"
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

    .az-manage-categories button,
    .az-manage-categories input,
    .az-manage-categories select {
      border-radius: 0 !important;
    }

    .az-manage-categories .no-scrollbar::-webkit-scrollbar {
      display: none;
    }

    .az-snackbar {
      opacity: 0;
      transform: translateY(10px);
      transition:
        opacity 220ms ease,
        transform 220ms ease;
    }

    .az-snackbar.is-visible {
      opacity: 1;
      transform: translateY(0);
    }

    .az-manage-categories .az-confirm-modal {
      backdrop-filter: blur(4px);
    }

    .az-manage-categories .az-confirm-panel {
      box-shadow: 0 24px 64px rgba(0, 0, 0, 0.35);
    }
  </style>
</asp:Content>

<asp:Content
  ID="ManageCategoriesMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <div
    class="az-manage-categories bg-background text-on-background selection:bg-secondary selection:text-on-secondary overflow-hidden"
  >
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
            ID="txtCategorySearch"
            runat="server"
            AutoPostBack="true"
            OnTextChanged="txtCategorySearch_TextChanged"
            class="bg-transparent border-none focus:ring-0 text-sm w-64 placeholder:text-outline text-on-surface"
            placeholder="Search categories..."
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
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
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
          class="flex items-center gap-4 px-4 py-3 text-[#e9c349] border-r-2 border-[#e9c349] bg-[#1f1f1f] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/ManageCategories/ManageCategory.aspx"
        >
          <span
            class="material-symbols-outlined text-[20px]"
            data-icon="category"
            >category</span
          >
          Manage Categories
        </a>
        <a
          class="flex items-center gap-4 px-4 py-3 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e9c349] transition-all duration-200 translate-x-1 transition-transform font-['Inter'] text-sm font-medium uppercase tracking-[0.1em]"
          href="/Admin/ProcessOrders/ProcessOrders.aspx"
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

    <main
      class="ml-64 pt-16 h-screen overflow-y-auto no-scrollbar bg-surface-dim"
    >
      <div class="p-8 max-w-7xl mx-auto">
        <header
          class="flex flex-col md:flex-row md:items-end justify-between mb-12 gap-6"
        >
          <div>
            <span
              class="text-secondary font-label text-xs font-bold uppercase tracking-[0.2em] mb-2 block"
              >Taxonomy Management</span
            >
            <h1
              class="text-4xl font-headline font-bold text-on-surface tracking-tight"
            >
              Manage Categories
            </h1>
          </div>

          <div class="flex items-center gap-4">
            <a
              href="/Admin/ManageCategories/addcategorymodal.aspx"
              class="bg-secondary text-on-secondary px-8 py-2.5 font-bold uppercase text-xs tracking-widest flex items-center gap-2 hover:bg-secondary-container transition-colors shadow-lg shadow-secondary/5"
            >
              <span
                class="material-symbols-outlined text-sm"
                data-icon="add"
                style="font-variation-settings: 'wght' 600;"
                >add</span
              >
              Add Category
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

        <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-12">
          <div class="bg-surface-container p-6 border-l border-secondary/40">
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Total Categories
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litTotalCategories" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Categories In Use
            </p>
            <p class="text-2xl font-bold text-secondary">
              <asp:Literal ID="litCategoriesInUse" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Unused Categories
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litUnusedCategories" runat="server" />
            </p>
          </div>
          <div
            class="bg-surface-container p-6 border-l border-outline-variant/20"
          >
            <p
              class="text-on-surface-variant text-xs uppercase tracking-widest mb-1"
            >
              Created This Month
            </p>
            <p class="text-2xl font-bold text-on-surface">
              <asp:Literal ID="litCreatedThisMonth" runat="server" />
            </p>
          </div>
        </div>

        <section class="bg-surface-container-lowest overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr
                  class="border-b border-outline-variant/10 text-on-surface-variant uppercase text-[10px] font-bold tracking-[0.2em]"
                >
                  <th class="px-6 py-5 font-bold">Category Name</th>
                  <th class="px-6 py-5 font-bold">Description</th>
                  <th class="px-6 py-5 font-bold">Created On</th>
                  <th class="px-6 py-5 font-bold text-center">Products</th>
                  <th class="px-6 py-5 font-bold text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-outline-variant/5">
                <asp:Repeater
                  ID="rptCategories"
                  runat="server"
                  OnItemCommand="rptCategories_ItemCommand"
                >
                  <ItemTemplate>
                    <tr
                      class="group hover:bg-surface-container-low transition-colors duration-200"
                    >
                      <td class="px-6 py-5">
                        <p class="text-on-surface font-semibold text-sm">
                          <%#: Eval("Name") %>
                        </p>
                        <p class="text-on-surface-variant text-xs mt-1">
                          ID: <%#: Eval("Id") %>
                        </p>
                      </td>
                      <td class="px-6 py-5 text-on-surface-variant text-sm">
                        <%#: Eval("DescriptionPreview") %>
                      </td>
                      <td class="px-6 py-5 text-on-surface text-sm">
                        <%#: Eval("CreatedDateDisplay") %>
                      </td>
                      <td class="px-6 py-5 text-center">
                        <span
                          class='<%#: Eval("ProductCountBadgeCssClass") %>'
                          ><%#: Eval("ProductCount") %></span
                        >
                      </td>
                      <td class="px-6 py-5 text-right">
                        <div
                          class="flex items-center justify-end gap-3 opacity-0 group-hover:opacity-100 transition-opacity"
                        >
                          <asp:LinkButton
                            ID="btnEditCategory"
                            runat="server"
                            CommandName="EditCategory"
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
                            ID="btnDeleteCategory"
                            runat="server"
                            CommandName="DeleteCategory"
                            CommandArgument='<%#: Eval("Id") %>'
                            CausesValidation="false"
                            OnClientClick='<%# "return showDeleteCategoryDialog(" + Eval("Id") + ");" %>'
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
            No categories found.
          </asp:Panel>

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
              categories
            </p>
            <span
              class="text-on-surface-variant text-xs uppercase tracking-widest"
              >Live category index</span
            >
          </div>
        </section>
      </div>
    </main>

    <asp:Panel
      ID="pnlSnackbar"
      runat="server"
      Visible="false"
      CssClass="az-snackbar fixed bottom-6 right-6 z-[70] border border-secondary/30 bg-surface-container px-4 py-3 text-xs uppercase tracking-widest text-secondary shadow-xl"
    >
      <asp:Literal ID="litSnackbarMessage" runat="server" />
    </asp:Panel>

    <asp:HiddenField ID="hfDeleteCategoryId" runat="server" />
    <asp:Button
      ID="btnDeleteCategoryConfirmed"
      runat="server"
      OnClick="btnDeleteCategoryConfirmed_Click"
      CausesValidation="false"
      UseSubmitBehavior="false"
      Style="display: none"
    />

    <div
      id="categoryDeleteDialog"
      class="az-confirm-modal fixed inset-0 z-[80] bg-black/70 hidden items-center justify-center p-4"
      role="dialog"
      aria-modal="true"
      aria-labelledby="categoryDeleteDialogTitle"
      onclick="
        if (event.target === this) {
          closeDeleteCategoryDialog();
        }
      "
    >
      <div
        class="az-confirm-panel w-full max-w-md border border-outline-variant/20 bg-surface-container px-6 py-6"
      >
        <h2
          id="categoryDeleteDialogTitle"
          class="text-lg font-bold tracking-tight text-on-surface"
        >
          Delete Category
        </h2>
        <p class="mt-3 text-sm text-on-surface-variant leading-relaxed">
          Are you sure you want to delete this item? This action cannot be
          undone.
        </p>
        <div class="mt-6 flex items-center justify-end gap-3">
          <button
            type="button"
            onclick="closeDeleteCategoryDialog()"
            class="px-4 py-2 border border-outline-variant/30 text-on-surface-variant text-xs font-bold uppercase tracking-widest hover:text-on-surface hover:border-on-surface/40 transition-colors"
          >
            Cancel
          </button>
          <button
            type="button"
            onclick="confirmDeleteCategory()"
            class="px-5 py-2 bg-error text-on-error text-xs font-bold uppercase tracking-widest hover:brightness-110 transition-all"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>

  <script type="text/javascript">
    (function () {
      var snackbar = document.getElementById("<%= pnlSnackbar.ClientID %>");
      if (!snackbar) {
        return;
      }

      window.setTimeout(function () {
        snackbar.classList.add("is-visible");
      }, 30);

      window.setTimeout(function () {
        snackbar.classList.remove("is-visible");
      }, 3200);
    })();

    (function () {
      var dialog = document.getElementById("categoryDeleteDialog");
      var hiddenId = document.getElementById(
        "<%= hfDeleteCategoryId.ClientID %>",
      );
      var confirmButton = document.getElementById(
        "<%= btnDeleteCategoryConfirmed.ClientID %>",
      );

      window.showDeleteCategoryDialog = function (categoryId) {
        if (!dialog || !hiddenId) {
          return false;
        }

        hiddenId.value = categoryId;
        dialog.classList.remove("hidden");
        dialog.classList.add("flex");
        return false;
      };

      window.closeDeleteCategoryDialog = function () {
        if (!dialog || !hiddenId) {
          return;
        }

        hiddenId.value = "";
        dialog.classList.add("hidden");
        dialog.classList.remove("flex");
      };

      window.confirmDeleteCategory = function () {
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
          closeDeleteCategoryDialog();
        }
      });
    })();
  </script>
</asp:Content>
