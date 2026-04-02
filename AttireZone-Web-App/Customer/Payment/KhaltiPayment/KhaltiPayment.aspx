<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KhaltiPayment.aspx.cs" Inherits="AttireZone_Web_App.KhaltiPayment" %>
<!DOCTYPE html>
<html lang="en" class="dark">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Redirecting to Khalti | AttireZone</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
</head>
<body class="bg-[#131313] text-[#e2e2e2] min-h-screen flex items-center justify-center px-6">
    <% if (ShowError) { %>
        <div class="max-w-xl w-full bg-[#1f1f1f] border border-[#43474e] p-10 text-center space-y-5">
            <h1 class="text-2xl font-bold uppercase tracking-[0.12em]">Unable to Start Khalti Payment</h1>
            <p class="text-sm text-[#ffb4ab]"><%= ErrorMessage %></p>
            <a class="inline-block bg-[#e9c349] text-[#3c2f00] px-6 py-3 text-xs uppercase tracking-[0.15em] font-bold" href="<%= ResolveUrl("~/Customer/Checkout.aspx") %>">Back to Checkout</a>
        </div>
    <% } else { %>
        <div class="max-w-xl w-full text-center space-y-6">
            <div class="mx-auto h-12 w-12 border-4 border-[#af8d11] border-t-transparent rounded-full animate-spin"></div>
            <h1 class="text-2xl font-bold uppercase tracking-[0.12em]">Redirecting to Khalti</h1>
            <p class="text-sm text-[#c4c6cf] uppercase tracking-[0.12em]">Please wait while we initialize your payment session.</p>
        </div>
    <% } %>
</body>
</html>
