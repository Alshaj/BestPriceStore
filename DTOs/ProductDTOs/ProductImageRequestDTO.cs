using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.ProductDTOs
{
    public class ProductImageRequestDTO
    {
        [Required]
        [Url]
        public required string ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock cannot be negative.")]
        public int QuantityInStock { get; set; }

        public bool IsPrimary { get; set; }
    }
}
