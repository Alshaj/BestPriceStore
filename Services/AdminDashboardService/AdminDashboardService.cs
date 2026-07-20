using BestPriceStore.Data;
using BestPriceStore.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.AdminDashboardService
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResponse<AdminDashboardDTO>> GetDashboardStatsAsync()
        {
            // 1. Sales metrics (Sum of Delivered orders)
            var deliveredOrders = await _context.Orders
                .Where(o => o.OrderStatusId == 4)
                .Select(o => new { o.TotalAmountYer, o.TotalAmountSar })
                .ToListAsync();

            double totalSalesYer = deliveredOrders.Sum(o => o.TotalAmountYer);
            double totalSalesSar = deliveredOrders.Sum(o => o.TotalAmountSar);

            // 2. Orders metrics
            var orderStatusCounts = await _context.Orders
                .GroupBy(o => o.OrderStatusId)
                .Select(g => new { StatusId = g.Key, Count = g.Count() })
                .ToListAsync();

            int totalOrders = orderStatusCounts.Sum(x => x.Count);
            int pending = orderStatusCounts.FirstOrDefault(x => x.StatusId == 1)?.Count ?? 0;
            int processing = orderStatusCounts.FirstOrDefault(x => x.StatusId == 2)?.Count ?? 0;
            int shipped = orderStatusCounts.FirstOrDefault(x => x.StatusId == 3)?.Count ?? 0;
            int delivered = orderStatusCounts.FirstOrDefault(x => x.StatusId == 4)?.Count ?? 0;
            int cancelled = orderStatusCounts.FirstOrDefault(x => x.StatusId == 5)?.Count ?? 0;

            // 3. Products metrics
            int totalActiveProducts = await _context.Products.CountAsync(p => p.IsActive);
            
            // Out of stock means products where all variations (ProductImages) have 0 stock
            int outOfStockProducts = await _context.Products
                .Where(p => p.ProductImages.All(pi => pi.QuantityInStock == 0))
                .CountAsync();

            // 4. Representatives metrics
            var representatives = await _userManager.GetUsersInRoleAsync("Representative");
            int totalReps = representatives.Count;
            int activeReps = representatives.Count(u => u.IsActive);

            var dashboardDto = new AdminDashboardDTO
            {
                TotalSalesYer = totalSalesYer,
                TotalSalesSar = totalSalesSar,
                TotalOrders = totalOrders,
                PendingOrdersCount = pending,
                ProcessingOrdersCount = processing,
                ShippedOrdersCount = shipped,
                DeliveredOrdersCount = delivered,
                CancelledOrdersCount = cancelled,
                TotalActiveProducts = totalActiveProducts,
                OutOfStockProductsCount = outOfStockProducts,
                TotalRepresentatives = totalReps,
                ActiveRepresentatives = activeReps
            };

            return new ApiResponse<AdminDashboardDTO>(200, dashboardDto);
        }
    }
}
