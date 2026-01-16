using iCreditoApi.Modules.Auth.Application.Ports;
using iCreditoApi.Modules.Auth.Application.Services;
using iCreditoApi.Modules.Auth.Domain.Repositories;
using iCreditoApi.Modules.Auth.Infrastructure.Adapters;
using iCreditoApi.Modules.Auth.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace iCreditoApi.Modules.Auth;

/// <summary>
/// Extensiones para registrar el módulo Auth en el contenedor de DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();

        // Servicios de infraestructura
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // Servicios de aplicación
        services.AddScoped<AuthService>();

        return services;
    }
}
