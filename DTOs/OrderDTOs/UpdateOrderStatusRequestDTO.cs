using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.OrderDTOs
{
    public class UpdateOrderStatusRequestDTO
    {
        [Required]
        [Range(1, 5, ErrorMessage = "OrderStatusId must be between 1 and 5.")]
        public int OrderStatusId { get; set; }
    }
}
