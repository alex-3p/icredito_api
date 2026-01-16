namespace iCreditoApi.Shared.Application.Interfaces;

/// <summary>
/// Interfaz para el patrón Unit of Work
/// Permite agrupar múltiples operaciones en una transacción
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
