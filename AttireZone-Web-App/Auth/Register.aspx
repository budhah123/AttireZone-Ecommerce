<%@ Page Title="Join the Collective | AttireZone" Language="C#"
MasterPageFile="~/Site.Master" AutoEventWireup="true"
CodeBehind="Register.aspx.cs" Inherits="AttireZone_Web_App.Auth.Register" %>

<asp:Content
  ID="RegisterStyles"
  ContentPlaceHolderID="StylesPlaceholder"
  runat="server"
>
  <link
    href="/Assets/CSS/authentication.css"
    rel="stylesheet"
    type="text/css"
  />
</asp:Content>

<asp:Content
  ID="RegisterMain"
  ContentPlaceHolderID="MainContent"
  runat="server"
>
  <main
    class="az-auth-register-shell flex-grow flex items-center justify-center w-full px-6 py-12 relative overflow-hidden"
  >
    <div
      class="absolute top-[-10%] right-[-5%] w-96 h-96 bg-primary-container/20 rounded-full blur-[120px]"
    ></div>
    <div
      class="absolute bottom-[-5%] left-[-5%] w-[500px] h-[500px] bg-secondary-container/5 rounded-full blur-[150px]"
    ></div>

    <section class="w-full max-w-[480px] z-10 az-auth-register-form">
      <div class="text-center mb-10">
        <span
          class="text-xs font-medium tracking-[0.2em] uppercase text-secondary mb-3 block"
          >Membership Registration</span
        >
        <h2 class="text-4xl font-semibold tracking-tight text-on-surface mb-4">
          Join the Collective
        </h2>
        <p
          class="text-on-surface-variant text-sm font-light max-w-xs mx-auto leading-relaxed"
        >
          Access exclusive seasonal drops, editorial content, and a curated
          shopping experience.
        </p>
      </div>

      <div class="flex space-x-8 border-b border-outline-variant/20 pb-2 mb-8">
        <a
          class="text-on-surface-variant/60 font-semibold tracking-tight text-lg hover:text-on-surface pb-2 transition-all"
          href="/Auth/Login.aspx"
          >Sign In</a
        >
        <a
          class="text-on-surface font-semibold tracking-tight text-lg border-b-2 border-secondary pb-2 transition-all"
          href="/Auth/Register.aspx"
          >Create Account</a
        >
      </div>

      <div class="space-y-8">
        <div class="space-y-6">
          <div class="group relative">
            <label
              class="block text-[10px] uppercase tracking-[0.15em] text-on-surface-variant mb-1 group-focus-within:text-secondary transition-colors"
              for="txtFullName"
              >Full Name</label
            >
            <asp:TextBox
              ID="txtFullName"
              runat="server"
              ClientIDMode="Static"
              CssClass="w-full bg-transparent border-b border-outline-variant py-3 text-sm placeholder:text-surface-variant transition-all focus:border-secondary rounded-none px-0"
              MaxLength="100"
              placeholder="ALEXANDER VOGUE"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="rfvFullName"
              runat="server"
              ControlToValidate="txtFullName"
              ErrorMessage="Full name is required."
              Display="Dynamic"
              CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
              ValidationGroup="RegisterForm"
            ></asp:RequiredFieldValidator>
          </div>

          <div class="group relative">
            <label
              class="block text-[10px] uppercase tracking-[0.15em] text-on-surface-variant mb-1 group-focus-within:text-secondary transition-colors"
              for="txtEmail"
              >Email Address</label
            >
            <asp:TextBox
              ID="txtEmail"
              runat="server"
              ClientIDMode="Static"
              TextMode="Email"
              CssClass="w-full bg-transparent border-b border-outline-variant py-3 text-sm placeholder:text-surface-variant transition-all focus:border-secondary rounded-none px-0"
              MaxLength="100"
              placeholder="ALEX@COLLECTIVE.COM"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="rfvEmail"
              runat="server"
              ControlToValidate="txtEmail"
              ErrorMessage="Email is required."
              Display="Dynamic"
              CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
              ValidationGroup="RegisterForm"
            ></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator
              ID="revEmail"
              runat="server"
              ControlToValidate="txtEmail"
              ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
              ErrorMessage="Enter a valid email address."
              Display="Dynamic"
              CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
              ValidationGroup="RegisterForm"
            ></asp:RegularExpressionValidator>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="group relative">
              <label
                class="block text-[10px] uppercase tracking-[0.15em] text-on-surface-variant mb-1 group-focus-within:text-secondary transition-colors"
                for="txtPassword"
                >Password</label
              >
              <asp:TextBox
                ID="txtPassword"
                runat="server"
                ClientIDMode="Static"
                TextMode="Password"
                CssClass="w-full bg-transparent border-b border-outline-variant py-3 text-sm placeholder:text-surface-variant transition-all focus:border-secondary rounded-none px-0"
                MaxLength="50"
                placeholder="********"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="rfvPassword"
                runat="server"
                ControlToValidate="txtPassword"
                ErrorMessage="Password is required."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="RegisterForm"
              ></asp:RequiredFieldValidator>
            </div>

            <div class="group relative">
              <label
                class="block text-[10px] uppercase tracking-[0.15em] text-on-surface-variant mb-1 group-focus-within:text-secondary transition-colors"
                for="txtConfirmPassword"
                >Confirm Password</label
              >
              <asp:TextBox
                ID="txtConfirmPassword"
                runat="server"
                ClientIDMode="Static"
                TextMode="Password"
                CssClass="w-full bg-transparent border-b border-outline-variant py-3 text-sm placeholder:text-surface-variant transition-all focus:border-secondary rounded-none px-0"
                MaxLength="50"
                placeholder="********"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="rfvConfirmPassword"
                runat="server"
                ControlToValidate="txtConfirmPassword"
                ErrorMessage="Confirm password is required."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="RegisterForm"
              ></asp:RequiredFieldValidator>
              <asp:CompareValidator
                ID="cvPasswordMatch"
                runat="server"
                ControlToValidate="txtConfirmPassword"
                ControlToCompare="txtPassword"
                ErrorMessage="Passwords do not match."
                Display="Dynamic"
                CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
                ValidationGroup="RegisterForm"
              ></asp:CompareValidator>
            </div>
          </div>
        </div>

        <div class="flex items-start gap-3">
          <div class="flex items-center h-5">
            <asp:CheckBox
              ID="chkTerms"
              runat="server"
              ClientIDMode="Static"
              CssClass="h-4 w-4 bg-transparent border-outline-variant text-secondary focus:ring-0 rounded-none cursor-pointer"
            />
          </div>
          <div class="text-xs leading-5">
            <label class="text-on-surface-variant font-light" for="chkTerms">
              I agree to the
              <a
                class="text-on-surface underline decoration-outline-variant hover:decoration-secondary transition-all"
                href="#"
                >Terms of Service</a
              >
              and
              <a
                class="text-on-surface underline decoration-outline-variant hover:decoration-secondary transition-all"
                href="#"
                >Privacy Policy</a
              >.
            </label>
            <asp:CustomValidator
              ID="cvTerms"
              runat="server"
              ErrorMessage="Please accept terms and conditions."
              ClientValidationFunction="validateTerms"
              OnServerValidate="cvTerms_ServerValidate"
              Display="Dynamic"
              CssClass="mt-2 block text-[11px] tracking-[0.08em] uppercase text-error"
              ValidationGroup="RegisterForm"
            ></asp:CustomValidator>
          </div>
        </div>

        <asp:LinkButton
          ID="btnRegister"
          runat="server"
          OnClientClick="if (typeof(Page_ClientValidate) === 'function' && !Page_ClientValidate('RegisterForm')) { window.azSnackbar && window.azSnackbar.show && window.azSnackbar.show('Please correct the highlighted fields.', 'error'); return false; }"
          OnClick="btnRegister_Click"
          ValidationGroup="RegisterForm"
          CssClass="w-full bg-secondary hover:bg-secondary-container text-on-secondary font-semibold py-5 tracking-[0.1em] uppercase text-xs transition-all duration-300 shadow-xl shadow-secondary/5 group relative overflow-hidden text-center block"
        >
          <span class="relative z-10">Join the Collective</span>
          <div
            class="absolute inset-0 bg-white/10 translate-y-full group-hover:translate-y-0 transition-transform duration-300"
          ></div>
        </asp:LinkButton>

        <div class="mt-4 text-center">
          <p
            class="text-[11px] uppercase tracking-[0.1em] text-on-surface-variant/80"
          >
            Already have an account?
            <a
              class="text-secondary font-semibold hover:underline underline-offset-4 decoration-1 transition-all ml-1"
              href="/Auth/Login.aspx"
              >Login</a
            >
          </p>
        </div>

        <div class="mt-12">
          <div class="relative flex items-center mb-8">
            <div class="flex-grow border-t border-outline-variant/30"></div>
            <span
              class="flex-shrink mx-4 text-[10px] uppercase tracking-[0.2em] text-on-surface-variant/60"
              >Or continue with</span
            >
            <div class="flex-grow border-t border-outline-variant/30"></div>
          </div>
          <div class="grid grid-cols-2 gap-4">
            <button
              type="button"
              class="flex items-center justify-center gap-3 py-4 border border-outline-variant/40 hover:border-secondary/50 transition-all group"
            >
              <span
                class="material-symbols-outlined text-xl group-hover:text-secondary transition-colors"
                >brand_family</span
              >
              <span class="text-[10px] uppercase tracking-widest font-medium"
                >Google</span
              >
            </button>
            <button
              type="button"
              class="flex items-center justify-center gap-3 py-4 border border-outline-variant/40 hover:border-secondary/50 transition-all group"
            >
              <span
                class="material-symbols-outlined text-xl group-hover:text-secondary transition-colors"
                >potted_plant</span
              >
              <span class="text-[10px] uppercase tracking-widest font-medium"
                >Apple</span
              >
            </button>
          </div>
        </div>

        <div class="mt-12 text-center">
          <p class="text-sm font-light text-on-surface-variant">
            Already part of the collective?
            <a
              class="text-secondary font-medium ml-1 hover:underline underline-offset-4 decoration-1"
              href="/Auth/Login.aspx"
              >Sign In</a
            >
          </p>
        </div>

        <asp:Label
          ID="lblFormMessage"
          runat="server"
          CssClass="sr-only"
          EnableViewState="false"
        ></asp:Label>
      </div>
    </section>

    <div
      class="hidden lg:block fixed bottom-12 right-12 w-48 h-72 opacity-40 hover:opacity-100 transition-opacity duration-700 overflow-hidden grayscale hover:grayscale-0"
    >
      <img
        alt="Editorial fashion"
        class="w-full h-full object-cover"
        src="https://lh3.googleusercontent.com/aida-public/AB6AXuBNDtoGZw8SY_HS0klCJj2NFvRTZx07dWPRNhU76X0tPiXv7XmYIjjzzFqCusVM_pix0vJQN4FkC0DEWoS03CctMyJzkznPqIwuNHkeCsBu4d5YQ6K1y1hMXRTrH07J7FPoPgEaTzX-WpvTjsn3bukJ2mTiqqfL_KTLoVJjyfWpwRL1PDKZgkgRm-0U2j_WzsEEIXoeKm6xpxysxGy-MtVs0uf4L0nSS9cDENTfIrS4IMr0SYHKIIEcMChU_xS594KEdEnLgz6S5Aw"
      />
    </div>
  </main>

  <script type="text/javascript">
    function validateTerms(source, args) {
      var terms = document.getElementById("chkTerms");
      args.IsValid = !!(terms && terms.checked);
    }
  </script>
</asp:Content>
