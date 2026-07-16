using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.OrderDTOs;
using BestPriceStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<OrderResponseDTO>> PlaceOrderAsync(int userId, CreateOrderRequestDTO model)
        {
            // Verify User exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return new ApiResponse<OrderResponseDTO>(404, $"User with ID {userId} not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                double totalAmountYer = 0.0;
                double totalAmountSar = 0.0;
                var orderProducts = new List<OrderProduct>();
                var responseItems = new List<OrderProductResponseDTO>();

                foreach (var item in model.Items)
                {
                    // Fetch variation (ProductImage) with parent Product
                    var productImage = await _context.ProductImages
                        .Include(pi => pi.Product)
                        .FirstOrDefaultAsync(pi => pi.Id == item.ProductImageId);

                    if (productImage == null)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<OrderResponseDTO>(400, $"Product variation (Image ID: {item.ProductImageId}) does not exist.");
                    }

                    if (productImage.Product == null)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<OrderResponseDTO>(400, $"Product associated with Image ID {item.ProductImageId} does not exist.");
                    }

                    // Check stock
                    if (productImage.QuantityInStock < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<OrderResponseDTO>(400, 
                            $"Insufficient stock for product '{productImage.Product.Name}' (ID: {productImage.Product.Id}, Variation ID: {productImage.Id}). Available: {productImage.QuantityInStock}, Requested: {item.Quantity}.");
                    }

                    // Reduce stock
                    productImage.QuantityInStock -= item.Quantity;
                    _context.ProductImages.Update(productImage);

                    // Compute pricing details
                    double unitPrice = productImage.Product.Price;
                    double itemTotal = unitPrice * item.Quantity;
                    int currencyId = productImage.Product.CurrencyId;

                    if (currencyId == 1) // ريال يمني (YER)
                    {
                        totalAmountYer += itemTotal;
                    }
                    else if (currencyId == 2) // ريال سعودي (SAR)
                    {
                        totalAmountSar += itemTotal;
                    }

                    // Create order product
                    var orderProduct = new OrderProduct
                    {
                        ProductId = productImage.ProductId,
                        ProductImageId = productImage.Id,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        TotalAmount = itemTotal,
                        CurrencyId = currencyId
                    };

                    orderProducts.Add(orderProduct);

                    // Map item for response DTO
                    responseItems.Add(new OrderProductResponseDTO
                    {
                        ProductId = productImage.ProductId,
                        ProductName = productImage.Product.Name,
                        ProductImageId = productImage.Id,
                        ImageUrl = productImage.ImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        TotalAmount = itemTotal,
                        CurrencyId = currencyId
                    });
                }

                // Create Order
                var order = new Order
                {
                    UserId = userId,
                    OrderStatusId = 1, // Pending status (Seeded lookup ID)
                    TotalAmountYer = totalAmountYer,
                    TotalAmountSar = totalAmountSar,
                    CreatedAt = DateTime.UtcNow,
                    OrderProducts = orderProducts
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Build final response DTO
                var response = new OrderResponseDTO
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderStatusId = order.OrderStatusId,
                    TotalAmountYer = order.TotalAmountYer,
                    TotalAmountSar = order.TotalAmountSar,
                    CreatedAt = order.CreatedAt,
                    Items = responseItems
                };

                return new ApiResponse<OrderResponseDTO>(201, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<OrderResponseDTO>(500, $"An error occurred while placing the order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<OrderSummaryResponseDTO>>> GetUserOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryResponseDTO
                {
                    Id = o.Id,
                    OrderStatusId = o.OrderStatusId,
                    TotalAmountYer = o.TotalAmountYer,
                    TotalAmountSar = o.TotalAmountSar,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return new ApiResponse<List<OrderSummaryResponseDTO>>(200, orders);
        }

        public async Task<ApiResponse<OrderResponseDTO>> GetOrderDetailsAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.ProductImage)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            // Ownership check
            if (order.UserId != userId)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            return new ApiResponse<OrderResponseDTO>(200, MapToOrderResponseDTO(order));
        }

        public async Task<ApiResponse<OrderResponseDTO>> CancelOrderAsync(int userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.ProductImage)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            // Ownership check
            if (order.UserId != userId)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            // Only pending orders can be cancelled
            if (order.OrderStatusId != 1)
            {
                return new ApiResponse<OrderResponseDTO>(400, "Only orders with 'Pending' status can be cancelled.");
            }

            // Update status to Cancelled (5)
            order.OrderStatusId = 5;
            order.CancelledAt = DateTime.UtcNow;

            // Restore stock for each item
            foreach (var item in order.OrderProducts)
            {
                if (item.ProductImage != null)
                {
                    item.ProductImage.QuantityInStock += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<OrderResponseDTO>(200, MapToOrderResponseDTO(order));
        }

        public async Task<ApiResponse<List<AdminOrderSummaryResponseDTO>>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AdminOrderSummaryResponseDTO
                {
                    Id = o.Id,
                    OrderStatusId = o.OrderStatusId,
                    TotalAmountYer = o.TotalAmountYer,
                    TotalAmountSar = o.TotalAmountSar,
                    CreatedAt = o.CreatedAt,
                    UserId = o.UserId,
                    StoreName = o.User != null ? o.User.StoreName : null
                })
                .ToListAsync();

            return new ApiResponse<List<AdminOrderSummaryResponseDTO>>(200, orders);
        }

        public async Task<ApiResponse<OrderResponseDTO>> GetOrderDetailsForAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.ProductImage)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            return new ApiResponse<OrderResponseDTO>(200, MapToOrderResponseDTO(order));
        }

        public async Task<ApiResponse<OrderResponseDTO>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequestDTO model)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.ProductImage)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new ApiResponse<OrderResponseDTO>(404, "Order not found.");
            }

            // Check terminal states
            if (order.OrderStatusId == 5)
            {
                return new ApiResponse<OrderResponseDTO>(400, "Cannot update status of a cancelled order.");
            }
            if (order.OrderStatusId == 4)
            {
                return new ApiResponse<OrderResponseDTO>(400, "Cannot update status of a delivered order.");
            }

            // Check target status validation
            var targetStatusExists = await _context.OrderStatuses.AnyAsync(s => s.Id == model.OrderStatusId);
            if (!targetStatusExists)
            {
                return new ApiResponse<OrderResponseDTO>(400, $"Order status with ID {model.OrderStatusId} does not exist.");
            }

            int current = order.OrderStatusId;
            int target = model.OrderStatusId;

            // Strict sequence enforcement: Pending (1) -> Processing (2) -> Shipped (3) -> Delivered (4)
            // Any active status (1, 2, 3) can also transition to Cancelled (5)
            bool isValidTransition = false;
            if (current == 1) // Pending
            {
                isValidTransition = (target == 2 || target == 5);
            }
            else if (current == 2) // Processing
            {
                isValidTransition = (target == 3 || target == 5);
            }
            else if (current == 3) // Shipped
            {
                isValidTransition = (target == 4 || target == 5);
            }

            if (!isValidTransition)
            {
                string expectedText = "";
                if (current == 1) expectedText = "Processing (2) or Cancelled (5)";
                if (current == 2) expectedText = "Shipped (3) or Cancelled (5)";
                if (current == 3) expectedText = "Delivered (4) or Cancelled (5)";

                return new ApiResponse<OrderResponseDTO>(400, 
                    $"Invalid status transition from {GetStatusName(current)} ({current}) to {GetStatusName(target)} ({target}). Allowed next states: {expectedText}.");
            }

            // If target status is Cancelled (5), restore stock
            if (target == 5)
            {
                order.CancelledAt = DateTime.UtcNow;
                foreach (var item in order.OrderProducts)
                {
                    if (item.ProductImage != null)
                    {
                        item.ProductImage.QuantityInStock += item.Quantity;
                    }
                }
            }

            // Update status
            order.OrderStatusId = target;
            await _context.SaveChangesAsync();

            return new ApiResponse<OrderResponseDTO>(200, MapToOrderResponseDTO(order));
        }

        private string GetStatusName(int id)
        {
            return id switch
            {
                1 => "Pending",
                2 => "Processing",
                3 => "Shipped",
                4 => "Delivered",
                5 => "Cancelled",
                _ => "Unknown"
            };
        }

        private OrderResponseDTO MapToOrderResponseDTO(Order order)
        {
            return new OrderResponseDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderStatusId = order.OrderStatusId,
                TotalAmountYer = order.TotalAmountYer,
                TotalAmountSar = order.TotalAmountSar,
                CreatedAt = order.CreatedAt,
                Items = order.OrderProducts.Select(op => new OrderProductResponseDTO
                {
                    ProductId = op.ProductId,
                    ProductName = op.Product?.Name,
                    ProductImageId = op.ProductImageId,
                    ImageUrl = op.ProductImage?.ImageUrl,
                    Quantity = op.Quantity,
                    UnitPrice = op.UnitPrice,
                    TotalAmount = op.TotalAmount,
                    CurrencyId = op.CurrencyId
                }).ToList()
            };
        }
    }
}
