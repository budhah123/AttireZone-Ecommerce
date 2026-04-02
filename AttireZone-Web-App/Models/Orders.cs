using System;
using System.Collections.Generic;

namespace AttireZone_Web_App.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public string DeliveryAddress { get; set; }

        public string OrderNotes { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public IList<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}