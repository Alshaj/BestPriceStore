using System.Collections.Generic;

namespace BestPriceStore.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string? ImageUrl { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
