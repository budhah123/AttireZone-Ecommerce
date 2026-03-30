<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs"
Inherits="AttireZone_Web_App.Admin.AdminLogin" %>

<!DOCTYPE html>
<html class="dark" lang="en" xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Executive Portal | AttireZone</title>
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
              "secondary-fixed-dim": "#e9c349",
              "on-surface-variant": "#c4c6cf",
              "on-primary-container": "#6f88ad",
              "on-secondary-fixed": "#241a00",
              "on-error": "#690005",
              "tertiary-container": "#1d1f1f",
              "surface-variant": "#353535",
              "surface-tint": "#afc8f0",
              "secondary-fixed": "#ffe088",
              "on-error-container": "#ffdad6",
              "error-container": "#93000a",
              surface: "#131313",
              outline: "#8e9198",
              "inverse-primary": "#476083",
              tertiary: "#c6c6c7",
              "on-surface": "#e2e2e2",
              "surface-container-high": "#2a2a2a",
              secondary: "#e9c349",
              "inverse-surface": "#e2e2e2",
              background: "#131313",
              "inverse-on-surface": "#303030",
              "secondary-container": "#af8d11",
              "surface-container-lowest": "#0e0e0e",
              "on-tertiary-container": "#858687",
              "primary-fixed": "#d4e3ff",
              "tertiary-fixed": "#e2e2e2",
              "surface-container": "#1f1f1f",
              "on-secondary-fixed-variant": "#574500",
              "on-tertiary-fixed-variant": "#454747",
              "surface-container-highest": "#353535",
              "outline-variant": "#43474e",
              "primary-fixed-dim": "#afc8f0",
              error: "#ffb4ab",
              "surface-container-low": "#1b1b1b",
              "surface-bright": "#393939",
              primary: "#afc8f0",
              "on-primary-fixed-variant": "#2f486a",
              "primary-container": "#001f3f",
              "on-primary": "#163152",
              "surface-dim": "#131313",
              "on-secondary-container": "#342800",
              "on-tertiary-fixed": "#1a1c1c",
              "on-primary-fixed": "#001c3a",
              "on-background": "#e2e2e2",
              "tertiary-fixed-dim": "#c6c6c7",
              "on-tertiary": "#2f3131",
              "on-secondary": "#3c2f00",
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
        font-weight: 200;
        font-style: normal;
        line-height: 1;
      }

      .luxury-gradient {
        background: linear-gradient(135deg, #e9c349 0%, #af8d11 100%);
      }

      .noir-overlay {
        background: radial-gradient(
          circle at center,
          rgba(0, 31, 63, 0.2) 0%,
          rgba(19, 19, 19, 1) 100%
        );
      }

      input:focus {
        outline: none;
        box-shadow: none;
      }
    </style>
  </head>
  <body
    class="bg-background text-on-background font-body selection:bg-secondary selection:text-on-secondary"
  >
    <form id="form1" runat="server">
      <nav
        class="fixed top-0 w-full z-50 bg-[#001f3f]/80 backdrop-blur-md dark:bg-[#0e0e0e]/80 shadow-[0_0_40px_rgba(0,0,0,0.08)]"
      >
        <div
          class="flex justify-between items-center px-8 py-6 w-full max-w-none"
        >
          <div class="text-2xl font-semibold tracking-[-0.02em] text-[#e2e2e2]">
            AttireZone
          </div>
          <div class="hidden md:flex gap-8 items-center">
            <a
              class="font-['Inter'] tracking-tight text-[14px] uppercase font-medium text-[#c4c6cf] hover:text-[#e2e2e2] transition-colors duration-300"
              href="#"
              >Collections</a
            >
            <a
              class="font-['Inter'] tracking-tight text-[14px] uppercase font-medium text-[#c4c6cf] hover:text-[#e2e2e2] transition-colors duration-300"
              href="#"
              >New Arrivals</a
            >
            <a
              class="font-['Inter'] tracking-tight text-[14px] uppercase font-medium text-[#c4c6cf] hover:text-[#e2e2e2] transition-colors duration-300"
              href="#"
              >Archive</a
            >
            <a
              class="font-['Inter'] tracking-tight text-[14px] uppercase font-medium text-[#c4c6cf] hover:text-[#e2e2e2] transition-colors duration-300"
              href="#"
              >Editorial</a
            >
          </div>
          <div class="flex gap-6 items-center">
            <button
              class="text-[#e9c349] dark:text-[#af8d11] hover:bg-[#e9c349]/5 transition-all duration-300 p-2"
              type="button"
            >
              <span class="material-symbols-outlined" data-icon="shopping_bag"
                >shopping_bag</span
              >
            </button>
            <button
              class="text-[#e9c349] dark:text-[#af8d11] hover:bg-[#e9c349]/5 transition-all duration-300 p-2"
              type="button"
            >
              <span class="material-symbols-outlined" data-icon="person"
                >person</span
              >
            </button>
          </div>
        </div>
      </nav>

      <main
        class="min-h-screen flex items-center justify-center pt-24 pb-12 px-6 relative overflow-hidden"
      >
        <div class="absolute inset-0 z-0 noir-overlay"></div>
        <div
          class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[800px] bg-primary-container/10 rounded-full blur-[120px] z-0"
        ></div>

        <div class="w-full max-w-md z-10">
          <div class="text-center mb-12">
            <span
              class="font-['Inter'] text-[12px] tracking-[0.3em] uppercase text-secondary mb-4 block"
              >Secure Environment</span
            >
            <h1
              class="text-4xl md:text-5xl font-bold tracking-[-0.03em] text-on-surface font-headline leading-tight"
            >
              Executive Portal
            </h1>
            <p class="mt-4 text-on-surface-variant font-light max-w-xs mx-auto">
              Please authenticate to access the AttireZone management console.
            </p>
          </div>

          <div class="bg-surface-container border-0 p-8 md:p-12 relative">
            <div
              class="absolute top-0 left-0 w-full h-[1px] bg-gradient-to-r from-transparent via-secondary/30 to-transparent"
            ></div>

            <div class="space-y-10">
              <div class="space-y-2 group">
                <label
                  class="font-['Inter'] text-[11px] tracking-[0.2em] uppercase text-on-surface-variant group-focus-within:text-secondary transition-colors duration-300"
                  for="txtAdminIdentifier"
                  >Admin Email</label
                >
                <asp:TextBox
                  ID="txtAdminIdentifier"
                  runat="server"
                  ClientIDMode="Static"
                  CssClass="w-full bg-transparent border-b border-outline-variant/30 py-3 text-on-surface placeholder:text-outline-variant/50 focus:border-secondary transition-all duration-500 rounded-none text-sm"
                  MaxLength="150"
                  placeholder="executive@attirezone.com"
                ></asp:TextBox>
                <asp:RequiredFieldValidator
                  ID="rfvAdminIdentifier"
                  runat="server"
                  ControlToValidate="txtAdminIdentifier"
                  ErrorMessage="Admin email or username is required."
                  Display="Dynamic"
                  CssClass="block mt-2 text-[10px] tracking-[0.14em] uppercase text-error"
                  ValidationGroup="AdminLoginForm"
                ></asp:RequiredFieldValidator>
              </div>

              <div class="space-y-2 group">
                <label
                  class="font-['Inter'] text-[11px] tracking-[0.2em] uppercase text-on-surface-variant group-focus-within:text-secondary transition-colors duration-300"
                  for="txtAdminSecretKey"
                  >Secret Key</label
                >
                <div class="relative">
                  <asp:TextBox
                    ID="txtAdminSecretKey"
                    runat="server"
                    ClientIDMode="Static"
                    TextMode="Password"
                    CssClass="w-full bg-transparent border-b border-outline-variant/30 py-3 text-on-surface placeholder:text-outline-variant/50 focus:border-secondary transition-all duration-500 rounded-none text-sm pr-10"
                    MaxLength="80"
                    placeholder="************"
                  ></asp:TextBox>
                  <button
                    class="absolute right-0 top-1/2 -translate-y-1/2 text-on-surface-variant hover:text-secondary transition-colors"
                    id="btnToggleSecret"
                    type="button"
                  >
                    <span
                      class="material-symbols-outlined text-sm"
                      data-icon="visibility"
                      id="secretToggleIcon"
                      >visibility</span
                    >
                  </button>
                </div>
                <asp:RequiredFieldValidator
                  ID="rfvAdminSecretKey"
                  runat="server"
                  ControlToValidate="txtAdminSecretKey"
                  ErrorMessage="Secret key is required."
                  Display="Dynamic"
                  CssClass="block mt-2 text-[10px] tracking-[0.14em] uppercase text-error"
                  ValidationGroup="AdminLoginForm"
                ></asp:RequiredFieldValidator>
              </div>

              <div class="pt-4">
                <asp:Button
                  ID="btnAdminLogin"
                  runat="server"
                  Text="Secure Access"
                  CssClass="w-full luxury-gradient text-on-secondary font-bold tracking-[0.1em] uppercase py-5 text-sm hover:brightness-110 active:scale-[0.98] transition-all duration-200"
                  OnClick="btnAdminLogin_Click"
                  ValidationGroup="AdminLoginForm"
                  UseSubmitBehavior="false"
                />

                <asp:Label
                  ID="lblAdminLoginMessage"
                  runat="server"
                  Visible="false"
                  CssClass="block mt-5 text-center text-[10px] tracking-[0.14em] uppercase text-error"
                ></asp:Label>

                <div
                  class="mt-8 flex justify-between items-center text-[10px] tracking-widest uppercase text-on-surface-variant/60"
                >
                  <a class="hover:text-secondary transition-colors" href="#"
                    >Forgot Key?</a
                  >
                  <div class="flex items-center gap-2">
                    <span
                      class="w-1.5 h-1.5 rounded-full bg-emerald-500/80"
                    ></span>
                    <span>System Operational</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="mt-12 flex justify-center items-center gap-8 opacity-20">
            <div class="h-[1px] w-12 bg-outline-variant"></div>
            <div class="text-[10px] tracking-[0.5em] uppercase">
              Private Terminal
            </div>
            <div class="h-[1px] w-12 bg-outline-variant"></div>
          </div>
        </div>
      </main>

      <footer
        class="w-full border-t border-[#1f1f1f] bg-[#0e0e0e] dark:bg-[#0a0a0a]"
      >
        <div
          class="flex flex-col items-center justify-center py-12 px-8 w-full gap-8"
        >
          <div class="text-xl font-bold text-[#e2e2e2]">AttireZone</div>
          <div class="flex flex-wrap justify-center gap-8">
            <a
              class="font-['Inter'] text-[12px] tracking-widest uppercase text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-200"
              href="#"
              >Privacy Policy</a
            >
            <a
              class="font-['Inter'] text-[12px] tracking-widest uppercase text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-200"
              href="#"
              >Terms of Service</a
            >
            <a
              class="font-['Inter'] text-[12px] tracking-widest uppercase text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-200"
              href="#"
              >Contact</a
            >
            <a
              class="font-['Inter'] text-[12px] tracking-widest uppercase text-[#c4c6cf] hover:text-[#e9c349] transition-colors duration-200"
              href="#"
              >Sustainability</a
            >
          </div>
          <div
            class="font-['Inter'] text-[12px] tracking-widest uppercase text-[#af8d11] opacity-80"
          >
            &copy; 2024 AttireZone. All Rights Reserved.
          </div>
        </div>
      </footer>
    </form>

    <script>
      (function () {
        var toggleButton = document.getElementById("btnToggleSecret");
        var secretInput = document.getElementById("txtAdminSecretKey");
        var icon = document.getElementById("secretToggleIcon");

        if (!toggleButton || !secretInput || !icon) {
          return;
        }

        toggleButton.addEventListener("click", function () {
          var showSecret = secretInput.type === "password";
          secretInput.type = showSecret ? "text" : "password";
          icon.textContent = showSecret ? "visibility_off" : "visibility";
        });
      })();
    </script>
  </body>
</html>
