using BestPriceStore.DTOs;
using BestPriceStore.DTOs.CurrencyDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BestPriceStore.Services.CurrencyService
{
    public interface ICurrencyService
    {
        Task<ApiResponse<List<CurrencyResponseDTO>>> GetAllCurrenciesAsync();
    }
}
