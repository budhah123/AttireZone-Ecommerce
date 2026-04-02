using System;

namespace AttireZone_Web_App.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int SelectedQuantity { get; set; }

        public string SelectedSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}