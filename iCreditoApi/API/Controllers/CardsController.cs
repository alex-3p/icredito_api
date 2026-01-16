using iCreditoApi.Modules.Cards.Application.DTOs;
using iCreditoApi.Modules.Cards.Application.Services;
using iCreditoApi.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCreditoApi.API.Controllers;

[ApiController]
[Route("api/cards")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly CreditCardService _cardService;
    private readonly ICurrentUserService _currentUser;

    public CardsController(CreditCardService cardService, ICurrentUserService currentUser)
    {
        _cardService = cardService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtiene todas las tarjetas del usuario
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCards(CancellationToken ct)
    {
        var result = await _cardService.GetUserCardsAsync(_currentUser.UserId, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el detalle de una tarjeta
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CardDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCard(Guid id, CancellationToken ct)
    {
        var result = await _cardService.GetByIdAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Agrega una nueva tarjeta
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CardDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCard(
        [FromBody] AddCardRequest request,
        CancellationToken ct)
    {
        var result = await _cardService.AddCardAsync(_currentUser.UserId, request, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return CreatedAtAction(nameof(GetCard), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza una tarjeta (solo el alias)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CardDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCard(
        Guid id,
        [FromBody] UpdateCardRequest request,
        CancellationToken ct)
    {
        var result = await _cardService.UpdateCardAsync(id, _currentUser.UserId, request, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Elimina una tarjeta
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(Guid id, CancellationToken ct)
    {
        var result = await _cardService.DeleteCardAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Bloquea una tarjeta
    /// </summary>
    [HttpPost("{id:guid}/block")]
    [ProducesResponseType(typeof(CardDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockCard(Guid id, CancellationToken ct)
    {
        var result = await _cardService.BlockCardAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Activa una tarjeta bloqueada
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(CardDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateCard(Guid id, CancellationToken ct)
    {
        var result = await _cardService.ActivateCardAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }
}
