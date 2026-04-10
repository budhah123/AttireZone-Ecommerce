<%@ Page Title="AttireZone | Category Modal" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="AddCategoryModal.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageCategories.AddCategoryModal" %>

<asp:Content
  ID="AddCategoryModalStyles"
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

    .az-category-modal input,
    .az-category-modal textarea,
    .az-category-modal button {
      border-radius: 0 !important;
    }
  </style>
</asp:Content>

<asp:Content
  ID="AddCategoryModalMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <div
    class="az-category-modal min-h-screen bg-background text-on-background relative overflow-hidden"
  >
    <div
      class="absolute inset-0 bg-gradient-to-br from-surface to-surface-container-low"
    ></div>
    <div
      class="relative z-10 min-h-screen flex items-center justify-center px-4 py-10"
    >
      <div
        class="w-full max-w-3xl border border-outline-variant/20 bg-surface-container-low shadow-2xl"
      >
        <header
          class="px-6 py-5 border-b border-outline-variant/20 flex items-start justify-between gap-4"
        >
          <div>
            <p
              class="text-[10px] tracking-[0.2em] uppercase font-bold text-secondary mb-1"
            >
              Category Editor
            </p>
            <h1 class="text-2xl font-bold tracking-tight">
              <asp:Literal ID="litFormHeading" runat="server"
                >Add Category</asp:Literal
              >
            </h1>
          </div>
          <a
            href="/Admin/ManageCategories/ManageCategory.aspx"
            class="text-on-surface-variant hover:text-secondary transition-colors"
          >
            <span class="material-symbols-outlined">close</span>
          </a>
        </header>

        <div class="p-6 md:p-8">
          <asp:Panel
            ID="pnlMessage"
            runat="server"
            Visible="false"
            CssClass="mb-6 border border-error/40 bg-error-container/30 px-4 py-3 text-xs uppercase tracking-widest text-error"
          >
            <asp:Literal ID="litMessage" runat="server" />
          </asp:Panel>

          <div class="space-y-6">
            <div class="space-y-2">
              <label
                for="txtCategoryName"
                class="block text-[11px] uppercase tracking-[0.18em] font-bold text-on-surface-variant"
              >
                Category Name
              </label>
              <input
                id="txtCategoryName"
                runat="server"
                type="text"
                maxlength="150"
                class="w-full bg-surface-container border border-outline-variant/30 px-4 py-3 text-sm focus:outline-none focus:border-secondary"
                placeholder="Enter category name"
                required="required"
              />
            </div>

            <div class="space-y-2">
              <label
                for="txtCategoryDescription"
                class="block text-[11px] uppercase tracking-[0.18em] font-bold text-on-surface-variant"
              >
                Description
              </label>
              <textarea
                id="txtCategoryDescription"
                runat="server"
                rows="5"
                class="w-full bg-surface-container border border-outline-variant/30 px-4 py-3 text-sm resize-y min-h-[140px] focus:outline-none focus:border-secondary"
                placeholder="Write a short category description"
              ></textarea>
            </div>
          </div>

          <div
            class="mt-8 pt-6 border-t border-outline-variant/20 flex flex-col sm:flex-row items-stretch sm:items-center justify-end gap-3"
          >
            <a
              href="/Admin/ManageCategories/ManageCategory.aspx"
              class="px-6 py-3 border border-outline-variant/30 text-on-surface-variant text-xs font-bold uppercase tracking-widest hover:text-on-surface hover:border-on-surface/40 transition-colors text-center"
              >Cancel</a
            >
            <button
              id="btnSaveCategory"
              runat="server"
              type="submit"
              onserverclick="btnSaveCategory_ServerClick"
              class="px-8 py-3 bg-secondary text-on-secondary text-xs font-bold uppercase tracking-widest hover:bg-secondary-container transition-colors"
            >
              Save Category
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</asp:Content>
