using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.CategoryDTOs
{
    public class UpdateCategoryRequestDTO
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }
    }
}
