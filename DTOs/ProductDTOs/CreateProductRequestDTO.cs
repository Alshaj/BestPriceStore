using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.ProductDTOs
{
    public class CreateProductRequestDTO
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid currency.")]
        public int CurrencyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

        public List<ProductImageRequestDTO> Images { get; set; } = new List<ProductImageRequestDTO>();
    }
}
