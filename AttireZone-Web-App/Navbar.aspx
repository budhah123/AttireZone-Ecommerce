<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Navbar.aspx.cs"
Inherits="AttireZone_Web_App.Navbar" %>

<nav
  class="az-navbar sticky top-0 w-full z-50 rounded-none bg-white/80 dark:bg-slate-950/80 backdrop-blur-md shadow-sm dark:shadow-none"
>
  <div class="max-w-[1920px] mx-auto px-4 sm:px-6 py-4">
    <div class="flex items-center justify-between gap-4">
      <a
        class="text-2xl font-black tracking-tighter text-slate-900 dark:text-slate-50 uppercase"
        href="<%= ResolveUrl("~/Default.aspx") %>"
      >
        AttireZone
      </a>

      <div class="hidden md:flex items-center gap-8">
        <a
          class="az-nav-link font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#essential-categories") %>"
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

      <div class="flex items-center gap-4 sm:gap-6">
        <a
          class="material-symbols-outlined text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= CartNavigationUrl %>"
          aria-label="Open shopping bag"
          >shopping_bag</a
        >
        <a
          class="<%= ProfileIconCssClass %>"
          href="<%= ProfileNavigationUrl %>"
          style="<%= ProfileIconInlineStyle %>"
          aria-label="Open profile"
          >person</a
        >
      </div>
    </div>

    <div class="mt-4 overflow-x-auto md:hidden">
      <div class="flex min-w-max items-center gap-6 pb-1">
        <a
          class="az-nav-link font-sans tracking-tight text-xs uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors duration-200"
          href="<%= ResolveUrl("~/Default.aspx#essential-categories") %>"
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
