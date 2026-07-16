using BestPriceStore.DTOs;
using BestPriceStore.DTOs.OrderDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPriceStore.Services.OrderService
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderResponseDTO>> PlaceOrderAsync(int userId, CreateOrderRequestDTO model);
        Task<ApiResponse<List<OrderSummaryResponseDTO>>> GetUserOrdersAsync(int userId);
        Task<ApiResponse<OrderResponseDTO>> GetOrderDetailsAsync(int userId, int orderId);
        Task<ApiResponse<OrderResponseDTO>> CancelOrderAsync(int userId, int orderId);
        Task<ApiResponse<List<AdminOrderSummaryResponseDTO>>> GetAllOrdersAsync();
        Task<ApiResponse<OrderResponseDTO>> GetOrderDetailsForAdminAsync(int orderId);
        Task<ApiResponse<OrderResponseDTO>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDTO model);
    }
}
