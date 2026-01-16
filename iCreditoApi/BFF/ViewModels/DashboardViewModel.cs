namespace iCreditoApi.BFF.ViewModels;

/// <summary>
/// ViewModel completo del dashboard para el frontend
/// </summary>
public record DashboardViewModel(
    UserSummary User,
    CardsSummary Cards,
    SpendingSummary Spending,
    IReadOnlyList<RecentTransaction> RecentTransactions);

public record UserSummary(
    Guid Id,
    string FullName,
    string Email,
    DateTime? LastLoginAt);

public record CardsSummary(
    int TotalCards,
    int ActiveCards,
    decimal TotalCreditLimit,
    decimal TotalAvailableCredit,
    decimal TotalBalance,
    IReadOnlyList<CardQuickView> Cards);

public record CardQuickView(
    Guid Id,
    string MaskedNumber,
    string Brand,
    string Type,
    string? Alias,
    decimal Balance,
    decimal AvailableCredit,
    string Status);

public record SpendingSummary(
    decimal ThisMonth,
    decimal LastMonth,
    decimal PercentageChange);

public record RecentTransaction(
    Guid Id,
    string CardMaskedNumber,
    string Type,
    string? MerchantName,
    string Description,
    decimal Amount,
    string Currency,
    DateTime Date);
