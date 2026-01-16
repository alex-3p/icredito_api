using iCreditoApi.Modules.Cards.Application.DTOs;
using iCreditoApi.Modules.Cards.Application.Errors;
using iCreditoApi.Modules.Cards.Domain.Entities;
using iCreditoApi.Modules.Cards.Domain.Enums;
using iCreditoApi.Modules.Cards.Domain.Repositories;
using iCreditoApi.Modules.Cards.Domain.ValueObjects;
using iCreditoApi.Shared.Application.Interfaces;
using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Cards.Application.Services;

/// <summary>
/// Servicio de aplicación para tarjetas de crédito
/// </summary>
public class CreditCardService
{
    private readonly ICreditCardRepository _cardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreditCardService(
        ICreditCardRepository cardRepository,
        IUnitOfWork unitOfWork)
    {
        _cardRepository = cardRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtiene todas las tarjetas de un usuario
    /// </summary>
    public async Task<Result<IReadOnlyList<CardSummaryDto>>> GetUserCardsAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var cards = await _cardRepository.GetByUserIdAsync(userId, ct);
        var dtos = cards.Select(MapToSummary).ToList();
        return Result.Success<IReadOnlyList<CardSummaryDto>>(dtos);
    }

    /// <summary>
    /// Obtiene el detalle de una tarjeta
    /// </summary>
    public async Task<Result<CardDetailDto>> GetByIdAsync(
        Guid cardId,
        Guid userId,
        CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);

        if (card is null)
            return Result.Failure<CardDetailDto>(CardErrors.CardNotFound);

        return Result.Success(MapToDetail(card));
    }

    /// <summary>
    /// Agrega una nueva tarjeta
    /// </summary>
    public async Task<Result<CardDetailDto>> AddCardAsync(
        Guid userId,
        AddCardRequest request,
        CancellationToken ct = default)
    {
        // Validar que el número de tarjeta no exista
        if (await _cardRepository.CardNumberExistsAsync(request.CardNumber.Replace(" ", "").Replace("-", ""), ct))
            return Result.Failure<CardDetailDto>(CardErrors.CardAlreadyExists);

        // Crear value objects
        var cardNumberResult = CardNumber.Create(request.CardNumber);
        if (cardNumberResult.IsFailure)
            return Result.Failure<CardDetailDto>(cardNumberResult.Error);

        var expirationResult = ExpirationDate.Create(request.ExpirationMonth, request.ExpirationYear);
        if (expirationResult.IsFailure)
            return Result.Failure<CardDetailDto>(expirationResult.Error);

        var cvvResult = CVV.Create(request.CVV);
        if (cvvResult.IsFailure)
            return Result.Failure<CardDetailDto>(cvvResult.Error);

        var creditLimitResult = CreditLimit.Create(request.CreditLimit);
        if (creditLimitResult.IsFailure)
            return Result.Failure<CardDetailDto>(creditLimitResult.Error);

        // Parsear enums
        if (!Enum.TryParse<CardBrand>(request.Brand, true, out var brand))
            return Result.Failure<CardDetailDto>(new Error("Card.InvalidBrand", "Marca de tarjeta inválida"));

        if (!Enum.TryParse<CardType>(request.Type, true, out var type))
            return Result.Failure<CardDetailDto>(new Error("Card.InvalidType", "Tipo de tarjeta inválido"));

        // Crear entidad
        var cardResult = CreditCard.Create(
            userId,
            cardNumberResult.Value,
            request.CardholderName,
            expirationResult.Value,
            cvvResult.Value,
            brand,
            type,
            creditLimitResult.Value,
            request.Alias);

        if (cardResult.IsFailure)
            return Result.Failure<CardDetailDto>(cardResult.Error);

        var card = cardResult.Value;

        await _cardRepository.AddAsync(card, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDetail(card));
    }

    /// <summary>
    /// Actualiza una tarjeta (solo el alias)
    /// </summary>
    public async Task<Result<CardDetailDto>> UpdateCardAsync(
        Guid cardId,
        Guid userId,
        UpdateCardRequest request,
        CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);

        if (card is null)
            return Result.Failure<CardDetailDto>(CardErrors.CardNotFound);

        card.UpdateAlias(request.Alias);
        _cardRepository.Update(card);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDetail(card));
    }

    /// <summary>
    /// Elimina una tarjeta
    /// </summary>
    public async Task<Result<bool>> DeleteCardAsync(
        Guid cardId,
        Guid userId,
        CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);

        if (card is null)
            return Result.Failure<bool>(CardErrors.CardNotFound);

        _cardRepository.Delete(card);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    /// <summary>
    /// Bloquea una tarjeta
    /// </summary>
    public async Task<Result<CardDetailDto>> BlockCardAsync(
        Guid cardId,
        Guid userId,
        CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);

        if (card is null)
            return Result.Failure<CardDetailDto>(CardErrors.CardNotFound);

        card.Block();
        _cardRepository.Update(card);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDetail(card));
    }

    /// <summary>
    /// Activa una tarjeta bloqueada
    /// </summary>
    public async Task<Result<CardDetailDto>> ActivateCardAsync(
        Guid cardId,
        Guid userId,
        CancellationToken ct = default)
    {
        var card = await _cardRepository.GetByIdAndUserIdAsync(cardId, userId, ct);

        if (card is null)
            return Result.Failure<CardDetailDto>(CardErrors.CardNotFound);

        card.Activate();
        _cardRepository.Update(card);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDetail(card));
    }

    private static CardSummaryDto MapToSummary(CreditCard card) => new(
        card.Id,
        card.GetMaskedNumber(),
        card.CardholderName,
        card.Brand.ToString(),
        card.Type.ToString(),
        card.Status.ToString(),
        card.CurrentBalance,
        card.CreditLimit,
        card.AvailableCredit,
        card.GetExpirationDateFormatted(),
        card.Alias);

    private static CardDetailDto MapToDetail(CreditCard card) => new(
        card.Id,
        card.GetMaskedNumber(),
        card.CardholderName,
        card.Brand.ToString(),
        card.Type.ToString(),
        card.Status.ToString(),
        card.CurrentBalance,
        card.CreditLimit,
        card.AvailableCredit,
        card.GetExpirationDateFormatted(),
        card.Alias,
        card.CreatedAt,
        card.UpdatedAt);
}
