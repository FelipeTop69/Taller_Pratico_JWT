using Business.Interfaces;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ejercicio1/")]
        public async Task<IActionResult> GenerateToken([FromBody] AuthRequestDTO request)
        {
            var result = await _authService.AuthenticateAsync(request);
            if (result == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(result);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("ejercicio2/")]
        public async Task<IActionResult> AuthenticateBasic([FromBody] AuthRequestDTO request)
        {
            var user = await _authService.AuthenticateAsync(request);
            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(new { message = $"Bienvenido, {user.Username}" });
        }
    }


}
