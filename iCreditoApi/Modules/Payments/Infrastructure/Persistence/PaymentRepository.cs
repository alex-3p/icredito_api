using iCreditoApi.Modules.Payments.Domain.Entities;
using iCreditoApi.Modules.Payments.Domain.Repositories;
using iCreditoApi.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace iCreditoApi.Modules.Payments.Infrastructure.Persistence;

/// <summary>
/// Implementación del repositorio de pagos
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Payment?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, ct);
    }

    public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken ct = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.Reference == reference, ct);
    }

    public async Task<IReadOnlyList<Payment>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Payment>> GetByCardIdAsync(
        Guid cardId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        return await _context.Payments
            .Where(p => p.CreditCardId == cardId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Payments
            .CountAsync(p => p.UserId == userId, ct);
    }

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        await _context.Payments.AddAsync(payment, ct);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}
