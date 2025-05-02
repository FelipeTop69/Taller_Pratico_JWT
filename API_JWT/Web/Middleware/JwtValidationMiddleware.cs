using System.Security.Claims;

namespace Web.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtValidationMiddleware> _logger;

        public JwtValidationMiddleware(RequestDelegate next, ILogger<JwtValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Solo aplica a rutas protegidas
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var username = context.User.Identity.Name;
                var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                _logger.LogInformation("JWT válido para {Username} con rol {Role}", username, role);
            }
            else if (path.StartsWithSegments("/api/oauth/protected"))
            {
                // Ruta protegida sin token válido
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Token JWT inválido o ausente. Acceso denegado."
                });
                return;
            }

            await _next(context);
        }
    }
}
