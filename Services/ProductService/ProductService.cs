using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.ProductDTOs;
using BestPriceStore.Helpers;
using BestPriceStore.Models;
using BestPriceStore.Services.ImageService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public ProductService(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(CreateProductRequestDTO model)
        {
            // Verify Category exists
            var category = await _context.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                return new ApiResponse<ProductResponseDTO>(400, $"Category with ID {model.CategoryId} does not exist.");
            }

            // Verify Currency exists
            var currency = await _context.Currencies.FindAsync(model.CurrencyId);
            if (currency == null)
            {
                return new ApiResponse<ProductResponseDTO>(400, $"Currency with ID {model.CurrencyId} does not exist.");
            }

            // Start Transaction to ensure product and its images are both saved successfully
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var yemenNow = DateTimeHelper.GetYemeniTime();

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CurrencyId = model.CurrencyId,
                    CategoryId = model.CategoryId,
                    CreatedAt = yemenNow,
                    UpdatedAt = yemenNow,
                    IsActive = true
                };

                _context.Products.Add(product);
                // Save changes first to generate Product ID
                await _context.SaveChangesAsync();

                var productImages = new List<ProductImage>();

                if (model.Images != null && model.Images.Count > 0)
                {
                    // Check if the request explicitly marked any image as primary
                    bool hasPrimaryInRequest = model.Images.Any(img => img.IsPrimary);

                    for (int i = 0; i < model.Images.Count; i++)
                    {
                        var imgDto = model.Images[i];
                        
                        // Automatically set the first image as primary if none is explicitly selected
                        bool isPrimary = imgDto.IsPrimary || (!hasPrimaryInRequest && i == 0);

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = imgDto.ImageUrl,
                            QuantityInStock = imgDto.QuantityInStock,
                            IsPrimary = isPrimary
                        };

                        productImages.Add(productImage);
                    }

                    _context.ProductImages.AddRange(productImages);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // Build response DTO by fetching loaded entities to ensure names/relationships are populated correctly
                var responseImages = productImages.Select(pi => new ProductImageResponseDTO
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageUrl,
                    QuantityInStock = pi.QuantityInStock,
                    IsPrimary = pi.IsPrimary
                }).ToList();

                var response = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CurrencyId = product.CurrencyId,
                    CurrencyName = currency.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = category.Name,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsActive = product.IsActive,
                    Images = responseImages
                };

                return new ApiResponse<ProductResponseDTO>(201, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<ProductResponseDTO>(500, $"An error occurred while creating the product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync(string? search, int? categoryId, bool isAdmin)
        {
            try
            {
                var query = _context.Products
                    .Where(p => !p.IsDeleted)
                    .Include(p => p.Category)
                    .Include(p => p.Currency)
                    .Include(p => p.ProductImages)
                    .AsQueryable();

                if (!isAdmin)
                {
                    query = query.Where(p => p.IsActive);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var cleanSearch = search.Trim();
                    query = query.Where(p => (p.Name != null && p.Name.Contains(cleanSearch)) || 
                                             (p.Description != null && p.Description.Contains(cleanSearch)));
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                var products = await query
                    .Select(p => new ProductResponseDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        CurrencyId = p.CurrencyId,
                        CurrencyName = p.Currency != null ? p.Currency.Name : null,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : null,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        IsActive = p.IsActive,
                        Images = p.ProductImages.Where(pi => !pi.IsDeleted).Select(pi => new ProductImageResponseDTO
                        {
                            Id = pi.Id,
                            ImageUrl = pi.ImageUrl,
                            QuantityInStock = pi.QuantityInStock,
                            IsPrimary = pi.IsPrimary
                        }).ToList()
                    })
                    .ToListAsync();

                return new ApiResponse<List<ProductResponseDTO>>(200, products);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, $"An error occurred while retrieving products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaginatedList<ProductBrowseResponseDTO>>> GetBrowseProductsAsync(string? search, int? categoryId, int pageNumber, int pageSize, bool isAdmin)
        {
            try
            {
                var query = _context.Products
                    .Where(p => !p.IsDeleted)
                    .AsQueryable();

                if (!isAdmin)
                {
                    query = query.Where(p => p.IsActive);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var cleanSearch = search.Trim();
                    query = query.Where(p => (p.Name != null && p.Name.Contains(cleanSearch)) || 
                                             (p.Description != null && p.Description.Contains(cleanSearch)));
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(p => p.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductBrowseResponseDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        CurrencyId = p.CurrencyId,
                        PrimaryImageUrl = p.ProductImages
                            .Where(pi => !pi.IsDeleted)
                            .OrderByDescending(pi => pi.IsPrimary)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var paginatedList = new PaginatedList<ProductBrowseResponseDTO>(items, totalCount, pageNumber, pageSize);
                return new ApiResponse<PaginatedList<ProductBrowseResponseDTO>>(200, paginatedList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<ProductBrowseResponseDTO>>(500, $"An error occurred while retrieving browse products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync(int id, bool isAdmin)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Currency)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (product == null || (!product.IsActive && !isAdmin))
                {
                    return new ApiResponse<ProductResponseDTO>(404, "Product not found.");
                }

                var response = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CurrencyId = product.CurrencyId,
                    CurrencyName = product.Currency != null ? product.Currency.Name : null,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category != null ? product.Category.Name : null,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsActive = product.IsActive,
                    Images = product.ProductImages.Where(pi => !pi.IsDeleted).Select(pi => new ProductImageResponseDTO
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageUrl,
                        QuantityInStock = pi.QuantityInStock,
                        IsPrimary = pi.IsPrimary
                    }).ToList()
                };

                return new ApiResponse<ProductResponseDTO>(200, response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, $"An error occurred while retrieving the product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductRequestDTO model)
        {
            // Verify Category exists
            var category = await _context.Categories.FindAsync(model.CategoryId);
            if (category == null)
            {
                return new ApiResponse<ProductResponseDTO>(400, $"Category with ID {model.CategoryId} does not exist.");
            }

            // Verify Currency exists
            var currency = await _context.Currencies.FindAsync(model.CurrencyId);
            if (currency == null)
            {
                return new ApiResponse<ProductResponseDTO>(400, $"Currency with ID {model.CurrencyId} does not exist.");
            }

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return new ApiResponse<ProductResponseDTO>(404, "Product not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CurrencyId = model.CurrencyId;
                product.CategoryId = model.CategoryId;
                product.UpdatedAt = DateTimeHelper.GetYemeniTime();

                // 1. Identify deleted images (currently in DB, but not in update request Images list)
                var requestImageIds = model.Images
                    .Where(img => img.Id.HasValue)
                    .Select(img => img.Id!.Value)
                    .ToList();

                var deletedImages = product.ProductImages
                    .Where(pi => !requestImageIds.Contains(pi.Id))
                    .ToList();

                foreach (var deletedImg in deletedImages)
                {
                    deletedImg.IsDeleted = true;
                    deletedImg.QuantityInStock = 0;
                    _context.ProductImages.Update(deletedImg);
                }

                // 2. Process kept and new images
                var updatedProductImages = new List<ProductImage>();

                // Check if there is any primary image explicitly designated in the request
                bool hasPrimaryInRequest = model.Images.Any(img => img.IsPrimary);

                for (int i = 0; i < model.Images.Count; i++)
                {
                    var imgDto = model.Images[i];
                    bool isPrimary = imgDto.IsPrimary || (!hasPrimaryInRequest && i == 0);

                    if (imgDto.Id.HasValue)
                    {
                        // Existing image in DB
                        var dbImage = product.ProductImages.FirstOrDefault(pi => pi.Id == imgDto.Id.Value);
                        if (dbImage != null)
                        {
                            // If the image URL changed (replaced), delete the old file first!
                            if (dbImage.ImageUrl != imgDto.ImageUrl)
                            {
                                if (!string.IsNullOrWhiteSpace(dbImage.ImageUrl))
                                {
                                    await _imageService.DeleteImageAsync(dbImage.ImageUrl);
                                }
                                dbImage.ImageUrl = imgDto.ImageUrl;
                            }

                            dbImage.QuantityInStock = imgDto.QuantityInStock;
                            dbImage.IsPrimary = isPrimary;
                            
                            _context.ProductImages.Update(dbImage);
                            updatedProductImages.Add(dbImage);
                        }
                    }
                    else
                    {
                        // New image to insert
                        var newImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = imgDto.ImageUrl,
                            QuantityInStock = imgDto.QuantityInStock,
                            IsPrimary = isPrimary
                        };
                        _context.ProductImages.Add(newImage);
                        updatedProductImages.Add(newImage);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Build response
                var responseImages = updatedProductImages.Select(pi => new ProductImageResponseDTO
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageUrl,
                    QuantityInStock = pi.QuantityInStock,
                    IsPrimary = pi.IsPrimary
                }).ToList();

                var response = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CurrencyId = product.CurrencyId,
                    CurrencyName = currency.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = category.Name,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    IsActive = product.IsActive,
                    Images = responseImages
                };

                return new ApiResponse<ProductResponseDTO>(200, response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<ProductResponseDTO>(500, $"An error occurred while updating the product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> ActivateProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }

                product.IsActive = true;
                product.UpdatedAt = DateTimeHelper.GetYemeniTime();

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO
                {
                    Message = "Product has been successfully activated."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An error occurred while activating the product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeactivateProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }

                product.IsActive = false;
                product.UpdatedAt = DateTimeHelper.GetYemeniTime();

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO
                {
                    Message = "Product has been successfully deactivated."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An error occurred while deactivating the product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductBrowseResponseDTO>>> GetLatestProductsAsync(bool isAdmin)
        {
            try
            {
                var query = _context.Products
                    .Where(p => !p.IsDeleted);

                if (!isAdmin)
                {
                    query = query.Where(p => p.IsActive);
                }

                var products = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new ProductBrowseResponseDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        CurrencyId = p.CurrencyId,
                        PrimaryImageUrl = p.ProductImages
                            .Where(pi => !pi.IsDeleted)
                            .OrderByDescending(pi => pi.IsPrimary)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return new ApiResponse<List<ProductBrowseResponseDTO>>(200, products);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductBrowseResponseDTO>>(500, $"An error occurred while fetching latest products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductBestSellerResponseDTO>>> GetTopSellingProductsAsync(bool isAdmin)
        {
            try
            {
                var topSelling = await _context.OrderProducts
                    .Include(op => op.Order)
                    .Where(op => op.Order != null && op.Order.OrderStatusId == 4) // Only delivered orders
                    .GroupBy(op => op.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalSold = g.Sum(op => op.Quantity)
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(10)
                    .ToListAsync();

                var productIds = topSelling.Select(x => x.ProductId).ToList();

                var query = _context.Products.Where(p => productIds.Contains(p.Id) && !p.IsDeleted);
                if (!isAdmin)
                {
                    query = query.Where(p => p.IsActive);
                }

                var products = await query
                    .Include(p => p.ProductImages)
                    .ToListAsync();

                var result = topSelling
                    .Select(ts => {
                        var p = products.FirstOrDefault(prod => prod.Id == ts.ProductId);
                        if (p == null) return null;

                        return new ProductBestSellerResponseDTO
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            CurrencyId = p.CurrencyId,
                            PrimaryImageUrl = p.ProductImages
                                .Where(pi => !pi.IsDeleted)
                                .OrderByDescending(pi => pi.IsPrimary)
                                .Select(pi => pi.ImageUrl)
                                .FirstOrDefault(),
                            TotalQuantitySold = ts.TotalSold
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                return new ApiResponse<List<ProductBestSellerResponseDTO>>(200, result!);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductBestSellerResponseDTO>>(500, $"An error occurred while fetching top selling products: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> SoftDeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }

                product.IsDeleted = true;
                product.UpdatedAt = DateTimeHelper.GetYemeniTime();

                foreach (var img in product.ProductImages)
                {
                    img.IsDeleted = true;
                    img.QuantityInStock = 0;
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return new ApiResponse<ConfirmationResponseDTO>(200, new ConfirmationResponseDTO
                {
                    Message = "Product and its images have been successfully deleted softly."
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An error occurred while deleting the product: {ex.Message}");
            }
        }
    }
}
