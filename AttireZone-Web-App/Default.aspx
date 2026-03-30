<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Default.aspx.cs"
Inherits="AttireZone_Web_App._Default" %>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <!-- HERO SECTION -->
  <section class="relative w-full min-h-screen bg-[#0a0a0a] flex items-center justify-center overflow-hidden">
    <div class="absolute inset-0 opacity-30">
      <div class="absolute top-20 left-20 w-72 h-72 bg-[#e9c349] rounded-full blur-3xl"></div>
      <div class="absolute bottom-20 right-20 w-96 h-96 bg-[#e9c349]/30 rounded-full blur-3xl"></div>
    </div>
    <div class="relative z-10 max-w-7xl mx-auto px-6 text-center">
      <span class="text-xs font-bold uppercase tracking-[0.4em] text-[#e9c349] mb-4 block">Welcome to AttireZone</span>
      <h1 class="text-6xl md:text-7xl font-black tracking-tighter uppercase leading-tight mb-6 text-[#e2e2e2]">
        Discover Your <span class="text-[#e9c349]">Style</span>
      </h1>
      <p class="text-lg md:text-xl text-[#c4c6cf] max-w-2xl mx-auto mb-10 leading-relaxed">
        Explore our curated collection of premium fashion and lifestyle products. From timeless classics to cutting-edge trends.
      </p>
      <div class="flex flex-col sm:flex-row gap-4 justify-center items-center">
        <button class="btn-primary-lg">Shop Now</button>
        <button class="btn-secondary">Explore Collection</button>
      </div>
    </div>
  </section>

  <!-- ESSENTIAL CATEGORIES SECTION -->
  <section class="py-24 px-6 bg-[#0a0a0a]">
    <div class="max-w-7xl mx-auto">
      <div class="text-center mb-16">
        <span class="text-xs font-bold uppercase tracking-[0.2em] text-[#e9c349] mb-3 block">Our Collections</span>
        <h2 class="text-5xl font-black tracking-tighter uppercase text-[#e2e2e2]">Essential Categories</h2>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-6 gap-6">
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">T-Shirts</h3>
          <p class="text-[#999999] text-sm">Comfortable & Stylish</p>
        </div>
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">Hoodies</h3>
          <p class="text-[#999999] text-sm">Warm & Cozy</p>
        </div>
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">Shoes</h3>
          <p class="text-[#999999] text-sm">Premium Quality</p>
        </div>
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">Bags</h3>
          <p class="text-[#999999] text-sm">Durable & Trendy</p>
        </div>
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">Watches</h3>
          <p class="text-[#999999] text-sm">Elegant Timepieces</p>
        </div>
        <div class="bg-[#1a1a1a] border border-[#2a2a2a] p-8 rounded-md hover:border-[#e9c349] transition-colors cursor-pointer group">
          <h3 class="font-bold text-lg mb-2 uppercase text-[#e2e2e2] group-hover:text-[#e9c349] transition-colors">Sunglasses</h3>
          <p class="text-[#999999] text-sm">Eye Protection</p>
        </div>
      </div>
    </div>
  </section>

  <!-- FEATURED PRODUCTS SECTION -->
  <main class="flex bg-[#0a0a0a] text-[#f5f0e8] px-6 py-24">
    <div class="max-w-7xl mx-auto w-full flex gap-8 px-6 py-12">
      <!-- Sidebar (editorial filters) -->
      <aside class="hidden lg:flex flex-col w-72 p-8 space-y-10 bg-[#0f0f0f] rounded-md border border-[#1f1f1f]">
        <section>
          <h3 class="text-xs font-bold tracking-[0.2em] uppercase text-[#e9c349] mb-6">Category</h3>
          <ul class="space-y-3">
            <li><a class="text-sm font-medium text-[#c4c6cf] hover:text-[#e9c349] transition-colors block py-1" href="/Pages/Products.aspx?cat=1">T-shirts</a></li>
            <li><a class="text-sm font-medium text-[#c4c6cf] hover:text-[#e9c349] transition-colors block py-1" href="/Pages/Products.aspx?cat=2">Hoodies</a></li>
            <li><a class="text-sm font-medium text-[#c4c6cf] hover:text-[#e9c349] transition-colors block py-1" href="/Pages/Products.aspx?cat=3">Shoes</a></li>
            <li><a class="text-sm font-medium text-[#c4c6cf] hover:text-[#e9c349] transition-colors block py-1" href="/Pages/Products.aspx?cat=4">Bags</a></li>
          </ul>
        </section>
        <section>
          <h3 class="text-xs font-bold tracking-[0.2em] uppercase text-[#e9c349] mb-6">Price Range</h3>
          <ul class="space-y-3">
            <li><label class="text-sm font-medium text-[#c4c6cf]"><input type="checkbox" /> Under $50</label></li>
            <li><label class="text-sm font-medium text-[#c4c6cf]"><input type="checkbox" /> $50 - $100</label></li>
            <li><label class="text-sm font-medium text-[#c4c6cf]"><input type="checkbox" /> $100 - $200</label></li>
            <li><label class="text-sm font-medium text-[#c4c6cf]"><input type="checkbox" /> Over $200</label></li>
          </ul>
        </section>
      </aside>

      <!-- Main Products Grid -->
      <div class="flex-1">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <!-- Product Card Template -->
          <div class="card-hover">
            <div class="w-full h-64 bg-[#0f0f0f] rounded-md mb-4 flex items-center justify-center">
              <span class="text-[#666666]">Product Image</span>
            </div>
            <h3 class="text-lg font-bold text-[#e2e2e2] mb-2">Premium Outfit</h3>
            <p class="text-sm text-[#999999] mb-4">High-quality fashion item for everyday wear</p>
            <div class="flex justify-between items-center">
              <span class="text-xl font-bold text-[#e9c349]">$99.99</span>
              <button class="btn-primary">Add to Cart</button>
            </div>
          </div>

          <!-- Additional Product Cards (repeat structure above) -->
          <div class="card-hover">
            <div class="w-full h-64 bg-[#0f0f0f] rounded-md mb-4 flex items-center justify-center">
              <span class="text-[#666666]">Product Image</span>
            </div>
            <h3 class="text-lg font-bold text-[#e2e2e2] mb-2">Casual Wear</h3>
            <p class="text-sm text-[#999999] mb-4">Comfortable everyday fashion</p>
            <div class="flex justify-between items-center">
              <span class="text-xl font-bold text-[#e9c349]">$79.99</span>
              <button class="btn-primary">Add to Cart</button>
            </div>
          </div>

          <div class="card-hover">
            <div class="w-full h-64 bg-[#0f0f0f] rounded-md mb-4 flex items-center justify-center">
              <span class="text-[#666666]">Product Image</span>
            </div>
            <h3 class="text-lg font-bold text-[#e2e2e2] mb-2">Elegant Design</h3>
            <p class="text-sm text-[#999999] mb-4">Sophisticated style for special occasions</p>
            <div class="flex justify-between items-center">
              <span class="text-xl font-bold text-[#e9c349]">$129.99</span>
              <button class="btn-primary">Add to Cart</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</asp:Content>
