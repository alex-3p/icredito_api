using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Payments.Domain.Enums;
using iCreditoApi.Modules.Payments.Domain.Events;

namespace iCreditoApi.Modules.Payments.Domain.Entities;

/// <summary>
/// Entidad Payment - Agregado raíz del módulo de pagos
/// </summary>
public class Payment : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid CreditCardId { get; private set; }

    // Información del pago
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public string Reference { get; private set; } = null!;

    // Información del comercio
    public string MerchantName { get; private set; } = null!;
    public string MerchantCategory { get; private set; } = null!;
    public string? Description { get; private set; }

    // Estado
    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public string? AuthorizationCode { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private Payment() : base() { }

    private Payment(
        Guid id,
        Guid userId,
        Guid creditCardId,
        decimal amount,
        string currency,
        string merchantName,
        string merchantCategory,
        string? description) : base(id)
    {
        UserId = userId;
        CreditCardId = creditCardId;
        Amount = amount;
        Currency = currency;
        Reference = GenerateReference();
        MerchantName = merchantName;
        MerchantCategory = merchantCategory;
        Description = description;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method para crear un nuevo pago
    /// </summary>
    public static Payment Create(
        Guid userId,
        Guid creditCardId,
        decimal amount,
        string currency,
        string merchantName,
        string merchantCategory,
        string? description = null)
    {
        var payment = new Payment(
            Guid.NewGuid(),
            userId,
            creditCardId,
            amount,
            currency,
            merchantName.Trim(),
            merchantCategory.Trim(),
            description?.Trim());

        payment.AddDomainEvent(new PaymentInitiatedEvent(
            payment.Id,
            userId,
            creditCardId,
            amount));

        return payment;
    }

    /// <summary>
    /// Marca el pago como en procesamiento
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = PaymentStatus.Processing;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Completa el pago exitosamente
    /// </summary>
    public void Complete(string authorizationCode)
    {
        Status = PaymentStatus.Completed;
        AuthorizationCode = authorizationCode;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentCompletedEvent(Id, UserId, Amount));
    }

    /// <summary>
    /// Marca el pago como fallido
    /// </summary>
    public void Fail(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentFailedEvent(Id, UserId, reason));
    }

    /// <summary>
    /// Marca el pago como reembolsado
    /// </summary>
    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Solo se pueden reembolsar pagos completados");

        Status = PaymentStatus.Refunded;
        AddDomainEvent(new PaymentRefundedEvent(Id, UserId, Amount));
    }

    /// <summary>
    /// Genera una referencia única para el pago
    /// </summary>
    private static string GenerateReference()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"PAY-{date}-{random}";
    }
}
