namespace iCreditoApi.Modules.Cards.Application.DTOs;

// Request DTOs
public record AddCardRequest(
    string CardNumber,
    string CardholderName,
    int ExpirationMonth,
    int ExpirationYear,
    string CVV,
    string Brand,
    string Type,
    decimal CreditLimit,
    string? Alias);

public record UpdateCardRequest(
    string? Alias);

// Response DTOs
public record CardSummaryDto(
    Guid Id,
    string MaskedNumber,
    string CardholderName,
    string Brand,
    string Type,
    string Status,
    decimal CurrentBalance,
    decimal CreditLimit,
    decimal AvailableCredit,
    string ExpirationDate,
    string? Alias);

public record CardDetailDto(
    Guid Id,
    string MaskedNumber,
    string CardholderName,
    string Brand,
    string Type,
    string Status,
    decimal CurrentBalance,
    decimal CreditLimit,
    decimal AvailableCredit,
    string ExpirationDate,
    string? Alias,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
