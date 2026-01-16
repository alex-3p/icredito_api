using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Cards.Application.Errors;

namespace iCreditoApi.Modules.Cards.Domain.ValueObjects;

/// <summary>
/// Value Object para límite de crédito
/// </summary>
public sealed class CreditLimit : ValueObject
{
    public decimal Amount { get; }

    private CreditLimit(decimal amount)
    {
        Amount = amount;
    }

    public static Result<CreditLimit> Create(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure<CreditLimit>(CardErrors.InvalidCreditLimit);

        if (amount > 1_000_000) // Límite máximo de 1 millón
            return Result.Failure<CreditLimit>(CardErrors.CreditLimitExceedsMaximum);

        return Result.Success(new CreditLimit(Math.Round(amount, 2)));
    }

    public override string ToString() => $"${Amount:N2}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}
