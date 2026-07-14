using System.Collections.Generic;
using BestPriceStore.Data;

namespace BestPriceStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
