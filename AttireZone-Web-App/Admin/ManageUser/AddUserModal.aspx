<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUserModal.aspx.cs"
Inherits="AttireZone_Web_App.Admin.ManageUser.AddUserModal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" class="dark" lang="en">
  <head runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <title>AttireZone | Add User</title>
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
    </style>
  </head>
  <body
    class="bg-background text-on-surface font-body selection:bg-secondary selection:text-on-secondary overflow-hidden"
  >
    <form id="form1" runat="server">
      <nav
        class="bg-[#131313]/80 backdrop-blur-md text-[#e9c349] font-['Inter'] tracking-tight fixed top-0 w-full z-40 border-b border-[#c4c6cf]/10 shadow-[0_0_40px_rgba(0,0,0,0.08)] flex justify-between items-center px-8 h-16 w-full"
      >
        <div
          class="text-lg font-semibold tracking-tighter text-[#e2e2e2] uppercase"
        >
          AttireZone
        </div>
        <div class="flex items-center gap-6">
          <span
            class="material-symbols-outlined text-[#c4c6cf] hover:text-[#e9c349] cursor-pointer transition-all duration-200"
            >settings</span
          >
          <span
            class="material-symbols-outlined text-[#c4c6cf] hover:text-[#e9c349] cursor-pointer transition-all duration-200"
            >account_circle</span
          >
        </div>
      </nav>

      <aside
        class="bg-[#0e0e0e] h-screen w-64 fixed left-0 top-0 z-30 flex flex-col py-8 px-4 gap-4 border-r border-[#c4c6cf]/5 hidden md:flex"
      >
        <div class="mt-16 mb-8">
          <div class="text-xl font-bold text-[#e2e2e2]">Admin Console</div>
          <div
            class="text-[10px] text-[#e9c349] tracking-[0.2em] uppercase font-bold opacity-70"
          >
            AttireZone Portal
          </div>
        </div>
        <div class="flex flex-col gap-1">
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="/Admin/Dashboard.aspx"
          >
            <span class="material-symbols-outlined">dashboard</span> Dashboard
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#c4c6cf] hover:bg-[#1f1f1f] hover:text-[#e2e2e2] transition-all font-['Inter'] text-sm tracking-wide uppercase"
            href="/Admin/ManageProduct/ManageProducts.aspx"
          >
            <span class="material-symbols-outlined">inventory_2</span> Products
          </a>
          <a
            class="flex items-center gap-3 py-3 px-4 text-[#e9c349] bg-[#1f1f1f] font-bold font-['Inter'] text-sm tracking-wide uppercase"
            href="/Admin/ManageUser/ManageUser.aspx"
          >
            <span
              class="material-symbols-outlined"
              style="font-variation-settings: &quot;FILL&quot; 1"
              >group</span
            >
            User Management
          </a>
        </div>
      </aside>

      <main class="pl-64 pt-16 h-screen w-full bg-background overflow-y-auto">
        <div class="p-8 space-y-8 opacity-30 grayscale pointer-events-none">
          <div>
            <h1 class="text-3xl font-headline font-semibold tracking-tight">
              System Access Control
            </h1>
            <p class="text-on-surface-variant mt-2 max-w-xl">
              Configure platform users, privileges, and account lifecycle
              controls.
            </p>
          </div>
        </div>
      </main>

      <div
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/90 backdrop-blur-sm p-1 sm:p-2.5"
      >
        <div
          class="bg-surface-dim w-full max-w-[min(95vw,58rem)] max-h-[calc(100vh-1rem)] sm:max-h-[calc(100vh-1.5rem)] overflow-y-auto border border-outline-variant/20 relative shadow-2xl flex flex-col md:flex-row no-scrollbar origin-center scale-[0.95] sm:scale-[0.93]"
        >
          <a
            class="absolute top-3 right-3 sm:top-4 sm:right-4 text-on-surface-variant hover:text-secondary transition-colors z-10"
            href="/Admin/ManageUser/ManageUser.aspx"
          >
            <span class="material-symbols-outlined text-xl sm:text-2xl"
              >close</span
            >
          </a>

          <div
            class="w-full md:w-2/5 bg-surface-container-lowest p-3 sm:p-4 lg:p-5 flex flex-col justify-between border-r border-outline-variant/10"
          >
            <div>
              <h2
                class="text-xs font-bold tracking-[0.2em] text-secondary uppercase mb-4"
              >
                Access Brief
              </h2>
              <div
                class="aspect-[4/5] bg-surface border border-dashed border-outline-variant/30 flex flex-col items-center justify-center"
              >
                <div
                  class="flex flex-col items-center gap-2.5 p-4 sm:p-6 text-center"
                >
                  <span
                    class="material-symbols-outlined text-3xl text-on-surface-variant"
                    >person_add</span
                  >
                  <span
                    class="text-xs text-on-surface-variant uppercase tracking-widest"
                    >Invite New User</span
                  >
                </div>
                <div
                  class="absolute inset-0 bg-surface-container opacity-20 pointer-events-none"
                ></div>
              </div>
              <p
                class="text-[10px] text-on-surface-variant mt-3 uppercase tracking-tighter leading-relaxed"
              >
                Create a new account and assign the correct permission role.
              </p>
            </div>
            <div class="mt-4 space-y-2">
              <div class="h-[1px] bg-outline-variant/20 w-full"></div>
              <div
                class="flex justify-between items-center text-[10px] uppercase tracking-widest text-on-surface-variant"
              >
                <span>Status</span>
                <span class="text-secondary font-bold">New Invitation</span>
              </div>
            </div>
          </div>

          <div class="w-full md:w-3/5 p-3 sm:p-4 lg:p-5 xl:p-6">
            <header class="mb-4">
              <h1
                class="text-lg md:text-xl font-headline font-semibold tracking-tight uppercase"
              >
                <asp:Literal
                  ID="litFormHeading"
                  runat="server"
                  Text="Add User"
                />
              </h1>
              <div class="w-12 h-1 bg-secondary mt-3"></div>
            </header>

            <asp:Panel
              ID="pnlMessage"
              runat="server"
              Visible="false"
              CssClass="mb-4 border border-error/40 bg-error-container/30 px-3 py-2.5 text-xs uppercase tracking-widest text-error"
            >
              <asp:Literal ID="litMessage" runat="server" />
            </asp:Panel>

            <div class="space-y-4 sm:space-y-5">
              <div class="relative group">
                <label
                  class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                  >Full Name</label
                >
                <input
                  id="txtFullName"
                  runat="server"
                  class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all uppercase tracking-wide"
                  maxlength="100"
                  placeholder="e.g., ALEXANDER STONE"
                  type="text"
                />
              </div>

              <div class="relative group">
                <label
                  class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                  >Email</label
                >
                <input
                  id="txtEmail"
                  runat="server"
                  class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all"
                  maxlength="150"
                  placeholder="name@attirezone.com"
                  type="email"
                />
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Password</label
                  >
                  <input
                    id="txtPassword"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2 px-0 focus:ring-0 focus:border-secondary text-on-surface placeholder:text-on-surface-variant/30 transition-all"
                    maxlength="200"
                    placeholder="Min 6 characters (optional when editing)"
                    type="password"
                  />
                </div>

                <div class="relative group">
                  <label
                    class="block text-[10px] font-bold uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                    >Role</label
                  >
                  <select
                    id="ddlRole"
                    runat="server"
                    class="w-full bg-transparent border-0 border-b border-outline-variant/40 py-2 px-0 focus:ring-0 focus:border-secondary text-on-surface appearance-none cursor-pointer uppercase text-xs tracking-widest"
                  >
                    <option class="bg-surface-container" value="Customer">
                      Customer
                    </option>
                    <option class="bg-surface-container" value="Admin">
                      Admin
                    </option>
                  </select>
                </div>
              </div>

              <div class="flex flex-col sm:flex-row gap-3 pt-1">
                <button
                  id="btnAddUser"
                  runat="server"
                  onserverclick="btnAddUser_ServerClick"
                  class="flex-1 bg-secondary text-on-secondary py-2 px-6 text-xs font-bold tracking-[0.2em] uppercase transition-all hover:bg-secondary-container active:scale-[0.98]"
                  type="submit"
                >
                  Add User
                </button>
                <a
                  class="flex-1 border border-secondary text-secondary py-2 px-6 text-xs font-bold tracking-[0.2em] uppercase transition-all hover:bg-secondary/5 active:scale-[0.98] text-center"
                  href="/Admin/ManageUser/ManageUser.aspx"
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
