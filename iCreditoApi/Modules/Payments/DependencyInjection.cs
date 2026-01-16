using iCreditoApi.Modules.Payments.Application.Services;
using iCreditoApi.Modules.Payments.Domain.Repositories;
using iCreditoApi.Modules.Payments.Domain.Services;
using iCreditoApi.Modules.Payments.Infrastructure.Persistence;
using iCreditoApi.Modules.Payments.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace iCreditoApi.Modules.Payments;

/// <summary>
/// Extensiones para registrar el módulo Payments en el contenedor de DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Servicios de infraestructura
        services.AddScoped<IPaymentProcessor, SimulatedPaymentProcessor>();

        // Servicios de aplicación
        services.AddScoped<PaymentService>();

        return services;
    }
}
