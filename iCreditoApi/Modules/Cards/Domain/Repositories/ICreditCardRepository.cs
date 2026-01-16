using iCreditoApi.Modules.Cards.Domain.Entities;

namespace iCreditoApi.Modules.Cards.Domain.Repositories;

/// <summary>
/// Puerto de salida para persistencia de tarjetas de crédito
/// </summary>
public interface ICreditCardRepository
{
    Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CreditCard?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<CreditCard>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> CardNumberExistsAsync(string cardNumber, CancellationToken ct = default);
    Task AddAsync(CreditCard card, CancellationToken ct = default);
    void Update(CreditCard card);
    void Delete(CreditCard card);
}
