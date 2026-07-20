using BestPriceStore.DTOs;
using BestPriceStore.DTOs.OrderDTOs;
using BestPriceStore.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Retrieves all orders in the system for all users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrdersAsync();

            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the full details of any order by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var response = await _orderService.GetOrderDetailsForAdminAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Updates the status of an order following strict transition sequence validation.
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _orderService.UpdateOrderStatusAsync(id, model);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Edits quantities/items of an order for returns or adjustments (reductions only).
        /// </summary>
        [HttpPut("{id}/items")]
        public async Task<IActionResult> EditOrderItems(int id, [FromBody] EditOrderItemsRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _orderService.EditOrderItemsAsync(id, model);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            
            return Ok(response);
        }
    }
}
