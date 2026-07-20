using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.OrderDTOs
{
    public class EditOrderItemsRequestDTO
    {
        [Required]
        public List<EditOrderItemModel> Items { get; set; } = new List<EditOrderItemModel>();
    }

    public class EditOrderItemModel
    {
        [Required]
        public int ProductImageId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
        public int Quantity { get; set; }
    }
}
