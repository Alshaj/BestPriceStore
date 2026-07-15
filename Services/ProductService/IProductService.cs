using BestPriceStore.DTOs;
using BestPriceStore.DTOs.ProductDTOs;
using System.Threading.Tasks;

namespace BestPriceStore.Services.ProductService
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(CreateProductRequestDTO model);
        Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync(string? search, int? categoryId);
        Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductRequestDTO model);
        Task<ApiResponse<ConfirmationResponseDTO>> ActivateProductAsync(int id);
        Task<ApiResponse<ConfirmationResponseDTO>> DeactivateProductAsync(int id);
    }
}
