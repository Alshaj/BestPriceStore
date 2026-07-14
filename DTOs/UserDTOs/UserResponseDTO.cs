namespace BestPriceStore.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string? StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }
}
