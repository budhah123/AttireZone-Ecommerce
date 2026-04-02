<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Navbar.aspx.cs"
Inherits="AttireZone_Web_App.Navbar" %>

<nav
  class="az-navbar sticky top-0 w-full z-50 rounded-none bg-white/80 dark:bg-slate-950/80 backdrop-blur-md shadow-sm dark:shadow-none"
>
  <div
    class="flex justify-between items-center px-6 py-4 max-w-[1920px] mx-auto"
  >
    <a
      class="text-2xl font-black tracking-tighter text-slate-900 dark:text-slate-50 uppercase"
      href="<%= ResolveUrl("~/Default.aspx") %>"
    >
      AttireZone
    </a>
    <div class="hidden md:flex items-center space-x-8">
      <a
        class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors"
        href="<%= ResolveUrl("~/Default.aspx#essential-categories") %>"
        >Collections</a
      >
      <a
        class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors"
        href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
        >New Arrivals</a
      >
      <a
        class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors"
        href="<%= ResolveUrl("~/Default.aspx#curated-section") %>"
        >Sale</a
      >
      <a
        class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors"
        href="<%= ResolveUrl("~/Default.aspx#journal-section") %>"
        >Journal</a
      >
    </div>
    <div class="flex items-center space-x-6">
      <a
        class="material-symbols-outlined text-slate-600 dark:text-slate-400 hover:opacity-80 transition-opacity"
        href="/Customer/Cart.aspx"
        >shopping_bag</a
      >
      <a
        class="material-symbols-outlined text-amber-500 border-b-2 border-amber-500 pb-1"
        href="/Customer/OrderHistory.aspx"
        >person</a
      >
    </div>
  </div>
  <div class="bg-slate-100 dark:bg-slate-900 h-[1px]"></div>
</nav>
