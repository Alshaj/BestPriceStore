using BestPriceStore.DTOs;
using BestPriceStore.DTOs.UserDTOs;
using BestPriceStore.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BestPriceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Any authenticated user can access this controller, but specific methods may require Admin
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")] // Only Admin can approve
        public async Task<IActionResult> Approve(int id)
        {
            var response = await _userService.ApproveUserAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("{id}/suspend")]
        [Authorize(Roles = "Admin")] // Only Admin can suspend
        public async Task<IActionResult> Suspend(int id)
        {
            var response = await _userService.SuspendUserAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequestDTO updateProfileRequestDTO)
        {
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int id))
            {
                return Unauthorized(new ApiResponse<ConfirmationResponseDTO>(401, "User is not authenticated properly."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.UpdateProfileAsync(id, updateProfileRequestDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
