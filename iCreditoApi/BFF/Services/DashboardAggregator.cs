using iCreditoApi.BFF.ViewModels;
using iCreditoApi.Modules.Auth.Application.Services;
using iCreditoApi.Modules.Cards.Application.Services;
using iCreditoApi.Modules.Transactions.Application.Services;
using iCreditoApi.Modules.Transactions.Domain.Repositories;
using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.BFF.Services;

/// <summary>
/// Agregador para el dashboard
/// Combina datos de múltiples módulos en una sola respuesta optimizada
/// </summary>
public class DashboardAggregator
{
    private readonly AuthService _authService;
    private readonly CreditCardService _cardService;
    private readonly TransactionService _transactionService;
    private readonly ITransactionRepository _transactionRepository;

    public DashboardAggregator(
        AuthService authService,
        CreditCardService cardService,
        TransactionService transactionService,
        ITransactionRepository transactionRepository)
    {
        _authService = authService;
        _cardService = cardService;
        _transactionService = transactionService;
        _transactionRepository = transactionRepository;
    }

    /// <summary>
    /// Obtiene los datos agregados del dashboard
    /// </summary>
    public async Task<Result<DashboardViewModel>> GetDashboardAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        // Ejecutar consultas secuencialmente
        // (DbContext no es thread-safe, todos los servicios comparten el mismo DbContext)
        var userResult = await _authService.GetProfileAsync(userId, ct);
        var cardsResult = await _cardService.GetUserCardsAsync(userId, ct);
        var transactionsResult = await _transactionService.GetHistoryAsync(userId, 1, 10, ct);

        // Verificar errores
        if (userResult.IsFailure)
            return Result.Failure<DashboardViewModel>(userResult.Error);

        var user = userResult.Value;
        var cards = cardsResult.IsSuccess ? cardsResult.Value : Array.Empty<Modules.Cards.Application.DTOs.CardSummaryDto>();
        var transactions = transactionsResult.IsSuccess ? transactionsResult.Value : null;

        // Construir resumen de usuario
        var userSummary = new UserSummary(
            user.Id,
            user.FullName,
            user.Email,
            user.LastLoginAt);

        // Construir resumen de tarjetas
        var activeCards = cards.Count(c => c.Status == "Active");
        var cardQuickViews = cards.Select(c => new CardQuickView(
            c.Id,
            c.MaskedNumber,
            c.Brand,
            c.Type,
            c.Alias,
            c.CurrentBalance,
            c.AvailableCredit,
            c.Status)).ToList();

        var cardsSummary = new CardsSummary(
            cards.Count,
            activeCards,
            cards.Sum(c => c.CreditLimit),
            cards.Sum(c => c.AvailableCredit),
            cards.Sum(c => c.CurrentBalance),
            cardQuickViews);

        // Construir resumen de gastos
        var thisMonth = transactions?.TotalSpentThisMonth ?? 0;
        var lastMonth = 0m; // TODO: Implementar consulta de mes anterior
        var percentageChange = lastMonth > 0
            ? ((thisMonth - lastMonth) / lastMonth) * 100
            : (thisMonth > 0 ? 100 : 0);

        var spendingSummary = new SpendingSummary(
            thisMonth,
            lastMonth,
            Math.Round(percentageChange, 2));

        // Construir transacciones recientes
        var recentTransactions = transactions?.Transactions
            .Select(t => new RecentTransaction(
                t.Id,
                t.CardMaskedNumber,
                t.Type,
                t.MerchantName,
                t.Description,
                t.Amount,
                t.Currency,
                t.CreatedAt))
            .ToList() ?? new List<RecentTransaction>();

        return Result.Success(new DashboardViewModel(
            userSummary,
            cardsSummary,
            spendingSummary,
            recentTransactions));
    }

    /// <summary>
    /// Obtiene los KPIs del usuario
    /// </summary>
    public async Task<Result<KpisViewModel>> GetKpisAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        // Obtener tarjetas (usa un DbContext diferente)
        var cardsResult = await _cardService.GetUserCardsAsync(userId, ct);
        var cards = cardsResult.IsSuccess ? cardsResult.Value : Array.Empty<Modules.Cards.Application.DTOs.CardSummaryDto>();

        // Ejecutar consultas de transacciones secuencialmente
        // (DbContext no es thread-safe, no se pueden ejecutar en paralelo)
        var spentThisMonth = await _transactionRepository.GetTotalSpentThisMonthAsync(userId, ct);
        var spentLastMonth = await _transactionRepository.GetTotalSpentLastMonthAsync(userId, ct);
        var paymentsThisMonth = await _transactionRepository.GetTotalPaymentsThisMonthAsync(userId, ct);
        var paymentsLastMonth = await _transactionRepository.GetTotalPaymentsLastMonthAsync(userId, ct);
        var transactionCount = await _transactionRepository.GetTransactionCountThisMonthAsync(userId, ct);
        var stats = await _transactionRepository.GetTransactionStatsAsync(userId, ct);

        // KPIs Financieros
        var totalCreditLimit = cards.Sum(c => c.CreditLimit);
        var totalUsedCredit = cards.Sum(c => c.CurrentBalance);
        var totalAvailableCredit = cards.Sum(c => c.AvailableCredit);
        var creditUtilization = totalCreditLimit > 0
            ? Math.Round((totalUsedCredit / totalCreditLimit) * 100, 2)
            : 0;

        var financialKpis = new FinancialKpis(
            totalCreditLimit,
            totalUsedCredit,
            totalAvailableCredit,
            creditUtilization,
            totalUsedCredit);

        // KPIs de Tarjetas
        var cardsByBrand = new CardByBrand(
            cards.Count(c => c.Brand == "Visa"),
            cards.Count(c => c.Brand == "Mastercard"),
            cards.Count(c => c.Brand == "AmericanExpress"));

        var cardsByType = new CardByType(
            cards.Count(c => c.Type == "Classic"),
            cards.Count(c => c.Type == "Gold"),
            cards.Count(c => c.Type == "Platinum"),
            cards.Count(c => c.Type == "Black"));

        var cardsKpis = new CardsKpis(
            cards.Count,
            cards.Count(c => c.Status == "Active"),
            cards.Count(c => c.Status == "Blocked"),
            cards.Count(c => c.Status == "Expired"),
            cardsByBrand,
            cardsByType);

        // KPIs de Transacciones
        TopMerchant? topMerchant = !string.IsNullOrEmpty(stats.TopMerchantName)
            ? new TopMerchant(stats.TopMerchantName, stats.TopMerchantTotal, stats.TopMerchantCount)
            : null;

        var transactionsKpis = new TransactionsKpis(
            transactionCount,
            stats.TotalPurchases,
            stats.TotalPayments,
            stats.TotalRefunds,
            stats.AverageAmount,
            stats.LargestAmount,
            topMerchant);

        // KPIs de Tendencias
        var spendingChangePercent = spentLastMonth > 0
            ? Math.Round(((spentThisMonth - spentLastMonth) / spentLastMonth) * 100, 2)
            : (spentThisMonth > 0 ? 100 : 0);

        var spendingTrend = spendingChangePercent > 0 ? "Aumentando"
            : spendingChangePercent < 0 ? "Disminuyendo"
            : "Estable";

        var trendsKpis = new TrendsKpis(
            spentThisMonth,
            spentLastMonth,
            spendingChangePercent,
            paymentsThisMonth,
            paymentsLastMonth,
            spendingTrend);

        return Result.Success(new KpisViewModel(
            financialKpis,
            cardsKpis,
            transactionsKpis,
            trendsKpis));
    }
}
