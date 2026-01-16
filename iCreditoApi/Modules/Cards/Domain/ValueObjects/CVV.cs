using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Cards.Application.Errors;

namespace iCreditoApi.Modules.Cards.Domain.ValueObjects;

/// <summary>
/// Value Object para CVV de tarjeta
/// Nota: En producción, el CVV no debería almacenarse
/// </summary>
public sealed class CVV : ValueObject
{
    public string Value { get; }

    private CVV(string value)
    {
        Value = value;
    }

    public static Result<CVV> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<CVV>(CardErrors.CVVRequired);

        var cleaned = value.Trim();

        if (cleaned.Length < 3 || cleaned.Length > 4)
            return Result.Failure<CVV>(CardErrors.InvalidCVVLength);

        if (!cleaned.All(char.IsDigit))
            return Result.Failure<CVV>(CardErrors.InvalidCVVFormat);

        return Result.Success(new CVV(cleaned));
    }

    /// <summary>
    /// CVV enmascarado para mostrar
    /// </summary>
    public string GetMasked() => "***";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
