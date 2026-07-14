using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.UserDTOs
{
    public class UpdateProfileRequestDTO
    {
        [StringLength(50, MinimumLength = 2)]
        public string StoreName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string? Location { get; set; }
    }
}
