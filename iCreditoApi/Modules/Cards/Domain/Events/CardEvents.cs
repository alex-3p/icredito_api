using iCreditoApi.Shared.Domain.Primitives;

namespace iCreditoApi.Modules.Cards.Domain.Events;

/// <summary>
/// Evento cuando se crea una tarjeta
/// </summary>
public sealed record CardCreatedEvent(Guid CardId, Guid UserId) : DomainEvent;

/// <summary>
/// Evento cuando se actualiza una tarjeta
/// </summary>
public sealed record CardUpdatedEvent(Guid CardId) : DomainEvent;

/// <summary>
/// Evento cuando se bloquea una tarjeta
/// </summary>
public sealed record CardBlockedEvent(Guid CardId, Guid UserId) : DomainEvent;

/// <summary>
/// Evento cuando se elimina una tarjeta
/// </summary>
public sealed record CardDeletedEvent(Guid CardId, Guid UserId) : DomainEvent;
