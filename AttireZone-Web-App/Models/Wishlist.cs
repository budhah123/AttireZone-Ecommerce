using System;

namespace AttireZone_Web_App.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
