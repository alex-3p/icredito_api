using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Transactions.Application.Errors;

/// <summary>
/// Errores específicos del módulo de transacciones
/// </summary>
public static class TransactionErrors
{
    public static readonly Error TransactionNotFound =
        new("Transaction.NotFound", "Transacción no encontrada");

    public static readonly Error CardNotFound =
        new("Transaction.CardNotFound", "Tarjeta no encontrada");

    public static readonly Error UnauthorizedAccess =
        new("Transaction.Unauthorized", "No tiene permiso para acceder a esta transacción");
}
