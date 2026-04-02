<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EsewaSuccess.aspx.cs" Inherits="AttireZone_Web_App.EsewaSuccess" %>
<!DOCTYPE html>
<html lang="en" class="dark">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Verifying eSewa Payment | AttireZone</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
</head>
<body class="bg-[#131313] text-[#e2e2e2] min-h-screen flex items-center justify-center px-6">
    <div class="max-w-xl w-full text-center space-y-6">
        <div class="mx-auto h-12 w-12 border-4 border-[#af8d11] border-t-transparent rounded-full animate-spin"></div>
        <h1 class="text-2xl font-bold uppercase tracking-[0.12em]">Verifying eSewa Payment</h1>
        <p class="text-sm text-[#c4c6cf] uppercase tracking-[0.12em]">Please wait while we confirm your transaction.</p>
        <noscript>
            <p class="text-sm text-[#ffb4ab]">JavaScript is disabled. <a class="underline" href="<%= ResolveUrl("~/OrderFailed.aspx") %>">Click here</a> if redirection does not happen.</p>
        </noscript>
    </div>
</body>
</html>
