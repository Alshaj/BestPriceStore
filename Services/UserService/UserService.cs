using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.UserDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> ApproveUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new ApiResponse<ConfirmationResponseDTO>(404, "User not found.");
            }

            if (user.IsActive)
            {
                return new ApiResponse<ConfirmationResponseDTO>(400, "User is already approved.");
            }

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO { Message = "User has been successfully approved." });
            }

            return new ApiResponse<ConfirmationResponseDTO>
            {
                StatusCode = 400,
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> SuspendUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new ApiResponse<ConfirmationResponseDTO>(404, "User not found.");
            }

            if (!user.IsActive)
            {
                return new ApiResponse<ConfirmationResponseDTO>(400, "User is already suspended.");
            }

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO { Message = "User has been successfully suspended." });
            }

            return new ApiResponse<ConfirmationResponseDTO>
            {
                StatusCode = 400,
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProfileAsync(int id, UpdateProfileRequestDTO updateProfileRequestDTO)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new ApiResponse<ConfirmationResponseDTO>(404, "User not found.");
            }

            bool isChanged = false;

            if (user.StoreName != updateProfileRequestDTO.StoreName)
            {
                user.StoreName = updateProfileRequestDTO.StoreName;
                isChanged = true;
            }

            if (user.PhoneNumber != updateProfileRequestDTO.PhoneNumber)
            {
                user.PhoneNumber = updateProfileRequestDTO.PhoneNumber;
                user.UserName = updateProfileRequestDTO.PhoneNumber; // Update internal Identity username to match new phone number
                isChanged = true;
            }

            if (user.Location != updateProfileRequestDTO.Location)
            {
                user.Location = updateProfileRequestDTO.Location;
                isChanged = true;
            }

            if (!isChanged)
            {
                return new ApiResponse<ConfirmationResponseDTO>(400, "No changes were made to the profile.");
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO { Message = "Profile has been successfully updated." });
            }

            return new ApiResponse<ConfirmationResponseDTO>
            {
                StatusCode = 400,
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<ApiResponse<List<UserResponseDTO>>> GetAllRepresentativesAsync(string? search)
        {
            var users = await _userManager.GetUsersInRoleAsync("Representative");
            var usersQuery = users.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim();
                usersQuery = usersQuery.Where(u => 
                    (u.StoreName != null && u.StoreName.Contains(cleanSearch, System.StringComparison.OrdinalIgnoreCase)) ||
                    (u.Location != null && u.Location.Contains(cleanSearch, System.StringComparison.OrdinalIgnoreCase)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(cleanSearch, System.StringComparison.OrdinalIgnoreCase))
                );
            }

            var responseData = usersQuery.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                StoreName = u.StoreName,
                PhoneNumber = u.PhoneNumber,
                Location = u.Location,
                IsActive = u.IsActive
            }).ToList();

            return new ApiResponse<List<UserResponseDTO>>(200, responseData);
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> HardDeleteRepresentativeAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return new ApiResponse<ConfirmationResponseDTO>(404, "User not found.");
            }

            var isRepresentative = await _userManager.IsInRoleAsync(user, "Representative");
            if (!isRepresentative)
            {
                return new ApiResponse<ConfirmationResponseDTO>(400, "The specified user is not a representative.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete user's Cart & CartItems
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == id);

                if (cart != null)
                {
                    _context.CartItems.RemoveRange(cart.CartItems);
                    _context.Carts.Remove(cart);
                }

                // Delete user's Orders & OrderProducts
                var orders = await _context.Orders
                    .Include(o => o.OrderProducts)
                    .Where(o => o.UserId == id)
                    .ToListAsync();

                foreach (var order in orders)
                {
                    _context.OrderProducts.RemoveRange(order.OrderProducts);
                }
                _context.Orders.RemoveRange(orders);

                await _context.SaveChangesAsync();

                // Delete user entity and Identity user roles/claims/tokens
                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<ConfirmationResponseDTO>
                    {
                        StatusCode = 400,
                        Success = false,
                        Errors = deleteResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                await transaction.CommitAsync();

                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO
                {
                    Message = "Representative and all associated data have been permanently deleted."
                });
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An error occurred while deleting the representative: {ex.Message}");
            }
        }
    }
}
