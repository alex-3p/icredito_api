using iCreditoApi.Modules.Cards.Application.Services;
using iCreditoApi.Modules.Cards.Domain.Repositories;
using iCreditoApi.Modules.Cards.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace iCreditoApi.Modules.Cards;

/// <summary>
/// Extensiones para registrar el módulo Cards en el contenedor de DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCardsModule(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<ICreditCardRepository, CreditCardRepository>();

        // Servicios de aplicación
        services.AddScoped<CreditCardService>();

        return services;
    }
}
