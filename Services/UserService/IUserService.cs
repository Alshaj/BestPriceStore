using BestPriceStore.DTOs;
using System.Threading.Tasks;

namespace BestPriceStore.Services.UserService
{
    public interface IUserService
    {
        Task<ApiResponse<ConfirmationResponseDTO>> ApproveUserAsync(int id);
        Task<ApiResponse<ConfirmationResponseDTO>> SuspendUserAsync(int id);
        Task<ApiResponse<ConfirmationResponseDTO>> UpdateProfileAsync(int id, BestPriceStore.DTOs.UserDTOs.UpdateProfileRequestDTO model);
        Task<ApiResponse<System.Collections.Generic.List<BestPriceStore.DTOs.UserDTOs.UserResponseDTO>>> GetAllRepresentativesAsync();
    }
}
