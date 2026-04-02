<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="AttireZone_Web_App.Pages.ProductDetails" %>

<!DOCTYPE html>

<html class="dark" lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <title>AttireZone | Product Details</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@100;200;300;400;500;600;700;800;900&amp;display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet" />
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #131313;
        }

        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 200, 'GRAD' 0, 'opsz' 24;
        }

        .glass-nav {
            background-color: rgba(14, 14, 14, 0.8);
            backdrop-filter: blur(12px);
        }

        .az-size-select {
            background-color: #131313 !important;
            color: #e2e2e2 !important;
        }

        .az-size-select option {
            background-color: #1b1b1b !important;
            color: #e2e2e2 !important;
        }

        ::-webkit-scrollbar {
            width: 4px;
        }

        ::-webkit-scrollbar-track {
            background: #131313;
        }

        ::-webkit-scrollbar-thumb {
            background: #af8d11;
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
                        "DEFAULT": "0px",
                        "lg": "0px",
                        "xl": "0px",
                        "full": "9999px"
                    }
                }
            }
        }
    </script>
</head>
<body class="bg-background text-on-background selection:bg-secondary selection:text-on-secondary">
    <form id="form1" runat="server">
        <nav class="sticky top-0 w-full z-50 glass-nav shadow-sm dark:shadow-none">
            <div class="flex justify-between items-center px-6 py-4 max-w-[1920px] mx-auto">
                <div class="text-2xl font-black tracking-tighter text-slate-900 dark:text-slate-50 uppercase">
                    AttireZone
                </div>
                <div class="hidden md:flex space-x-8">
                    <a class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" href="<%= ResolveUrl("~/Pages/Product.aspx") %>">Collections</a>
                    <a class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" href="#">New Arrivals</a>
                    <a class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" href="#">Sale</a>
                    <a class="font-sans tracking-tight text-sm uppercase font-semibold text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" href="#">Journal</a>
                </div>
                <div class="flex items-center space-x-5">
                    <button class="material-symbols-outlined text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" type="button">shopping_bag</button>
                    <button class="material-symbols-outlined text-slate-600 dark:text-slate-400 hover:text-amber-500 transition-colors" type="button">person</button>
                    <button class="md:hidden material-symbols-outlined text-slate-600 dark:text-slate-400" type="button">menu</button>
                </div>
            </div>
            <div class="bg-slate-100 dark:bg-slate-900 h-[1px]"></div>
        </nav>

        <main class="max-w-[1320px] mx-auto px-5 lg:px-10 py-10">
            <asp:PlaceHolder ID="phProductNotFound" runat="server" Visible="false">
                <section class="border border-outline-variant/20 bg-surface-container-low px-8 py-16 text-center">
                    <h1 class="text-3xl font-semibold tracking-tight mb-4">Product Not Found</h1>
                    <p class="text-on-surface-variant mb-8">The requested product does not exist or is unavailable.</p>
                    <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="inline-flex items-center justify-center px-8 py-3 bg-secondary text-on-secondary text-xs font-bold uppercase tracking-[0.2em]">Back to Collections</a>
                </section>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phProductContent" runat="server">
                <nav class="mb-12">
                    <ol class="flex items-center space-x-4 text-[10px] tracking-[0.2em] uppercase font-medium text-on-surface-variant">
                        <li><a class="hover:text-secondary transition-colors" href="<%= ResolveUrl("~/Default.aspx") %>">Home</a></li>
                        <li class="material-symbols-outlined text-[12px]">chevron_right</li>
                        <li><a class="hover:text-secondary transition-colors" href="<%= ResolveUrl("~/Pages/Product.aspx") %>"><asp:Literal ID="litBreadcrumbCategory" runat="server"></asp:Literal></a></li>
                        <li class="material-symbols-outlined text-[12px]">chevron_right</li>
                        <li class="text-on-surface"><asp:Literal ID="litBreadcrumbProductName" runat="server"></asp:Literal></li>
                    </ol>
                </nav>

                <div class="grid grid-cols-1 lg:grid-cols-12 gap-14 items-start">
                    <div class="lg:col-span-7 grid grid-cols-1 gap-8">
                        <div class="relative group overflow-hidden lg:max-w-[78%] xl:max-w-[74%] mx-auto">
                            <asp:Image ID="imgProductMain" runat="server" AlternateText="Product Main View" CssClass="w-full aspect-[3/4] object-cover transition-transform duration-700 group-hover:scale-105" />
                            <asp:PlaceHolder ID="phLimitedBadge" runat="server" Visible="false">
                                <div class="absolute top-6 left-6 bg-secondary text-on-secondary px-4 py-1 text-[10px] tracking-widest font-bold uppercase">
                                    Limited Edition
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <div class="lg:col-span-5 lg:sticky lg:top-28 lg:scale-[0.84] lg:origin-top">
                        <div class="space-y-7">
                            <div>
                                <h1 class="text-4xl xl:text-5xl font-semibold tracking-tighter text-on-background mb-3"><asp:Literal ID="litProductName" runat="server"></asp:Literal></h1>
                                <p class="text-xl xl:text-2xl font-light text-secondary tracking-tight"><asp:Literal ID="litPrice" runat="server"></asp:Literal></p>
                            </div>

                            <div class="space-y-4">
                                <p class="text-on-surface-variant leading-relaxed text-sm max-w-md">
                                    <asp:Literal ID="litDescription" runat="server"></asp:Literal>
                                </p>
                            </div>

                            <div class="space-y-4">
                                <div class="flex justify-between items-end">
                                    <span class="text-[10px] tracking-widest uppercase font-bold text-on-surface">Select Size</span>
                                    <a class="text-[10px] tracking-widest uppercase text-on-surface-variant hover:text-secondary underline decoration-secondary/30 underline-offset-4" href="#">Size Guide</a>
                                </div>
                                <asp:DropDownList ID="ddlSelectedSize" runat="server" CssClass="w-full py-4 text-xs font-medium border border-outline-variant bg-transparent hover:border-secondary focus:border-secondary transition-all az-size-select">
                                    <asp:ListItem Value="S">S</asp:ListItem>
                                    <asp:ListItem Value="M" Selected="True">M</asp:ListItem>
                                    <asp:ListItem Value="L">L</asp:ListItem>
                                    <asp:ListItem Value="XL">XL</asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="space-y-2">
                                <span class="text-[10px] tracking-widest uppercase font-bold text-on-surface">Quantity</span>
                                <asp:TextBox ID="txtSelectedQuantity" runat="server" TextMode="Number" Text="1" min="1" CssClass="w-full max-w-32 py-3 px-3 text-xs font-medium border border-outline-variant bg-transparent hover:border-secondary focus:border-secondary transition-all"></asp:TextBox>
                            </div>

                            <div class="pt-4 flex flex-col gap-4">
                                <asp:Button ID="btnAddToCart" runat="server" Text="Add to Shopping Bag" CssClass="w-full py-5 bg-gradient-to-tr from-secondary to-secondary-container text-on-secondary font-bold uppercase tracking-[0.2em] text-xs transition-transform active:scale-[0.98]" OnClick="btnAddToCart_Click" />
                                <button class="w-full py-5 border border-outline-variant text-on-surface font-bold uppercase tracking-[0.2em] text-xs hover:bg-on-surface/5 transition-colors" type="button">
                                    Wishlist
                                </button>
                            </div>

                            <div class="pt-8 border-t border-outline-variant/20 space-y-4">
                                <div class="flex items-center text-[10px] tracking-widest text-on-surface-variant">
                                    <span class="material-symbols-outlined mr-3 text-sm">local_shipping</span>
                                    COMPLIMENTARY WORLDWIDE SHIPPING
                                </div>
                                <div class="flex items-center text-[10px] tracking-widest text-on-surface-variant">
                                    <span class="material-symbols-outlined mr-3 text-sm">verified</span>
                                    CERTIFIED SUSTAINABLE SOURCING
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <section class="mt-24 pt-16 border-t border-outline-variant/20">
                    <div class="flex flex-col md:flex-row justify-between items-baseline mb-10 gap-6">
                        <div>
                            <h2 class="text-3xl font-semibold tracking-tight mb-2">Similar Products</h2>
                            <p class="text-on-surface-variant text-sm">You might also like these curated picks.</p>
                        </div>
                    </div>
                    <asp:Repeater ID="rptSimilarProducts" runat="server">
                        <HeaderTemplate>
                            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <article class="bg-surface-container border border-outline-variant/15 overflow-hidden group">
                                <a href='<%# Eval("DetailsUrl") %>' class="block">
                                    <img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("ProductName") %>' class="w-full aspect-[4/4.6] object-cover transition-transform duration-500 group-hover:scale-105" />
                                </a>
                                <div class="p-4 space-y-1.5">
                                    <p class="text-[10px] uppercase tracking-[0.2em] text-on-surface-variant"><%# Eval("Category") %></p>
                                    <h3 class="text-base font-medium tracking-tight"><%# Eval("ProductName") %></h3>
                                    <p class="text-secondary text-xs font-semibold"><%# Eval("Price") %></p>
                                    <p class="text-on-surface-variant text-[10px] uppercase tracking-widest"><%# Eval("Status") %></p>
                                </div>
                            </article>
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </section>

                <section class="mt-32 pt-24 border-t border-outline-variant/20">
                    <div class="flex flex-col md:flex-row justify-between items-baseline mb-16 gap-8">
                        <div>
                            <h2 class="text-3xl font-semibold tracking-tight mb-2">Curated Feedback</h2>
                            <p class="text-on-surface-variant text-sm">4.9 Average Rating based on 124 verified purchasers.</p>
                        </div>
                        <button class="text-xs font-bold uppercase tracking-[0.2em] border-b-2 border-secondary pb-1 hover:text-secondary transition-colors" type="button">Write a Review</button>
                    </div>
                    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                        <div class="bg-surface-container p-10 flex flex-col justify-between group">
                            <div>
                                <div class="flex text-secondary mb-6 space-x-1">
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                </div>
                                <h3 class="text-lg font-medium mb-4">"Exceptional Drape"</h3>
                                <p class="text-on-surface-variant text-sm leading-relaxed italic">
                                    The weight of the wool is perfect. It holds its shape beautifully throughout the day. A truly architectural piece for my wardrobe.
                                </p>
                            </div>
                            <div class="mt-8 pt-8 border-t border-outline-variant/10 flex items-center justify-between">
                                <span class="text-[10px] tracking-widest uppercase font-bold">Julian V.</span>
                                <span class="text-[10px] tracking-widest uppercase text-on-surface-variant/50">March 2024</span>
                            </div>
                        </div>

                        <div class="bg-surface-container p-10 flex flex-col justify-between md:col-span-2 lg:col-span-1 border-l-4 border-secondary">
                            <div>
                                <div class="flex text-secondary mb-6 space-x-1">
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                </div>
                                <h3 class="text-lg font-medium mb-4">"A Lifetime Investment"</h3>
                                <p class="text-on-surface-variant text-sm leading-relaxed">
                                    I was hesitant about the price point but the craftsmanship justifies every cent. The silk lining is incredibly soft and the tailoring is pinpoint accurate.
                                </p>
                            </div>
                            <div class="mt-8 pt-8 border-t border-outline-variant/10 flex items-center justify-between">
                                <span class="text-[10px] tracking-widest uppercase font-bold">Marcus Chen</span>
                                <span class="text-[10px] tracking-widest uppercase text-on-surface-variant/50">Feb 2024</span>
                            </div>
                        </div>

                        <div class="bg-surface-container-low p-10 flex flex-col justify-between opacity-80">
                            <div>
                                <div class="flex text-secondary mb-6 space-x-1">
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm" style="font-variation-settings: 'FILL' 1;">star</span>
                                    <span class="material-symbols-outlined text-sm">star_half</span>
                                </div>
                                <h3 class="text-lg font-medium mb-4">"Sophisticated Utility"</h3>
                                <p class="text-on-surface-variant text-sm leading-relaxed">
                                    Warm, stylish, and durable. The deep navy is almost black, which I love. Slightly larger than expected, but ideal for layering.
                                </p>
                            </div>
                            <div class="mt-8 pt-8 border-t border-outline-variant/10 flex items-center justify-between">
                                <span class="text-[10px] tracking-widest uppercase font-bold">Elena S.</span>
                                <span class="text-[10px] tracking-widest uppercase text-on-surface-variant/50">Jan 2024</span>
                            </div>
                        </div>
                    </div>
                </section>
            </asp:PlaceHolder>
        </main>

        <footer class="bg-slate-50 dark:bg-slate-950 mt-32">
            <div class="bg-slate-200 dark:bg-slate-800 h-[1px]"></div>
            <div class="flex flex-col md:flex-row justify-between items-center px-12 py-20 w-full gap-8 max-w-[1920px] mx-auto">
                <div class="text-lg font-bold text-slate-900 dark:text-slate-50 uppercase tracking-tighter">
                    AttireZone
                </div>
                <div class="flex flex-wrap justify-center gap-8">
                    <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Sustainability</a>
                    <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Shipping</a>
                    <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Returns</a>
                    <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Privacy Policy</a>
                    <a class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200" href="#">Terms of Service</a>
                </div>
                <div class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500">
                    © 2024 AttireZone. All Rights Reserved.
                </div>
            </div>
        </footer>
    </form>
</body>
</html>
