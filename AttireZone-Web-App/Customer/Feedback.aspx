<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="AttireZone_Web_App.Customer.Feedback" %>

<!DOCTYPE html>

<html class="dark" lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <title>AttireZone | Write a Review</title>
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@100;300;400;600;700;900&amp;display=swap" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet" />
    <style>
        body {
            font-family: 'Inter', sans-serif;
            background-color: #131313;
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
            background: #af8d11;
        }

        .az-rating-select {
            background-color: #1f1f1f !important;
            color: #e2e2e2 !important;
        }

        .az-rating-select option {
            background-color: #1b1b1b !important;
            color: #e2e2e2 !important;
        }
    </style>
    <script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    colors: {
                        "background": "#131313",
                        "surface": "#131313",
                        "surface-container": "#1f1f1f",
                        "surface-container-low": "#1b1b1b",
                        "surface-container-high": "#2a2a2a",
                        "on-background": "#e2e2e2",
                        "on-surface": "#e2e2e2",
                        "on-surface-variant": "#c4c6cf",
                        "secondary": "#e9c349",
                        "secondary-container": "#af8d11",
                        "on-secondary": "#3c2f00",
                        "outline-variant": "#43474e",
                        "error": "#ffb4ab"
                    }
                }
            }
        }
    </script>
</head>
<body class="bg-background text-on-background selection:bg-secondary selection:text-on-secondary">
    <form id="form1" runat="server">
        <% Server.Execute("~/Navbar.aspx"); %>

        <main class="max-w-[1080px] mx-auto px-5 lg:px-10 py-10">
            <asp:PlaceHolder ID="phFeedbackUnavailable" runat="server" Visible="false">
                <section class="border border-outline-variant/20 bg-surface-container-low px-8 py-16 text-center">
                    <h1 class="text-3xl font-semibold tracking-tight mb-4">Review Unavailable</h1>
                    <p class="text-on-surface-variant mb-8"><asp:Literal ID="litUnavailableMessage" runat="server"></asp:Literal></p>
                    <a href="<%= ResolveUrl("~/Pages/Product.aspx") %>" class="inline-flex items-center justify-center px-8 py-3 bg-secondary text-on-secondary text-xs font-bold uppercase tracking-[0.2em]">Back to Collections</a>
                </section>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phFeedbackForm" runat="server">
                <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 xl:gap-12">
                    <section class="lg:col-span-5 bg-surface-container-low p-6 md:p-8 space-y-5">
                        <asp:Image ID="imgProductPreview" runat="server" AlternateText="Product" CssClass="w-full aspect-[4/5] object-cover" />
                        <div class="space-y-2">
                            <span class="text-[10px] tracking-widest uppercase text-on-surface-variant">Reviewing Product</span>
                            <h1 class="text-2xl font-semibold tracking-tight"><asp:Literal ID="litProductName" runat="server"></asp:Literal></h1>
                            <p class="text-lg text-secondary"><asp:Literal ID="litProductPrice" runat="server"></asp:Literal></p>
                        </div>
                        <asp:HyperLink ID="lnkBackToProduct" runat="server" CssClass="inline-flex items-center text-xs font-bold uppercase tracking-[0.2em] text-on-surface hover:text-secondary transition-colors">
                            <span class="material-symbols-outlined !text-sm mr-2">arrow_back</span>
                            Back to Product
                        </asp:HyperLink>
                    </section>

                    <section class="lg:col-span-7 bg-surface-container p-6 md:p-8 space-y-6 border border-outline-variant/20">
                        <div>
                            <h2 class="text-3xl font-semibold tracking-tight mb-2">Write a Review</h2>
                            <p class="text-on-surface-variant text-sm">Tell other shoppers about the quality, fit, and overall experience with this item.</p>
                        </div>

                        <asp:Label ID="lblFeedbackMessage" runat="server" Visible="false" CssClass="block px-4 py-3 text-sm border"></asp:Label>

                        <div class="space-y-2">
                            <label for="ddlRating" class="text-[10px] tracking-widest uppercase font-bold">Rating</label>
                            <asp:DropDownList ID="ddlRating" runat="server" CssClass="w-full py-3 text-sm border border-outline-variant/60 focus:border-secondary transition-colors az-rating-select">
                                <asp:ListItem Value="">Select a rating</asp:ListItem>
                                <asp:ListItem Value="5">5 - Excellent</asp:ListItem>
                                <asp:ListItem Value="4">4 - Very Good</asp:ListItem>
                                <asp:ListItem Value="3">3 - Good</asp:ListItem>
                                <asp:ListItem Value="2">2 - Fair</asp:ListItem>
                                <asp:ListItem Value="1">1 - Poor</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="space-y-2">
                            <label for="txtComment" class="text-[10px] tracking-widest uppercase font-bold">Comment</label>
                            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="7" MaxLength="1000" CssClass="w-full py-3 text-sm bg-transparent border border-outline-variant/60 focus:border-secondary transition-colors" placeholder="What stood out to you about this product?"></asp:TextBox>
                            <p class="text-[11px] text-on-surface-variant uppercase tracking-wider">Minimum 5 characters. Maximum 1000 characters.</p>
                        </div>

                        <div class="pt-2 flex flex-col sm:flex-row gap-3">
                            <asp:Button ID="btnSubmitFeedback" runat="server" Text="Submit Review" OnClick="btnSubmitFeedback_Click" CssClass="px-7 py-3 bg-gradient-to-tr from-secondary to-secondary-container text-on-secondary text-xs font-bold uppercase tracking-[0.2em]" />
                            <asp:HyperLink ID="lnkCancelFeedback" runat="server" CssClass="px-7 py-3 border border-outline-variant/40 text-xs font-bold uppercase tracking-[0.2em] text-center hover:border-secondary hover:text-secondary transition-colors">Cancel</asp:HyperLink>
                        </div>
                    </section>
                </div>
            </asp:PlaceHolder>
        </main>
    </form>
</body>
</html>
