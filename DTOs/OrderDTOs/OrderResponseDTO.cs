using System;
using System.Collections.Generic;

namespace BestPriceStore.DTOs.OrderDTOs
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderStatusId { get; set; }
        public double TotalAmountYer { get; set; }
        public double TotalAmountSar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderProductResponseDTO> Items { get; set; } = new List<OrderProductResponseDTO>();
    }

    public class OrderProductResponseDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int ProductImageId { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalAmount { get; set; }
        public int CurrencyId { get; set; }
    }
}
