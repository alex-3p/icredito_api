using Microsoft.EntityFrameworkCore;
using iCreditoApi.Shared.Application.Interfaces;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Auth.Domain.Entities;
using iCreditoApi.Modules.Cards.Domain.Entities;
using iCreditoApi.Modules.Payments.Domain.Entities;
using iCreditoApi.Modules.Transactions.Domain.Entities;

namespace iCreditoApi.Shared.Infrastructure.Persistence;

/// <summary>
/// DbContext principal de la aplicación
/// Implementa IUnitOfWork para el patrón Unit of Work
/// </summary>
public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // DbSets por módulo
    public DbSet<User> Users => Set<User>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar todas las configuraciones del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Obtener eventos de dominio antes de guardar
        var aggregateRoots = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(a => a.DomainEvents)
            .ToList();

        // Guardar cambios
        var result = await base.SaveChangesAsync(cancellationToken);

        // Limpiar eventos después de guardar
        foreach (var aggregate in aggregateRoots)
        {
            aggregate.ClearDomainEvents();
        }

        // TODO: Publicar eventos de dominio si se implementa un event dispatcher
        // foreach (var domainEvent in domainEvents)
        // {
        //     await _eventDispatcher.Dispatch(domainEvent, cancellationToken);
        // }

        return result;
    }
}
