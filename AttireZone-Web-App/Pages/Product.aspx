<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="AttireZone_Web_App.Pages.Product" %>

<!DOCTYPE html>

<html class="dark" lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <title>AttireZone | Curated Collections</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@100;300;400;600;700;900&amp;display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet" />
    <style>
        html {
            font-size: 15px;
        }

        body {
            font-family: 'Inter', sans-serif;
        }

        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 200, 'GRAD' 0, 'opsz' 24;
            vertical-align: middle;
        }

        ::-webkit-scrollbar {
            width: 4px;
        }

        ::-webkit-scrollbar-track {
            background: #131313;
        }

        ::-webkit-scrollbar-thumb {
            background: #2a2a2a;
        }

        ::-webkit-scrollbar-thumb:hover {
            background: #e9c349;
        }
    </style>
    <script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    colors: {
                        "on-secondary-fixed": "#241a00",
                        "surface": "#131313",
                        "on-tertiary-fixed-variant": "#454747",
                        "background": "#131313",
                        "on-error-container": "#ffdad6",
                        "on-primary-container": "#6f88ad",
                        "inverse-primary": "#476083",
                        "on-secondary-container": "#342800",
                        "primary": "#afc8f0",
                        "surface-container-low": "#1b1b1b",
                        "primary-fixed-dim": "#afc8f0",
                        "on-primary-fixed-variant": "#2f486a",
                        "on-background": "#e2e2e2",
                        "tertiary-fixed": "#e2e2e2",
                        "tertiary": "#c6c6c7",
                        "on-surface-variant": "#c4c6cf",
                        "primary-container": "#001f3f",
                        "on-secondary": "#3c2f00",
                        "error": "#ffb4ab",
                        "surface-tint": "#afc8f0",
                        "outline": "#8e9198",
                        "secondary": "#e9c349",
                        "secondary-container": "#af8d11",
                        "on-surface": "#e2e2e2",
                        "tertiary-container": "#1d1f1f",
                        "outline-variant": "#43474e",
                        "error-container": "#93000a",
                        "inverse-on-surface": "#303030",
                        "surface-container-high": "#2a2a2a",
                        "surface-container-highest": "#353535",
                        "surface-container": "#1f1f1f",
                        "inverse-surface": "#e2e2e2",
                        "on-primary": "#163152",
                        "surface-container-lowest": "#0e0e0e",
                        "on-primary-fixed": "#001c3a",
                        "surface-dim": "#131313",
                        "tertiary-fixed-dim": "#c6c6c7",
                        "on-tertiary-fixed": "#1a1c1c",
                        "surface-variant": "#353535",
                        "secondary-fixed": "#ffe088",
                        "primary-fixed": "#d4e3ff",
                        "on-secondary-fixed-variant": "#574500",
                        "secondary-fixed-dim": "#e9c349",
                        "surface-bright": "#393939",
                        "on-error": "#690005",
                        "on-tertiary-container": "#858687",
                        "on-tertiary": "#2f3131"
                    },
                    fontFamily: {
                        "headline": ["Inter"],
                        "body": ["Inter"],
                        "label": ["Inter"]
                    },
                    borderRadius: {
                        "DEFAULT": "0.25rem",
                        "lg": "0.5rem",
                        "xl": "0.75rem",
                        "full": "9999px"
                    }
                }
            }
        }
    </script>
</head>
<body class="bg-background text-on-background selection:bg-secondary selection:text-on-secondary">
    <form id="form1" runat="server">
        <% Server.Execute("~/Navbar.aspx"); %>

        <main class="flex min-h-screen">
            <aside class="hidden lg:flex flex-col w-72 p-10 space-y-12 border-r border-outline-variant/10 bg-surface-container-lowest">
                <section>
                    <h3 class="text-xs font-bold tracking-[0.2em] uppercase text-secondary mb-8">Category</h3>
                    <ul class="space-y-4">
                        <li>
                            <a href="<%= AllCategoryUrl %>" class="<%= AllCategoryCssClass %>">All</a>
                        </li>
                        <asp:Repeater ID="rptCategories" runat="server">
                            <ItemTemplate>
                                <li>
                                    <a href="<%# Eval("Url") %>" class="<%# Eval("CssClass") %>"><%#: Eval("Name") %></a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </section>

                <section>
                    <h3 class="text-xs font-bold tracking-[0.2em] uppercase text-secondary mb-8">Refinement</h3>
                    <div class="space-y-6">
                        <div>
                            <span class="text-[10px] uppercase tracking-widest text-on-surface-variant block mb-3">Material</span>
                            <div class="flex flex-wrap gap-2">
                                <button class="px-3 py-1 text-[10px] border border-outline-variant/30 uppercase tracking-tighter hover:border-secondary transition-all" type="button">Organic Cotton</button>
                                <button class="px-3 py-1 text-[10px] border border-secondary uppercase tracking-tighter text-secondary" type="button">Recycled Polyester</button>
                                <button class="px-3 py-1 text-[10px] border border-outline-variant/30 uppercase tracking-tighter hover:border-secondary transition-all" type="button">Italian Leather</button>
                            </div>
                        </div>
                    </div>
                </section>

                <section>
                    <div class="mt-auto">
                        <img alt="Editorial Fashion" class="w-full opacity-60 grayscale hover:grayscale-0 transition-all duration-700" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBt80FpDUQbjsEYrfW1ysAkXbBVWnqSu-kj7YxEDHAzdd2fMDydL5S9QMomNqUsh_V9c6cKJ4i3Lf_8TW0ENOFnCzwen8rjLmF2SF124lZn7zlqLYT1bkLsCsExgrgqJPieVlVNyVBzD0iylFmr0OmCpXor9S1XUKoLH9QX_LS3y-fnz3tff1Es7Pcayxhne4tU1Ti1OsbowwfDlN85sPTg3BqpX86wtiIZeH8eGrz8XNGtuMqMm9GeT3BKZpODxFKDOye7wwyJQG4" />
                    </div>
                </section>
            </aside>

            <section class="flex-1 bg-surface p-6 md:p-12">
                <header class="mb-16">
                    <div class="flex flex-col md:flex-row md:items-end justify-between gap-6">
                        <div>
                            <span class="text-[10px] font-bold uppercase tracking-[0.4em] text-on-surface-variant mb-4 block">Curated Selection</span>
                            <h1 class="text-3xl md:text-5xl font-black tracking-tighter uppercase leading-none">The Autumn <br />
                                <span class="text-secondary">Collective</span>
                            </h1>
                        </div>
                        <div class="flex flex-wrap items-center gap-4 text-[10px] font-bold uppercase tracking-widest">
                            <div class="relative w-full sm:w-auto sm:min-w-[16rem]">
                                <asp:TextBox ID="txtSearchProducts" runat="server" AutoPostBack="true" OnTextChanged="txtSearchProducts_TextChanged" CssClass="w-full bg-transparent border border-outline-variant/30 focus:border-secondary py-2 pl-3 pr-10 text-xs transition-colors outline-none placeholder:text-on-surface-variant/70" placeholder="Search curated items..."></asp:TextBox>
                                <asp:LinkButton ID="btnSearchProducts" runat="server" OnClick="btnSearchProducts_Click" CausesValidation="false" CssClass="absolute right-2 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-secondary transition-colors" aria-label="Search products">
                                    <span class="material-symbols-outlined !text-base">search</span>
                                </asp:LinkButton>
                                <div id="productSearchSuggestions" class="hidden absolute left-0 right-0 top-full mt-2 bg-surface-container border border-outline-variant/30 shadow-2xl z-50 max-h-64 overflow-auto"></div>
                            </div>
                            <div class="relative">
                                <asp:DropDownList ID="ddlSortProducts" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortProducts_SelectedIndexChanged" CssClass="appearance-none bg-transparent border-none text-[10px] font-bold uppercase tracking-widest pr-5 cursor-pointer hover:text-secondary transition-colors focus:ring-0">
                                    <asp:ListItem Value="featured">Sort: Featured</asp:ListItem>
                                    <asp:ListItem Value="newest">Sort: Newest</asp:ListItem>
                                    <asp:ListItem Value="price_asc">Sort: Price Low-High</asp:ListItem>
                                    <asp:ListItem Value="price_desc">Sort: Price High-Low</asp:ListItem>
                                    <asp:ListItem Value="name_asc">Sort: Name A-Z</asp:ListItem>
                                </asp:DropDownList>
                                <span class="material-symbols-outlined !text-sm absolute right-0 top-1/2 -translate-y-1/2 pointer-events-none">expand_more</span>
                            </div>
                            <button class="flex items-center gap-2 hover:text-secondary transition-colors" type="button">Layout: Grid <span class="material-symbols-outlined !text-sm">grid_view</span></button>
                        </div>
                    </div>
                </header>

                <asp:PlaceHolder ID="phNoProducts" runat="server" Visible="false">
                    <div class="border border-outline-variant/20 bg-surface-container-low px-8 py-16 text-center mb-12">
                        <h2 class="text-xl font-semibold tracking-tight uppercase mb-3">No Products Found</h2>
                        <p class="text-on-surface-variant text-sm uppercase tracking-widest">Try another category to explore curated pieces.</p>
                    </div>
                </asp:PlaceHolder>

                <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                    <HeaderTemplate>
                        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-y-14 gap-x-8">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <article class="group relative w-full max-w-[18rem] mx-auto">
                            <div class="aspect-[4/5] overflow-hidden bg-surface-container-low mb-5 relative">
                                <img alt="<%#: Eval("ImageAlt") %>" class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110" src="<%#: Eval("ImageUrl") %>" />
                                <div class="absolute inset-0 bg-black/0 group-hover:bg-black/25 transition-colors duration-500 flex items-center justify-center">
                                    <div class="opacity-0 group-hover:opacity-100 translate-y-4 group-hover:translate-y-0 transition-all duration-300 flex flex-col gap-3 px-4 w-full max-w-[12rem]">
                                        <asp:DropDownList ID="ddlSelectedSize" runat="server" CssClass="w-full bg-surface-container-high/80 text-on-surface border border-white/20 text-[10px] font-bold uppercase tracking-widest py-1.5 px-2">
                                            <asp:ListItem Value="S">Size S</asp:ListItem>
                                            <asp:ListItem Value="M" Selected="True">Size M</asp:ListItem>
                                            <asp:ListItem Value="L">Size L</asp:ListItem>
                                            <asp:ListItem Value="XL">Size XL</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtSelectedQuantity" runat="server" TextMode="Number" Text="1" min="1" CssClass="w-full bg-surface-container-high/80 text-on-surface border border-white/20 text-[10px] font-bold tracking-widest py-1.5 px-2"></asp:TextBox>
                                        <asp:LinkButton ID="btnAddToCart" runat="server" CssClass="bg-secondary text-on-secondary px-4 py-2 text-[11px] font-bold uppercase tracking-widest w-full text-center" CommandName="AddToCart" CommandArgument='<%# Eval("ProductId") %>'>
                                            Add to Cart
                                        </asp:LinkButton>
                                        <a href="<%# Eval("ViewDetailsUrl") %>" class="bg-white/85 text-slate-900 px-4 py-2 text-[11px] font-bold uppercase tracking-widest text-center w-full hover:bg-white transition-colors">
                                            View Details
                                        </a>
                                    </div>
                                </div>
                                <span class="<%# Eval("BadgeCssClass") %>">New</span>
                            </div>
                            <div class="space-y-1">
                                <p class="text-[10px] uppercase tracking-widest text-on-surface-variant"><%#: Eval("CategoryLabel") %></p>
                                <div class="flex justify-between items-start gap-3">
                                    <h2 class="text-base font-bold tracking-tight uppercase group-hover:text-secondary transition-colors"><%#: Eval("ProductName") %></h2>
                                    <p class="text-base font-light whitespace-nowrap"><%#: Eval("PriceLabel") %></p>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>

                <div class="mt-24 flex items-center justify-between border-t border-outline-variant/10 pt-12">
                    <p class="text-xs uppercase tracking-widest text-on-surface-variant">
                        <asp:Literal ID="litShowingSummary" runat="server"></asp:Literal>
                    </p>
                    <div class="flex gap-4">
                        <button class="w-10 h-10 border border-secondary text-secondary flex items-center justify-center font-bold" type="button">1</button>
                        <button class="w-10 h-10 border border-outline-variant/30 hover:border-secondary transition-all flex items-center justify-center" type="button">2</button>
                        <button class="w-10 h-10 border border-outline-variant/30 hover:border-secondary transition-all flex items-center justify-center" type="button">3</button>
                        <button class="px-6 border border-outline-variant/30 hover:border-secondary transition-all flex items-center justify-center text-[10px] uppercase font-bold tracking-widest" type="button">Next</button>
                    </div>
                </div>
            </section>
        </main>

        <footer class="w-full border-t-0 rounded-none bg-slate-50 dark:bg-slate-950">
            <div class="bg-slate-200 dark:bg-slate-800 h-[1px]"></div>
            <div class="flex flex-col md:flex-row justify-between items-center px-12 py-20 w-full gap-8">
                <div class="text-lg font-bold text-slate-900 dark:text-slate-50 uppercase tracking-tighter">
                    AttireZone
                </div>
                <div class="flex flex-wrap justify-center gap-8 font-sans text-xs uppercase tracking-widest">
                    <a class="text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Sustainability</a>
                    <a class="text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Shipping</a>
                    <a class="text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Returns</a>
                    <a class="text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Privacy Policy</a>
                    <a class="text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Terms of Service</a>
                </div>
                <div class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500">
                    © 2024 AttireZone. All Rights Reserved.
                </div>
            </div>
        </footer>

        <script type="text/javascript">
            (function () {
                var input = document.getElementById('<%= txtSearchProducts.ClientID %>');
                var suggestionBox = document.getElementById('productSearchSuggestions');
                if (!input || !suggestionBox) {
                    return;
                }

                var endpoint = '<%= ResolveUrl("~/Pages/Product.aspx/GetSearchSuggestions") %>';
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
                        option.className = 'w-full text-left px-4 py-2 text-xs uppercase tracking-wide text-on-surface border-b border-outline-variant/20 last:border-b-0 hover:bg-surface-container-high transition-colors';
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
    </form>
</body>
</html>
