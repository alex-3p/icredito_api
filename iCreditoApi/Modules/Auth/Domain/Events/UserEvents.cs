using iCreditoApi.Shared.Domain.Primitives;

namespace iCreditoApi.Modules.Auth.Domain.Events;

/// <summary>
/// Evento cuando un usuario se registra
/// </summary>
public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string Username) : DomainEvent;

/// <summary>
/// Evento cuando un usuario inicia sesión
/// </summary>
public sealed record UserLoggedInEvent(Guid UserId) : DomainEvent;
