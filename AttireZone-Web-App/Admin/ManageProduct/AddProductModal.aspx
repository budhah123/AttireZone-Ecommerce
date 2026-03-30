<%@ Page Language="C#" AutoEventWireup="true"
CodeBehind="AddProductModal.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageProduct.AddProductModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" class="dark" lang="en">
  <head runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <title>The Silent Curator - Add New Product</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link
      href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&amp;display=swap"
      rel="stylesheet"
    />
    <link
      href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap"
      rel="stylesheet"
    />
    <script id="tailwind-config">
      tailwind.config = {
        darkMode: "class",
        theme: {
          extend: {
            colors: {
              "outline-variant": "#43474e",
              "on-surface": "#e2e2e2",
              "on-primary-fixed": "#001c3a",
              "inverse-surface": "#e2e2e2",
              "secondary-fixed": "#ffe088",
              "surface-container-low": "#1b1b1b",
              "surface-variant": "#353535",
              "surface-tint": "#afc8f0",
              "on-error": "#690005",
              "error-container": "#93000a",
              "tertiary-container": "#1d1f1f",
              "inverse-primary": "#476083",
              "surface-bright": "#393939",
              "on-tertiary-fixed": "#1a1c1c",
              "primary-container": "#001f3f",
              "tertiary-fixed": "#e2e2e2",
              tertiary: "#c6c6c7",
              "tertiary-fixed-dim": "#c6c6c7",
              "secondary-container": "#af8d11",
              "on-surface-variant": "#c4c6cf",
              secondary: "#e9c349",
              "primary-fixed-dim": "#afc8f0",
              "on-tertiary": "#2f3131",
              "surface-container-highest": "#353535",
              "on-primary-fixed-variant": "#2f486a",
              "on-primary-container": "#6f88ad",
              "on-background": "#e2e2e2",
              "surface-container": "#1f1f1f",
              primary: "#afc8f0",
              "surface-container-high": "#2a2a2a",
              "on-secondary-fixed": "#241a00",
              "inverse-on-surface": "#303030",
              "on-primary": "#163152",
              "on-secondary-container": "#342800",
              "on-secondary-fixed-variant": "#574500",
              "secondary-fixed-dim": "#e9c349",
              error: "#ffb4ab",
              "on-secondary": "#3c2f00",
              "on-error-container": "#ffdad6",
              "on-tertiary-fixed-variant": "#454747",
              "on-tertiary-container": "#858687",
              "surface-dim": "#131313",
              "primary-fixed": "#d4e3ff",
              surface: "#131313",
              outline: "#8e9198",
              "surface-container-lowest": "#0e0e0e",
              background: "#131313",
            },
            fontFamily: {
              headline: ["Inter"],
              body: ["Inter"],
              label: ["Inter"],
            },
            borderRadius: {
              DEFAULT: "0px",
              lg: "0px",
              xl: "0px",
              full: "9999px",
            },
          },
        },
      };
    </script>
    <style>
      .material-symbols-outlined {
        font-variation-settings:
          "FILL" 0,
          "wght" 200,
          "GRAD" 0,
          "opsz" 24;
      }

      .no-scrollbar::-webkit-scrollbar {
        display: none;
      }
      .glass-effect {
        backdrop-filter: blur(12px);
      }
    </style>
  </head>
  <body
    class="bg-background text-on-surface font-body selection:bg-secondary selection:text-on-secondary overflow-hidden"
  >
    <form id="form1" runat="server" enctype="multipart/form-data">
      <!-- Top Navigation Bar -->
      <nav
        class="bg-[#131313]/80 backdrop-blur-md text-[#e9c349] font-['Inter'] tracking-tight fixed top-0 w-full z-40 border-b border-[#c4c6cf]/10 shadow-[0_0_40px_rgba(0,0,0,0.08)] flex justify-between items-center px-8 h-16 w-full"
      >
        <div
          class="text-lg font-semibold tracking-tighter text-[#e2e2e2] uppercase"
        >
          The Silent Curator
        </div>
        <div class="hidden md:flex items-center gap-8">
          <a
            class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300"
            href="#"
            >Collections</a
          >
          <a
            class="text-[#e9c349] border-b border-[#e9c349] transition-colors duration-300"
            href="#"
            >Inventory</a
          >
          <a
            class="text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-300"
            href="#"
            >Analytics</a
          >
        </div>
        <div class="flex items-center gap-6">
          <span
            class="material-symbols-outlined text-[#c4c6cf] hover:text-[#e9c349] cursor-pointer transition-all duration-200"
            >notifications</span
          >
          <span
            class="material-symbols-outlined text-[#c4c6cf] hover:text-[#e9c349] cursor-pointer transition-all duration-200"
            >settings</span
          >
          <div class="w-8 h-8 bg-surface-container overflow-hidden">
            <img
              alt="Admin Profile"
              class="w-full h-full object-cover grayscale opacity-80"
              data-alt="close-up portrait of a professional male art curator with glasses in a dimly lit studio"
              src="https://lh3.googleusercontent.com/aida-public/AB6AXuBmXLi63HoxjMwZ_TVRzcaYayI0-MQbrPTWpE4wCBX6sX9caBoGHfnp4ZK2RMnqKxBRGNnESRvFmDAiNZ7Du5JnPDILiT_V2ig9AkX-ZD9Kf61G5XCLTvrCPJ9aeb1wLNfmlDsK8_3L_R-YS9IZbaVtGyUVsDWAGPEisKDTukQclb3RZ2jHKeEhKI57atzK_KNjKhrefkVu_NlUUWc1M29UFzY28Hfta4AMi56I-WMmgxo3QAPcthb5fz517wELDBiIOIVP4vyXgs8"
            />
          </div>
        </div>
      </nav>

      <!-- Side Navigation Bar -->
      <aside
        class="bg-[#0e0e0e] h-screen w-64 fixed left-0 top-0 z-30 flex flex-col py-8 px-4 gap-4 border-r border-[#c4c6cf]/5 hidden md:flex"
      >
        <div class="mt-16 mb-8">
          <div class="text-xl font-bold text-[#e2e2e2]">Editorial Admin</div>
          <div
            class="text-[10px] text-[#e9c349] tracking-[0.2em] uppercase font-bold opacity-70"
          >
            Premium Tier
          </div>
        </div>
        <div class="flex flex-col gap-1">
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="#"
          >
            <span class="material-symbols-outlined">dashboard</span> Dashboard
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#e9c349] bg-[#1f1f1f] font-bold font-['Inter'] text-sm tracking-wide uppercase"
            href="#"
          >
            <span
              class="material-symbols-outlined"
              style="font-variation-settings: &quot;FILL&quot; 1"
              >inventory_2</span
            >
            Products
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="#"
          >
            <span class="material-symbols-outlined">shopping_bag</span> Orders
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="#"
          >
            <span class="material-symbols-outlined">group</span> Customers
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="#"
          >
            <span class="material-symbols-outlined">settings</span> Settings
          </a>
        </div>
        <a
          class="mt-auto bg-secondary text-on-secondary py-3 text-xs font-bold tracking-[0.15em] uppercase hover:bg-secondary-container transition-colors text-center"
          href="/Admin/ManageProduct/AddProductModal.aspx"
          >Add New Product</a
        >
      </aside>

      <!-- Main Content Background (Mock Inventory) -->
      <main class="pl-64 pt-16 h-screen w-full bg-background overflow-y-auto">
        <div class="p-12 space-y-12 opacity-30 grayscale pointer-events-none">
          <div class="flex justify-between items-end">
            <div>
              <h1 class="text-4xl font-headline font-semibold tracking-tight">
                Active Inventory
              </h1>
              <p class="text-on-surface-variant mt-2 max-w-xl">
                Curating the finest essential garments for the modern
                silhouette.
              </p>
            </div>
          </div>
          <div class="grid grid-cols-3 gap-8">
            <div class="space-y-4">
              <div class="aspect-[3/4] bg-surface-container relative">
                <img
                  class="w-full h-full object-cover"
                  data-alt="minimalist studio photography of a sharp black tailored coat on a designer mannequin against a deep charcoal background"
                  src="https://lh3.googleusercontent.com/aida-public/AB6AXuB_MGvOLeZkih-7bNZ7RvichOfCCEVB3ewJDIcUcUC6-3df_I6oJVOVnPy30O0lji32P2X9ELFO29Th_SEIWPRPhFlgadLNCe9dO5RZPdkQh_QABg-HNtxD4xItEFOJRrH7c7zTecnIcz0VdzYL9hE2xnHdzun2zDvBeoXxGUJ26hB2GkP0nG0zZoQUw5ofkgsxqaEY80P0znobPRkbYY9CKwgJGu1XzhBGVnSfOgStUUruRgaTzLlSL_qtp6KHrMzDDK_GFWXKX2U"
                />
              </div>
              <div class="flex justify-between items-baseline">
                <h3 class="text-lg font-medium">Obsidian Tailored Coat</h3>
                <span class="text-on-surface-variant">$1,250</span>
              </div>
            </div>
            <div class="space-y-4">
              <div class="aspect-[3/4] bg-surface-container">
                <img
                  class="w-full h-full object-cover"
                  data-alt="high fashion editorial shot of a textured charcoal wool sweater on a minimalist hangers in a gallery-like setting"
                  src="https://lh3.googleusercontent.com/aida-public/AB6AXuDaTNVyUAvJ4IXr2M7VrsGSrrk6b5C35nGboSvHP35i27qdYt9I_GrlS4aMYbrQNUvsy5OU0T_5AtvKGfuXwoamQqrT5nniWhIgLlYS8l4xU2R5QaNnwoqfWU-Zo_0jkLuUO4xO-0QFuytrp5J1jVb_o3LYUQv-lBorJiFPHk14u1R-rOZniVG1ZIM_QyLVO7aTIwfGkPsDudsCFTDwx8_456jF6_fqNXLeguSwbH_7oy4924XZhoA6HGwYAxFqeSRv2cLoMUzOoBg"
                />
              </div>
              <div class="flex justify-between items-baseline">
                <h3 class="text-lg font-medium">Merino Structure Knit</h3>
                <span class="text-on-surface-variant">$480</span>
              </div>
            </div>
            <div class="space-y-4">
              <div class="aspect-[3/4] bg-surface-container">
                <img
                  class="w-full h-full object-cover"
                  data-alt="close-up of elegant wide-leg pleated trousers in dark navy silk against a neutral architectural background"
                  src="https://lh3.googleusercontent.com/aida-public/AB6AXuAMQ-dDXqLxp0JyAn7OdbwNtX8pRW0TRoi3lCtFrr-EpKP7DDS6PFFPmnLtUcEUfZRjm7996VqH6oBXdw0Mravan1mcjqJxT3NX4pGW0rJIuwTFV3W2ru5bK0Pm4FqihvAtTjh_-za06iPwpPsHhqklTPQWZgm3r_taeaNrHGvHIRLd_wPLnsI8VedvVX2Db7nOJ1cteIEz9x_wuLKV-T_TVj0evtFjelmCSnas4fSO-w5XN0YkOuK5d3COQXgRHNme5bNuftpNuqw"
                />
              </div>
              <div class="flex justify-between items-baseline">
                <h3 class="text-lg font-medium">Midnight Silk Trousers</h3>
                <span class="text-on-surface-variant">$620</span>
              </div>
            </div>
          </div>
        </div>
      </main>

      <!-- Modal Overlay -->
      <div
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/90 backdrop-blur-sm p-1.5 sm:p-3"
      >
        <!-- Modal Container -->
        <div
          class="bg-surface-dim w-full max-w-[min(96vw,64rem)] max-h-[calc(100vh-1.5rem)] sm:max-h-[calc(100vh-2rem)] overflow-y-auto border border-outline-variant/20 relative shadow-2xl flex flex-col md:flex-row no-scrollbar origin-center scale-[0.96] sm:scale-[0.94]"
        >
          <!-- Close Button -->
          <a
            class="absolute top-4 right-4 sm:top-5 sm:right-5 text-on-surface-variant hover:text-secondary transition-colors z-10"
            href="/Admin/ManageProduct/ManageProducts.aspx"
          >
            <span class="material-symbols-outlined text-2xl">close</span>
          </a>

          <!-- Image/Preview Section -->
          <div
            class="w-full md:w-2/5 bg-surface-container-lowest p-4 sm:p-5 lg:p-6 flex flex-col justify-between border-r border-outline-variant/10"
          >
            <div>
              <h2
                class="text-xs font-bold tracking-[0.2em] text-secondary uppercase mb-6"
              >
                Visual Asset
              </h2>
              <div
                class="aspect-[4/5] bg-surface border border-dashed border-outline-variant/30 flex flex-col items-center justify-center group cursor-pointer hover:border-secondary transition-colors relative overflow-hidden"
              >
                <div
                  class="flex flex-col items-center gap-3 p-6 sm:p-8 text-center pointer-events-none"
                >
                  <span
                    class="material-symbols-outlined text-4xl text-on-surface-variant group-hover:text-secondary transition-colors"
                    >cloud_upload</span
                  >
                  <span
                    class="text-sm text-on-surface-variant uppercase tracking-widest"
                    >Upload Master Shot</span
                  >
                </div>
                <input
                  id="fuImage"
                  runat="server"
                  type="file"
                  accept=".jpg,.jpeg,.png,.webp"
                  class="absolute inset-0 opacity-0 cursor-pointer"
                />
                <div
                  class="absolute inset-0 bg-surface-container opacity-20 pointer-events-none"
                ></div>
              </div>
              <p
                class="text-[11px] text-on-surface-variant mt-4 uppercase tracking-tighter leading-relaxed"
              >
                Recommended: 2400x3200px, Minimalist background, RAW or
                high-quality JPEG.
              </p>
            </div>
            <div class="mt-6 space-y-3">
              <div class="h-[1px] bg-outline-variant/20 w-full"></div>
              <div
                class="flex justify-between items-center text-[10px] uppercase tracking-widest text-on-surface-variant"
              >
                <span>Status</span>
                <span class="text-secondary font-bold">New Entry</span>
              </div>
            </div>
          </div>

          <!-- Form Section -->
          <div class="w-full md:w-3/5 p-4 sm:p-5 lg:p-7 xl:p-8">
            <header class="mb-6">
              <h1
                class="text-xl md:text-2xl font-headline font-semibold tracking-tight uppercase"
              >
                <asp:Literal
                  ID="litFormHeading"
                  runat="server"
                  Text="Add New Product"
                />
              </h1>
              <div class="w-12 h-1 bg-secondary mt-4"></div>
            </header>

            <asp:Panel
              ID="pnlMessage"
              runat="server"
              Visible="false"
              CssClass="mb-6 border border-error/40 bg-error-container/30 px-4 py-3 text-xs uppercase tracking-widest text-error"
            >
              <asp:Literal ID="litMessage" runat="server" />
            </asp:Panel>

            <div class="space-y-5 sm:space-y-6">
              <!-- Product Name -->
              <div class="relative group">
                <label
                  class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                  >Product Name</label
                >
                <input
                  id="txtProductName"
                  runat="server"
                  class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all uppercase tracking-wide"
                  maxlength="200"
                  placeholder="e.g., LITHIC SHELL JACKET"
                  type="text"
                />
              </div>

              <div class="grid grid-cols-2 gap-4 sm:gap-5">
                <!-- Price -->
                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Retail Price (USD)</label
                  >
                  <input
                    id="txtPrice"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all"
                    min="0"
                    placeholder="0.00"
                    step="0.01"
                    type="number"
                  />
                </div>

                <!-- Status -->
                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Inventory Status</label
                  >
                  <select
                    id="ddlStatus"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface appearance-none cursor-pointer uppercase text-xs tracking-widest"
                  >
                    <option class="bg-surface-container" value="In Stock">
                      In Stock
                    </option>
                    <option class="bg-surface-container" value="Low Stock">
                      Low Stock
                    </option>
                    <option class="bg-surface-container" value="Out Of Stock">
                      Out of Stock
                    </option>
                    <option class="bg-surface-container" value="Coming Soon">
                      Coming Soon
                    </option>
                  </select>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4 sm:gap-5">
                <!-- Edition -->
                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Edition Type</label
                  >
                  <select
                    id="ddlEdition"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface appearance-none cursor-pointer uppercase text-xs tracking-widest"
                  >
                    <option class="bg-surface-container" value="Standard">
                      Standard
                    </option>
                    <option
                      class="bg-surface-container"
                      value="Limited Edition"
                    >
                      Limited Edition
                    </option>
                    <option class="bg-surface-container" value="Exclusive">
                      Exclusive
                    </option>
                  </select>
                </div>

                <!-- Size -->
                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Size Architecture</label
                  >
                  <select
                    id="ddlSize"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface appearance-none cursor-pointer uppercase text-xs tracking-widest"
                  >
                    <option class="bg-surface-container" value="Small">
                      Small
                    </option>
                    <option class="bg-surface-container" value="Medium">
                      Medium
                    </option>
                    <option class="bg-surface-container" value="Large">
                      Large
                    </option>
                    <option class="bg-surface-container" value="Extra Large">
                      Extra Large
                    </option>
                  </select>
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4 sm:gap-5">
                <!-- Category -->
                <div class="relative group col-span-2">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Category</label
                  >
                  <select
                    id="ddlCategory"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface appearance-none cursor-pointer uppercase text-xs tracking-widest"
                  >
                    <option class="bg-surface-container" value="">
                      Select Category
                    </option>
                  </select>
                </div>
              </div>

              <!-- Description -->
              <div class="relative group">
                <label
                  class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                  >Curator's Notes (Description)</label
                >
                <textarea
                  id="txtDescription"
                  runat="server"
                  class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2.5 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all resize-none"
                  maxlength="4000"
                  placeholder="Describe the silhouette, materiality, and craftsmanship..."
                  rows="3"
                ></textarea>
              </div>

              <!-- Action Buttons -->
              <div class="flex flex-col sm:flex-row gap-4 pt-2">
                <button
                  id="btnAddProduct"
                  runat="server"
                  onserverclick="btnAddProduct_ServerClick"
                  class="flex-1 bg-secondary text-on-secondary py-2.5 px-8 text-xs font-bold tracking-[0.2em] uppercase transition-all hover:bg-secondary-container active:scale-[0.98]"
                  type="submit"
                >
                  Add Product
                </button>
                <a
                  class="flex-1 border border-secondary text-secondary py-2.5 px-8 text-xs font-bold tracking-[0.2em] uppercase transition-all hover:bg-secondary/5 active:scale-[0.98] text-center"
                  href="/Admin/ManageProduct/ManageProducts.aspx"
                >
                  Cancel
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </form>
  </body>
</html>
