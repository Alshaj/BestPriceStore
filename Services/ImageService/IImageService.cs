using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BestPriceStore.Services.ImageService
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
