using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Shared.Application.Result;
using iCreditoApi.Modules.Cards.Domain.Enums;
using iCreditoApi.Modules.Cards.Domain.ValueObjects;
using iCreditoApi.Modules.Cards.Domain.Events;
using iCreditoApi.Modules.Cards.Application.Errors;

namespace iCreditoApi.Modules.Cards.Domain.Entities;

/// <summary>
/// Entidad CreditCard - Agregado raíz del módulo de tarjetas
/// </summary>
public class CreditCard : AggregateRoot
{
    public Guid UserId { get; private set; }

    // Value Objects - almacenados como propiedades para EF
    public string CardNumber { get; private set; } = null!;
    public string CardholderName { get; private set; } = null!;
    public int ExpirationMonth { get; private set; }
    public int ExpirationYear { get; private set; }
    public string CVV { get; private set; } = null!;
    public decimal CreditLimit { get; private set; }

    // Propiedades de negocio
    public decimal CurrentBalance { get; private set; }
    public decimal AvailableCredit => CreditLimit - CurrentBalance;
    public CardBrand Brand { get; private set; }
    public CardType Type { get; private set; }
    public CardStatus Status { get; private set; }
    public string? Alias { get; private set; }

    // Metadatos
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CreditCard() : base() { }

    private CreditCard(
        Guid id,
        Guid userId,
        string cardNumber,
        string cardholderName,
        int expirationMonth,
        int expirationYear,
        string cvv,
        CardBrand brand,
        CardType type,
        decimal creditLimit,
        string? alias) : base(id)
    {
        UserId = userId;
        CardNumber = cardNumber;
        CardholderName = cardholderName;
        ExpirationMonth = expirationMonth;
        ExpirationYear = expirationYear;
        CVV = cvv;
        Brand = brand;
        Type = type;
        CreditLimit = creditLimit;
        CurrentBalance = 0;
        Status = CardStatus.Active;
        Alias = alias;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method para crear una nueva tarjeta
    /// </summary>
    public static Result<CreditCard> Create(
        Guid userId,
        CardNumber cardNumber,
        string cardholderName,
        ExpirationDate expirationDate,
        CVV cvv,
        CardBrand brand,
        CardType type,
        CreditLimit creditLimit,
        string? alias = null)
    {
        if (string.IsNullOrWhiteSpace(cardholderName))
            return Result.Failure<CreditCard>(CardErrors.CardholderNameRequired);

        var card = new CreditCard(
            Guid.NewGuid(),
            userId,
            cardNumber.Value,
            cardholderName.Trim().ToUpperInvariant(),
            expirationDate.Month,
            expirationDate.Year,
            cvv.Value,
            brand,
            type,
            creditLimit.Amount,
            alias?.Trim());

        card.AddDomainEvent(new CardCreatedEvent(card.Id, userId));

        return Result.Success(card);
    }

    /// <summary>
    /// Realiza un cargo a la tarjeta
    /// </summary>
    public Result<bool> Charge(decimal amount)
    {
        if (Status != CardStatus.Active)
            return Result.Failure<bool>(CardErrors.CardNotActive);

        if (IsExpired())
            return Result.Failure<bool>(CardErrors.CardExpired);

        if (amount <= 0)
            return Result.Failure<bool>(CardErrors.InvalidAmount);

        if (amount > AvailableCredit)
            return Result.Failure<bool>(CardErrors.InsufficientCredit);

        CurrentBalance += amount;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success(true);
    }

    /// <summary>
    /// Realiza un pago a la tarjeta (reduce el saldo)
    /// </summary>
    public Result<decimal> MakePayment(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure<decimal>(CardErrors.InvalidAmount);

        // Si el pago es mayor al saldo, solo cobra el saldo
        var actualPayment = Math.Min(amount, CurrentBalance);
        CurrentBalance -= actualPayment;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success(actualPayment);
    }

    /// <summary>
    /// Actualiza el alias de la tarjeta
    /// </summary>
    public void UpdateAlias(string? alias)
    {
        Alias = alias?.Trim();
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new CardUpdatedEvent(Id));
    }

    /// <summary>
    /// Bloquea la tarjeta
    /// </summary>
    public void Block()
    {
        if (Status == CardStatus.Active)
        {
            Status = CardStatus.Blocked;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CardBlockedEvent(Id, UserId));
        }
    }

    /// <summary>
    /// Activa la tarjeta (si estaba bloqueada)
    /// </summary>
    public void Activate()
    {
        if (Status == CardStatus.Blocked)
        {
            Status = CardStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Cancela la tarjeta permanentemente
    /// </summary>
    public void Cancel()
    {
        Status = CardStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica si la tarjeta está expirada
    /// </summary>
    public bool IsExpired()
    {
        var now = DateTime.UtcNow;
        var expirationEndOfMonth = new DateTime(ExpirationYear, ExpirationMonth, 1)
            .AddMonths(1).AddDays(-1);
        return expirationEndOfMonth < now;
    }

    /// <summary>
    /// Retorna el número enmascarado
    /// </summary>
    public string GetMaskedNumber() => $"**** **** **** {CardNumber[^4..]}";

    /// <summary>
    /// Retorna la fecha de expiración formateada
    /// </summary>
    public string GetExpirationDateFormatted() => $"{ExpirationMonth:D2}/{ExpirationYear % 100:D2}";
}
