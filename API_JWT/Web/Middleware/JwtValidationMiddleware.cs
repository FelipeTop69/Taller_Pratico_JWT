using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
            var endpoint = context.GetEndpoint();
            var requiresAuth = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null;

            if (requiresAuth)
            {
                var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

                if (!isAuthenticated)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Token inválido o ausente" });
                    return;
                }

                var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var username = context.User.Identity?.Name;

                if (string.IsNullOrEmpty(role))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Token sin rol. Acceso denegado" });
                    return;
                }

                // Solo permite rol "Administrador"
                if (role != "Admin")
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = $"Acceso restringido. Se requiere rol 'Admin'. Tu rol: '{role}'",
                        user = username
                    });
                    return;
                }

                _logger.LogInformation("Acceso autorizado: {Username} con rol {Role}", username, role);
            }

            await _next(context);
        }
    }
}
