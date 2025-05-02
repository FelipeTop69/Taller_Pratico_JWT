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

        private string BuildToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<AuthResponseDTO?> AuthenticateAsync(AuthRequestDTO request)
        {
            var user = await _userData.GetByUsernameAsync(request.Username);
            if (user == null || user.Password != request.Password)
                return null;

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var token = BuildToken(user.Username, role);

            return new AuthResponseDTO
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(3).ToLocalTime(),
                Username = user.Username,
                Role = role
            };
        }

        public async Task<AccessTokenDTO> GenerateTokenAsync(string username)
        {
            var user = await _userData.GetByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var token = BuildToken(user.Username, role);

            return new AccessTokenDTO
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(3).ToLocalTime()    
            };
        }
    }
}
