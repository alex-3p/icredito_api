using iCreditoApi.Modules.Cards.Domain.Entities;
using iCreditoApi.Modules.Cards.Domain.Enums;
using iCreditoApi.Modules.Payments.Domain.Services;
using Microsoft.Extensions.Logging;

namespace iCreditoApi.Modules.Payments.Infrastructure.Services;

/// <summary>
/// Procesador de pagos simulado
/// Simula el comportamiento de una pasarela de pagos real
/// </summary>
public class SimulatedPaymentProcessor : IPaymentProcessor
{
    private readonly ILogger<SimulatedPaymentProcessor> _logger;
    private readonly Random _random = new();

    // Configuración de simulación
    private const double SuccessRate = 0.95; // 95% de éxito
    private const int MinDelayMs = 100;
    private const int MaxDelayMs = 500;

    public SimulatedPaymentProcessor(ILogger<SimulatedPaymentProcessor> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentProcessResult> ProcessAsync(
        CreditCard card,
        decimal amount,
        string merchantName,
        CancellationToken ct = default)
    {
        // Simular latencia de red
        await Task.Delay(_random.Next(MinDelayMs, MaxDelayMs), ct);

        _logger.LogInformation(
            "Procesando pago simulado: Tarjeta={CardId}, Monto={Amount}, Merchant={Merchant}",
            card.Id, amount, merchantName);

        // Validaciones de negocio
        if (card.Status != CardStatus.Active)
        {
            _logger.LogWarning("Pago rechazado: Tarjeta no activa");
            return new PaymentProcessResult(
                Success: false,
                AuthorizationCode: null,
                ErrorMessage: "Tarjeta no activa");
        }

        if (card.IsExpired())
        {
            _logger.LogWarning("Pago rechazado: Tarjeta expirada");
            return new PaymentProcessResult(
                Success: false,
                AuthorizationCode: null,
                ErrorMessage: "Tarjeta expirada");
        }

        if (amount > card.AvailableCredit)
        {
            _logger.LogWarning("Pago rechazado: Crédito insuficiente");
            return new PaymentProcessResult(
                Success: false,
                AuthorizationCode: null,
                ErrorMessage: "Crédito insuficiente");
        }

        // Simular tasa de éxito/fallo aleatorio
        if (_random.NextDouble() > SuccessRate)
        {
            var errors = new[]
            {
                "Error de conexión con el banco",
                "Transacción rechazada por el emisor",
                "Error temporal del sistema",
                "Límite de transacciones excedido"
            };

            var error = errors[_random.Next(errors.Length)];
            _logger.LogWarning("Pago rechazado (simulado): {Error}", error);

            return new PaymentProcessResult(
                Success: false,
                AuthorizationCode: null,
                ErrorMessage: error);
        }

        // Generar código de autorización simulado
        var authCode = GenerateAuthorizationCode();

        _logger.LogInformation(
            "Pago simulado exitoso: AuthCode={AuthCode}",
            authCode);

        return new PaymentProcessResult(
            Success: true,
            AuthorizationCode: authCode,
            ErrorMessage: null);
    }

    private string GenerateAuthorizationCode()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = _random.Next(100000, 999999);
        return $"SIM-{date}-{random}";
    }
}
