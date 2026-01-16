using iCreditoApi.Modules.Transactions.Domain.Entities;

namespace iCreditoApi.Modules.Transactions.Domain.Repositories;

/// <summary>
/// Puerto de salida para persistencia de transacciones
/// </summary>
public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Transaction?> GetByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<Transaction>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<IReadOnlyList<Transaction>> GetByCardIdAsync(
        Guid cardId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default);

    Task<int> GetCountByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetCountByCardIdAsync(Guid cardId, CancellationToken ct = default);

    Task<decimal> GetTotalSpentThisMonthAsync(Guid userId, CancellationToken ct = default);
    Task<decimal> GetTotalSpentLastMonthAsync(Guid userId, CancellationToken ct = default);
    Task<decimal> GetTotalPaymentsThisMonthAsync(Guid userId, CancellationToken ct = default);
    Task<decimal> GetTotalPaymentsLastMonthAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetTransactionCountThisMonthAsync(Guid userId, CancellationToken ct = default);
    Task<TransactionStats> GetTransactionStatsAsync(Guid userId, CancellationToken ct = default);

    Task AddAsync(Transaction transaction, CancellationToken ct = default);
}

/// <summary>
/// Estadísticas de transacciones para KPIs
/// </summary>
public record TransactionStats(
    int TotalPurchases,
    int TotalPayments,
    int TotalRefunds,
    decimal AverageAmount,
    decimal LargestAmount,
    string? TopMerchantName,
    decimal TopMerchantTotal,
    int TopMerchantCount);
