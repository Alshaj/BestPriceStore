namespace BestPriceStore.DTOs.AuthDTOs
{
    public class MeResponseDTO
    {
        public int Id { get; set; }
        public string? StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
}
