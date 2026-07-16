using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.OrderDTOs
{
    public class CreateOrderRequestDTO
    {
        [Required]
        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
        public List<OrderItemRequestDTO> Items { get; set; } = new List<OrderItemRequestDTO>();
    }

    public class OrderItemRequestDTO
    {
        [Required]
        public int ProductImageId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
