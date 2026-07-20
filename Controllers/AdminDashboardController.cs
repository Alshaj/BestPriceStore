using BestPriceStore.Services.AdminDashboardService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var response = await _dashboardService.GetDashboardStatsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
