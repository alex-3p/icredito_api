using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Cards.Application.Errors;

/// <summary>
/// Errores específicos del módulo de tarjetas
/// </summary>
public static class CardErrors
{
    // Errores de número de tarjeta
    public static readonly Error CardNumberRequired =
        new("Card.NumberRequired", "El número de tarjeta es requerido");

    public static readonly Error InvalidCardNumberLength =
        new("Card.InvalidNumberLength", "El número de tarjeta debe tener entre 13 y 19 dígitos");

    public static readonly Error InvalidCardNumberFormat =
        new("Card.InvalidNumberFormat", "El número de tarjeta solo debe contener dígitos");

    public static readonly Error InvalidCardNumber =
        new("Card.InvalidNumber", "El número de tarjeta no es válido");

    public static readonly Error CardAlreadyExists =
        new("Card.AlreadyExists", "Ya existe una tarjeta con este número");

    // Errores de fecha de expiración
    public static readonly Error InvalidExpirationMonth =
        new("Card.InvalidExpirationMonth", "El mes de expiración debe estar entre 1 y 12");

    public static readonly Error CardExpired =
        new("Card.Expired", "La tarjeta ha expirado");

    // Errores de CVV
    public static readonly Error CVVRequired =
        new("Card.CVVRequired", "El CVV es requerido");

    public static readonly Error InvalidCVVLength =
        new("Card.InvalidCVVLength", "El CVV debe tener 3 o 4 dígitos");

    public static readonly Error InvalidCVVFormat =
        new("Card.InvalidCVVFormat", "El CVV solo debe contener dígitos");

    // Errores de límite de crédito
    public static readonly Error InvalidCreditLimit =
        new("Card.InvalidCreditLimit", "El límite de crédito debe ser mayor a 0");

    public static readonly Error CreditLimitExceedsMaximum =
        new("Card.CreditLimitExceedsMaximum", "El límite de crédito excede el máximo permitido");

    public static readonly Error InsufficientCredit =
        new("Card.InsufficientCredit", "Crédito insuficiente para realizar la operación");

    // Errores de operaciones
    public static readonly Error CardNotFound =
        new("Card.NotFound", "Tarjeta no encontrada");

    public static readonly Error CardNotActive =
        new("Card.NotActive", "La tarjeta no está activa");

    public static readonly Error InvalidAmount =
        new("Card.InvalidAmount", "El monto debe ser mayor a 0");

    public static readonly Error UnauthorizedAccess =
        new("Card.Unauthorized", "No tiene permiso para acceder a esta tarjeta");

    public static readonly Error CardholderNameRequired =
        new("Card.CardholderNameRequired", "El nombre del titular es requerido");

    public static readonly Error InvalidCardholderName =
        new("Card.InvalidCardholderName", "El nombre del titular no es válido");
}
