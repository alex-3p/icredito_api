namespace iCreditoApi.Modules.Cards.Domain.Enums;

/// <summary>
/// Estado de la tarjeta de crédito
/// </summary>
public enum CardStatus
{
    Active,
    Blocked,
    Expired,
    Cancelled
}

/// <summary>
/// Tipo de tarjeta
/// </summary>
public enum CardType
{
    Classic,
    Gold,
    Platinum,
    Black
}

/// <summary>
/// Marca de la tarjeta
/// </summary>
public enum CardBrand
{
    Visa,
    Mastercard,
    AmericanExpress
}
