using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUser _userData;
        private  readonly IConfiguration _configuration;

        public AuthService(IUser userData, IConfiguration configuration)
        {
            _userData = userData;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> AuthenticateAsync(AuthRequestDTO request)
        {
            var user = await _userData.GetByUsernameAsync(request.Username);
            if (user == null) return null;

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(3);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires.ToLocalTime(),
                Username = user.Username,
                Role = role
            };
        }
    }
}
