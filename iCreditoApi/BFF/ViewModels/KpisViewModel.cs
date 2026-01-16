namespace iCreditoApi.BFF.ViewModels;

/// <summary>
/// ViewModel con KPIs principales para el dashboard
/// </summary>
public record KpisViewModel(
    FinancialKpis Financial,
    CardsKpis Cards,
    TransactionsKpis Transactions,
    TrendsKpis Trends);

/// <summary>
/// KPIs financieros principales
/// </summary>
public record FinancialKpis(
    decimal TotalCreditLimit,
    decimal TotalUsedCredit,
    decimal TotalAvailableCredit,
    decimal CreditUtilizationPercent,
    decimal TotalDebt);

/// <summary>
/// KPIs de tarjetas
/// </summary>
public record CardsKpis(
    int TotalCards,
    int ActiveCards,
    int BlockedCards,
    int ExpiredCards,
    CardByBrand CardsByBrand,
    CardByType CardsByType);

public record CardByBrand(
    int Visa,
    int Mastercard,
    int AmericanExpress);

public record CardByType(
    int Classic,
    int Gold,
    int Platinum,
    int Black);

/// <summary>
/// KPIs de transacciones
/// </summary>
public record TransactionsKpis(
    int TotalTransactionsThisMonth,
    int TotalPurchases,
    int TotalPayments,
    int TotalRefunds,
    decimal AverageTransactionAmount,
    decimal LargestTransaction,
    TopMerchant? TopMerchant);

public record TopMerchant(
    string Name,
    decimal TotalSpent,
    int TransactionCount);

/// <summary>
/// KPIs de tendencias
/// </summary>
public record TrendsKpis(
    decimal SpendingThisMonth,
    decimal SpendingLastMonth,
    decimal SpendingChangePercent,
    decimal PaymentsThisMonth,
    decimal PaymentsLastMonth,
    string SpendingTrend);
