using iCreditoApi.Modules.Transactions.Application.DTOs;
using iCreditoApi.Modules.Transactions.Application.Services;
using iCreditoApi.Shared.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCreditoApi.API.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly ICurrentUserService _currentUser;

    public TransactionsController(
        TransactionService transactionService,
        ICurrentUserService currentUser)
    {
        _transactionService = transactionService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtiene el historial de transacciones del usuario
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TransactionHistoryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _transactionService.GetHistoryAsync(
            _currentUser.UserId, page, pageSize, ct);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el detalle de una transacción
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransaction(Guid id, CancellationToken ct)
    {
        var result = await _transactionService.GetByIdAsync(id, _currentUser.UserId, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene las transacciones de una tarjeta específica
    /// </summary>
    [HttpGet("card/{cardId:guid}")]
    [ProducesResponseType(typeof(TransactionHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCardTransactions(
        Guid cardId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _transactionService.GetCardHistoryAsync(
            cardId, _currentUser.UserId, page, pageSize, ct);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Code, message = result.Error.Message });

        return Ok(result.Value);
    }
}
