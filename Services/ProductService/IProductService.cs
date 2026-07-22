using BestPriceStore.DTOs;
using BestPriceStore.DTOs.ProductDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPriceStore.Services.ProductService
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(CreateProductRequestDTO model);
        Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync(string? search, int? categoryId, bool isAdmin);
        Task<ApiResponse<PaginatedList<ProductBrowseResponseDTO>>> GetBrowseProductsAsync(string? search, int? categoryId, int pageNumber, int pageSize, bool isAdmin);
        Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync(int id, bool isAdmin);
        Task<ApiResponse<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductRequestDTO model);
        Task<ApiResponse<ConfirmationResponseDTO>> ActivateProductAsync(int id);
        Task<ApiResponse<ConfirmationResponseDTO>> DeactivateProductAsync(int id);
        Task<ApiResponse<ConfirmationResponseDTO>> SoftDeleteProductAsync(int id);
        Task<ApiResponse<List<ProductBrowseResponseDTO>>> GetLatestProductsAsync(bool isAdmin);
        Task<ApiResponse<List<ProductBestSellerResponseDTO>>> GetTopSellingProductsAsync(bool isAdmin);
    }
}
