using System;

namespace AttireZone_Web_App.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Edition { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SelectedSize { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPopular { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
    }
}