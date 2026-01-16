using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Payments.Application.Errors;

/// <summary>
/// Errores específicos del módulo de pagos
/// </summary>
public static class PaymentErrors
{
    public static readonly Error NegativeAmount =
        new("Payment.NegativeAmount", "El monto no puede ser negativo");

    public static readonly Error InvalidCurrency =
        new("Payment.InvalidCurrency", "La moneda no es válida");

    public static readonly Error PaymentNotFound =
        new("Payment.NotFound", "Pago no encontrado");

    public static readonly Error CardNotFound =
        new("Payment.CardNotFound", "Tarjeta no encontrada");

    public static readonly Error CardNotActive =
        new("Payment.CardNotActive", "La tarjeta no está activa");

    public static readonly Error InsufficientCredit =
        new("Payment.InsufficientCredit", "Crédito insuficiente");

    public static readonly Error UnauthorizedCard =
        new("Payment.UnauthorizedCard", "No tiene permiso para usar esta tarjeta");

    public static readonly Error MerchantNameRequired =
        new("Payment.MerchantNameRequired", "El nombre del comercio es requerido");

    public static readonly Error InvalidAmount =
        new("Payment.InvalidAmount", "El monto debe ser mayor a 0");

    public static readonly Error PaymentAlreadyProcessed =
        new("Payment.AlreadyProcessed", "El pago ya fue procesado");

    public static readonly Error RefundNotAllowed =
        new("Payment.RefundNotAllowed", "Solo se pueden reembolsar pagos completados");

    public static Error PaymentFailed(string reason) =>
        new("Payment.Failed", $"El pago falló: {reason}");
}
