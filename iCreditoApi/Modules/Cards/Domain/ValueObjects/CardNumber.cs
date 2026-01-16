using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Cards.Application.Errors;

namespace iCreditoApi.Modules.Cards.Domain.ValueObjects;

/// <summary>
/// Value Object para número de tarjeta de crédito
/// </summary>
public sealed class CardNumber : ValueObject
{
    public string Value { get; }
    public string LastFourDigits => Value[^4..];

    private CardNumber(string value)
    {
        Value = value;
    }

    public static Result<CardNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<CardNumber>(CardErrors.CardNumberRequired);

        var cleaned = value.Replace(" ", "").Replace("-", "");

        if (cleaned.Length < 13 || cleaned.Length > 19)
            return Result.Failure<CardNumber>(CardErrors.InvalidCardNumberLength);

        if (!cleaned.All(char.IsDigit))
            return Result.Failure<CardNumber>(CardErrors.InvalidCardNumberFormat);

        if (!IsValidLuhn(cleaned))
            return Result.Failure<CardNumber>(CardErrors.InvalidCardNumber);

        return Result.Success(new CardNumber(cleaned));
    }

    /// <summary>
    /// Retorna el número enmascarado (ej: **** **** **** 1234)
    /// </summary>
    public string GetMasked() => $"**** **** **** {LastFourDigits}";

    /// <summary>
    /// Validación usando algoritmo de Luhn
    /// </summary>
    private static bool IsValidLuhn(string number)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = number[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
