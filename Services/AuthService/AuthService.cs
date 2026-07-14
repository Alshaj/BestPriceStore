using BestPriceStore.Data;
using BestPriceStore.DTOs;
using BestPriceStore.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BestPriceStore.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<ApiResponse<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerRequestDTO.PhoneNumber, // Use PhoneNumber as the internal Identity username
                StoreName = registerRequestDTO.StoreName,
                PhoneNumber = registerRequestDTO.PhoneNumber,
                Location = registerRequestDTO.Location,
                IsActive = false // Set to false until the admin approve this user
            };

            var result = await _userManager.CreateAsync(user, registerRequestDTO.Password);

            if (result.Succeeded)
            {
                // The user can be added to the default role here if needed, for instance "Representative".

                await _userManager.AddToRoleAsync(user, "Representative");

                var token = GenerateJwtToken(user);

                return new ApiResponse<RegisterResponseDTO>(201, new RegisterResponseDTO
                {
                    Id = user.Id,
                    Token = token,
                    StoreName = user.StoreName,
                    PhoneNumber = user.PhoneNumber,
                    Location = user.Location,
                    IsActive = user.IsActive
                });
            }
            
            return new ApiResponse<RegisterResponseDTO>
            {
                StatusCode = 400,
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_userManager.Users, u => u.PhoneNumber == loginRequestDTO.PhoneNumber);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password))
            {
                return new ApiResponse<LoginResponseDTO>
                {
                    StatusCode = 401,
                    Success = false,
                    Errors = new List<string> { "Invalid phone number or password." }
                };
            }
            var token = GenerateJwtToken(user);
            return new ApiResponse<LoginResponseDTO>(200, new LoginResponseDTO
            {
                Id = user.Id,
                Token = token,
                StoreName = user.StoreName,
                PhoneNumber = user.PhoneNumber,
                Location = user.Location,
                IsActive =  user.IsActive
            });
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Add user roles
            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(100), // Changed to make token essentially unexpired
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
