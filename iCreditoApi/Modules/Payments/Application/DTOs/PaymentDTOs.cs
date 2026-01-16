namespace iCreditoApi.Modules.Payments.Application.DTOs;

// Request DTOs
public record ProcessPaymentRequest(
    Guid CreditCardId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string MerchantCategory,
    string? Description);

// Response DTOs
public record PaymentDto(
    Guid Id,
    string Reference,
    Guid CreditCardId,
    decimal Amount,
    string Currency,
    string MerchantName,
    string MerchantCategory,
    string? Description,
    string Status,
    string? FailureReason,
    string? AuthorizationCode,
    DateTime CreatedAt,
    DateTime? CompletedAt);

public record PaymentResultDto(
    Guid PaymentId,
    string Reference,
    string Status,
    string? AuthorizationCode,
    decimal Amount,
    string Currency,
    DateTime ProcessedAt);

public record PaymentListDto(
    IReadOnlyList<PaymentDto> Payments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
