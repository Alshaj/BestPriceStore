using System;
using System.Collections.Generic;
using BestPriceStore.Enums;
using BestPriceStore.Data;

namespace BestPriceStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int OrderStatusId { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public double TotalAmountYer { get; set; }
        public double TotalAmountSar { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}
