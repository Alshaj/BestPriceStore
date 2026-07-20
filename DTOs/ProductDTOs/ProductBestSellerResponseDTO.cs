namespace BestPriceStore.DTOs.ProductDTOs
{
    public class ProductBestSellerResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int CurrencyId { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
