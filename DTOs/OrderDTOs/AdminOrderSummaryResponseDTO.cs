using System;

namespace BestPriceStore.DTOs.OrderDTOs
{
    public class AdminOrderSummaryResponseDTO
    {
        public int Id { get; set; }
        public int OrderStatusId { get; set; }
        public double TotalAmountYer { get; set; }
        public double TotalAmountSar { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? StoreName { get; set; }
    }
}
