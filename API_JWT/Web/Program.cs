using Business.Interfaces;
using Business.Services;
using Data;
using Data.Interfaces;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Web.Extensions;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Configuraciones
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)    // Configuracion public
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true) // Configuracion privada (opcional)
    .AddEnvironmentVariables(); // Para producción (sobrescribe todo)

// Add services to the container.

builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwtSupport(); 

//Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(connectionString));

//JWT
//var jwtKey = builder.Configuration["Jwt:Key"];
//Console.WriteLine($"JWT Key: {jwtKey}"); 
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUser, UserData>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<JwtValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
