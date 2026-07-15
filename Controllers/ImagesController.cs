using BestPriceStore.DTOs;
using BestPriceStore.Services.ImageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")] // Usually only admins should be uploading generic images, adjust if needed
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var url = await _imageService.UploadImageAsync(file);
                
                return Ok(new ApiResponse<object>(200, new 
                { 
                    Message = "Image uploaded successfully",
                    Url = url
                }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, $"Internal server error: {ex.Message}"));
            }
        }

        //[HttpPost]

        //public async Task<IActionResult> DeleteImage(string request)
        //{
        //    try
        //    {
        //        await _imageService.DeleteImageAsync(request);
        //        return Ok(new ApiResponse<object>(200, "Image deleted successfully"));
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new ApiResponse<object>(400, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<object>(500, $"Internal server error: {ex.Message}"));
        //    }
        //}
    }
}
