<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Default.aspx.cs"
Inherits="AttireZone_Web_App._Default" %>
<asp:Content ID="StylesContent" ContentPlaceHolderID="StylesPlaceholder" runat="server">
  <style type="text/css">
    html {
      scroll-behavior: smooth;
    }

    .container.body-content {
      max-width: 100% !important;
      width: 100% !important;
      margin: 0 !important;
      padding: 0 !important;
    }

    .az-home-page {
      background-color: #131313;
    }

    .az-scroll-target {
      scroll-margin-top: 96px;
    }
  </style>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="az-home-page">
    <main>
    <section class="relative h-[820px] flex items-center overflow-hidden bg-primary-container">
      <div class="absolute inset-0 z-0">
        <img alt="Editorial Hero" class="w-full h-full object-cover opacity-60 mix-blend-luminosity" src="https://lh3.googleusercontent.com/aida-public/AB6AXuD1hF2BkMw_Znip9qGTxt9z-FinpyBOi5b7e1j9i9VM9MM8RSfXe8eXNSm-_Lz2G3iU3Ks1zBAq7AOIPnXk3sRufR2zoAeFAH9Y_h3au6hRPgrfz5kHMPqWcje6vqkMXXZm8b-Eq6W7H7utZqRcXBUAHk7Vs6635W_XCY5ns_2wwg5z4zZw-wWUf_g9a2GFJKYojEIC9hSIooOTG-hDA0QXsVm9Qk3jBVNWOXhoktqGLG6FxwyasYPoWibqVpCUSP1IzXNn4-6h7ds" />
      </div>
      <div class="relative z-10 px-5 md:px-14 lg:px-20 w-full">
        <div class="max-w-4xl">
          <p class="text-secondary font-label tracking-[0.2em] uppercase text-sm mb-3">Autumn / Winter 2024</p>
          <h1 class="text-5xl md:text-7xl lg:text-8xl font-headline font-bold -ml-1 leading-[0.9] tracking-tighter text-on-background">THE SILENT<br />CURATOR</h1>
          <div class="mt-10 flex flex-col md:flex-row gap-4 items-start">
            <a class="px-9 py-3 bg-secondary text-on-secondary font-bold text-sm tracking-widest uppercase hover:opacity-90 transition-all" href="<%= ResolveUrl("~/Pages/Product.aspx") %>">Shop Collection</a>
            <a class="px-9 py-3 border border-outline-variant/30 text-on-background font-bold text-sm tracking-widest uppercase hover:bg-white/5 transition-all" href="#">View Editorial</a>
          </div>
          <div class="mt-8 max-w-md">
            <div class="relative group">
              <asp:TextBox ID="txtHomeSearch" runat="server" AutoPostBack="true" OnTextChanged="txtHomeSearch_TextChanged" CssClass="w-full bg-surface-container-low/80 border border-outline-variant/30 focus:border-secondary focus:ring-1 focus:ring-secondary text-sm px-4 py-3 placeholder:text-on-surface-variant/70" placeholder="Search curated styles..."></asp:TextBox>
              <asp:LinkButton ID="btnHomeSearch" runat="server" OnClick="btnHomeSearch_Click" CausesValidation="false" CssClass="absolute right-3 top-1/2 -translate-y-1/2 text-outline hover:text-secondary transition-colors" aria-label="Search catalogue">
                <span class="material-symbols-outlined">search</span>
              </asp:LinkButton>
              <div id="homeSearchSuggestions" class="hidden absolute left-0 right-0 top-full mt-2 bg-surface-container border border-outline-variant/30 shadow-2xl z-50 max-h-64 overflow-auto"></div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section id="essential-categories" class="py-20 px-5 md:px-14 lg:px-20 bg-surface az-scroll-target">
      <div class="mb-12">
        <h2 class="text-3xl font-headline font-semibold text-on-background tracking-tight">Essential Categories</h2>
        <p class="text-on-surface-variant mt-2 max-w-lg">Defining modern silhouettes through premium fabrics and precise tailoring.</p>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-4 lg:grid-cols-6 gap-5 h-auto md:h-[740px]">
        <div class="md:col-span-2 md:row-span-2 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAU2dyIFpYOVxXL3iBaH_PozLyXHV_AJnpJ0uWelt6WkIutAwTajnQhRAjxiGR19QNimHc_Pc3ASNkTxh3fsJSZaukT7PnNzDhw3-I5rfn4Isv1ExAidcWlTKryA_2Zs_HLSPpWT-BvP1nKcl8Si4wmrLi44iJtWoWvh_jMKWW9B-DMDCCPYosOG0G3bRd0CsEK-MwPjBbi7Yc5zkIE9YgfV_rj9rBzxlVnBzpIMgp1N5VQgmDOn6Qu-ip1xxQzVey5_PLtIBiDn0A" alt="T-Shirts" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-7">
            <h3 class="text-2xl font-bold text-white uppercase tracking-tighter">T-Shirts</h3>
            <p class="text-secondary text-xs tracking-widest mt-2 uppercase">18 Products</p>
          </div>
        </div>
        <div class="md:col-span-2 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuChHxaynip_FBBGfHtyWSaEU6v6HYj1ylHc2TyEzuCQ9D0Ux0hd4d-_NteMUmc-xiLUc6Q43r5Ui1eIF7wUiwF56j_U24v3a89hi2TddtpWEkX7wTNabguoBjL_dMYA622VmWqAQiJKlxoXu4VsFjpSkqQg2xvpPHM_Kh5zZv0JX1qM0t91NUCWu5VAtCmUM5F-JJufb7SgeEl-4vVOt9Nz-VGIPu0H6uRjQQgNqUAsyxrnfTSqiolZmVaMwggKFrSc7v6J9BsxCuY" alt="Hoodies" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-7">
            <h3 class="text-2xl font-bold text-white uppercase tracking-tighter">Hoodies</h3>
          </div>
        </div>
        <div class="md:col-span-2 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAORhMnKpNryu0eYF-IIxL6a5fW4fitFfBhy8512RRq1-hHC2ZGl7nn47f5McPWk9FEIP9vbzJZXcATc76TzdGj7KaU3kyBQtc6_2HFVodjj2aNalmHPou7xgQnY720GpF825hPqnaJSNlpsbVGaE7oyowwlTSC38-Xf-AR2ey2FOLhewtvVBGC43u-8Q1xwPNUbKAcbOwbIANPQygbsep73VPLaDNQtq7OlXJ1spxTwLVzuYiKQAIWbyvuB_uGIEwg8evHbR3tPZA" alt="Shoes" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-7">
            <h3 class="text-2xl font-bold text-white uppercase tracking-tighter">Shoes</h3>
          </div>
        </div>
        <div class="md:col-span-2 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBK3yF5Zm7rpEus41hqnwJ8z0tbcvxfXXPI345vuhZeZwGWDHuJj4buBpwohe5I47a_n9ZsddL8CXdcZjci4FSQT-M3q6uWOeSfrENoSEeqOdrruvTuuilt9lvS5TcK_0ROrURWLY7MDcOp5GmmONJvOvLs58bfmKlERkfS67sNZrSyYkk_HDTAY4DmNhAhV1_mSRAA2aQxfM3NHh92k5wS55OByrewko-F86ORp076sGyAiLlt46KM-zmjJO7nhcOOHDWOIuao8Fc" alt="Bags" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-7">
            <h3 class="text-2xl font-bold text-white uppercase tracking-tighter">Bags</h3>
          </div>
        </div>
        <div class="md:col-span-1 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuDZI7o1X4xj09abWekHpsNnQqaJEbTI0CPjuQ9KmCogITxnqsRfgPUnzKXldP4ZF5FSG491V32l9rWyx_MI8EdNTXk5lfyffF_XhbaHAzJwl4HsNH6p9eg8xByaeTyUK8Q_U9JYMUBsKGlT5xQoYpWamAoVcwpQYmIig1nNKvhuEiKkHjBIQNBB-Hy9qy8GO-vz3uNoBSHmQDWcoRhyjRMasbpjGp5_d0OFNXQr2PZ6GGu3P4_TjRNki2a9zoC9l2Guey3hpUYGwlE" alt="Watches" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-5">
            <h3 class="text-lg font-bold text-white uppercase tracking-tighter">Watches</h3>
          </div>
        </div>
        <div class="md:col-span-1 group relative overflow-hidden bg-surface-container">
          <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCa9OBUc-DD21EKEWN52k1YXGvHotL84JJtyRXoVa2S7t4wQVK6bIRfLxRtwsHHhmbh-QhrdmC0mgklveQ-izlCNST7-22QkRYQOm2GqaBOrD5-xpWpvEMDO1q0y4dXy_leyqAOlLFTAbCLTrh1APahdNysprdVoTLuFzPKUv13VA26QLE9S2pkdp-V8gAdGfYBr6lygqP8IxV92d-b8dLp-Sg3l3Cemv4W4jqmb_sVfSO87AMDoKefvyFfp8XFKXh4rB8Skrq7Fvg" alt="Sunglasses" />
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent flex flex-col justify-end p-5">
            <h3 class="text-lg font-bold text-white uppercase tracking-tighter">Sunglasses</h3>
          </div>
        </div>
      </div>
    </section>

    <section id="curated-section" class="py-20 px-5 md:px-14 lg:px-20 bg-surface-container-lowest az-scroll-target">
      <div class="flex flex-col md:flex-row justify-between items-end mb-12 gap-3">
        <div>
          <h2 class="text-3xl font-headline font-semibold text-on-background tracking-tight">Curated Selection</h2>
          <p class="text-on-surface-variant mt-2">The season's most-wanted pieces, meticulously crafted.</p>
        </div>
        <a class="text-secondary text-sm font-bold tracking-widest uppercase group flex items-center gap-2" href="<%= ResolveUrl("~/Pages/Product.aspx") %>">
          View All Products <span class="material-symbols-outlined transition-transform group-hover:translate-x-1">arrow_right_alt</span>
        </a>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-10">
        <asp:Repeater ID="rptCuratedProducts" runat="server">
          <ItemTemplate>
            <div class="group cursor-pointer">
              <div class="relative aspect-[3/4] overflow-hidden bg-surface-container-high mb-5">
                <img class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" src="<%#: Eval("ImageUrl") %>" alt="<%#: Eval("ImageAlt") %>" />
                <asp:PlaceHolder runat="server" Visible='<%# ShowBadge(Eval("BadgeText")) %>'>
                  <div class="absolute top-4 left-4">
                    <span class='<%# Eval("BadgeCssClass") %>'><%#: Eval("BadgeText") %></span>
                  </div>
                </asp:PlaceHolder>
                <button type="button" class="absolute bottom-4 right-4 bg-white/10 backdrop-blur-md p-2 rounded-full opacity-0 group-hover:opacity-100 transition-opacity">
                  <span class="material-symbols-outlined text-white">shopping_bag</span>
                </button>
              </div>
              <div class="space-y-1">
                <p class="text-on-surface-variant text-xs uppercase tracking-widest"><%#: Eval("CategoryLabel") %></p>
                <h3 class="text-lg font-medium text-on-surface group-hover:text-secondary transition-colors"><%#: Eval("ProductName") %></h3>
                <p class="text-secondary font-bold"><%#: Eval("PriceLabel") %></p>
              </div>
            </div>
          </ItemTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="phNoCuratedProducts" runat="server" Visible="false">
          <div class="sm:col-span-2 lg:col-span-4 border border-outline-variant/40 bg-surface-container px-8 py-10 text-center">
            <p class="text-on-surface-variant text-sm tracking-wide">No curated products are available yet. Add products from Admin to populate this section.</p>
          </div>
        </asp:PlaceHolder>
      </div>
    </section>

    <section id="journal-section" class="py-24 px-5 md:px-14 lg:px-20 grid grid-cols-1 lg:grid-cols-2 gap-16 items-center bg-surface az-scroll-target">
      <div class="relative">
        <div class="aspect-square bg-surface-container-high overflow-hidden">
          <img class="w-full h-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuD_2U99Fqx15meJX2QS6TzZtdVMiaa4QNFySNdgMthR0_CClC_UaQUktyWxhZif79fwOcut03yVshZVQ8D6vQK10CX191V-1kL8KkkQDTgX_o82_6mJL_xrgpIo5JluvPHyBbQRrU06FmAPRuuwb_lpgHp6ajIEQlyKskrUrI_d43Dnih92fGDB0EGi8zlevSOtoyg27EE8mxTn8Gl3s3bnXDKSIOQTSw9wO5rum_j-ZV26kz6Pbw5-s60nSDZos2qmt68MjYnK-7s" alt="Our Philosophy" />
        </div>
        <div class="absolute -bottom-8 -right-8 hidden md:block w-56 aspect-square border-8 border-surface bg-surface-container overflow-hidden">
          <img class="w-full h-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCaV6TDJntECHVok0NuBaoZo7_75YLcE57u5GazRZHCDIwJZoj7qbYT6AUdCO_5gsSWnR8m0CtCErycf6DCuoR1WBu4GXQRpqykik5n6VUvXayZSipEGvoYBRhHSy4PIXZl9ayYx4PqiEmx4Q8s8fiOnxePtjb4hGnFlHIgvCEJF81N4GExAcit03lbu_ZZLkjHt2C6r1P6OgMP2nTMhKbXPfMrthh38WjhiKiwBCpzwRu3Eaaz-bCjn7hsATG3LBthsQHdbN3AkXI" alt="Editorial Detail" />
        </div>
      </div>
      <div class="space-y-6">
        <span class="text-secondary font-label tracking-[0.2em] uppercase text-xs">Our Philosophy</span>
        <h2 class="text-4xl font-headline font-bold leading-tight text-on-background tracking-tighter">QUALITY IN THE<br />UNSEEN DETAILS</h2>
        <p class="text-on-surface-variant text-base leading-relaxed max-w-xl">
          AttireZone is more than a label. It's an exploration of form, function, and the quiet confidence that comes from impeccably constructed garments. Each piece is curated to exist beyond seasons.
        </p>
        <div class="pt-4">
          <a class="inline-block text-on-background font-bold text-sm tracking-widest uppercase border-b-2 border-secondary pb-1 hover:text-secondary transition-all" href="#journal-section">Read The Journal</a>
        </div>
      </div>
    </section>
    </main>

    <footer class="bg-slate-50 dark:bg-slate-950">
      <div class="bg-slate-200 dark:bg-slate-800 h-[1px]"></div>
      <div class="flex flex-col md:flex-row justify-between items-center px-10 py-16 w-full gap-6">
        <div class="flex flex-col items-center md:items-start gap-4">
          <a class="text-lg font-bold text-slate-900 dark:text-slate-50" href="#">AttireZone</a>
          <p class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500">Curating the modern silhouette since 2024.</p>
        </div>
        <div class="flex flex-wrap justify-center gap-8">
          <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Sustainability</a>
          <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Shipping</a>
          <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Returns</a>
          <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Privacy Policy</a>
          <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Terms of Service</a>
        </div>
        <div class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500">© 2024 AttireZone. All Rights Reserved.</div>
      </div>
    </footer>
  </div>

  <script type="text/javascript">
    (function () {
      var input = document.getElementById('<%= txtHomeSearch.ClientID %>');
      var suggestionBox = document.getElementById('homeSearchSuggestions');
      if (!input || !suggestionBox) {
        return;
      }

      var endpoint = '<%= ResolveUrl("~/Default.aspx/GetSearchSuggestions") %>';
      var debounceHandle = 0;
      var activeRequestId = 0;

      function hideSuggestions() {
        suggestionBox.classList.add('hidden');
        suggestionBox.innerHTML = '';
      }

      function renderSuggestions(items) {
        suggestionBox.innerHTML = '';

        if (!items || !items.length) {
          hideSuggestions();
          return;
        }

        var fragment = document.createDocumentFragment();
        for (var i = 0; i < items.length; i++) {
          var text = items[i];
          if (!text) {
            continue;
          }

          var option = document.createElement('button');
          option.type = 'button';
          option.className = 'w-full text-left px-4 py-2 text-sm text-on-surface border-b border-outline-variant/20 last:border-b-0 hover:bg-surface-container-high transition-colors';
          option.textContent = text;

          option.addEventListener('mousedown', function (event) {
            event.preventDefault();
          });

          option.addEventListener('click', function (event) {
            input.value = event.currentTarget.textContent || '';
            hideSuggestions();
          });

          fragment.appendChild(option);
        }

        if (!fragment.childNodes.length) {
          hideSuggestions();
          return;
        }

        suggestionBox.appendChild(fragment);
        suggestionBox.classList.remove('hidden');
      }

      function fetchSuggestions(query) {
        var requestId = ++activeRequestId;

        fetch(endpoint, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json; charset=utf-8'
          },
          body: JSON.stringify({ term: query })
        })
          .then(function (response) {
            if (!response.ok) {
              throw new Error('Unable to fetch suggestions.');
            }

            return response.json();
          })
          .then(function (payload) {
            if (requestId !== activeRequestId) {
              return;
            }

            var suggestions = payload && Array.isArray(payload.d) ? payload.d : [];
            renderSuggestions(suggestions);
          })
          .catch(function () {
            if (requestId === activeRequestId) {
              hideSuggestions();
            }
          });
      }

      input.addEventListener('input', function () {
        var query = (input.value || '').trim();
        window.clearTimeout(debounceHandle);

        if (query.length < 2) {
          hideSuggestions();
          return;
        }

        debounceHandle = window.setTimeout(function () {
          fetchSuggestions(query);
        }, 180);
      });

      input.addEventListener('focus', function () {
        var query = (input.value || '').trim();
        if (query.length >= 2) {
          fetchSuggestions(query);
        }
      });

      input.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
          hideSuggestions();
        }
      });

      document.addEventListener('click', function (event) {
        if (event.target === input || suggestionBox.contains(event.target)) {
          return;
        }

        hideSuggestions();
      });
    })();
  </script>
</asp:Content>
