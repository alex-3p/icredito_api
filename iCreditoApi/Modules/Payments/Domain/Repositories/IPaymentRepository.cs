using iCreditoApi.Modules.Payments.Domain.Entities;

namespace iCreditoApi.Modules.Payments.Domain.Repositories;

/// <summary>
/// Puerto de salida para persistencia de pagos
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Payment?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<Payment?> GetByReferenceAsync(string reference, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetByCardIdAsync(Guid cardId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Payment payment, CancellationToken ct = default);
    void Update(Payment payment);
}
