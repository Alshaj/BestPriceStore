namespace BestPriceStore.DTOs.AuthDTOs
{
    public class RegisterResponseDTO
    {
        public int Id { get; set; } 
        public string? StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
    }
}
