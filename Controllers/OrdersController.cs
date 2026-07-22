using BestPriceStore.DTOs;
using BestPriceStore.DTOs.OrderDTOs;
using BestPriceStore.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequestDTO model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new ApiResponse<OrderResponseDTO>(401, "User is not properly authenticated."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
            var response = await _orderService.PlaceOrderAsync(userId, model, isAdmin);
            if (response.StatusCode != 201)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] int? orderStatusId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new ApiResponse<object>(401, "User is not properly authenticated."));
            }

            var response = await _orderService.GetUserOrdersAsync(userId, orderStatusId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new ApiResponse<OrderResponseDTO>(401, "User is not properly authenticated."));
            }

            var response = await _orderService.GetOrderDetailsAsync(userId, id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new ApiResponse<OrderResponseDTO>(401, "User is not properly authenticated."));
            }

            var response = await _orderService.CancelOrderAsync(userId, id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
