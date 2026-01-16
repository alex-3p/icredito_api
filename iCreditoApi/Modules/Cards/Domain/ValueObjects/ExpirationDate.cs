using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Cards.Application.Errors;

namespace iCreditoApi.Modules.Cards.Domain.ValueObjects;

/// <summary>
/// Value Object para fecha de expiración de tarjeta
/// </summary>
public sealed class ExpirationDate : ValueObject
{
    public int Month { get; }
    public int Year { get; }

    private ExpirationDate(int month, int year)
    {
        Month = month;
        Year = year;
    }

    public static Result<ExpirationDate> Create(int month, int year)
    {
        if (month < 1 || month > 12)
            return Result.Failure<ExpirationDate>(CardErrors.InvalidExpirationMonth);

        // Si el año viene en formato corto (ej: 25), convertir a completo
        var fullYear = year < 100 ? 2000 + year : year;

        var now = DateTime.UtcNow;
        var expirationEndOfMonth = new DateTime(fullYear, month, 1).AddMonths(1).AddDays(-1);

        if (expirationEndOfMonth < now)
            return Result.Failure<ExpirationDate>(CardErrors.CardExpired);

        return Result.Success(new ExpirationDate(month, fullYear));
    }

    /// <summary>
    /// Verifica si la tarjeta está expirada
    /// </summary>
    public bool IsExpired()
    {
        var now = DateTime.UtcNow;
        var expirationEndOfMonth = new DateTime(Year, Month, 1).AddMonths(1).AddDays(-1);
        return expirationEndOfMonth < now;
    }

    /// <summary>
    /// Formato MM/YY
    /// </summary>
    public override string ToString() => $"{Month:D2}/{Year % 100:D2}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Month;
        yield return Year;
    }
}
