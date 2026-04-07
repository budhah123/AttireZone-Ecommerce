<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Navbar.aspx.cs"
Inherits="AttireZone_Web_App.Navbar" %>

<nav
  class="az-navbar sticky top-0 w-full z-[1200] rounded-none bg-white/80 dark:bg-slate-950/80 backdrop-blur-md shadow-sm dark:shadow-none pointer-events-auto"
>
  <div class="max-w-[1920px] mx-auto px-4 sm:px-6 py-4">
    <div class="flex items-center justify-between gap-4 relative">
      <a
        class="text-2xl font-black tracking-tighter text-slate-900 dark:text-slate-50 uppercase"
        href="<%= ResolveUrl("~/Default.aspx") %>"
      >
        AttireZone
      </a>

      <div class="hidden md:flex items-center gap-8">
        <a
          class="az-nav-link font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Pages/Product.aspx") %>"
          >Collections</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
          >New Arrivals</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
          >Sale</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#journal-section") %>"
          >Journal</a
        >
      </div>

      <% if (IsHomePage) { %>
      <div id="az-home-search-slot" class="flex-1 max-w-md mx-8 hidden lg:block relative z-[1100] pointer-events-auto"></div>
      <% } %>

      <div class="relative z-[1500] flex items-center gap-4 sm:gap-6 shrink-0 pointer-events-auto">
        <a
          id="az-navbar-cart-link"
          class="relative z-[1600] inline-flex h-10 w-10 items-center justify-center rounded-full text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200 cursor-pointer pointer-events-auto"
          href="<%= CartNavigationUrl %>"
          aria-label="Open shopping bag"
        >
          <span class="material-symbols-outlined">shopping_bag</span>
          <span id="az-cart-badge" class="absolute -top-1.5 -right-1.5 min-w-[1.2rem] h-5 px-1 inline-flex items-center justify-center rounded-full bg-secondary text-on-secondary text-[10px] font-bold leading-none <%= CartItemCount > 0 ? string.Empty : "hidden" %>" aria-live="polite"><%= CartItemCount %></span>
        </a>
        <a
          class="<%= ProfileIconCssClass %>"
          href="<%= ProfileNavigationUrl %>"
          style="<%= ProfileIconInlineStyle %>"
          aria-label="Open profile"
        >person</a>
      </div>
    </div>

    <div class="mt-4 overflow-x-auto md:hidden">
      <div class="flex min-w-max items-center gap-6 pb-1">
        <a
          class="az-nav-link font-sans tracking-tight text-xs uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Pages/Product.aspx") %>"
          >Collections</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-xs uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
          >New Arrivals</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-xs uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
          >Sale</a
        >
        <a
          class="az-nav-link font-sans tracking-tight text-xs uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#journal-section") %>"
          >Journal</a
        >
      </div>
    </div>
  </div>
  <div class="bg-slate-100 dark:bg-slate-900 h-[1px]"></div>
</nav>

<script type="text/javascript">
  (function () {
    var badgeElement = document.getElementById('az-cart-badge');

    function normalizeCount(value) {
      var parsed = parseInt(value, 10);
      if (isNaN(parsed) || parsed < 0) {
        return 0;
      }

      return parsed;
    }

    function setCount(value) {
      if (!badgeElement) {
        return;
      }

      var normalized = normalizeCount(value);
      badgeElement.textContent = normalized.toString();

      if (normalized > 0) {
        badgeElement.classList.remove('hidden');
        return;
      }

      badgeElement.classList.add('hidden');
    }

    window.azCartBadge = window.azCartBadge || {};
    window.azCartBadge.setCount = setCount;
    window.azCartBadge.increment = function (delta) {
      var current = badgeElement ? normalizeCount(badgeElement.textContent) : 0;
      setCount(current + normalizeCount(delta));
    };
  })();
</script>
