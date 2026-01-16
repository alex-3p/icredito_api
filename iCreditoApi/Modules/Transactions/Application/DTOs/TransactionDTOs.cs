namespace iCreditoApi.Modules.Transactions.Application.DTOs;

// Response DTOs
public record TransactionDto(
    Guid Id,
    Guid CreditCardId,
    string CardMaskedNumber,
    string Type,
    decimal Amount,
    string Currency,
    string Description,
    string? MerchantName,
    string? Category,
    decimal BalanceBefore,
    decimal BalanceAfter,
    DateTime CreatedAt);

public record TransactionHistoryDto(
    IReadOnlyList<TransactionDto> Transactions,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    decimal TotalSpentThisMonth);
