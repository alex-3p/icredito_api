using iCreditoApi.Modules.Transactions.Application.Services;
using iCreditoApi.Modules.Transactions.Domain.Repositories;
using iCreditoApi.Modules.Transactions.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace iCreditoApi.Modules.Transactions;

/// <summary>
/// Extensiones para registrar el módulo Transactions en el contenedor de DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
        // Repositorios
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Servicios de aplicación
        services.AddScoped<TransactionService>();

        return services;
    }
}
