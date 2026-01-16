using iCreditoApi.BFF.Services;
using Microsoft.Extensions.DependencyInjection;

namespace iCreditoApi.BFF;

/// <summary>
/// Extensiones para registrar la capa BFF en el contenedor de DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBffServices(this IServiceCollection services)
    {
        // Agregadores
        services.AddScoped<DashboardAggregator>();

        return services;
    }
}
