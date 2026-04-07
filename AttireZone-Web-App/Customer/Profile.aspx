<%@ Page Title="Profile | AttireZone" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="Profile.aspx.cs" Inherits="AttireZone_Web_App.Customer.Profile" %>

<asp:Content
  ID="ProfileStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <link
    href="/Assets/CSS/authentication.css"
    rel="stylesheet"
    type="text/css"
  />
  <style type="text/css">
    .az-navbar {
      z-index: 5000 !important;
    }

    .az-navbar #az-navbar-cart-link {
      z-index: 6000 !important;
      pointer-events: auto !important;
    }

    .az-profile-shell {
      /*min-height: calc(100vh - 220px);*/
    }

    .az-profile-input:focus {
      outline: none;
      border-bottom-color: #e9c349 !important;
      box-shadow: none !important;
    }

    @media (max-width: 1024px) {
      .az-profile-shell {
        min-height: calc(100vh - 180px);
      }
    }

    @media (max-width: 768px) {
      .az-profile-shell {
        min-height: auto;
      }
    }
  </style>
</asp:Content>

<asp:Content ID="ProfileMain" ContentPlaceHolderID="MainContent" runat="server">
  <main class="az-profile-shell pt-12 pb-24 px-6 max-w-7xl mx-auto">
    <header class="mb-20">
      <h1
        class="text-5xl md:text-7xl font-bold tracking-tighter mb-4 text-on-background uppercase"
      >
        Account <span class="text-secondary">Profile</span>
      </h1>
      <p class="text-on-surface-variant max-w-md text-lg leading-relaxed">
        Manage your personal details and track your sartorial journey with
        AttireZone.
      </p>
    </header>

    <div class="grid grid-cols-1 lg:grid-cols-12 gap-12">
      <section class="lg:col-span-8 space-y-16">
        <div
          class="bg-surface-container p-8 md:p-12 shadow-2xl relative overflow-hidden"
        >
          <div
            class="absolute top-0 right-0 w-32 h-32 bg-secondary/5 -mr-16 -mt-16 rotate-45"
          ></div>
          <h2
            class="text-sm font-bold tracking-[0.2em] uppercase text-secondary mb-12"
          >
            Personal Information
          </h2>
          <div class="space-y-12">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-12">
              <div class="group relative">
                <label
                  class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2 group-focus-within:text-secondary transition-colors"
                  >Full Name</label
                >
                <asp:TextBox
                  ID="txtFullName"
                  runat="server"
                  CssClass="az-profile-input w-full bg-transparent border-0 border-b border-outline-variant py-3 px-0 text-xl font-medium focus:ring-0 transition-all duration-300 rounded-none"
                ></asp:TextBox>
              </div>
              <div class="group relative">
                <label
                  class="block text-[10px] uppercase tracking-widest text-on-surface-variant mb-2 group-focus-within:text-secondary transition-colors"
                  >Email Address</label
                >
                <asp:TextBox
                  ID="txtEmail"
                  runat="server"
                  TextMode="Email"
                  CssClass="az-profile-input w-full bg-transparent border-0 border-b border-outline-variant py-3 px-0 text-xl font-medium focus:ring-0 transition-all duration-300 rounded-none"
                ></asp:TextBox>
              </div>
            </div>
            <div class="pt-8">
              <asp:Button
                ID="btnUpdateInformation"
                runat="server"
                Text="Update Information"
                OnClick="btnUpdateInformation_Click"
                CssClass="bg-secondary text-on-secondary px-10 py-4 text-sm font-bold uppercase tracking-widest hover:brightness-110 active:scale-95 transition-all rounded-none"
              />
            </div>
          </div>
        </div>

        <div
          class="flex flex-col md:flex-row items-center gap-8 bg-primary-container p-8 md:p-10 border-l-4 border-secondary"
        >
          <div class="flex-1">
            <h3 class="text-2xl font-bold tracking-tight mb-2">
              Track Your Style
            </h3>
            <p class="text-on-primary-container">
              You have 3 active shipments arriving this week. Review your
              selection.
            </p>
          </div>
          <asp:HyperLink
            ID="lnkOrderHistory"
            runat="server"
            class="group flex items-center gap-2 text-secondary font-bold uppercase tracking-widest text-sm whitespace-nowrap"
            NavigateUrl="~/Customer/OrderHistory.aspx"
          >
            Order History
            <span
              class="material-symbols-outlined transition-transform group-hover:translate-x-2"
              >arrow_forward</span
            >
          </asp:HyperLink>
        </div>
      </section>

      <aside class="lg:col-span-4 space-y-8">
        <div
          class="relative aspect-square bg-surface-container-high group overflow-hidden"
        >
          <img
            alt="User Profile"
            class="w-full h-full object-cover grayscale hover:grayscale-0 transition-all duration-700 scale-100 group-hover:scale-105"
            src="https://lh3.googleusercontent.com/aida-public/AB6AXuAjv6Iq9kukBLWn58NqvoOUftoz_CiMAjJgeYdPGIlUThYL3MU-dqfgGxp-p9yk_LQygnpqvdJNyKxLjPMMZqoMRZT_CczvOK1tBQn78tSRu2VLKVGzgtEGscsb-RkZ47dyK1a01k4Vx2QQqdoJaKZhRNgJGGKdJss0addEE8VdThVJpLLzoJqQ2TNgHYlstJzY9TvDYwRi8_WopBU8YB6criHbt4n6ZQIG3UDVVLstb97tnilQXRDB44q_NUIJvzVxdFYZ4ldaphg"
          />
          <div
            class="absolute inset-0 bg-primary-container/20 group-hover:bg-transparent transition-colors duration-500"
          ></div>
          <div class="absolute bottom-6 left-6">
            <span
              class="bg-secondary text-on-secondary px-3 py-1 text-[10px] font-bold uppercase tracking-widest"
              >Platinum Member</span
            >
          </div>
        </div>

        <div class="space-y-1">
          <a
            class="flex justify-between items-center p-6 bg-surface-container-low hover:bg-surface-container-high transition-colors group"
            href="#"
          >
            <span class="text-sm font-semibold uppercase tracking-widest"
              >Saved Items</span
            >
            <span class="material-symbols-outlined text-secondary text-lg"
              >favorite</span
            >
          </a>
          <a
            class="flex justify-between items-center p-6 bg-surface-container-low hover:bg-surface-container-high transition-colors group"
            href="#"
          >
            <span class="text-sm font-semibold uppercase tracking-widest"
              >Shipping Addresses</span
            >
            <span
              class="material-symbols-outlined text-on-surface-variant text-lg group-hover:text-secondary"
              >location_on</span
            >
          </a>
          <a
            class="flex justify-between items-center p-6 bg-surface-container-low hover:bg-surface-container-high transition-colors group border-t border-outline-variant/10"
            href="#"
          >
            <span class="text-sm font-semibold uppercase tracking-widest"
              >Security</span
            >
            <span
              class="material-symbols-outlined text-on-surface-variant text-lg group-hover:text-secondary"
              >shield</span
            >
          </a>
          <asp:LinkButton
            ID="btnSignOut"
            runat="server"
            OnClick="btnSignOut_Click"
            CausesValidation="false"
            CssClass="w-full flex justify-between items-center p-6 bg-surface-container-lowest hover:bg-error-container/20 transition-colors group text-error mt-4"
          >
            <span class="text-sm font-bold uppercase tracking-widest"
              >Sign Out</span
            >
            <span class="material-symbols-outlined text-lg">logout</span>
          </asp:LinkButton>
        </div>
      </aside>
    </div>
  </main>

  <script type="text/javascript">
    (function () {
      function wireProfileCartNavigation() {
        var cartLink = document.getElementById("az-navbar-cart-link");
        if (!cartLink) {
          return;
        }

        var cartUrl = '<%= ResolveUrl("~/Customer/Cart.aspx") %>';
        cartLink.setAttribute("href", cartUrl);
        cartLink.setAttribute("data-cart-url", cartUrl);

        if (cartLink.getAttribute("data-az-cart-bound") === "1") {
          return;
        }

        cartLink.setAttribute("data-az-cart-bound", "1");
        cartLink.addEventListener("click", function (event) {
          event.preventDefault();
          window.location.assign(cartUrl);
        });
      }

      if (document.readyState === "loading") {
        document.addEventListener(
          "DOMContentLoaded",
          wireProfileCartNavigation,
        );
      } else {
        wireProfileCartNavigation();
      }

      window.addEventListener("load", wireProfileCartNavigation);
    })();
  </script>
</asp:Content>
