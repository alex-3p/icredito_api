using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using iCreditoApi.Shared.Infrastructure.Persistence;
using iCreditoApi.Shared.Infrastructure.Services;
using iCreditoApi.Shared.Application.Interfaces;
using iCreditoApi.Modules.Auth;
using iCreditoApi.Modules.Cards;
using iCreditoApi.Modules.Payments;
using iCreditoApi.Modules.Transactions;
using iCreditoApi.BFF;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Servicios básicos
// ========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "iCredito API",
        Version = "v1",
        Description = "API para gestión de tarjetas de crédito - Arquitectura Hexagonal"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ========================================
// Base de datos
// ========================================
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork>(sp =>
    sp.GetRequiredService<AppDbContext>());

// ========================================
// Autenticación JWT
// ========================================
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured in appsettings.json");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ========================================
// Servicios compartidos
// ========================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ========================================
// Módulos (Arquitectura Modular)
// ========================================
builder.Services.AddAuthModule();
builder.Services.AddCardsModule();
builder.Services.AddPaymentsModule();
builder.Services.AddTransactionsModule();

// ========================================
// BFF (Backend for Frontend)
// ========================================
builder.Services.AddBffServices();

// ========================================
// CORS (para React Frontend)
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:9002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ========================================
// Pipeline de Middleware
// ========================================

// Swagger (siempre habilitado para desarrollo)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "iCredito API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========================================
// Aplicar migraciones automáticamente (desarrollo)
// ========================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
