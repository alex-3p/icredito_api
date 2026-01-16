using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Transactions.Domain.Enums;
using iCreditoApi.Modules.Transactions.Domain.Events;

namespace iCreditoApi.Modules.Transactions.Domain.Entities;

/// <summary>
/// Entidad Transaction - Agregado raíz del módulo de transacciones
/// </summary>
public class Transaction : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid CreditCardId { get; private set; }
    public Guid? PaymentId { get; private set; }

    // Información de la transacción
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Balance
    public decimal BalanceBefore { get; private set; }
    public decimal BalanceAfter { get; private set; }

    // Metadatos
    public string? MerchantName { get; private set; }
    public string? Category { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() : base() { }

    private Transaction(
        Guid id,
        Guid userId,
        Guid creditCardId,
        Guid? paymentId,
        TransactionType type,
        decimal amount,
        string currency,
        string description,
        decimal balanceBefore,
        decimal balanceAfter,
        string? merchantName,
        string? category) : base(id)
    {
        UserId = userId;
        CreditCardId = creditCardId;
        PaymentId = paymentId;
        Type = type;
        Amount = amount;
        Currency = currency;
        Description = description;
        BalanceBefore = balanceBefore;
        BalanceAfter = balanceAfter;
        MerchantName = merchantName;
        Category = category;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Crea una transacción de compra
    /// </summary>
    public static Transaction CreatePurchase(
        Guid userId,
        Guid creditCardId,
        Guid paymentId,
        decimal amount,
        string currency,
        string merchantName,
        decimal balanceBefore)
    {
        var transaction = new Transaction(
            Guid.NewGuid(),
            userId,
            creditCardId,
            paymentId,
            TransactionType.Purchase,
            amount,
            currency,
            $"Compra en {merchantName}",
            balanceBefore,
            balanceBefore + amount,
            merchantName,
            "Compras");

        transaction.AddDomainEvent(new TransactionRecordedEvent(
            transaction.Id,
            userId,
            creditCardId,
            TransactionType.Purchase,
            amount));

        return transaction;
    }

    /// <summary>
    /// Crea una transacción de pago
    /// </summary>
    public static Transaction CreatePayment(
        Guid userId,
        Guid creditCardId,
        decimal amount,
        string currency,
        decimal balanceBefore)
    {
        var transaction = new Transaction(
            Guid.NewGuid(),
            userId,
            creditCardId,
            null,
            TransactionType.Payment,
            amount,
            currency,
            "Pago de tarjeta de crédito",
            balanceBefore,
            balanceBefore - amount,
            null,
            "Pagos");

        transaction.AddDomainEvent(new TransactionRecordedEvent(
            transaction.Id,
            userId,
            creditCardId,
            TransactionType.Payment,
            amount));

        return transaction;
    }

    /// <summary>
    /// Crea una transacción de reembolso
    /// </summary>
    public static Transaction CreateRefund(
        Guid userId,
        Guid creditCardId,
        Guid originalPaymentId,
        decimal amount,
        string currency,
        string merchantName,
        decimal balanceBefore)
    {
        var transaction = new Transaction(
            Guid.NewGuid(),
            userId,
            creditCardId,
            originalPaymentId,
            TransactionType.Refund,
            amount,
            currency,
            $"Reembolso de {merchantName}",
            balanceBefore,
            balanceBefore - amount,
            merchantName,
            "Reembolsos");

        transaction.AddDomainEvent(new TransactionRecordedEvent(
            transaction.Id,
            userId,
            creditCardId,
            TransactionType.Refund,
            amount));

        return transaction;
    }
}
