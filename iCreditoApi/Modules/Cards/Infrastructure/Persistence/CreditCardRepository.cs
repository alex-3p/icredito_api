using iCreditoApi.Modules.Cards.Domain.Entities;
using iCreditoApi.Modules.Cards.Domain.Repositories;
using iCreditoApi.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace iCreditoApi.Modules.Cards.Infrastructure.Persistence;

/// <summary>
/// Implementación del repositorio de tarjetas de crédito
/// </summary>
public class CreditCardRepository : ICreditCardRepository
{
    private readonly AppDbContext _context;

    public CreditCardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CreditCard?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<CreditCard?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<CreditCard>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.CreditCards
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.CreditCards
            .AnyAsync(c => c.Id == id, ct);
    }

    public async Task<bool> CardNumberExistsAsync(string cardNumber, CancellationToken ct = default)
    {
        return await _context.CreditCards
            .AnyAsync(c => c.CardNumber == cardNumber, ct);
    }

    public async Task AddAsync(CreditCard card, CancellationToken ct = default)
    {
        await _context.CreditCards.AddAsync(card, ct);
    }

    public void Update(CreditCard card)
    {
        _context.CreditCards.Update(card);
    }

    public void Delete(CreditCard card)
    {
        _context.CreditCards.Remove(card);
    }
}
