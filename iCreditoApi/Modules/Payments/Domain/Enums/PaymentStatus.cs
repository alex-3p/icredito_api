namespace iCreditoApi.Modules.Payments.Domain.Enums;

/// <summary>
/// Estado del pago
/// </summary>
public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded
}
