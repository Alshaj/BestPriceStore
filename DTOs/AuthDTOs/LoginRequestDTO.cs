using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.AuthDTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [Phone]
        [StringLength(100)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
