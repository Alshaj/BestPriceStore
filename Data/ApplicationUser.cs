using Microsoft.AspNetCore.Identity;

namespace BestPriceStore.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string StoreName { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Models.Order> Orders { get; set; } = new List<Models.Order>();
        public Models.Cart? Cart { get; set; }
    }
}
