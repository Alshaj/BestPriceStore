using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.CurrencyDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestPriceStore.Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ApplicationDbContext _context;

        public CurrencyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CurrencyResponseDTO>>> GetAllCurrenciesAsync()
        {
            try
            {
                var currencies = await _context.Currencies
                    .Select(c => new CurrencyResponseDTO
                    {
                        Id = c.Id,
                        Name = c.Name ?? string.Empty
                    })
                    .ToListAsync();

                return new ApiResponse<List<CurrencyResponseDTO>>(200, currencies);
            }
            catch (Exception)
            {
                return new ApiResponse<List<CurrencyResponseDTO>>(500, "An error occurred while retrieving currencies.");
            }
        }
    }
}
