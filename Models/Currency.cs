using System;
using System.Collections.Generic;

namespace BestPriceStore.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
