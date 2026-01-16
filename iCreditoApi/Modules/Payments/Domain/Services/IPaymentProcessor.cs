using iCreditoApi.Modules.Cards.Domain.Entities;

namespace iCreditoApi.Modules.Payments.Domain.Services;

/// <summary>
/// Puerto para el procesador de pagos
/// </summary>
public interface IPaymentProcessor
{
    Task<PaymentProcessResult> ProcessAsync(
        CreditCard card,
        decimal amount,
        string merchantName,
        CancellationToken ct = default);
}

/// <summary>
/// Resultado del procesamiento de pago
/// </summary>
public record PaymentProcessResult(
    bool Success,
    string? AuthorizationCode,
    string? ErrorMessage);
