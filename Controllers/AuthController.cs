using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.AuthDTOs;
using BestPriceStore.Services.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestPriceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponseDTO>>> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            var response = await _authService.RegisterAsync(registerRequestDTO);
            if (response.StatusCode != 201)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login(LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }
            var response = await _authService.LoginAsync(loginRequestDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);

        }
    }
}
