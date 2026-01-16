using iCreditoApi.Modules.Transactions.Domain.Entities;
using iCreditoApi.Modules.Transactions.Domain.Enums;
using iCreditoApi.Modules.Transactions.Domain.Repositories;
using iCreditoApi.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using static iCreditoApi.Modules.Transactions.Domain.Repositories.ITransactionRepository;

namespace iCreditoApi.Modules.Transactions.Infrastructure.Persistence;

/// <summary>
/// Implementación del repositorio de transacciones
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Transaction?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByCardIdAsync(
        Guid cardId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        return await _context.Transactions
            .Where(t => t.CreditCardId == cardId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId && t.CreatedAt >= from && t.CreatedAt <= to)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Transactions
            .CountAsync(t => t.UserId == userId, ct);
    }

    public async Task<int> GetCountByCardIdAsync(Guid cardId, CancellationToken ct = default)
    {
        return await _context.Transactions
            .CountAsync(t => t.CreditCardId == cardId, ct);
    }

    public async Task<decimal> GetTotalSpentThisMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Type == TransactionType.Purchase
                && t.CreatedAt >= startOfMonth)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<decimal> GetTotalSpentLastMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var startOfLastMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1);
        var endOfLastMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1);

        return await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Type == TransactionType.Purchase
                && t.CreatedAt >= startOfLastMonth
                && t.CreatedAt <= endOfLastMonth)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<decimal> GetTotalPaymentsThisMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Type == TransactionType.Payment
                && t.CreatedAt >= startOfMonth)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<decimal> GetTotalPaymentsLastMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var startOfLastMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1);
        var endOfLastMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1);

        return await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Type == TransactionType.Payment
                && t.CreatedAt >= startOfLastMonth
                && t.CreatedAt <= endOfLastMonth)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<int> GetTransactionCountThisMonthAsync(Guid userId, CancellationToken ct = default)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return await _context.Transactions
            .CountAsync(t => t.UserId == userId && t.CreatedAt >= startOfMonth, ct);
    }

    public async Task<TransactionStats> GetTransactionStatsAsync(Guid userId, CancellationToken ct = default)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && t.CreatedAt >= startOfMonth)
            .ToListAsync(ct);

        var totalPurchases = transactions.Count(t => t.Type == TransactionType.Purchase);
        var totalPayments = transactions.Count(t => t.Type == TransactionType.Payment);
        var totalRefunds = transactions.Count(t => t.Type == TransactionType.Refund);
        var averageAmount = transactions.Count > 0 ? transactions.Average(t => t.Amount) : 0;
        var largestAmount = transactions.Count > 0 ? transactions.Max(t => t.Amount) : 0;

        // Top merchant
        var topMerchant = transactions
            .Where(t => t.Type == TransactionType.Purchase && !string.IsNullOrEmpty(t.MerchantName))
            .GroupBy(t => t.MerchantName)
            .Select(g => new { Name = g.Key, Total = g.Sum(t => t.Amount), Count = g.Count() })
            .OrderByDescending(x => x.Total)
            .FirstOrDefault();

        return new TransactionStats(
            totalPurchases,
            totalPayments,
            totalRefunds,
            Math.Round(averageAmount, 2),
            largestAmount,
            topMerchant?.Name,
            topMerchant?.Total ?? 0,
            topMerchant?.Count ?? 0);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _context.Transactions.AddAsync(transaction, ct);
    }
}
