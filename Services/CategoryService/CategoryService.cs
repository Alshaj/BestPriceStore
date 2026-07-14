using BestPriceStore.Data;
using BestPriceStore.Models;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.CategoryDTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

                return new ApiResponse<List<CategoryResponseDTO>>(200, categories);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryResponseDTO>>(500, "An error occurred while retrieving categories.");
            }
        }

        public async Task<ApiResponse<CategoryResponseDTO>> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return new ApiResponse<CategoryResponseDTO>(404, "Category not found.");
            }

            var response = new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name
            };

            return new ApiResponse<CategoryResponseDTO>(200, response);
        }

        public async Task<ApiResponse<CategoryResponseDTO>> CreateCategoryAsync(CreateCategoryRequestDTO model)
        {

            try
            {
                var category = new Category
                {
                    Name = model.Name
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var response = new CategoryResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return new ApiResponse<CategoryResponseDTO>(201, response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500, "An error occurred while creating the category.");
            }
        }

        public async Task<ApiResponse<CategoryResponseDTO>> UpdateCategoryAsync(int id, UpdateCategoryRequestDTO model)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return new ApiResponse<CategoryResponseDTO>(404, "Category not found.");
            }

            category.Name = model.Name;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            var response = new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name
            };

            return new ApiResponse<CategoryResponseDTO>(200, response);
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return new ApiResponse<ConfirmationResponseDTO>(404, "Category not found.");
            }

            // Find all products in this category
            var productIds = await _context.Products
                .Where(p => p.CategoryId == id)
                .Select(p => p.Id)
                .ToListAsync();

            if (productIds.Any())
            {
                // Delete related OrderProducts to avoid foreign key conflicts (DeleteBehavior.Restrict)
                var orderProducts = await _context.OrderProducts
                    .Where(op => productIds.Contains(op.ProductId))
                    .ToListAsync();

                if (orderProducts.Any())
                {
                    _context.OrderProducts.RemoveRange(orderProducts);
                }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO { Message = "Category has been successfully deleted." });
        }
    }
}
