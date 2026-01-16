namespace iCreditoApi.Shared.Application.Interfaces;

/// <summary>
/// Proveedor de fecha/hora para facilitar testing
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
