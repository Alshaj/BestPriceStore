using BestPriceStore.DTOs;
using System.Threading.Tasks;

namespace BestPriceStore.Services.AdminDashboardService
{
    public interface IAdminDashboardService
    {
        Task<ApiResponse<AdminDashboardDTO>> GetDashboardStatsAsync();
    }
}
