using BestPriceStore.DTOs;
using BestPriceStore.DTOs.AuthDTOs;
using System.Threading.Tasks;

namespace BestPriceStore.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO registerRequestDTO);
        Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequestDTO);
    }
}
