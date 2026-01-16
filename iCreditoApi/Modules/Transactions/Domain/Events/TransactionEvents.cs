using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Transactions.Domain.Enums;

namespace iCreditoApi.Modules.Transactions.Domain.Events;

/// <summary>
/// Evento cuando se registra una transacción
/// </summary>
public sealed record TransactionRecordedEvent(
    Guid TransactionId,
    Guid UserId,
    Guid CreditCardId,
    TransactionType Type,
    decimal Amount) : DomainEvent;
