<%@ Page Title="Sign In | AttireZone" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
Inherits="AttireZone_Web_App.Auth.Login" %>

<asp:Content
  ID="LoginStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <link
    href="/Assets/CSS/authentication.css"
    rel="stylesheet"
    type="text/css"
  />
</asp:Content>

<asp:Content ID="LoginMain" ContentPlaceHolderID="MainContent" runat="server">
  <main
    class="az-auth-login-shell min-h-screen flex flex-col lg:flex-row items-stretch overflow-hidden"
  >
    <section
      class="hidden lg:flex w-1/2 relative bg-primary-container overflow-hidden"
    >
      <img
        class="absolute inset-0 w-full h-full object-cover mix-blend-overlay opacity-60"
        alt="Editorial model in dark AttireZone aesthetic"
        src="https://lh3.googleusercontent.com/aida-public/AB6AXuDCBNAoSIvxx-dfX9ob9yG1Z-OtRETsHV0jC5vtbh9VAKPHFpbVaKjgqt7Sf8gl0eSmMtAlRkBVZxRXPzNmkEbgabr-2uN6EaUsNQBr0vPItHYT1hQDHhGytJ3MmwhTBipmZTcELSqtB5T3NvHl8wUn0ME1QMe6pj_8FlE2uAAauVcdscFbenMAe5tyqMZnREBr5HqaR1wtrSbWqfC8y81mtEdfqWz3tuUkCzQP_Vnl4GjALWj2JiVXSuvi3QcitLWokl1h5Dq0yAc"
      />
      <div class="relative z-10 p-20 flex flex-col justify-end h-full">
        <p class="text-secondary text-xs uppercase tracking-[0.2em] mb-4">
          Established MMXXIV
        </p>
        <h1
          class="text-6xl font-extrabold tracking-tighter text-on-surface mb-6 leading-none"
        >
          THE NEW<br />STANDARD OF<br />LUXURY.
        </h1>
        <div class="w-24 h-1 bg-secondary"></div>
      </div>
    </section>

    <section
      class="flex-1 flex items-center justify-center p-6 md:p-12 lg:p-24 bg-surface-dim"
    >
      <div class="w-full max-w-md space-y-8">
        <div class="flex space-x-8 border-b border-outline-variant/20 pb-2">
          <a
            class="text-on-surface font-semibold tracking-tight text-lg border-b-2 border-secondary pb-2 transition-all"
            href="/Auth/Login.aspx"
            >Sign In</a
          >
          <a
            class="text-on-surface-variant/60 font-semibold tracking-tight text-lg hover:text-on-surface pb-2 transition-all"
            href="/Auth/Register.aspx"
            >Create Account</a
          >
        </div>

        <div class="space-y-8">
          <div class="space-y-6">
            <div class="relative group">
              <label
                class="block text-xs uppercase tracking-widest text-on-surface-variant mb-2 transition-colors group-focus-within:text-secondary"
                for="txtLoginEmail"
                >Email Address</label
              >
              <asp:TextBox
                ID="txtLoginEmail"
                runat="server"
                ClientIDMode="Static"
                TextMode="Email"
                CssClass="w-full bg-transparent border-0 border-b border-outline-variant/30 px-0 py-3 text-on-surface placeholder:text-on-surface-variant/30 focus:ring-0 focus:border-secondary transition-all outline-none"
                MaxLength="150"
                placeholder="curator@attirezone.com"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="rfvLoginEmail"
                runat="server"
                ControlToValidate="txtLoginEmail"
                ErrorMessage="Email is required."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="LoginForm"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="revLoginEmail"
                runat="server"
                ControlToValidate="txtLoginEmail"
                ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
                ErrorMessage="Enter a valid email address."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="LoginForm"
              ></asp:RegularExpressionValidator>
            </div>

            <div class="relative group">
              <div class="flex justify-between items-center mb-2">
                <label
                  class="block text-xs uppercase tracking-widest text-on-surface-variant transition-colors group-focus-within:text-secondary"
                  for="txtLoginPassword"
                  >Password</label
                >
                <a
                  class="text-[10px] uppercase tracking-tighter text-on-surface-variant hover:text-secondary transition-colors"
                  href="#"
                  >Forgot password?</a
                >
              </div>
              <asp:TextBox
                ID="txtLoginPassword"
                runat="server"
                ClientIDMode="Static"
                TextMode="Password"
                CssClass="w-full bg-transparent border-0 border-b border-outline-variant/30 px-0 py-3 text-on-surface placeholder:text-on-surface-variant/30 focus:ring-0 focus:border-secondary transition-all outline-none"
                MaxLength="50"
                placeholder="********"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="rfvLoginPassword"
                runat="server"
                ControlToValidate="txtLoginPassword"
                ErrorMessage="Password is required."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="LoginForm"
              ></asp:RequiredFieldValidator>
            </div>
          </div>

          <div class="space-y-4 pt-4">
            <asp:Button
              ID="btnLogin"
              runat="server"
              Text="Enter the Zone"
              CssClass="w-full az-editorial-gradient text-on-secondary font-bold py-5 px-6 rounded-none tracking-widest uppercase text-xs active:scale-[0.98] transition-transform shadow-lg shadow-secondary/10"
              OnClientClick="if (typeof(Page_ClientValidate) === 'function' && !Page_ClientValidate('LoginForm')) { window.azSnackbar && window.azSnackbar.show && window.azSnackbar.show('Please enter your email and password.', 'error'); return false; }"
              OnClick="btnLogin_Click"
              ValidationGroup="LoginForm"
              UseSubmitBehavior="false"
            />

            <div class="text-center mt-6">
              <p
                class="text-[10px] uppercase tracking-widest text-on-surface-variant/60"
              >
                New to the account?
                <a
                  class="text-secondary font-bold hover:opacity-80 transition-opacity ml-1"
                  href="/Auth/Register.aspx"
                  >Sign Up</a
                >
              </p>
            </div>

            <div class="relative py-4 flex items-center">
              <div class="flex-grow border-t border-outline-variant/10"></div>
              <span
                class="flex-shrink mx-4 text-[10px] uppercase tracking-widest text-on-surface-variant/40"
                >Or continue with</span
              >
              <div class="flex-grow border-t border-outline-variant/10"></div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <button
                type="button"
                class="flex items-center justify-center space-x-2 py-4 border border-outline-variant/20 hover:bg-surface-container transition-colors rounded-none"
              >
                <svg class="w-4 h-4" viewBox="0 0 24 24" aria-hidden="true">
                  <path
                    d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                    fill="currentColor"
                  ></path>
                  <path
                    d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                    fill="currentColor"
                  ></path>
                  <path
                    d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                    fill="currentColor"
                  ></path>
                  <path
                    d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                    fill="currentColor"
                  ></path>
                </svg>
                <span class="text-[10px] font-bold uppercase tracking-widest"
                  >Google</span
                >
              </button>
              <button
                type="button"
                class="flex items-center justify-center space-x-2 py-4 border border-outline-variant/20 hover:bg-surface-container transition-colors rounded-none"
              >
                <svg class="w-4 h-4" viewBox="0 0 24 24" aria-hidden="true">
                  <path
                    d="M17.05 20.28c-.98.95-2.05.8-3.08.35-1.08-.46-2.07-.48-3.2 0-1.44.62-2.2.44-3.06-.35C2.79 15.25 3.51 7.59 9.05 7.31c1.35.07 2.29.74 3.05.78.75-.04 2.15-.9 3.65-.74 1.57.2 2.73.81 3.43 1.83-3.15 1.86-2.65 6.13.52 7.42-.58 1.48-1.36 2.95-2.65 3.68zM12.03 7.25c-.15-2.23 1.66-4.07 3.74-4.25.29 2.58-2.34 4.51-3.74 4.25z"
                    fill="currentColor"
                  ></path>
                </svg>
                <span class="text-[10px] font-bold uppercase tracking-widest"
                  >Apple</span
                >
              </button>
            </div>
          </div>
        </div>

        <p
          class="text-[10px] text-center text-on-surface-variant/40 uppercase tracking-[0.2em] leading-relaxed"
        >
          By entering, you agree to our
          <a class="underline hover:text-secondary transition-colors" href="#"
            >Digital Manifesto</a
          >
          and
          <a class="underline hover:text-secondary transition-colors" href="#"
            >Privacy Protocols</a
          >.
        </p>
      </div>
    </section>
  </main>

  <div
    class="fixed top-0 left-0 w-full h-full pointer-events-none z-[-1] opacity-20"
  >
    <div
      class="absolute top-[10%] right-[5%] w-[40vw] h-[40vw] rounded-full bg-primary-container blur-[120px]"
    ></div>
    <div
      class="absolute bottom-[5%] left-[5%] w-[30vw] h-[30vw] rounded-full bg-secondary-container/20 blur-[100px]"
    ></div>
  </div>
</asp:Content>
