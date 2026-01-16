using iCreditoApi.Shared.Application.Interfaces;

namespace iCreditoApi.Shared.Infrastructure.Services;

/// <summary>
/// Implementación del proveedor de fecha/hora
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
