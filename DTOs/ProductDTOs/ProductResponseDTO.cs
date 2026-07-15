using System;
using System.Collections.Generic;

namespace BestPriceStore.DTOs.ProductDTOs
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        
        public int CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public List<ProductImageResponseDTO> Images { get; set; } = new List<ProductImageResponseDTO>();
    }
}
