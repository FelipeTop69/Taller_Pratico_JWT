using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Web.Options;

namespace Web.Extensions
{
    public static class JwtConfigurationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Validación explícita
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration is missing");

            if (string.IsNullOrEmpty(jwtOptions.Key))
                throw new ArgumentNullException("Jwt:Key is required");

            // Registrar opciones para DI
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            // Configurar autenticación
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.Key))
                    };
                });

            return services;
        }
    }
}
