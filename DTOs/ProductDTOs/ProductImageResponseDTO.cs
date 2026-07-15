namespace BestPriceStore.DTOs.ProductDTOs
{
    public class ProductImageResponseDTO
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsPrimary { get; set; }
    }
}
