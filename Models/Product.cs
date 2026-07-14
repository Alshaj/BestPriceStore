using System;
using System.Collections.Generic;

namespace BestPriceStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
