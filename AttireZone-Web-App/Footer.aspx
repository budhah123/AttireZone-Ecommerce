<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Footer.aspx.cs"
Inherits="AttireZone_Web_App.Footer" %>

<footer
  class="az-footer w-full border-t-0 rounded-none bg-slate-50 dark:bg-slate-950"
>
  <div
    class="flex flex-col md:flex-row justify-between items-center px-12 py-20 w-full gap-8 max-w-[1920px] mx-auto"
  >
    <div class="flex flex-col items-center md:items-start space-y-4">
      <div
        class="text-lg font-bold text-slate-900 dark:text-slate-50 tracking-tighter uppercase"
      >
        AttireZone
      </div>
      <p
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500"
      >
        © <%: DateTime.Now.Year %> AttireZone. All Rights Reserved.
      </p>
    </div>

    <div class="flex flex-wrap justify-center gap-6">
      <a
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200"
        href="#"
        >Sustainability</a
      >
      <a
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200"
        href="#"
        >Shipping</a
      >
      <a
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200"
        href="#"
        >Returns</a
      >
      <a
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200"
        href="#"
        >Privacy Policy</a
      >
      <a
        class="font-sans text-xs uppercase tracking-widest text-slate-500 dark:text-slate-500 hover:text-amber-500 transition-colors duration-200"
        href="#"
        >Terms of Service</a
      >
    </div>

    <div class="flex space-x-4">
      <button
        type="button"
        class="w-8 h-8 flex items-center justify-center border border-slate-200 dark:border-slate-800 hover:border-amber-500 transition-colors"
      >
        <span class="material-symbols-outlined text-sm">public</span>
      </button>
      <button
        type="button"
        class="w-8 h-8 flex items-center justify-center border border-slate-200 dark:border-slate-800 hover:border-amber-500 transition-colors"
      >
        <span class="material-symbols-outlined text-sm">mail</span>
      </button>
    </div>
  </div>
</footer>
