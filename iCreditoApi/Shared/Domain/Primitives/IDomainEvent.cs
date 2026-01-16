namespace iCreditoApi.Shared.Domain.Primitives;

/// <summary>
/// Interfaz marcadora para eventos de dominio
/// Los eventos de dominio permiten comunicación desacoplada entre módulos
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}

/// <summary>
/// Clase base para implementar eventos de dominio
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
