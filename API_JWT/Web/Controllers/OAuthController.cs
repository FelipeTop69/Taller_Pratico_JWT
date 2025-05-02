using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class OAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUser _userData;
        private readonly IAuthService _authService;

        // Flujo Authorization Code Grant

        // Simulacion en memoria
        private static readonly Dictionary<string, string> AuthCodes = [];
        private const string ClientId = "demo-client";
        private const string ClientSecret = "demo-secret";
        private const string RedirectUri = "https://localhost:5003/callback";

        public OAuthController(IConfiguration configuration,IUser userData, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
            _userData = userData;
        }

        // Solicitud del codigo
        [HttpPost("authorize/")]
        public async Task<IActionResult> Authorize([FromBody] AuthorizeRequestDTO request)
        {
            if (request.ResponseType != "code")
                return BadRequest("response_type must be 'code'");

            if (request.ClientId != ClientId || request.RedirectUri != RedirectUri)
                return Unauthorized("Invalid client_id or redirect_uri");

            var user = await _userData.GetByUsernameAsync(request.Username);
            if (user == null || user.Password != request.Password)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var code = Guid.NewGuid().ToString("N");
            AuthCodes[code] = request.Username;

            return Ok(new { code, redirect = $"{request.RedirectUri}?code={code}" });
        }


        //Intercambio de codigo por token
        [HttpPost("token/")]
        public async Task<IActionResult> Token([FromForm] string grant_type,
                                               [FromForm] string code,
                                               [FromForm] string client_id,
                                               [FromForm] string client_secret,
                                               [FromForm] string redirect_uri)
        {
            if (grant_type != "authorization_code")
                return BadRequest("grant_type must be 'authorization_code'");

            if (client_id != ClientId || client_secret != ClientSecret || redirect_uri != RedirectUri)
                return Unauthorized("Invalid client credentials");

            if (!AuthCodes.ContainsKey(code))
                return BadRequest("Invalid or expired authorization code");

            var username = AuthCodes[code];
            AuthCodes.Remove(code); // single-use code

            var user = await _userData.GetByUsernameAsync(username);
            if (user == null)
                return Unauthorized("User not found");

            var tokenData = await _authService.GenerateTokenAsync(username);
            return Ok(new
            {
                access_token = tokenData.Token,
                token_type = "Bearer",
                expires_in = tokenData.Expiration
            });
        }

        [Authorize]
        [HttpGet("protected/saludar/")]
        public IActionResult ProtectedSaludar()
        {
            var user = User.Identity?.Name ?? "Unknown";
            return Ok(new { message = $"Hola {user}, accediste al recurso protegido via OAuth" });
        }

        //===========================================================================================
        //===========================================================================================

        // Flujo Client Credentials Grant
        [HttpPost("client/token/")]
        public IActionResult ClientCredentials([FromForm] string client_id, [FromForm] string client_secret)
        {
            if (client_id != ClientId || client_secret != ClientSecret)
                return Unauthorized("Credenciales de Cliente Invalidas");

            // Simular token sin usuario
            var claims = new[]
            {
                new Claim("client_id", client_id),
                new Claim("scope", "api.read")
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

            return Ok(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "Bearer",
                expires_in = expires
            });
        }

        //===========================================================================================
        //===========================================================================================

        // Flujo Resource Owner Password Grant(password grant)
        [HttpPost("password/token/")]
        public async Task<IActionResult> PasswordGrant([FromForm] string username, [FromForm] string password)
        {
            var user = await _userData.GetByUsernameAsync(username);
            if (user == null || user.Password != password)
                return Unauthorized("Credenciales Invalidas");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var tokenData = await _authService.GenerateTokenAsync(username);

            return Ok(new
            {
                access_token = tokenData.Token,
                token_type = "Bearer",
                expires_in = tokenData.Expiration,
                role
            });
        }
    }
}
