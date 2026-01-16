using iCreditoApi.Modules.Cards.Domain.Repositories;
using iCreditoApi.Modules.Transactions.Application.DTOs;
using iCreditoApi.Modules.Transactions.Application.Errors;
using iCreditoApi.Modules.Transactions.Domain.Entities;
using iCreditoApi.Modules.Transactions.Domain.Repositories;
using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Transactions.Application.Services;

/// <summary>
/// Servicio de aplicación para transacciones
/// </summary>
public class TransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICreditCardRepository _cardRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ICreditCardRepository cardRepository)
    {
        _transactionRepository = transactionRepository;
        _cardRepository = cardRepository;
    }

    /// <summary>
    /// Obtiene una transacción por ID
    /// </summary>
    public async Task<Result<TransactionDto>> GetByIdAsync(
        Guid transactionId,
        Guid userId,
        CancellationToken ct = default)
    {
        var transaction = await _transactionRepository.GetByIdAndUserIdAsync(transactionId, userId, ct);

        if (transaction is null)
            return Result.Failure<TransactionDto>(TransactionErrors.TransactionNotFound);

        var card = await _cardRepository.GetByIdAsync(transaction.CreditCardId, ct);
        var maskedNumber = card?.GetMaskedNumber() ?? "****";

        return Result.Success(MapToDto(transaction, maskedNumber));
    }

    /// <summary>
    /// Obtiene el historial de transacciones de un usuario
    /// </summary>
    public async Task<Result<TransactionHistoryDto>> GetHistoryAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId, page, pageSize, ct);
        var totalCount = await _transactionRepository.GetCountByUserIdAsync(userId, ct);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var totalSpent = await _transactionRepository.GetTotalSpentThisMonthAsync(userId, ct);

        // Obtener números enmascarados de las tarjetas
        var cardIds = transactions.Select(t => t.CreditCardId).Distinct();
        var cards = await Task.WhenAll(
            cardIds.Select(id => _cardRepository.GetByIdAsync(id, ct)));

        var cardMasks = cards
            .Where(c => c is not null)
            .ToDictionary(c => c!.Id, c => c!.GetMaskedNumber());

        var dtos = transactions.Select(t =>
            MapToDto(t, cardMasks.GetValueOrDefault(t.CreditCardId, "****")))
            .ToList();

        return Result.Success(new TransactionHistoryDto(
            dtos,
            totalCount,
            page,
            pageSize,
            totalPages,
            totalSpent));
    }

    /// <summary>
    /// Obtiene las transacciones de una tarjeta específica
    /// </summary>
    public async Task<Result<TransactionHistoryDto>> GetCardHistoryAsync(
        Guid cardId,
        Guid userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        // Verificar que la tarjeta pertenece al usuario
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);
        if (card is null)
            return Result.Failure<TransactionHistoryDto>(TransactionErrors.CardNotFound);

        var transactions = await _transactionRepository.GetByCardIdAsync(cardId, page, pageSize, ct);
        var totalCount = await _transactionRepository.GetCountByCardIdAsync(cardId, ct);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var totalSpent = await _transactionRepository.GetTotalSpentThisMonthAsync(userId, ct);

        var maskedNumber = card.GetMaskedNumber();
        var dtos = transactions.Select(t => MapToDto(t, maskedNumber)).ToList();

        return Result.Success(new TransactionHistoryDto(
            dtos,
            totalCount,
            page,
            pageSize,
            totalPages,
            totalSpent));
    }

    private static TransactionDto MapToDto(Transaction transaction, string cardMaskedNumber) => new(
        transaction.Id,
        transaction.CreditCardId,
        cardMaskedNumber,
        transaction.Type.ToString(),
        transaction.Amount,
        transaction.Currency,
        transaction.Description,
        transaction.MerchantName,
        transaction.Category,
        transaction.BalanceBefore,
        transaction.BalanceAfter,
        transaction.CreatedAt);
}
