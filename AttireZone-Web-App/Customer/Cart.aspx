<%@ Page Title="Shopping Cart" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="AttireZone_Web_App.Customer.Cart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Cart Header -->
    <div class="container-max section-padding">
        <h1 class="text-heading-lg mb-2">Shopping Cart</h1>
        <p class="text-body">Review and manage your items</p>
    </div>

    <!-- Cart Content -->
    <div class="container-max mb-16">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Cart Items -->
            <div class="lg:col-span-2">
                <div class="card">
                    <h2 class="text-heading-sm mb-6 border-b border-[#2a2a2a] pb-4">Cart Items (0)</h2>
                    <p class="text-body-sm text-center py-12">Your cart is empty</p>
                </div>
            </div>

            <!-- Order Summary -->
            <div class="lg:col-span-1">
                <div class="card sticky top-4">
                    <h2 class="text-heading-sm mb-6 border-b border-[#2a2a2a] pb-4">Order Summary</h2>
                    
                    <div class="space-y-4 mb-6">
                        <div class="flex justify-between text-body-sm">
                            <span>Subtotal:</span>
                            <span>$0.00</span>
                        </div>
                        <div class="flex justify-between text-body-sm">
                            <span>Shipping:</span>
                            <span>$0.00</span>
                        </div>
                        <div class="flex justify-between text-body-sm">
                            <span>Tax:</span>
                            <span>$0.00</span>
                        </div>
                        <div class="border-t border-[#2a2a2a] pt-4 flex justify-between text-heading-sm">
                            <span>Total:</span>
                            <span class="text-[#e9c349]">$0.00</span>
                        </div>
                    </div>

                    <button class="btn-primary-lg w-full justify-center text-center">Proceed to Checkout</button>
                    <a href="/" class="btn-text block text-center mt-4 text-sm">Continue Shopping</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
