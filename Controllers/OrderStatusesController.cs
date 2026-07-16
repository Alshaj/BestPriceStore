using BestPriceStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderStatusesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all available order statuses (seeded lookup table).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var statuses = await _context.OrderStatuses
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();

            return Ok(new DTOs.ApiResponse<object>(200, statuses));
        }
    }
}
