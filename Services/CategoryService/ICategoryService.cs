using BestPriceStore.DTOs;
using BestPriceStore.DTOs.CategoryDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPriceStore.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryResponseDTO>> CreateCategoryAsync(CreateCategoryRequestDTO model);
        Task<ApiResponse<CategoryResponseDTO>> UpdateCategoryAsync(int id, UpdateCategoryRequestDTO model);
        Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategoryAsync(int id);
    }
}
