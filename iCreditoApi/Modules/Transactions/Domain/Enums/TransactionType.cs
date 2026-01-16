namespace iCreditoApi.Modules.Transactions.Domain.Enums;

/// <summary>
/// Tipo de transacción
/// </summary>
public enum TransactionType
{
    Purchase,   // Compra/cargo
    Payment,    // Pago a la tarjeta
    Refund,     // Reembolso
    Fee,        // Cargo por comisión
    Interest    // Interés
}
