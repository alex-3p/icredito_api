using iCreditoApi.Shared.Domain.Primitives;

namespace iCreditoApi.Modules.Payments.Domain.Events;

/// <summary>
/// Evento cuando se inicia un pago
/// </summary>
public sealed record PaymentInitiatedEvent(
    Guid PaymentId,
    Guid UserId,
    Guid CreditCardId,
    decimal Amount) : DomainEvent;

/// <summary>
/// Evento cuando se completa un pago
/// </summary>
public sealed record PaymentCompletedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount) : DomainEvent;

/// <summary>
/// Evento cuando falla un pago
/// </summary>
public sealed record PaymentFailedEvent(
    Guid PaymentId,
    Guid UserId,
    string Reason) : DomainEvent;

/// <summary>
/// Evento cuando se reembolsa un pago
/// </summary>
public sealed record PaymentRefundedEvent(
    Guid PaymentId,
    Guid UserId,
    decimal Amount) : DomainEvent;
