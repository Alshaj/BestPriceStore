namespace BestPriceStore.DTOs.ProductDTOs
{
    public class ProductBrowseResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int CurrencyId { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
