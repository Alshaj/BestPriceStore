using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.UserDTOs;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
    }
}
