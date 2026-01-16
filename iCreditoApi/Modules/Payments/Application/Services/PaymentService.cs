using iCreditoApi.Modules.Cards.Domain.Repositories;
using iCreditoApi.Modules.Payments.Application.DTOs;
using iCreditoApi.Modules.Payments.Application.Errors;
using iCreditoApi.Modules.Payments.Domain.Entities;
using iCreditoApi.Modules.Payments.Domain.Repositories;
using iCreditoApi.Modules.Payments.Domain.Services;
using iCreditoApi.Modules.Transactions.Domain.Entities;
using iCreditoApi.Modules.Transactions.Domain.Enums;
using iCreditoApi.Modules.Transactions.Domain.Repositories;
using iCreditoApi.Shared.Application.Interfaces;
using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Payments.Application.Services;

/// <summary>
/// Servicio de aplicación para pagos
/// </summary>
public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICreditCardRepository _cardRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        IPaymentRepository paymentRepository,
        ICreditCardRepository cardRepository,
        ITransactionRepository transactionRepository,
        IPaymentProcessor paymentProcessor,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
        _paymentProcessor = paymentProcessor;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Procesa un nuevo pago
    /// </summary>
    public async Task<Result<PaymentResultDto>> ProcessPaymentAsync(
        Guid userId,
        ProcessPaymentRequest request,
        CancellationToken ct = default)
    {
        // Validaciones básicas
        if (request.Amount <= 0)
            return Result.Failure<PaymentResultDto>(PaymentErrors.InvalidAmount);

        if (string.IsNullOrWhiteSpace(request.MerchantName))
            return Result.Failure<PaymentResultDto>(PaymentErrors.MerchantNameRequired);

        // Obtener tarjeta
        var card = await _cardRepository.GetByIdAndUserIdAsync(request.CreditCardId, userId, ct);
        if (card is null)
            return Result.Failure<PaymentResultDto>(PaymentErrors.CardNotFound);

        // Crear pago
        var payment = Payment.Create(
            userId,
            request.CreditCardId,
            request.Amount,
            request.Currency,
            request.MerchantName,
            request.MerchantCategory,
            request.Description);

        // Procesar pago
        payment.MarkAsProcessing();

        var processResult = await _paymentProcessor.ProcessAsync(
            card,
            request.Amount,
            request.MerchantName,
            ct);

        if (!processResult.Success)
        {
            payment.Fail(processResult.ErrorMessage!);
            await _paymentRepository.AddAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Failure<PaymentResultDto>(
                PaymentErrors.PaymentFailed(processResult.ErrorMessage!));
        }

        // Cargar a la tarjeta
        var chargeResult = card.Charge(request.Amount);
        if (chargeResult.IsFailure)
        {
            payment.Fail(chargeResult.Error.Message);
            await _paymentRepository.AddAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Failure<PaymentResultDto>(chargeResult.Error);
        }

        // Completar pago
        payment.Complete(processResult.AuthorizationCode!);

        // Crear transacción
        var transaction = Transaction.CreatePurchase(
            userId,
            request.CreditCardId,
            payment.Id,
            request.Amount,
            request.Currency,
            request.MerchantName,
            card.CurrentBalance - request.Amount);

        // Persistir todo
        await _paymentRepository.AddAsync(payment, ct);
        _cardRepository.Update(card);
        await _transactionRepository.AddAsync(transaction, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new PaymentResultDto(
            payment.Id,
            payment.Reference,
            payment.Status.ToString(),
            processResult.AuthorizationCode,
            payment.Amount,
            payment.Currency,
            payment.CompletedAt ?? DateTime.UtcNow));
    }

    /// <summary>
    /// Obtiene un pago por ID
    /// </summary>
    public async Task<Result<PaymentDto>> GetByIdAsync(
        Guid paymentId,
        Guid userId,
        CancellationToken ct = default)
    {
        var payment = await _paymentRepository.GetByIdAndUserIdAsync(paymentId, userId, ct);

        if (payment is null)
            return Result.Failure<PaymentDto>(PaymentErrors.PaymentNotFound);

        return Result.Success(MapToDto(payment));
    }

    /// <summary>
    /// Obtiene los pagos de un usuario
    /// </summary>
    public async Task<Result<PaymentListDto>> GetUserPaymentsAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(userId, page, pageSize, ct);
        var totalCount = await _paymentRepository.GetCountByUserIdAsync(userId, ct);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var dtos = payments.Select(MapToDto).ToList();

        return Result.Success(new PaymentListDto(
            dtos,
            totalCount,
            page,
            pageSize,
            totalPages));
    }

    /// <summary>
    /// Reembolsa un pago
    /// </summary>
    public async Task<Result<PaymentResultDto>> RefundPaymentAsync(
        Guid paymentId,
        Guid userId,
        CancellationToken ct = default)
    {
        var payment = await _paymentRepository.GetByIdAndUserIdAsync(paymentId, userId, ct);

        if (payment is null)
            return Result.Failure<PaymentResultDto>(PaymentErrors.PaymentNotFound);

        if (payment.Status != Payments.Domain.Enums.PaymentStatus.Completed)
            return Result.Failure<PaymentResultDto>(PaymentErrors.RefundNotAllowed);

        // Obtener tarjeta para devolver el crédito
        var card = await _cardRepository.GetByIdAsync(payment.CreditCardId, ct);
        if (card is not null)
        {
            card.MakePayment(payment.Amount);
            _cardRepository.Update(card);

            // Crear transacción de reembolso
            var transaction = Transaction.CreateRefund(
                userId,
                payment.CreditCardId,
                payment.Id,
                payment.Amount,
                payment.Currency,
                payment.MerchantName,
                card.CurrentBalance);

            await _transactionRepository.AddAsync(transaction, ct);
        }

        payment.Refund();
        _paymentRepository.Update(payment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new PaymentResultDto(
            payment.Id,
            payment.Reference,
            payment.Status.ToString(),
            payment.AuthorizationCode,
            payment.Amount,
            payment.Currency,
            DateTime.UtcNow));
    }

    private static PaymentDto MapToDto(Payment payment) => new(
        payment.Id,
        payment.Reference,
        payment.CreditCardId,
        payment.Amount,
        payment.Currency,
        payment.MerchantName,
        payment.MerchantCategory,
        payment.Description,
        payment.Status.ToString(),
        payment.FailureReason,
        payment.AuthorizationCode,
        payment.CreatedAt,
        payment.CompletedAt);
}
