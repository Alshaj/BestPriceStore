using System.ComponentModel.DataAnnotations;

namespace BestPriceStore.DTOs.AuthDTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string StoreName { get; set; }


        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

        public string? Location { get; set; }   
    }
}
