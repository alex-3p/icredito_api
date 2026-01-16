using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Payments.Application.Errors;

namespace iCreditoApi.Modules.Payments.Domain.ValueObjects;

/// <summary>
/// Value Object para representar dinero
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "MXN")
    {
        if (amount < 0)
            return Result.Failure<Money>(PaymentErrors.NegativeAmount);

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Money>(PaymentErrors.InvalidCurrency);

        return Result.Success(new Money(Math.Round(amount, 2), currency.ToUpperInvariant()));
    }

    public static Money Zero(string currency = "MXN") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("No se pueden sumar monedas de diferente divisa");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("No se pueden restar monedas de diferente divisa");

        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
